using System.Collections.Generic;
using UnityEngine;
using L = Locale;

public class C_GameMan : MonoBehaviour {

	#region Variables

		public LanguageManager langMan;

		public int blueScore;
	//	public int greenScore;
		public int redScore;
		
		public bool blueFlagHome;
	//	public bool greenFlagHome;
		public bool redFlagHome;
	
	//	public List <string> gameNotifications;
	
		public float cronTime = 5f;
		
		public int serverTimeToEnd;
		
		public S_GameMan.EndGameType endGameState;
		
		public bool showEndGame;
		
		public Rect endGameWindow;
	
	#endregion
	
	#region UnityMethods
		
		void Awake () {
		
			if ((!Network.isClient || Network.isServer) || (!Network.isClient && !Network.isServer)) {
				enabled = false;
				return;
			}
			
		}
		
		void OnServerInitialized () {
			
			enabled = true;
			
		//	InvokeRepeating ("CallRequestRefresh", 0f, cronTime);
			
			langMan = LanguageManager.Instance;
			
		}

		void OnConnectedToServer () {
		
			CallRequestRefresh ();

		}
		
		#region GUI
		
			void OnGUI () {
			
				if (!Network.isClient || Network.isServer) {
					enabled = false;
					return;
				}
			
				GUILayout.Space (20);
			
				GUILayout.Label ("Blue: " + blueScore.ToString ());
				GUILayout.Label ("Blue flag home: " + blueFlagHome.ToString());
				
				GUILayout.Label ("Red: " + redScore.ToString ());
				GUILayout.Label ("Red flag home: " + redFlagHome.ToString ());
				
				GUILayout.Label ("Time to end: " + serverTimeToEnd.ToString ());
			
				if (showEndGame) {
					
					endGameWindow.center = new Vector2 (Screen.width / 2, Screen.height / 2);
					
					endGameWindow = GUILayout.Window (0, endGameWindow, EndGameWindow, "Game Ended");
					
				}
				
			}
			
			void EndGameWindow (int WindowID) {
			
				GUILayout.Label (endGameState.ToString ());
				
			}
			
		#endregion GUI
		
	#endregion UnityMethods
	
	void CallRequestRefresh () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
	
		networkView.RPC ("RequestRefresh", RPCMode.Server, Network.player);
		
	}

	[RPC]
	public void RefreshGameState (int blue, int red, bool blueFlag, bool redFlag) {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
		
		blueScore = blue;
		//greenScore = green;
		redScore = red;
		
		blueFlagHome = blueFlag;
		redFlagHome = redFlag;

	}
	
	//TODO Add player score list
	
	#region Flags
	
		[RPC]
		public void C_FlagTaken (bool isBlue) {
		
			if (!Network.isClient || Network.isServer) {
				enabled = false;
				return;
			}
			
		//	gameNotifications.Add (string.Format (langMan.GetTextValue ("Flag.Taken"), C_PlayerMan.GetTeam (team).ToString ().ToLower ()));
			
			if (isBlue)
				blueFlagHome = false;
			else
				redFlagHome = false;
			
		//	PlaySound
			
		}
	
		[RPC]
		public void C_FlagReturned (bool isBlue) {
		
			if (!Network.isClient || Network.isServer) {
				enabled = false;
				return;
			}
			
		//	gameNotifications.Add (string.Format (langMan.GetTextValue ("Flag.Returned"), C_PlayerMan.GetTeam (team).ToString ().ToLower ()));
		//	Debug.Log (string.Format (L.localeEN.flagReturned, team.ToString ().ToLower ()));
			
		//	PlaySound
			
			if (isBlue)
				blueFlagHome = true;
			else
				redFlagHome = true;
			
		}
	
		[RPC]
		public void C_FlagReturnedSelf (bool isBlue) {
		
			if (!Network.isClient || Network.isServer) {
				enabled = false;
				return;
			}
			
		//	gameNotifications.Add (string.Format (langMan.GetTextValue ("Flag.ReturnedSelf"), C_PlayerMan.GetTeam (team).ToString ().ToLower ()));
			
		//	PlaySound
			
			if (isBlue)
				blueFlagHome = true;
			else
				redFlagHome = true;
			
		}
	
		[RPC]
		public void C_FlagCaptured (bool isBlue) {
		
			if (!Network.isClient || Network.isServer) {
				enabled = false;
				return;
			}
			
			/*
			C_PlayerMan.Team tteam = (C_PlayerMan.Team) team;
			
			switch (tteam) {
				case C_PlayerMan.Team.Blue:
					blueScore++;
					break;
				case C_PlayerMan.Team.Green:
					greenScore++;
					break;
				case C_PlayerMan.Team.Red:
					redScore++;
					break;
				default:
					break;
			}
			
			gameNotifications.Add (string.Format (langMan.GetTextValue ("Flag.Captured"), C_PlayerMan.GetTeam (otherTeam).ToString ().ToLower ()));
			
			*/
			
			if (isBlue) {
				Debug.Log ("Blue scored.");
				blueScore++;
				blueFlagHome = true;
			} else {
				Debug.Log ("Red scored.");
				redScore++;
				redFlagHome = true;
			}
			
		//	PlaySound
			
		}
	
	#endregion Flags
	
	[RPC]
	public void TheEnd (int gameState) {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
		
		endGameState = (S_GameMan.EndGameType)gameState;
		
		showEndGame = true;
		
		Invoke ("StopEndGame", 5f);
		
	}
	
	void StopEndGame () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
	
		showEndGame = false;
		
	//	Application.LoadLevel (S_NetMan.levelName);
		
	}
	
		
}
