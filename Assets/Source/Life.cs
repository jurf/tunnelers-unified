// vim:set ts=4 sw=4 sts=4 noet:
//
//  Life.cs is part of Tunnelers: Unified
//  <https://github.com/VacuumGames/tunnelers-unified/>
//
//  Copyright (c) 2015-2016 Juraj Fiala <doctorjellyface@riseup.net>
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
using UnityEngine.Networking;

public class Life: NetworkBehaviour, ILife {

	Player playerType;
	// To find out whether we are moving
	IMovable <sbyte> controller;

	// Current energy 
	[SyncVar]
	float energy;
	public float Energy {
		get { return energy; }
		set { 
			// Clamp energy before assigning
			energy = Mathf.Clamp (value, 0f, maxEnergy);
		}
	}

	// Current shield
	[SyncVar]
	float shield ;
	public float Shield {
		get { return shield; }
		set {
			// Check if we're dead
			if (value < 0) {
				var killer = GetComponent <IKillable <float>> ();
				killer.Kill ();
			}

			// Clamp the shield before assigning
			shield = Mathf.Clamp (value, 0f, maxShield);
		}
	}

	// Weapons need to know if they can shoot
	public bool HaveEnergy {
		get { return energy > 0;}
	}

	// Maximum energy/shield we can get by regenerating
	float maxRegEnergy;
	float maxRegShield;
	
	// The most energy/shield we can get with power-ups
	[Range (50, 200)]
	public float maxEnergy = 100f;
	[Range (50, 200)]
	public float maxShield = 100f;

	// What does moving/shooting cost us
	[Range (0, 10)]
	public float moveEnergyConsumption = 2f;
	[Range (0, 30)]
	public float shootEnergyConsumption = 5f;

	// How quickly we regenerate energy/shield
	[Range (0, 20)]
	public float energyRegRate = 10f;
	[Range (0, 20)]
	public float shieldRegRate = 5f;

	// Are we in a base?
	bool inBase;
	// How less do we regenerate in enemy bases?
	[Range (0, 5)]
	public float otherBaseDivider = 2f;

	void Awake () {

		// Initialize references between objects
		playerType = GetComponent <Player> ();
		controller = GetComponent <IMovable <sbyte>> ();

		// Set the maximum possible regeneration values to
		// exactly two thirds of the overall maximum
		energy = maxRegEnergy = maxEnergy * (2f/3f);
		shield = maxRegShield = maxShield * (2f/3f);
	}

	void Update () {

		if (!isServer)
			return;

		// If we're moving and not in any kind of base, use up energy
		if (controller.IsMoving && !inBase)
			Energy -= moveEnergyConsumption * Time.deltaTime;
	}

	// The shooter needs to know if they've killed us,
	// in order to sign up for exp
	public void Damage (float amount, out bool killed) {
		
		if (!isServer) {
			killed = false;
			return;
		}

		Debug.Log ("A tank recieved " + amount + " damage.", gameObject);

		// Check if it would kill us
		if (amount > shield) {

			// If yes, notfiy the killer
			killed = true;
			Debug.Log ("A tank was killed.", gameObject);
			// The shield property will kill us before we get to return
			Shield -= amount;
			return;
		}

		killed = false;
		Shield -= amount;
	}

	void OnTriggerStay (Collider other) {

		if (!isServer)
			return;

		// We're not interested in triggers which are not bases
		if (other.tag != "BlueBase" && other.tag != "RedBase" && other.tag != "GreenBase")
			return;

		inBase = true;

		if (playerType.IsMyBase (other.tag)) {

			// If we're in our base, regenerate normally
			if (energy < maxRegEnergy)
				energy = Mathf.Clamp (energy + energyRegRate * Time.deltaTime, 0f, maxRegEnergy);
			if (shield < maxRegShield)
				shield = Mathf.Clamp (shield + shieldRegRate * Time.deltaTime, 0f, maxRegShield);

		} else if (!playerType.IsMyBase (other.tag)) {

			// If we're in an enemy base, divide the regeneration rate
			if (energy < maxRegEnergy)
				energy = Mathf.Clamp (energy + energyRegRate / otherBaseDivider * Time.deltaTime, 0f, maxRegEnergy);
			if (shield < maxRegShield)
				shield = Mathf.Clamp (shield + shieldRegRate / otherBaseDivider * Time.deltaTime, 0f, maxRegShield);
		}
	}

	void OnTriggerExit (Collider other) {

		if (!isServer)
			return;

		// Only do something when we exit a base
		if (other.tag != "BlueBase" && other.tag != "RedBase" && other.tag != "GreenBase")
			return;

		// This won't work properly if we're in two bases at once, but that shouldn't happen
		inBase = false;
		
	}

	// For now
	void OnGUI () {

		if (!isLocalPlayer)
			return;
		
		GUILayout.BeginArea (new Rect (0, 0, Screen.width, Screen.height));
		GUILayout.BeginVertical ();
		
		GUILayout.FlexibleSpace ();		
		GUILayout.BeginHorizontal ();
		
		GUILayout.Label ("Energy: " + (int) energy, "box");
		GUILayout.FlexibleSpace ();
		
		GUILayout.EndHorizontal ();
		GUILayout.BeginHorizontal ();
		
		GUILayout.Label ("Shield: " + (int) shield, "box");
		GUILayout.FlexibleSpace ();
		
		GUILayout.EndHorizontal ();
		
		GUILayout.EndVertical ();
		GUILayout.EndArea ();
		
	}
	
}
