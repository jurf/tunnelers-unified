import System.Collections.Generic;
#pragma strict

/**
 * Server-side implementation for the generic network manager.
 * In this class are ONLY functions that are called on or as the server
 */
class Netman extends MonoBehaviour {
	public var player : GameObject; //player object to instantiate
	public static var levelName : String = "test"; //current level name

	private var playerTracker : List.<C_PlayerManager> = new List.<C_PlayerManager>();
	private var scheduledSpawns : List.<NetworkPlayer> = new List.<NetworkPlayer>();

	private var processSpawnRequests : boolean = false;
	
	enum NetworkGroup {
		DEFAULT = 0,
		PLAYER = 1,
		SERVER = 2,
	}
	
	
	//Called on the server
	function OnPlayerConnected(player : NetworkPlayer) : void {
		Debug.Log("Spawning prefab for new client");
		scheduledSpawns.Add(player);
		processSpawnRequests = true;
	}
	
	@RPC
	function requestSpawn(requester : NetworkPlayer) {
		//Called from client to the server to request a new entity
		if (Network.isClient) {
			Debug.LogError("Client tried to spawn itself! Revise logic!");
			return; //Get lost! This is server business
		}
		if (!processSpawnRequests) {
			return; //silently ignore this
		}
		//Process all scheduled players
		for (var spawn : NetworkPlayer in scheduledSpawns) {
			Debug.Log("Checking player " + spawn.guid);
			if (spawn == requester) { //That is the one, lets make him an entity!
				var num : int = parseInt(spawn + "");
				var handle : GameObject =  Network.Instantiate(
													player, 
													transform.position, 
													Quaternion.identity, 
													NetworkGroup.PLAYER);
				var sc = handle.GetComponent(C_PlayerManager);
				if (!sc) {
					Debug.LogError("The prefab has no C_PlayerManager attached!");
				}
				playerTracker.Add(sc);
				//Get the network view of the player and add its owner
				var netView : NetworkView = handle.GetComponent(NetworkView);
				netView.RPC("setOwner", RPCMode.AllBuffered, spawn);
			}
		}
		scheduledSpawns.Remove(requester); //Remove the guy from the list now
		if (scheduledSpawns.Count == 0) {
			Debug.Log("spawns is empty! stopping spawn request processing");
			//If we have no more scheduled spawns, stop trying to process spawn requests
			processSpawnRequests = false;
		}
	}
	
	function OnPlayerDisconnected(player : NetworkPlayer) : void {
		Debug.Log("Player " + player.guid + " disconnected.");
		var found : C_PlayerManager = null;
		for (var man : C_PlayerManager in playerTracker) {
			if (man.getOwner() == player) {
				Network.RemoveRPCs(man.gameObject.networkView.viewID);
				Network.Destroy(man.gameObject);
			}
		}
		if (found) {
			playerTracker.Remove(found);
		}
	}
}