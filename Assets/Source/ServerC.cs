//
//  ServerC.cs is part of Tunnelers: Unified
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
using System.Text.RegularExpressions;

[RequireComponent (typeof (NetworkView))]

[AddComponentMenu ("Network/Menu")]

public class ServerC : MonoBehaviour {

	#region Variables

	public string version = "Tunnelers Beta";

	public string masterServerIP;
	public string facilitatorIP;
	
	public bool overrideIP;
	public bool OverrideIP {
		get {
			return overrideIP;
		}
		set {
			if (value != overrideIP) {
				overrideIP = value;
				SetIP ();
			}
		}
	}
	
	public string overMaster = "127.0.0.1";
	public string OverMaster {
		get {
			return overMaster;
		}
		set {
			overMaster = value;
			SetIP ();
		}
	}
	
	public int customServerPort = 25002;
	string userPort;

	public string typeName = "tunnelers_unified";
	public string gameName = "A cool game name";
//	public string roomComment = "";
	public int level;

	public bool server;
	public bool startServer;

	public enum Tabs {
		Client,
		Server,
		Options,
		Exit
	}
	
	public Tabs currentTab;
	
	public HostData[] data;
	
	public int serversMinWidth = 200;
	public int serversMinHeight = 200;

	public Rect windowRect;
	
	public string serverNotice = "Pressing 'Launch Server' will make this instance (window) a dedicated server. \n You will not be able to play. To play, you have to open a new instance (window) of Tunnelers, with it connect to this server. That is the way authoritative networking works. Stop fretting.";
	
	public Rect noticeRect;
	public bool showNotice;
	
	public static string playerNickname = "TestDude";
	public bool rememberNick;
	public bool showNameDialog = true;
	public Rect nameRect;

	public bool MuteSound {
		get {
			return AudioListener.pause;
		}
		set {
			AudioListener.pause = value;
			PlayerPrefs.SetInt ("MuteSound", System.Convert.ToInt32 (value));
		}
	}
	
	public Vector2 serverScroll;

	public string[] clientServer = {"Client", "Server", "Options", "Exit"};
	public string[] clientServerWithoutExit = {"Client", "Server", "Options"};
	
	public GUIStyle divider;

	public bool isWebPlayer;

	#endregion Variables
	
	#region UnityMethods
		
		void Awake () {
		
			SetIP ();
			userPort = customServerPort.ToString ();
			
			playerNickname = playerNickname + Random.Range (0,100);
			
			rememberNick = System.Convert.ToBoolean (PlayerPrefs.GetInt ("RememberNickname", 0));

			if (rememberNick) {
				playerNickname = PlayerPrefs.GetString ("PlayerNickname", playerNickname);
			}
			
			isWebPlayer = (Application.platform == RuntimePlatform.OSXWebPlayer ||
		                    Application.platform == RuntimePlatform.WindowsWebPlayer);
			
			AudioListener.pause = System.Convert.ToBoolean (PlayerPrefs.GetInt ("MuteSound", 0));

		/*	if (!server) {
				MasterServer.RequestHostList (typeName);
			}	*/
			
		}

	void Start () {

		RefreshGameList ();

	}
	
	#region GUI

	void OnGUI () {
	
		GUILayout.BeginArea (new Rect (0, 0, Screen.width, Screen.height));
			GUILayout.BeginVertical ();
				GUILayout.FlexibleSpace ();		
				GUILayout.BeginHorizontal ();
					GUILayout.FlexibleSpace ();
					GUILayout.Label (version, "box");
				GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();
		GUILayout.EndArea ();
	
		if (!Network.isClient && !Network.isServer) {
			
			windowRect.center = new Vector2 (Screen.width / 2, Screen.height / 2);
			
			if (showNameDialog) {
				nameRect.center = new Vector2 (Screen.width / 2, Screen.height / 2);
				nameRect = GUILayout.Window (2, nameRect, NameWindow, "Enter name:");
				return;
			}
			
			windowRect = GUILayout.Window (0, windowRect, MainWindow, "Main Menu");
			
			if (showNotice)
				noticeRect = GUILayout.Window (1, noticeRect, NoticeWindow, "Warning");
				
			

		} else if (!Network.isClient && Network.isServer) {
		
			if (GUILayout.Button ("Shut Down Server")) {
				Network.Disconnect ();
				MasterServer.UnregisterHost ();
			}
			
		} else if (Network.isClient && !Network.isServer) {
		
			if (GUILayout.Button ("Disconnect")) {
				Network.Disconnect ();
			}
			
		}
		
	}
	
	void MainWindow (int windowID) {

		GUILayout.BeginVertical ();
		
		if (!Network.isClient && !Network.isServer) {
		
			//server = GUILayout.Toggle (server, "Server?");
//					server = System.Convert.ToBoolean (GUILayout.Toolbar (System.Convert.ToInt32 (server), clientServer));
			currentTab = !isWebPlayer
				? (Tabs)GUILayout.Toolbar ((int)currentTab, clientServer)
				: (Tabs)GUILayout.Toolbar ((int)currentTab, clientServerWithoutExit);
		}

		if (currentTab == Tabs.Server) {
		
			GUILayout.Label ("Server name:");
			
			GUILayout.Space (5);
			
			gameName = GUILayout.TextField (gameName, 30);
			//roomComment = GUILayout.TextArea (roomComment, 200);
			
			GUILayout.Label ("Select a level:");
			
			string[] levels = GetLevelNames ();		
			
			level = GUILayout.SelectionGrid (level, levels, 1);
			
			GUILayout.Label ("Choose a port (don't touch if not sure):");
			userPort = GUILayout.TextField (userPort, 7);
			userPort = Regex.Replace (userPort, @"[^0-9 ]", "");
			
			GUILayout.Box ("", divider, GUILayout.Height (2));
			
			GUILayout.BeginHorizontal ();
			
			if (GUILayout.Button ("Launch Server")) {
			
				startServer = true;
				string levelToLoad = GetComponent <ReadSceneNames> ().scenes[level + 2];
				SNetMan.levelName = levelToLoad;
				Application.LoadLevel (levelToLoad);
				
			}
			
			showNotice |= GUILayout.Button ("?");
			
			GUILayout.EndHorizontal ();
			
		} else if (currentTab == Tabs.Client) {
			
			data = MasterServer.PollHostList();
				
			// Go through all the hosts in the host list
			GUILayout.Label ("Current games:");
				
			if (data.Length == 0) {
				GUILayout.Label ("No games found.");
			}
			
			serverScroll = GUILayout.BeginScrollView (serverScroll, GUILayout.MinWidth (serversMinWidth),  GUILayout.MinHeight (serversMinHeight));
			
			foreach (HostData element in data) {
			
				GUILayout.Space (5);
				string serverName = "Name: " + element.gameName;
				string connectedPlayers = (element.connectedPlayers - 1) + " out of " + element.playerLimit + " players connected.";
				
				GUILayout.Box ("", divider, GUILayout.Height (2));
				GUILayout.Label (serverName);
				GUILayout.Label (connectedPlayers);
				//GUILayout.Space (5);
										
				string hostInfo;					
				hostInfo = "IP: [";
					
				foreach (string host in element.ip) 
					hostInfo = hostInfo + host + ":" + element.port + " ";
					
				hostInfo = hostInfo + "]";	
										
				GUILayout.Label (hostInfo);								
				GUILayout.Space (5);						
				GUILayout.Label ("Map: " + element.comment);
				
				GUILayout.Space (5);
					
				if (GUILayout.Button ("Connect to this server")) {
					
					// Connect to HostData struct, internally the correct method is used (GUID when using NAT).
					SNetMan.levelName = element.comment;
					Network.Connect (element);	
								
				}
				
				GUILayout.Box ("", divider, GUILayout.Height (2));
				
			}
			
			GUILayout.EndScrollView ();
			
		//	GUILayout.FlexibleSpace();
			
			if (GUILayout.Button ("Refresh server list")) {
				
				RefreshGameList ();
					
			}

		} else if (currentTab == Tabs.Options) {

			OverrideIP = GUILayout.Toggle (OverrideIP, "Override master server IP");
			overMaster = GUILayout.TextField (overMaster, 30);
			
			MuteSound = GUILayout.Toggle (MuteSound, "Mute sound");

		} else {
		
			GUILayout.Label ("Really exit");
			
			if (!isWebPlayer) {
				if (GUILayout.Button ("Yes"))
					Application.Quit ();
			}

		}
		
		
		
		GUILayout.EndVertical ();

	}
	
	void NoticeWindow (int windowID) {
	
		GUILayout.Label (serverNotice);
			
		GUILayout.Space (5);
		
		showNotice &= !GUILayout.Button ("Alright, I understand.");
		
	}
	
	void NameWindow (int windowID) {
	
		GUI.BringWindowToFront (2);
	
		GUILayout.BeginHorizontal ();
	
			GUILayout.Label ("Name: ", GUILayout.MinWidth (75));
			
			playerNickname = GUILayout.TextField (playerNickname, 25, GUILayout.MinWidth (150));
			
		GUILayout.EndHorizontal ();

		rememberNick = GUILayout.Toggle (rememberNick, "Remember name?");
		
		if (GUILayout.Button ("OK")) {
			Debug.Log ("Name entered.");
			
			PlayerPrefs.SetInt ("RememberNickname", System.Convert.ToInt32 (rememberNick));

			if (rememberNick) {
				PlayerPrefs.SetString ("PlayerNickname", playerNickname);
			} else {
				PlayerPrefs.DeleteKey ("PlayerNickname");
			}
			
			showNameDialog = false;
		}
		
	}
	
	#endregion GUI	
	
		void OnLevelWasLoaded (int levelNum) {
		
			if (startServer && levelNum != 0) {
			
				StartServer ();
					startServer = false;
			
			}
		
		}
		
		void OnServerInitialized () {
		
		    Debug.Log ("Server Initializied");
		    
		}
		
		void OnDisconnectedFromServer (NetworkDisconnection info) {
	
			Debug.Log ("Disconnected from server: " + info);
			SNetMan.levelName = "";
			Application.LoadLevel ("main");
			RefreshGameList (); 
	
		}
			
	#endregion UnityMethods
	
	void StartServer () {
		
		customServerPort = int.Parse (userPort);
		
		// Use NAT punchthrough if no public IP present
		Network.InitializeServer (32, customServerPort, !Network.HavePublicAddress());
		MasterServer.RegisterHost (typeName, gameName, SNetMan.levelName);
		
	}

	void SetIP() {
	
		Debug.Log ("Setting the IP of the Master Server and Facilitator.");
		
		if (!overrideIP) {
			MasterServer.ipAddress = MasterServerIP.masterServerIP;
			MasterServer.port = 23466;
			Network.natFacilitatorIP = MasterServerIP.facilitatorIP;
			Network.natFacilitatorPort = 50005;
		} else {
			MasterServer.ipAddress = overMaster;
			MasterServer.port = 23466;
			Network.natFacilitatorIP = overMaster;
			Network.natFacilitatorPort = 50005;
		}
	}

	void RefreshGameList () {
		MasterServer.RequestHostList (typeName);
		data = MasterServer.PollHostList ();
	}
	
	public void DontDestroy () {
	
		DontDestroyOnLoad (transform.gameObject);
	
	}
	
	string[] GetLevelNames () {
		
		ReadSceneNames maps = GetComponent <ReadSceneNames> ();
		
		var levels = new string[maps.scenes.Length - 2];
		
		for (int i = 2; i < maps.scenes.Length; i++) {
			
			levels[i - 2] = maps.scenes[i];
			
		}
		
		return levels;
	
	}
	
}
