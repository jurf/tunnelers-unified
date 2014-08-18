//
//  S_Warhead.cs is part of Tunnelers: Unified
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
[RequireComponent (typeof (C_Warhead))]

[AddComponentMenu ("Network/Warhead")]

public class S_Warhead : MonoBehaviour {

	public bool left;
	public bool onLeftDown;
	public bool onLeftUp;
	
	bool previousLeft;

	void Awake () {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
		
	}
	
	void OnServerInitialized () {
		
		enabled = true;
			
	}
	
	void UpdateEvents () {
	
		if (left && left != previousLeft) {
			onLeftDown = true;
			onLeftUp = false;
		} else if (!left && left != previousLeft) {
			onLeftDown = false;
			onLeftUp = true;
		} else {
			onLeftDown = false;
			onLeftUp = false;
		}
		
		previousLeft = left;
		
	}
	
	[RPC]
	public void UpdateClientMouse (bool l) {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
	
		left = l;
	
		UpdateEvents ();
		
	}

}

