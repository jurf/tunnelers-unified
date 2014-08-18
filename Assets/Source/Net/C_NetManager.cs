//
//  C_NetManager.cs is part of Tunnelers: Unified
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
[RequireComponent (typeof (S_NetMan))]

[AddComponentMenu ("Network/Net Man")]

public class C_NetManager : MonoBehaviour {

	void OnConnectedToServer () {
	
		Debug.Log ("Disabling message queue!");
		Network.isMessageQueueRunning = false;
	
		Application.LoadLevel (S_NetMan.levelName);
	
	}
	
	void OnLevelWasLoaded (int level) {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
		
		if (level != 0 && level != 1) { //0 and 1 are my intro and menu scenes so ignore that
			
			Network.isMessageQueueRunning = true;
			Debug.Log ("Level was loaded, requesting spawn");
			Debug.Log ("Re-enabling message queue!");
			//Request a player instance form the server (done by spawn man)
			//networkView.RPC ("RequestSpawn", RPCMode.Server, Network.player);
			
			GetComponent <C_SpawnMan> ().Green = true;
		
		} else {
		
			S_NetMan.levelName = "";
			
		}
	
	}
	
	public static GameObject GetTankInstance (NetworkPlayer player) {
	
		GameObject [] tanks = GameObject.FindGameObjectsWithTag ("Tank");
		
		foreach (GameObject tank in tanks) {
			
			PlayerMan playerMan = tank.GetComponent <PlayerMan> ();
			
			if (playerMan.owner == player)
				return tank;
		
		}
		
		return null;
	
	}

}
