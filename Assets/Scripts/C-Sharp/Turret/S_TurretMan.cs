using UnityEngine;

public class S_TurretMan : MonoBehaviour {

	M_TankController controller;
	
	Quaternion toRotation;
	
	void Awake () {
		
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
		
	}
	
	void Start () {
		
		if (Network.isServer) {
			controller = GetComponent <M_TankController> ();
		}
		
	}
	
	void Update () {
		
		if (Network.isClient || (!Network.isServer && !Network.isClient)) {
			enabled = false;
			return; //Get lost, this is the server-side!
		}
		
		//Debug.Log("Processing clients movement commands on server");
		controller.Rotate (toRotation);
		
	}
	
	/**
     * The client calls this to notify the server about new motion data
     * @param	motion
     */
	
	[RPC]
	public void UpdateClientRotation (float toRotY) {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
		
		toRotation = Quaternion.Euler (new Vector3 (0,toRotY,0));
		
	}
}

