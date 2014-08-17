//
//  S_TankMan.cs is part of Tunnelers: Unified
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
[RequireComponent (typeof (C_TankMan))]
[RequireComponent (typeof (M_TankPredictor))]
[RequireComponent (typeof (M_TankController))]

[AddComponentMenu ("Network/Tank Man")]

public class S_TankMan : MonoBehaviour {
	
	public M_TankController controller;
	
	public int horizontalMotion;
	public int verticalMotion;
	
	void Awake () {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
	
	}
	
	public void Update () {
	
		if (Network.isClient || (!Network.isServer && !Network.isClient)) {
			enabled = false;
			return;
		}

		controller.Move (horizontalMotion, verticalMotion);
		
	}
     
	[RPC]
	public void UpdateClientMotion (int hor, int vert) {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
	
		horizontalMotion = hor;
		verticalMotion = vert;
	
	}
}
