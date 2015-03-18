//
//  SLifePoints.cs is part of Tunnelers: Unified
//  <https://github.com/VacuumGames/tunnelers-unified/>.
//
//  Copyright (c) 2014 Juraj Fiala<doctorjellyface@riseup.net>
//
//  Tunnelers: Unified is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  Tunnelers: Unified is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with Tunnelers: Unified.  If not, see <http://www.gnu.org/licenses/>.
//

using UnityEngine;

[RequireComponent (typeof (NetworkView))]
[RequireComponent (typeof (CLifePoints))]

[AddComponentMenu ("Network/Life")]

public class SLifePoints : MonoBehaviour {

	public GameObject parent;
	public GameObject tank;
	
	public PlayerMan playerType;
	public IMovable <int> controller;
	
	[SerializeField]
	float energyPoints = 100f;
	public float EnergyPoints {
		get { return energyPoints; }
	}
	public float maxRegEnergyPoints;
	public float maxEnergyPoints = 100f;
	public float moveEnergyConsumption = 2f;
	public float shootEnergyConsumption = 5f;
	
	[SerializeField]
	float shieldPoints = 100f;
	public float ShieldPoints {
		get { return shieldPoints; }
	}
	public float maxRegShieldPoints;
	public float maxShieldPoints = 100f;
	
	public float energyRegenerationRate = 10f;
	public float shieldRegenerationRate = 5f;
	public bool inBase;
	public float otherBaseDivider = 2f;
	
	void Awake () {
	
		if (!playerType) playerType = parent.GetComponent <PlayerMan> ();
		if (controller == null) controller = (IMovable <int>) tank.GetComponent (typeof (IMovable <int>));
		
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
		
		maxRegEnergyPoints = maxEnergyPoints * (2f/3f);
		maxRegShieldPoints = maxShieldPoints * (2f/3f);
		
	}
	
	void Update () {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
	
		energyPoints = Mathf.Clamp (energyPoints, -1f, maxEnergyPoints);
		shieldPoints = Mathf.Clamp (shieldPoints, -1f, maxShieldPoints);

		if (controller.IsMoving && !inBase && energyPoints > 0) {
		
			ChangeEnergy (-moveEnergyConsumption);
		
		}
	
	}
	
	public void ChangeEnergy (float amount) {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
	
		energyPoints += amount * Time.deltaTime;
	
	}
	
	public float GetEnergy () {
	
		return energyPoints;
	
	}
	
	public void ChangeShield (float amount) {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
	
		shieldPoints += amount * Time.deltaTime;
	
	}
	
	public bool ApplyDamage (float amount) {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return false;
		}
	
		Debug.Log ("Recieved " + amount + " damage!", gameObject);
	
		shieldPoints -= amount;
		//TODO Report system

		if (shieldPoints < 0) {

			Debug.Log ("I'm outta here.", gameObject);

			// Get IKillable instance and commit suicide
			var killer = (IKillable <float>) parent.GetComponent (typeof (IKillable <float>));
			killer.Kill ();

			return true;

		}

		return false;
		
	}
	
	public bool CanIShoot () {
	
		if (energyPoints > 0f) {
			return true;
		}
		
		return false;
	}
	
	public void IShot (float energyUsed) {
	
		if (energyPoints - energyUsed < 0f)
			Debug.LogError ("Shot without energy!", gameObject);
			
		energyPoints -= energyUsed;
		
	}
	
	public float GetShield () {
	
		return shieldPoints;
	
	}
	
	void OnTriggerStay (Collider other) {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
		
	//	Debug.Log ("I am in a trigger and it's name is: " + other.name, gameObject);
		
		if (other.tag != "BlueBase" && other.tag != "RedBase" && other.tag != "GreenBase")
			return;
	
		if (playerType.IsMyBase (other.tag)) {		
			inBase = true;
			if (energyPoints < maxRegEnergyPoints) ChangeEnergy (energyRegenerationRate);
			if (shieldPoints < maxRegShieldPoints) ChangeShield (shieldRegenerationRate);	
		} else if (!playerType.IsMyBase (other.tag)) {		
			inBase = true;
			if (energyPoints < maxRegEnergyPoints) ChangeEnergy (energyRegenerationRate / otherBaseDivider);
			if (shieldPoints < maxRegShieldPoints) ChangeShield (shieldRegenerationRate / otherBaseDivider);	
		}
	
	}
	
	void OnTriggerExit (Collider other) {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
		
	//	Debug.Log ("I just exited a trigger and it's name was: " + other.name, gameObject);
		
		if (other.tag != "BlueBase" && other.tag != "RedBase" && other.tag != "GreenBase")
			return;
		
		if (other.tag == "BlueBase" || other.tag == "RedBase" || other.tag == "GreenBase") {
		
			inBase = false;
			
		}
	
	}
	
	void OnSerializeNetworkView (BitStream stream) {
		
		int energy = 0;
		int shield = 0;
		
		if (stream.isWriting) {
		
			energy = (int) energyPoints;
			shield = (int) shieldPoints;
		
			stream.Serialize (ref energy);
			stream.Serialize (ref shield);
		
		} else if (playerType.owner == Network.player) {
			
			stream.Serialize (ref energy);
			stream.Serialize (ref shield);
			
			energyPoints = (float) energy;
			shieldPoints = (float) shield;
			
		}
	
	}
}
