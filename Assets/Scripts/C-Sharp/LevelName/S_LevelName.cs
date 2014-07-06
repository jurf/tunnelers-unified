using UnityEngine;

public class S_LevelName : MonoBehaviour {

	public static string levelName = "";
	
	void Awake () {
	
		DontDestroyOnLoad (gameObject);
		
	}
	
	void OnPlayerConnected (NetworkPlayer player) {
	
		Debug.Log ("Sending a client what to load. Current map: " + S_LevelName.levelName);
		
		networkView.RPC ("TheLevelIs", player, S_LevelName.levelName);
		
	}
	
	void OnLevelWasLoaded (int level) {
	
		if (level == 1)
			levelName = "";
		
	}

}
