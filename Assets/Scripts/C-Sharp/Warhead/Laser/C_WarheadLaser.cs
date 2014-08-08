//
//  C_WarheadLaser.cs is part of Tunnelers: Unified
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

public class C_WarheadLaser : MonoBehaviour {

	public S_WarheadLaser sscript;
	
	void Awake () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
		
	}
	
	void OnGUI () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
		
		if (Network.player != sscript.parent.GetComponent <PlayerMan> ().GetOwner ())
			return;
		
		int time = (int) -sscript.time + (int) sscript.coolDown;
	
		if (!sscript.cooled) {
			GUI.Label (new Rect (Input.mousePosition.x + 20, -Input.mousePosition.y + Screen.height, 20, 20),
				"<color=#ff0000>" + time + "</color>");
		}
	
	}
	
	[RPC]
	public void C_Shoot () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
		
		RaycastHit hit;
		if (Physics.Raycast (transform.position, transform.forward, out hit, sscript.range)) {
		
			if (hit.collider.tag == "Tank" || hit.collider.tag == "Turret") {
			
				sscript.line.SetPosition (1, new Vector3 (0, 0, hit.distance));
				Invoke ("ResetLaser", sscript.waitForSec);
				
			} else {
						
				sscript.line.SetPosition (1, new Vector3 (0, 0, hit.distance));
				Invoke ("ResetLaser", sscript.waitForSec);
				
			}
			
		} else {
		
			sscript.line.SetPosition (1, new Vector3 (0, 0, sscript.range));		
			Invoke ("ResetLaser", sscript.waitForSec);
		
		}
		
	}
	
	void ResetLaser () {
	
		sscript.line.SetPosition (1, Vector3.zero);
				
	}

}

