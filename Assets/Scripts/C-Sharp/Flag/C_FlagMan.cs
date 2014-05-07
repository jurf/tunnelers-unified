using UnityEngine;

public class C_FlagMan : MonoBehaviour {
	
	public S_FlagMan server;
	
	public GameObject carrier;
	
	public bool home;
	
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
	
		if (home) {	
			transform.position = server.spawn.position;
		} else if (carrier && !home) {
			transform.position = carrier.transform.position;
		}
	
	}
	
	[RPC]
	public void Taken (int playerID) {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
	
		Debug.Log ("Flag taken, checking player index");
	
		GameObject[] tanks = GameObject.FindGameObjectsWithTag ("Tank");
		
		Debug.Log ("Found " + tanks.Length + " tanks. Checking each and every one of them.");
		
		foreach (GameObject go in tanks) {
		
			if (go.transform.parent.gameObject.GetComponent <PlayerMan> ().id == playerID) {
				Debug.Log ("Found the one.");
				carrier = go;
				return;
			}
			
		}
		
		Debug.LogError ("Didn't find suitable tank instance.");
		
	}
	
	[RPC]
	public void Home () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
	
		Debug.Log ("Flag home.");
	
		home = true;
		carrier = null;
		
	}
	
	[RPC]
	public void Dropped () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
	
		Debug.Log ("Flag dropped.");
	
		home = false;
		carrier = null;
		
	}
	
	//TODO watch http://www.youtube.com/watch?v=PaOe2Ru9KMk&t=43
		
}
