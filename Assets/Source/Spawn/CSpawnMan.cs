//
//  CSpawnMan.cs is part of Tunnelers: Unified
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

	bool readyToGo; //is all loaded properly? set by client netman
	public bool ReadyToGo {
	
		get {
			return readyToGo;
		}
		
		set {
			if (!readyToGo)
				readyToGo = value;
		}
	
	}
	
	public Rect teamSelectionRect;

	NetworkView netView;
	
	#region UnityMethods
	
		void Awake () {
		
			if (!Network.isClient || Network.isServer) {
				enabled = false;
				return;
			}
			
			netView = GetComponent <NetworkView> ();
			
		}
		
		void ConnectedToServer () {
		
			enabled = true;
			
		}
		
		void OnGUI () {	
		
			if (!Network.isClient || Network.isServer) {
				enabled = false;
				return;
			}
			
			if (readyToGo) {
			
				teamSelectionRect.center = new Vector2 (Screen.width / 2, Screen.height / 2);
					
				teamSelectionRect = GUILayout.Window (0, teamSelectionRect, TeamSelection, "Team Selection");

			}
			
		}

	#endregion UnityMethods
		
	void TeamSelection (int windowID) {
           
//		int blueAmount;
        
		if (GUILayout.Button ("Blue", GUILayout.MinWidth (100))) {
			netView.RPC ("RequestGameEntry", RPCMode.Server, Network.player, true, ServerC.playerNickname);
			enabled = false;
		}
        
		if (GUILayout.Button ("Red", GUILayout.MinWidth (100))) {
			netView.RPC ("RequestGameEntry", RPCMode.Server, Network.player, false, ServerC.playerNickname);
			enabled = false;
		}

	}
	
}
