var player : GameObject;

function Awake() {
	
	MasterServer.ipAddress = "127.0.0.1";
	MasterServer.port = 23466;
	Network.natFacilitatorIP = "127.0.0.1";
	Network.natFacilitatorPort = 50005;
	MasterServer.RequestHostList("Tunnelers");
}

function OnGUI() {
	if (!Network.isClient && !Network.isServer) {
		var data : HostData[] = MasterServer.PollHostList();
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
			GUILayout.EndHorizontal();	
		}
	}
}