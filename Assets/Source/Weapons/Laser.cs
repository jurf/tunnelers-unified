//
//  Laser.cs is part of Tunnelers: Unified
//  <https://github.com/VacuumGames/tunnelers-unified/>.
//
//  Copyright (c) 2015 Juraj Fiala <doctorjellyface@riseup.net>
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

[AddComponentMenu ("Weapon/Laser")]

public class Laser: Weapon, IWeapon {

	public LineRenderer line;
	ILife life;
	public GameObject tank;
	
	public float laserSpeed;
	public float coolDown = 5f;
	public bool Cooled {
		get {
			return Time.time - time > coolDown;
		}
	}
	
	public float time;
	public float waitForSec = 0.5f;
	
	public float range = 5f;
	
	public float damage;
	
	public float energyConsumption = 10f;

	void Awake () {
	
		life = (ILife)tank.GetComponent (typeof(ILife));

	}
	
	void Update () {

//		if (Input.GetAxis ("Fire1") && cooled && life.HaveEnergy) {
//
//			Shoot ();
//			time = 0f;
//			life.Damage (energyConsumption);
//
//		}
//
//		if (time < coolDown) {
//			cooled = false;
//			time += Time.deltaTime;
//		}
//		
//		if (time > coolDown) {
//			cooled = true;
//			time = coolDown;
//		}
		
	}

	public void Shoot () {

		// If we don't have enrgy or aren't cooled yet, don't shoot
		if (!life.HaveEnergy || !Cooled)
			return;

		// If everything is alright, mark the time
		time = Time.time;

		// Shooting costs us energy
		life.Energy -= energyConsumption;
		
		RaycastHit hit;
		if (Physics.Raycast (transform.position, transform.forward, out hit, range)) {
		
			if (hit.collider.tag == "Tank") {

				DamagePlayer (hit.collider.gameObject, damage);
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


	void OnGUI () {

		float coolTime = (Time.time - time - coolDown) * -1f;

		if (!Cooled) {
			GUI.Label (new Rect (Input.mousePosition.x + 20, -Input.mousePosition.y + Screen.height, 20, 20),
				"<color=#ff0000>" + coolTime + "</color>");
		}

	}
	
}