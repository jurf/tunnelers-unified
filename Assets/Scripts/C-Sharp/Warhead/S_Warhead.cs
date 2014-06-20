using UnityEngine;

[RequireComponent (typeof (NetworkView))]

public class S_Warhead : MonoBehaviour {

	public bool left;
	public bool onLeftDown;
	public bool onLeftUp;
	
	bool previousLeft;

	void Awake () {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
		
	}
	
	void OnServerInitialized () {
		
		enabled = true;
			
	}
	
	void UpdateEvents () {
	
		if (left && left != previousLeft) {
			onLeftDown = true;
			onLeftUp = false;
		} else if (!left && left != previousLeft) {
			onLeftDown = false;
			onLeftUp = true;
		} else {
			onLeftDown = false;
			onLeftUp = false;
		}
		
		previousLeft = left;
		
	}
	
	[RPC]
	public void UpdateClientMouse (bool l) {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
	
		left = l;
	
		UpdateEvents ();
		
	}

}

