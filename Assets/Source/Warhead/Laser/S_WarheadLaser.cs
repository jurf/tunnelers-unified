//
//  S_WarheadLaser.cs is part of Tunnelers: Unified
//  <https://github.com/VacuumGames/Tunnelers-Unified/>.
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
[RequireComponent (typeof (C_WarheadLaser))]

[AddComponentMenu ("Network/Warhead/Laser")]

public class S_WarheadLaser : MonoBehaviour {

	public S_Warhead warhead;
	public LineRenderer line;
	public S_LifePoints life;
	public GameObject parent;
	
	public float laserSpeed;
	public float coolDown = 5f;
	public bool cooled;
	
	public float time;
	public float waitForSec = 0.5f;
	
	public float range = 5f;
	
	public float damage;
	
	public float energyConsumption = 10f;
	
	void Awake () {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
		
	}
	
	void Update () {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
	
		if (warhead.left && cooled && life.CanIShoot ()) {
		
			Shoot ();
			networkView.RPC ("C_Shoot", RPCMode.Others);
			time = 0f;
			life.IShot (energyConsumption);
		
		}
		
		if (time < coolDown) {
			cooled = false;
			time += Time.deltaTime;
		}
		
		if (time > coolDown) {
			cooled = true;
			time = coolDown;
		}
		
	}

	void Shoot () {
		
		RaycastHit hit;
		if (Physics.Raycast (transform.position, transform.forward, out hit, range)) {
		
			if (hit.collider.tag == "Tank" || hit.collider.tag == "Turret") {
			
				hit.collider.gameObject.transform.parent.GetComponent <PlayerMan> ().life.ApplyDamage (damage);
				line.SetPosition (1, new Vector3 (0, 0, hit.distance));
				Invoke ("ResetLaser", waitForSec);
				
			} else {
						
				line.SetPosition (1, new Vector3 (0, 0, hit.distance));
				Invoke ("ResetLaser", waitForSec);
				
			}
			
		} else {
		
			line.SetPosition (1, new Vector3 (0, 0, range));		
			Invoke ("ResetLaser", waitForSec);
		
		}
	}
	
	void ResetLaser () {
	
		line.SetPosition (1, Vector3.zero);
				
	}
	
	public void OnSerializeNetworkView (BitStream stream) {
	
		int coolDownTime = 0;
	
		if (stream.isWriting) {
			coolDownTime = (int) time;
			stream.Serialize (ref cooled);
			stream.Serialize (ref coolDownTime);
		} else if (stream.isReading) {
			stream.Serialize (ref cooled);
			stream.Serialize (ref coolDownTime);
			time = (float) coolDownTime;
		}
	
	}
	
}

