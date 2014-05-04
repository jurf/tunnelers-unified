using UnityEngine;

[AddComponentMenu ("Server/Tank Man")]
[RequireComponent (typeof (NetworkView))]

public class S_TankMan : MonoBehaviour {
	
	public M_TankController controller;
	
	public int horizontalMotion;
	public int verticalMotion;
	
	void Awake () {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
	
	}
	
	public void Update () {
	
		if (Network.isClient || (!Network.isServer && !Network.isClient)) {
			enabled = false;
			return;
		}

		controller.Move (horizontalMotion, verticalMotion);
		
	}
     
	[RPC]
	public void UpdateClientMotion (int hor, int vert) {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
	
		horizontalMotion = hor;
		verticalMotion = vert;
	
	}
}
