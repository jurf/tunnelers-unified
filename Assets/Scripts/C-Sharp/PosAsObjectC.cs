//
//  PosAsObjectC.cs is part of Tunnelers: Unified
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

public class PosAsObjectC : MonoBehaviour {
	
	public GameObject theObject;
	
	public bool rotate;
	
	public bool x;
	public bool y;
	public bool z;
	public Vector3 addPos;

	void Update () {
		
		/*if (!theObject) {
			GameObject[] tanks = GameObject.FindGameObjectsWithTag ("Tank");
			foreach (GameObject tank in tanks) {
				if (tank.networkView.isMine) {
					theObject = tank;
				}
			}
			if (!theObject) return;
		}*/
		
		if (theObject) {
		
			Vector3 temporary;
		
			if (x)
				temporary.x = theObject.transform.position.x + addPos.x;
			else 
				temporary.x = addPos.x;
		
			if (y)
				temporary.y = theObject.transform.position.y + addPos.y;
			else 
				temporary.y = addPos.y;
		
			if (z)
				temporary.z = theObject.transform.position.z + addPos.z;
			else 
				temporary.z = addPos.z;
		
			transform.position = temporary;
			
			if (rotate) {
				transform.rotation = theObject.transform.rotation;
			}
	
		}
	}
}
