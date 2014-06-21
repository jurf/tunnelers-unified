using UnityEngine;

public class LifePoints : MonoBehaviour {
	
	public PlayerMan playerType;
	public M_TankController controller;
	
	[SerializeField]
	float energyPoints = 100f;
	public float maxRegEnergyPoints;
	public float maxEnergyPoints = 100f;
	public float moveEnergyConsumption = 2f;
	public float shootEnergyConsumption = 5f;
	
	[SerializeField]
	float shieldPoints = 100f;
	public float maxRegShieldPoints;
	public float maxShieldPoints = 100f;
	
	public float energyRegenerationRate = 10f;
	public float shieldRegenerationRate = 5f;
	public bool inBase;
	public float otherBaseDivider = 2f;
	
	void Awake () {
	
		if (!playerType) playerType = gameObject.GetComponent <PlayerMan> ();
		if (!controller) controller = gameObject.GetComponent <M_TankController> ();
		
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
	
		energyPoints = Mathf.Clamp (energyPoints, 0f, maxEnergyPoints);
		shieldPoints = Mathf.Clamp (shieldPoints, 0f, maxShieldPoints);

		if (controller.isMoving && !inBase && energyPoints > 0) {
		
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
			GetComponent <S_Assassin> ().Assassinate ();
			return true;
		} else {
			return false;
		}
	
	}
	
	public bool CanIShoot () {
	
		if (energyPoints > 0f) {
			return true;
		}
		
		return false;
	}
	
	public float GetShield () {
	
		return shieldPoints;
	
	}
	
	void OnTriggerStay (Collider other) {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
	
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
	
		if (other.tag == "BlueBase" || other.tag == "RedBase" || other.tag == "GreenBase") {
		
			if (playerType.IsMyBase (other.tag)) {
				inBase = false;
			} else if (!playerType.IsMyBase (other.tag)) {
				inBase = false;
			}
			
		}
	
	}
	
	void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info) {
		
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
