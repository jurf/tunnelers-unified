using UnityEngine;
using System.Collections;

public class C_Warhead : MonoBehaviour {

	public PlayerMan parent;

	bool lastLeft;
	
	void Awake () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
		
	}
	
	void OnConnectedToServer () {
	
		enabled = true;
		
	}
	
	void Update () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
	
		if (parent.GetOwner () == null || parent.GetOwner () != Network.player) {
			return;
		}
	
		lastLeft = Input.GetMouseButton (0);

		networkView.RPC ("UpdateClientMouse", RPCMode.Server, lastLeft);
	
	}
}
