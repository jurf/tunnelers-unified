using UnityEngine;
using System.Collections;

public class C_Warhead : MonoBehaviour {

	public C_PlayerMan parent;

	bool lastLeft;
	bool lastMiddle;
	bool lastRight;
	
	void Awake () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
		
	}
	
	void Update () {
	
		if (parent.GetOwner () == null || parent.GetOwner () != Network.player) {
			return;
		}
	
		lastLeft = Input.GetMouseButton (0);
		lastMiddle =  Input.GetMouseButton (2);
		lastRight = Input.GetMouseButton (1);
	
		networkView.RPC ("UpdateClientMouse", RPCMode.Server, lastLeft, lastMiddle, lastRight);
	
	}
}
