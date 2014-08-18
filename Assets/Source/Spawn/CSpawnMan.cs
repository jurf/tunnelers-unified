//
//  C_SpawnMan.cs is part of Tunnelers: Unified
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
[RequireComponent (typeof (SSpawnMan))]

[AddComponentMenu ("Network/Spawn Man")]

public class CSpawnMan : MonoBehaviour {

	bool green; //is all loaded properly? set by client netman
	public bool Green {
	
		get {
			return green;
		}
		
		set {
			if (!green)
				green = value;
		}
	
	}
	
	public Rect teamSelectionRect;
	
	#region UnityMethods
	
		void Awake () {
		
			if (!Network.isClient || Network.isServer) {
				enabled = false;
				return;
			}
			
		}
		
		void ConnectedToServer () {
		
			enabled = true;
			
		}
		
		void OnGUI () {	
		
			if (!Network.isClient || Network.isServer) {
				enabled = false;
				return;
			}
			
			if (green) {
			
				teamSelectionRect.center = new Vector2 (Screen.width / 2, Screen.height / 2);
					
				teamSelectionRect = GUILayout.Window (0, teamSelectionRect, TeamSelection, "Team Selection");

			}
			
		}

	#endregion UnityMethods
		
	void TeamSelection (int windowID) {
           
//		int blueAmount;
        
		if (GUILayout.Button ("Blue")) {
			networkView.RPC ("RequestGameEntry", RPCMode.Server, Network.player, true, ServerC.name);
			enabled = false;
		}
        
		if (GUILayout.Button ("Red")) {
			networkView.RPC ("RequestGameEntry", RPCMode.Server, Network.player, false, ServerC.name);
			enabled = false;
		}

	}
	
}
