//
//function Start () {
//
//	LaunchServer ();
//
//}
//
//function LaunchServer () {
//	Network.incomingPassword = "";
//	var useNat = !Network.HavePublicAddress();
//	Network.InitializeServer(32, 25000, useNat);
//}

var typeName : String;
var gameName : String;

var server : boolean;

var player : GameObject;

function Awake () {
	ResetIP ();
	if (!server) {
		MasterServer.RequestHostList(typeName);
	}
}

function OnGUI() {
	if (server) {
		if (!Network.isClient && !Network.isServer) {
			if (GUILayout.Button ("Start Server")) {
				// Use NAT punchthrough if no public IP present
				Network.InitializeServer(32, 25002, !Network.HavePublicAddress());
				MasterServer.RegisterHost(typeName, gameName, "l33t game for all");
			}
		}
	} else {
		var data : HostData[] = MasterServer.PollHostList();
		if (data.Length == 0) Debug.Log ("NO SERVERS!");
		// Go through all the hosts in the host list
		for (var element in data) {
			GUILayout.BeginHorizontal();	
			var name = element.gameName + " " + element.connectedPlayers + " / " + element.playerLimit;
			GUILayout.Label(name);	
			GUILayout.Space(5);
			var hostInfo;
			hostInfo = "[";
			for (var host in element.ip) 
				hostInfo = hostInfo + host + ":" + element.port + " ";
			hostInfo = hostInfo + "]";
			GUILayout.Label(hostInfo);	
			GUILayout.Space(5);
			GUILayout.Label(element.comment);
			GUILayout.Space(5);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Connect")) {
				// Connect to HostData struct, internally the correct method is used (GUID when using NAT).
				Network.Connect(element);			
			}
		}
	}
}

function OnServerInitialized() {
    Debug.Log("Server Initializied");
    
}

function OnConnectedToServer() {
    SpawnPlayer();
}

function SpawnPlayer(){
    Network.Instantiate (player, Vector3 (0,1,0), Quaternion.identity, 0);
}

function ResetIP() {
	MasterServer.ipAddress = "127.0.0.1";
	MasterServer.port = 23466;
	Network.natFacilitatorIP = "127.0.0.1";
	Network.natFacilitatorPort = 50005;
}