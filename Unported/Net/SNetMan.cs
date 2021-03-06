//
//  SNetMan.cs is part of Tunnelers: Unified
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

using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (NetworkView))]
[RequireComponent (typeof (CNetManager))]

[AddComponentMenu ("Network/Life")]

public class SNetMan : MonoBehaviour {

//		public GameObject player; //player object to instantiate		
	
//		public List <C_PlayerMan> playerTracker = new List <C_PlayerMan> ();
//		public List <NetworkPlayer> scheduledSpawns = new List <NetworkPlayer> ();
	
//		public bool processSpawnRequests = false;

	public static string levelName;
	
	enum NetworkGroup {
		DEFAULT = 0,
		PLAYER = 1,
		SERVER = 2,	
	}
	
//	//Called on the server
//	void OnPlayerConnected (NetworkPlayer player) {
//	
//		Debug.Log ("Spawning prefab for new client");
//		scheduledSpawns.Add (player);
//		processSpawnRequests = true;
//		
//	}
	
/*	[RPC]
	void RequestSpawn.old (NetworkPlayer requester) {
	
		//Called from client to the server to request a new entity
		if (Network.isClient) {
		
			Debug.LogError("Client tried to spawn itself! Revise logic!");
			return; //Get lost! This is server business

		}
		
		if (!processSpawnRequests) {
			return; //silently ignore this
		}
		
		//Process all scheduled players
		foreach (ScheludedSpawn spawn in scheduledSpawns) {
		
			Debug.Log("Checking player " + spawn.player.guid);
			if (spawn.player == requester) { //That is the one, lets make him an entity!
				//int num = int.Parse (spawn + "");
				
				GameObject handle =  Network.Instantiate (
					spawn.prefab,
					spawn.pos, 
					spawn.rot, 
					(int)NetworkGroup.PLAYER
				) as GameObject;
					
				PlayerMan sc = handle.GetComponent <PlayerMan> ();
				
				if (!sc) {
					Debug.LogError("The prefab has no C_PlayerMan attached!");
				}
				
				playerTracker.Add (sc);
				
				//Get the network view of the player and add its owner
				NetworkView nView = handle.GetComponent <NetworkView> ();
				nView.RPC ("SetOwner", RPCMode.AllBuffered, spawn.player);
				
			}
		}
		
		scheduledSpawns.Remove (requester); //Remove the guy from the list now
		
		if (scheduledSpawns.Count == 0) {		
			Debug.Log("Spawns is empty! Stopping spawn request processing");
			//If we have no more scheduled spawns, stop trying to process spawn requests
			processSpawnRequests = false;			
		}
	}
	
*/

	void Awake () {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
	}
	
	void OnServerInitialized () {
	
		enabled = true;
		
	}
	
	public GameObject RequestSpawn (
		NetworkPlayer requester, GameObject prefab, Vector3 pos, Quaternion rot, string playerName
	) {
		
		/*
		//Called from client to the server to request a new entity
		if (Network.isClient) {
		
			Debug.LogError("Client tried to spawn itself! Spawn terminated.");
			return false; //Get lost! This is server business

		}	
		*/
		
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return null;
		}
		
		Debug.Log ("Actually spawning new player via the server NetMan.");
			
		GameObject handle = Network.Instantiate (
			prefab,
			pos, 
			rot, 
			(int)NetworkGroup.PLAYER
		) as GameObject;
						
		Player sc = handle.GetComponent <Player> ();
				
		if (!sc) {
			Debug.LogError("The prefab has no C_PlayerMan attached.");
		}
				
		//Get the network view of the player and add its owner
		NetworkView nView = handle.GetComponent <NetworkView> ();
		nView.RPC ("SetOwner", RPCMode.AllBuffered, requester);
		
		nView.RPC ("SetID", RPCMode.AllBuffered, GetComponent <SSpawnMan> ().AddTankToTracker (handle));
		
		nView.RPC ("SetName", RPCMode.AllBuffered, playerName);
		
		if (handle)
			return handle;
			
		return null;
		
	}
	
	public static void NetworkDestroy (GameObject theGameObject) {
		Debug.Log ("Network.Destroying a game object.");
		Network.RemoveRPCs (theGameObject.GetComponent<NetworkView>().viewID);
		//	Network.RemoveRPCs (GetComponent <PlayerMan> ().owner);
		Network.Destroy (theGameObject);
	}
	
/*	void OnPlayerDisconnected (NetworkPlayer player) {
	
		Debug.Log("Player " + player.guid + " disconnected.");
		C_PlayerMan found = null;
		
		foreach (C_PlayerMan man in playerTracker) {
		
			if (man.GetOwner () == player) {
				Network.RemoveRPCs (man.gameObject.nView.viewID);
				Network.Destroy (man.gameObject);
			}
			
		}
		
		if (found) {
			playerTracker.Remove (found);
		}
		
	}

	//Called from S_SpawnMan
	public void CreateInstance (NetworkPlayer player, GameObject prefab, Vector3 pos, Quaternion rot, int group) {
	
		scheludedSpawns.Add (new ScheludedSpawn (player, prefab, pos, rot, group));
		processSpawnRequests = true;
		RequestSpawn (player);

	}
	
*/

	public void ResetGame () {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
		
		List <SSpawnMan.PlayerTracker> playerTracker = GetComponent <SSpawnMan> ().playerTracker;
		
		foreach (SSpawnMan.PlayerTracker tracker in playerTracker) {
		
			Network.Destroy (tracker.instance.GetComponent<NetworkView>().viewID);
			
			tracker.alive = false;	
		
		}
		
	}

}
