using UnityEngine;

public class C_LevelName : MonoBehaviour {
	
	[RPC]
	public void TheLevelIs (string level) {	
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}	
				
		Debug.Log ("Disabling message queue!");
		Network.isMessageQueueRunning = false;
		
		Debug.Log ("The server told us what to load. The level is: " + level);
		
		S_LevelName.levelName = level;
		
	}

}
