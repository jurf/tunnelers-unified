﻿using UnityEngine;
using System.Collections.Generic;

public class S_SpawnMan : MonoBehaviour {
	
	#region variables
	
		public float spawnLoop = 15f;
		public float spawnHeight = 5f;
		
		public GameObject bluePrefab;
		public GameObject redPrefab;
			
		public Transform blue1;
		public Transform blue2;
		
		public Transform red1;
		public Transform red2;

		public List <PlayerTracker> playerTracker;
		
		public List <NetworkPlayer> connectedUnspawned = new List <NetworkPlayer> ();
		
		public List <GameObject> tankTracker;

	#endregion
	
	[System.Serializable]
	public class PlayerTracker {

		public NetworkPlayer player;
		
		public bool alive;
		
		public GameObject instance;

		public bool team;

		public PlayerTracker (NetworkPlayer id, bool isBlue) {
		
			player = id;
			team = isBlue;
		
		}
		
	}
	
	/*[RPC]
	public void RequestSpawn (NetworkPlayer player, bool team) {
	
		playerTracker.Add (new PlayerTracker (player, team));
	
	}*/
	
	public int AddTankToTracker (GameObject tank) {
	
		tankTracker.Add (tank);
		return tankTracker.Count - 1;
		
	}
	
	public void DeleteTracker (NetworkPlayer player) {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
	
		foreach (PlayerTracker tracker in playerTracker) {
		
			if (tracker.player == player) {
			
				tracker.instance.GetComponent <S_Assassin> ().Assassinate ();
				playerTracker.Remove (tracker);
				return;
			
			}
		
		}
	
	}
	
	#region UnityMethods
	
		void Awake () {
	
			if (!Network.isServer || Network.isClient) {
				enabled = false;
				return;
			}
			
		}
		
		void OnServerInitialized () {
		
			enabled = true;
		
			InvokeRepeating ("SpawnCron", 0f, spawnLoop);
			
		}
		
		void OnPlayerConnected (NetworkPlayer player) {
		
			Debug.Log ("Adding a player to unspawned.");
			connectedUnspawned.Add (player);
			Debug.Log ("Unspawned queue is " + connectedUnspawned.Count + " players long.");
			
		}
		
		void OnPlayerDisconnected (NetworkPlayer player) {
		
			foreach (NetworkPlayer unspawned in connectedUnspawned) {
				
				if (unspawned == player) {
				
					connectedUnspawned.Remove (unspawned);
					Network.RemoveRPCs (player);
					return;
				
				}
			
			}
			
			DeleteTracker (player);
	
		}
		
		void OnTriggerExit (Collider other) {
		
			if (!Network.isServer || Network.isClient) {
				enabled = false;
				return;
			}
		
			foreach (PlayerTracker tracker in playerTracker) {
			
				if (tracker.instance == other.collider.gameObject.transform.parent.gameObject) {
				
					Debug.Log ("Destroying the tank which fell out.");
					
					tracker.instance.GetComponent <S_Assassin> ().Assassinate ();
					
					tracker.alive = false;
					
					Debug.Log ("Found the fallen instance. Destroyed.");
					return;
					
				}
				
			}
			
			Debug.LogError ("Destroying anyway.");
			GetComponent <S_NetMan> ().NetworkDestroy (other.transform.parent.gameObject);

		}			
	
	#endregion UnityMethods
	
	void SpawnCron () {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
		
		Debug.Log ("Spawning dead players.");
	
		foreach (PlayerTracker tracker in playerTracker) {
		
			if (!tracker.alive && !tracker.instance || tracker.alive && !tracker.instance) {
			
				tracker.alive = false;
			
				GameObject instance = Spawn (tracker.player, tracker.team);
				
				if (instance) {
					tracker.alive = true;
					tracker.instance = instance;	
				}
				
			} else if (!tracker.alive && tracker.instance) {
			
				tracker.alive = true;
				
			}
			
		}
		
	}

	GameObject Spawn (NetworkPlayer player, bool isBlue) {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return null;
		}
		
		Debug.Log ("Going to spawn player, doing some spawn stuff and then requesting spawn from server NetMan.");

		Vector3 pos = new Vector3 (0,0, spawnHeight);
		GameObject prefab;

		if (isBlue) {
		
			Debug.Log ("Spawning a blue tank.");
		
			//pos.x = Random.Range (blue1.position.x, blue2.position.x);
			//pos.y = Random.Range (blue1.position.y, blue2.position.y);
			pos = blue1.position;
			
			prefab = bluePrefab;
		
		} else {
		
			Debug.Log ("Spawning a red tank.");
		
			//pos.x = Random.Range (red1.position.x, red2.position.x);
			//pos.y = Random.Range (red1.position.y, red2.position.y);
			pos = red1.position;
			
			prefab = redPrefab;
		
		}
		
		return GetComponent <S_NetMan> ().RequestSpawn (player, prefab, pos, Quaternion.identity);
		
		//Network.Instantiate (prefab, pos, Quaternion.identity, 0);
		
	}

	[RPC]
	public void RequestGameEntry (NetworkPlayer player, bool isBlue) {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
		
		Debug.Log ("Got request for player spawn, checking queue.");
	
		foreach (NetworkPlayer unspawned in connectedUnspawned) {
		
			if (unspawned == player) {
			
				playerTracker.Add (new PlayerTracker (player, isBlue));
				connectedUnspawned.Remove (unspawned);
				Debug.Log ("Found the correct player, added him to the tracker and removed from unspawned.");
				return;
			
			}
		
		}
		
		Debug.LogError ("Didn't find player in unspawned. Spawn terminated. Try reconnecting.");
		
	}

}
