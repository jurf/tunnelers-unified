using UnityEngine;
using System.Collections;
using Assets.Scenes.Scripts;
using System.Text.RegularExpressions;

public class ServerC : MonoBehaviour {

	#region Variables
		
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

		public string typeName;
		public string gameName = "A cool game name";
		public string roomComment = "A cool game description.";
		public int level;
	
		public bool server = false;
		public bool startServer;
	
		public HostData[] data;
		
		public int serversMinWidth = 200;
		public int serversMinHeight = 200;
	
		public Rect windowRect;
		
		public string serverNotice = "Pressing 'Launch Server' will make this instance (window) a dedicated server. \n You will not be able to play. To play, you have to open a new instance (window) of Tunnelers, with it connect to this server. That is the way authoritative networking works. Stop fretting.";
		
		public Rect noticeRect;
		public bool showNotice;
		
		public Vector2 serverScroll;
	
		public string[] clientServer = new string[] {"Client", "Server"};
		
		public GUIStyle divider;
	
	#endregion Variables
	
	#region UnityMethods
		
		void Awake () {
		
			SetIP ();
			userPort = customServerPort.ToString ();
			
		/*	if (!server) {
				MasterServer.RequestHostList (typeName);
			}	*/
			
		}
		
		#region GUI
	
			void OnGUI () {
			
				if (!Network.isClient && !Network.isServer) {
				
					windowRect.center = new Vector2 (Screen.width / 2, Screen.height / 2);
							
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
				
				OverrideIP = GUILayout.Toggle (OverrideIP, "Override master server IP?");
				
				if (OverrideIP)
					overMaster = GUILayout.TextField (overMaster, 30);
				
				if (!Network.isClient && !Network.isServer) {
				
					//server = GUILayout.Toggle (server, "Server?");
					server = System.Convert.ToBoolean (GUILayout.Toolbar (System.Convert.ToInt32 (server), clientServer));
				
				}
		
				if (server == true) {
				
					GUILayout.Label ("Server details:");
					
					GUILayout.Space (5);
					
					gameName = GUILayout.TextField (gameName, 30);
					roomComment = GUILayout.TextArea (roomComment, 200);
					
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
						string levelToLoad = Game.Levels[level + 2];
						S_LevelName.levelName = levelToLoad;
						Application.LoadLevel (levelToLoad);
						
					}
					
					if (GUILayout.Button ("?"))
						showNotice = true;
					
					GUILayout.EndHorizontal ();
					
				} else {
					
					data = MasterServer.PollHostList();
						
					// Go through all the hosts in the host list
					GUILayout.Label ("Current games:");
						
					if (data.Length == 0) {
						GUILayout.Label ("No games found.");
					}
					
					serverScroll = GUILayout.BeginScrollView (serverScroll, GUILayout.MinWidth (serversMinWidth),  GUILayout.MinHeight (serversMinHeight));
					
					foreach (HostData element in data) {
					
						GUILayout.Space (5);
						string name = "Name: " + element.gameName;
						string connectedPlayers = element.connectedPlayers + " out of " + element.playerLimit + " players connected.";
						
						GUILayout.Box ("", divider, GUILayout.Height (2));
						GUILayout.Label (name);
						GUILayout.Label (connectedPlayers);
						//GUILayout.Space (5);
												
						string hostInfo;					
						hostInfo = "IP: [";
							
						foreach (string host in element.ip) 
							hostInfo = hostInfo + host + ":" + element.port + " ";
							
						hostInfo = hostInfo + "]";	
												
						GUILayout.Label (hostInfo);								
						GUILayout.Space (5);						
						GUILayout.Label ("Description: " + element.comment);
						
						GUILayout.Space (5);
							
						if (GUILayout.Button ("Connect to this server")) {
							
							// Connect to HostData struct, internally the correct method is used (GUID when using NAT).
							Network.Connect (element);	
										
						}
						
						GUILayout.Box ("", divider, GUILayout.Height (2));
						
					}
					
					GUILayout.EndScrollView ();
					
				//	GUILayout.FlexibleSpace();
					
					if (GUILayout.Button ("Refresh server list")) {
						
						MasterServer.RequestHostList(typeName);
						data = MasterServer.PollHostList();
							
					}
				}
				
				GUILayout.EndVertical ();

			}
			
			void NoticeWindow (int windowID) {
			
				GUILayout.Label (serverNotice);
					
				GUILayout.Space (5);
				
				if (GUILayout.Button ("Alright, I understand."))
					showNotice = false;
				
			}
			
		#endregion GUI	
	
		void OnLevelWasLoaded (int level) {
		
			if (startServer && level != 0) {
			
				StartServer ();
					startServer = false;
			
			}
		
		}
		
		void OnServerInitialized() {
		
		    Debug.Log ("Server Initializied");
		    
		}
		
		void OnDisconnectedFromServer (NetworkDisconnection info) {
	
		Debug.Log ("Disconnected from server: " + info);
		S_LevelName.levelName = "";
		Application.LoadLevel ("main");
	
	}
			
	#endregion UnityMethods
	
	void StartServer () {
		
		customServerPort = int.Parse (userPort);
		
		// Use NAT punchthrough if no public IP present
		Network.InitializeServer(32, customServerPort, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName, roomComment);
		
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
	
	public void DontDestroy () {
	
		DontDestroyOnLoad (transform.gameObject);
	
	}
	
	string[] GetLevelNames () {
		
		
		string[] levels = new string[Game.Levels.Length - 2];
		
		for (int i = 2; i < Game.Levels.Length; i++) {
			
			levels[i - 2] = Game.Levels[i];
			
		}
		
		return levels;
	
	}
	
}
