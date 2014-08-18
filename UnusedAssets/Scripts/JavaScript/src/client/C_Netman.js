#pragma strict

class C_Netman extends MonoBehaviour {
	function OnConnectedToServer() {
		Debug.Log("Disabling message queue!");
		Network.isMessageQueueRunning = false;
		Application.LoadLevel(Netman.levelName);
	}
	
	function OnLevelWasLoaded(level : int) {
		if (/*level != 0 && */Network.isClient) { //0 is my menu scene so ignore that.
			Network.isMessageQueueRunning = true;
			Debug.Log("Level was loaded, requesting spawn");
			Debug.Log("Re-enabling message queue!");
			//Request a player instance form the server
			networkView.RPC("requestSpawn", RPCMode.Server, Network.player);
		}
	}
}