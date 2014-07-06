using System.Collections.Generic;
using UnityEngine;
using L = Locale;

public class S_GameMan : MonoBehaviour {

	public LanguageManager langMan;
	
	// Only CTF for now
	/*public enum GameType {
		FreeForAll,
		TeamShowdown,
		CaptureTheFlag,
		Dominion
	}
	
	public GameType gameType;*/
	
	public int minPlayers;
	public int maxPlayers;
	
	int bluePlayers;
	public int BluePlayers {
		
		get {
			int i = 0;
			foreach (S_SpawnMan.PlayerTracker player in GetComponent <S_SpawnMan> ().playerTracker) {
				if (player.team)
					i++;		
			}
			return i;
		}
		
	}
	
	int redPlayers;
	public int RedPlayers {
		
		get {
			int i = 0;
			foreach (S_SpawnMan.PlayerTracker player in GetComponent <S_SpawnMan> ().playerTracker) {
				if (!player.team)
					i++;		
			}
			return i;
		}
		
	} 
	
	public int gameTime;
	public int respawnTime;
	
	// TODO finish this, will be cool
	/*
	public enum LevelLoadType {
		_Same,
		_Random,
		_List,
		_Vote
	}
	
	
	public LevelLoadType levelLoadType;
	
	public List <string> levelList;
	*/
	
	public enum EndGameType {
		Blue,
		Red,
		Tie
	}
	/*
	// Have no idea whether this will make it - Answer: Nope.
	[System.Serializable]
	public class PlayerScore {
	
		public NetworkPlayer player;
		public int level;
		public int frags;
		public int deaths;
		public int assists;
		public int captures;
		public int score;
		public float connectTime;
		public float playTime;
		public float travelDistance;
		//public float digDistance;
		
		public PlayerScore (NetworkPlayer playerID) {
		
			player = playerID;
		
		}
		
		public void Reset () {
			
			frags = 0;
			deaths = 0;
			assists = 0;
			captures = 0;
			score = 0;
			travelDistance = 0;
			//digDistance = 0;
			
		}
		
	}
	
	public List <PlayerScore> playerScores;
	*/
	#region Team Scores
	
		public int maxScore;
		
		public bool red;
		int redScore;
	
		public int RedScore {
			get {
				return redScore;
			}
			set {
				if (value >= maxScore)
					ReportWin (false);
				redScore = value;
			}
		}
		
		public bool blue;
		int blueScore;
	
		public int BlueScore {
			get {
				return blueScore;
			}
			set {
				if (value >= maxScore)
					ReportWin (true);
				blueScore = value;
			}
		}
		
		// Only blu/red for now
		/*
		public bool green;
		int greenScore;
	
		public int GreenScore {
			get {
				return greenScore;
			}
			set {
				if (value >= maxScore)
					ReportWin (C_PlayerMan.Team.Green);
				greenScore = value;
			}
		}
		*/
		
	#endregion Team Scores
	
	#region Temporary Game Variables
	
		public float timeToEnd;
		
		public bool blueFlagHome;
		public bool redFlagHome;
		
		// Only sound for now
		/*
		public List <string> gameNotifications;
		*/
	
	#endregion
	
	// Only on connect, maybe TODO Sync (dealing with it)
	[RPC]
	public void RequestRefresh (NetworkPlayer player) {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
	
		networkView.RPC ("RefreshGameState", player, blueScore, redScore, blueFlagHome, redFlagHome);
	
	}
	
	#region Unity Methods
	
		void OnServerInitialized () {
		
			enabled = true;
			
			timeToEnd = gameTime;
			
			langMan = LanguageManager.Instance;
			
		}
	
		void Awake () {
		
			if (!Network.isServer || Network.isClient) {
				Debug.Log ("Disabling the game man.");
				enabled = false;
				return;
			}
		
		}
			
		void Update () {
		
			if (!Network.isServer || Network.isClient) {
				enabled = false;
				return;
			}
			
			if (timeToEnd > 0) {
				timeToEnd -= Time.deltaTime;
			} else {
				WhoWon ();
			}
			
			bluePlayers = BluePlayers;
			redPlayers = RedPlayers;
			
		}
		
		void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info) {
		
			stream.Serialize (ref timeToEnd);
			stream.Serialize (ref bluePlayers);
			stream.Serialize (ref redPlayers);
			
		}
		
		#region GUI
		
			void OnGUI () {
			
				if (!Network.isServer || Network.isClient) {
					enabled = false;
					return;
				}
			
				GUILayout.Space (25);
				
				GUILayout.BeginHorizontal ();
				
					GUILayout.Label ("<color=blue>Blue: " + blueScore.ToString () + "</color>", "box");
					GUILayout.FlexibleSpace ();
					
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal ();
				
					GUILayout.Label ("<color=blue>Blue flag home: " + blueFlagHome.ToString() + "</color>", "box");
					GUILayout.FlexibleSpace ();
					
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal ();
				
					GUILayout.Label ("<color=red>Red: " + redScore.ToString () + "</color>", "box");
					GUILayout.FlexibleSpace ();
				
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal ();
				
					GUILayout.Label ("<color=red>Red flag home: " + redFlagHome.ToString () + "</color>", "box");
					GUILayout.FlexibleSpace ();
				
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal ();
				
					GUILayout.Label ("Time to end: " + timeToEnd.ToString (), "box");
					GUILayout.FlexibleSpace ();
				
				GUILayout.EndHorizontal ();
				
			}
			
		#endregion GUI
		
	#endregion Unity Methods
		
	void ReportWin (bool isBlue) {
		
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
		
		if (isBlue) {
			Debug.Log ("Max flags reached. Winning team: BLUE.");
			EndOfMatch (EndGameType.Blue);
		} else {
			Debug.Log ("Max flags reached. Winning team: RED.");
			EndOfMatch (EndGameType.Red);
		}
	
	}
	
	public void ReportFrag (NetworkPlayer fragger, NetworkPlayer dier) {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
	/*
		int fIndex = GetPlayerIndex (fragger);
		int dIndex = GetPlayerIndex (dier);
		
		playerScores [fIndex].frags++;
		playerScores [dIndex].deaths++;
	*/
	}
	/*
	public int GetPlayerIndex (NetworkPlayer playerID) {
	
		foreach (PlayerScore player in playerScores) {
			if (player.player == playerID) {
				return playerScores.IndexOf (player);
			}
			
		}
		
		return 0;
	
	}
	*/
	public void WhoWon () {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
	
		if (BlueScore > RedScore)
			EndOfMatch (EndGameType.Blue);
		else if (BlueScore < RedScore)
			EndOfMatch (EndGameType.Red);
		else
			EndOfMatch (EndGameType.Tie);
			
	}
	
	public void EndOfMatch (EndGameType gameState) {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
	
		Debug.Log ("End of match.");
		
		networkView.RPC ("TheEnd", RPCMode.All, (int)gameState);
		
		Debug.Log ("Match status: " + gameState);
		
		Debug.Log ("Reseting game.");
		
		Invoke ("ResetGame", 5f);
		
	}
	
	public void ResetGame () {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
		
		blueScore = 0;
		redScore = 0;
		//greenScore = 0;
		
		timeToEnd = gameTime;
		/*
		foreach (PlayerScore score in playerScores) {
			score.Reset ();
		}
		*/
		GetComponent <S_NetMan> ().ResetGame ();
		
	}
	/*
	#region Players
	
		void OnPlayerConnected (NetworkPlayer player) {

			playerScores.Add (new PlayerScore (player));
			

		}

		void OnPlayerDisconnected (NetworkPlayer player) {

			playerScores.RemoveAt (GetPlayerIndex (player));

		}
	
	#endregion
	*/
	#region Scores
		
		public void BlueScored () {
		
			networkView.RPC ("C_BlueScored", RPCMode.All);
			
		}
		
		public void RedScored () {
		
			networkView.RPC ("C_RedScored", RPCMode.All);
		
		}
		
	#endregion Scores

	#region Flags

		//TODO game notifications
		
		public void FlagTaken (bool isBlue) {
		
			if (!Network.isServer || Network.isClient) {
				enabled = false;
				return;
			}
			
			if (isBlue) blueFlagHome = false; else redFlagHome = false;

			//gameNotifications.Add (string.Format (langMan.GetTextValue ("Flag.Taken"), team.ToString ().ToLower ()));
			networkView.RPC ("C_FlagTaken", RPCMode.All, isBlue);
		
		}
		
		public void FlagReturned (bool isBlue) {
		
			if (!Network.isServer || Network.isClient) {
				enabled = false;
				return;
			}
			
			if (isBlue) blueFlagHome = true; else redFlagHome = true;
		
			//gameNotifications.Add (string.Format (langMan.GetTextValue ("Flag.Returned"), team.ToString ().ToLower ()));
			networkView.RPC ("C_FlagReturned", RPCMode.All, isBlue);

		}
		
		public void FlagReturnedSelf (bool isBlue) {
		
			if (!Network.isServer || Network.isClient) {
				enabled = false;
				return;
			}
			
			if (isBlue) blueFlagHome = true; else redFlagHome = true;
		
			//gameNotifications.Add (string.Format (langMan.GetTextValue ("Flag.ReturnedSelf"), team.ToString ().ToLower ()));
			networkView.RPC ("C_FlagReturnedSelf", RPCMode.All, isBlue);
		
		}
		
		public void FlagCaptured (bool isBlue/*, C_PlayerMan.Team otherTeam, NetworkPlayer player*/) {
		
			if (!Network.isServer || Network.isClient) {
				enabled = false;
				return;
			}
		
			//gameNotifications.Add (string.Format (langMan.GetTextValue ("Flag.Captured"), otherTeam.ToString ().ToLower ()));
			
			if (isBlue) {
				Debug.Log ("Blue scored.");
				blueFlagHome = true;
				BlueScore++;
			} else {
				Debug.Log ("Red scored.");
				redFlagHome = true;
				RedScore++;
			}	
			
			/*switch (myTeam) {
				case C_PlayerMan.Team.Red:
					RedScore++;
					break;
				case C_PlayerMan.Team.Blue:
					BlueScore++;
					break;
				case C_PlayerMan.Team.Green:
					GreenScore++;
					break;
				default:
					Debug.LogError ("Unknown team.");
					break;
			}*/
			
			//playerScores[GetPlayerIndex (player)].captures++;
			networkView.RPC ("C_FlagCaptured", RPCMode.All, isBlue);		
		
		}

	#endregion Flags

}

