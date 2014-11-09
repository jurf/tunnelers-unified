//
//  STurretMan.cs is part of Tunnelers: Unified
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
[RequireComponent (typeof (CTurretMan))]
[RequireComponent (typeof (MTurretPredictor))]
[RequireComponent (typeof (IRotatable))]

[AddComponentMenu ("Network/Turret Man")]

public class STurretMan : MonoBehaviour {

	IRotatable controller;
	
	Quaternion toRotation;
	
	void Awake () {
		
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}

		if (controller == null)
			controller = (IRotatable) GetComponent (typeof (IRotatable));
		
	}
	
	void Update () {
		
		if (Network.isClient || (!Network.isServer && !Network.isClient)) {
			enabled = false;
			return; //Get lost, this is the server-side!
		}
		
		//Debug.Log("Processing clients movement commands on server");
		controller.Rotate (toRotation);
		
	}
	
	/**
     * The client calls this to notify the server about new motion data
     * @param	motion
     */
	
	[RPC]
	public void UpdateClientRotation (float toRotY) {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
		
		toRotation = Quaternion.Euler (new Vector3 (0, toRotY, 0));
		
	}
}

