using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Server/Net Man")]

public class S_NetMan : MonoBehaviour {

	#region Variables

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
	
	#endregion
	
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
				NetworkView netView = handle.GetComponent <NetworkView> ();
				netView.RPC ("SetOwner", RPCMode.AllBuffered, spawn.player);
				
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
	
	public GameObject RequestSpawn (NetworkPlayer requester, GameObject prefab, Vector3 pos, Quaternion rot) {
		
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
						
		PlayerMan sc = handle.GetComponent <PlayerMan> ();
				
		if (!sc) {
			Debug.LogError("The prefab has no C_PlayerMan attached.");
		}
				
		//Get the network view of the player and add its owner
		NetworkView netView = handle.GetComponent <NetworkView> ();
		netView.RPC ("SetOwner", RPCMode.AllBuffered, requester);
		
		netView.RPC ("SetID", RPCMode.AllBuffered, GetComponent <S_SpawnMan> ().AddTankToTracker (handle));
		
		if (handle)
			return handle;
			
		return null;
		
	}
	
	public void NetworkDestroy (GameObject gameObject) {
	
		Debug.Log ("Network.Destroying a game object.");
	
		Network.RemoveRPCs (gameObject.networkView.viewID);
	//	Network.RemoveRPCs (gameObject.GetComponent <PlayerMan> ().owner);
		Network.Destroy (gameObject);
	
	}
	
/*	void OnPlayerDisconnected (NetworkPlayer player) {
	
		Debug.Log("Player " + player.guid + " disconnected.");
		C_PlayerMan found = null;
		
		foreach (C_PlayerMan man in playerTracker) {
		
			if (man.GetOwner () == player) {
				Network.RemoveRPCs (man.gameObject.networkView.viewID);
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
		
		List <S_SpawnMan.PlayerTracker> playerTracker = GetComponent <S_SpawnMan> ().playerTracker;
		
		foreach (S_SpawnMan.PlayerTracker tracker in playerTracker) {
		
			Network.Destroy (tracker.instance.networkView.viewID);
			
			tracker.alive = false;	
		
		}
		
	}

}
