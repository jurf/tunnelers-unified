//
//  CGameMan.cs is part of Tunnelers: Unified
//  <https://github.com/VacuumGames/tunnelers-unified/>.
//
//  Copyright (c) 2014 Juraj Fiala<doctorjellyface@riseup.net>
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

[RequireComponent (typeof (NetworkView))]
[RequireComponent (typeof (SGameMan))]
[RequireComponent (typeof (MGameMan))]

[AddComponentMenu ("Network/Game Man")]

public class CGameMan : MonoBehaviour {

	#region Variables

		public LanguageManager langMan;

		public int blueScore;
	//	public int greenScore;
		public int redScore;
		
		public bool blueFlagHome;
	//	public bool greenFlagHome;
		public bool redFlagHome;
	
	//	public List <string> gameNotifications;
	
	//	public float cronTime = 5f;
		
		public int serverTimeToEnd;
		
		public SGameMan.EndGameType endGameState;
		
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
				
					GUILayout.Label ("Time to end: " + serverTimeToEnd, "box");
					GUILayout.FlexibleSpace ();
				
				GUILayout.EndHorizontal ();
			
				if (showEndGame) {
					
					endGameWindow.center = new Vector2 (Screen.width / 2, Screen.height / 2);
					
					endGameWindow = GUILayout.Window (0, endGameWindow, EndGameWindow, "Game Ended");
					
				}
				
			}
			
			void EndGameWindow (int windowId) {
			
				GUILayout.Label ("The game ended.\nState: " + endGameState, GUILayout.MinWidth (100));
				
			}
			
		#endregion GUI
		
	#endregion UnityMethods
	
	void CallRequestRefresh () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
	
		GetComponent<NetworkView>().RPC ("RequestRefresh", RPCMode.Server, Network.player);
		
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
		public void CFlagTaken (bool isBlue) {
		
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
		public void CFlagReturned (bool isBlue) {
		
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
		public void CFlagReturnedSelf (bool isBlue) {
		
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
		public void CFlagCaptured (bool isBlue) {
		
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
			} else {
				Debug.Log ("Red scored.");
				redScore++;
			}
			
			blueFlagHome = true;
			redFlagHome = true;
			
		//	PlaySound
			
		}
	
	#endregion Flags
	
	[RPC]
	public void TheEnd (int gameState) {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
		
		endGameState = (SGameMan.EndGameType)gameState;
		
		showEndGame = true;
		
		Invoke ("StopEndGame", 5f);
		
	}
	
	void StopEndGame () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
	
		showEndGame = false;
		
		CallRequestRefresh ();
		
	//	Application.LoadLevel (S_NetMan.levelName);
		
	}
	
		
}
