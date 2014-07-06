using UnityEngine;
using System.Collections;

public class C_NetManager : MonoBehaviour {

	void OnConnectedToServer () {
	
		StartCoroutine ("WaitForName");
	
	}

	IEnumerator WaitForName () {
	
		while (S_LevelName.levelName == "") {
			yield return 0;
		}
		
		Debug.Log ("Loading a level with the name: " + S_LevelName.levelName, gameObject);
		
		Application.LoadLevel (S_LevelName.levelName);
		
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
