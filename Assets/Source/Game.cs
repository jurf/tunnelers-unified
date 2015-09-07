//
//  Game.cs is part of Tunnelers: Unified
//  <https://github.com/VacuumGames/tunnelers-unified/>
//
//  Copyright (c) 2015 Juraj Fiala <doctorjellyface@riseup.net>
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
using System.Collections.Generic;

public class Game: MonoBehaviour, IGame {

	public GameMode gameMode;
	
	public int minPlayers;
	public int maxPlayers;
	
	int bluePlayers;
	public int BluePlayers {
		
		get {
			int i = 0;
			foreach (SSpawnMan.PlayerTracker player in GetComponent <SSpawnMan> ().playerTracker) {
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
			foreach (SSpawnMan.PlayerTracker player in GetComponent <SSpawnMan> ().playerTracker) {
				if (!player.team)
					i++;		
			}
			return i;
		}
		
	} 
	
	public int gameTime;
	public int respawnTime;

	public float resetTime;

	public LoadMode loadMode;
	
	public List <string> levelList;

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

	public int maxScore;

	public bool blue;
	int blueScore;
	
	public int BlueScore {
		get {
			return blueScore;
		}
		set {
			if (value >= maxScore)
				ReportWin (Team.Blue);
			blueScore = value;
		}
	}

	public bool green;
	int greenScore;

	public int GreenScore {
		get {
			return greenScore;
		}
		set {
			if (value >= maxScore)
				ReportWin (Team.Green);
			greenScore = value;
		}
	}

	public bool red;
	int redScore;
	
	public int RedScore {
		get {
			return redScore;
		}
		set {
			if (value >= maxScore)
				ReportWin (Team.Red);
			redScore = value;
		}
	}

	float timeToEnd;
	
	bool blueFlagHome = true;
	bool redFlagHome = true;
	
	public List <string> gameNotifications;

	void Awake () {

		timeToEnd = gameTime;
	
	}
	
	void Update () {
		
		if (timeToEnd > 0) {
			timeToEnd -= Time.deltaTime;
		} else {
			WhoWon ();
		}
		
		bluePlayers = BluePlayers;
		redPlayers = RedPlayers;
		
	}
	
	void OnSerializeNetworkView (BitStream stream) {
		
		stream.Serialize (ref timeToEnd);
		stream.Serialize (ref bluePlayers);
		stream.Serialize (ref redPlayers);
		
	}
	
	void OnGUI () {
		
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
		
		GUILayout.Space (25);
		
		GUILayout.BeginHorizontal ();
		
		GUILayout.Label ("<color=blue>Blue: " + blueScore + "</color>", "box");
		GUILayout.FlexibleSpace ();
		
		GUILayout.EndHorizontal ();
		GUILayout.BeginHorizontal ();
		
		GUILayout.Label ("<color=blue>Blue flag home: " + blueFlagHome + "</color>", "box");
		GUILayout.FlexibleSpace ();
		
		GUILayout.EndHorizontal ();
		GUILayout.BeginHorizontal ();
		
		GUILayout.Label ("<color=red>Red: " + redScore + "</color>", "box");
		GUILayout.FlexibleSpace ();
		
		GUILayout.EndHorizontal ();
		GUILayout.BeginHorizontal ();
		
		GUILayout.Label ("<color=red>Red flag home: " + redFlagHome + "</color>", "box");
		GUILayout.FlexibleSpace ();
		
		GUILayout.EndHorizontal ();
		GUILayout.BeginHorizontal ();
		
		GUILayout.Label ("Time to end: " + timeToEnd, "box");
		GUILayout.FlexibleSpace ();
		
		GUILayout.EndHorizontal ();
		
	}
	

	void ReportWin (Team team) {

		Debug.Log ("End of the match. Winning team: " + team);

		if (team == Team.Blue) {
			EndOfMatch (EndGame.Blue);
		} else if (team == Team.Green) {
			EndOfMatch (EndGame.Green);
		} else {
			EndOfMatch (EndGame.Red);
		}
		
	}
	
	//	public void ReportFrag (NetworkPlayer fragger, NetworkPlayer dier) {
	//	
	//		if (!Network.isServer || Network.isClient) {
	//			enabled = false;
	//			return;
	//		}
	//
	//		int fIndex = GetPlayerIndex (fragger);
	//		int dIndex = GetPlayerIndex (dier);
	//		
	//		playerScores [fIndex].frags++;
	//		playerScores [dIndex].deaths++;
	//
	//	}
	
	//	public int GetPlayerIndex (NetworkPlayer playerID) {
	//	
	//		foreach (PlayerScore player in playerScores) {
	//			if (player.player == playerID) {
	//				return playerScores.IndexOf (player);
	//			}
	//			
	//		}
	//		
	//		return 0;
	//	
	//	}
	
	public void WhoWon () {
		
		if (BlueScore > RedScore)
			EndOfMatch (EndGame.Blue);
		else if (BlueScore < RedScore)
			EndOfMatch (EndGame.Red);
		else
			EndOfMatch (EndGame.Tie);
		
	}
	
	public void EndOfMatch (EndGame gameState) {
		
		Debug.Log ("End of match. Match status: " + gameState);
		
		Debug.Log ("Reseting game.");
		
		Invoke ("ResetGame", resetTime);
		
	}
	
	public void ResetGame () {
		
		blueScore = 0;
		redScore = 0;
		greenScore = 0;
		
		timeToEnd = gameTime;
		
		GameObject [] flags = GameObject.FindGameObjectsWithTag ("Flag");
		
		foreach (GameObject flag in flags) {

			var currentFlag = (IFlag) flag.GetComponent (typeof (IFlag)); 
			currentFlag.Home = true;
			currentFlag.Carrier = null;
			
		}
		
		/*
		foreach (PlayerScore score in playerScores) {
			score.Reset ();
		}
		*/
		GetComponent <SNetMan> ().ResetGame ();
		
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

	public void BlueScored () {
		

	}
	
	public void RedScored () {
		

	}
	
	public void FlagTaken (Team team) {
		throw new System.NotImplementedException ();
	}

	public void FlagReturned (Team team) {
		throw new System.NotImplementedException ();
	}

	public void FlagReturnedSelf (Team team) {
		throw new System.NotImplementedException ();
	}

	public void FlagCaptured (Team team) {
		throw new System.NotImplementedException ();
	}

	//TODO game notifications
	
	public void FlagTaken (bool isBlue) {
		
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
		
		if (isBlue) blueFlagHome = false; else redFlagHome = false;
		
	}
	
	public void FlagReturned (bool isBlue) {
		
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
		
		if (isBlue) blueFlagHome = true; else redFlagHome = true;
		
	}
	
	public void FlagReturnedSelf (bool isBlue) {
		
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
		
		if (isBlue) blueFlagHome = true; else redFlagHome = true;
		
	}
	
	public void FlagCaptured (bool isBlue/*, C_PlayerMan.Team otherTeam, NetworkPlayer player*/) {
		
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
		
		if (isBlue) {
			Debug.Log ("Blue scored.");				
			BlueScore++;
		} else {
			Debug.Log ("Red scored.");				
			RedScore++;
		}
		
		blueFlagHome = true;
		redFlagHome = true;
		
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
	}
	
}