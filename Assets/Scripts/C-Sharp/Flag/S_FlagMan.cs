using UnityEngine;

[RequireComponent (typeof (NetworkView))]
[RequireComponent (typeof (C_FlagMan))]

public class S_FlagMan : MonoBehaviour {

	public bool home;

	public Transform spawn;
	public GameObject carrier;
	public NetworkPlayer carrierPlayer;
	public bool isCarrier;
	public float lastTouch;
	public float timeToReturn;
	public S_FlagMan otherFlag;
	public Vector3 addToHeight;
	
//	public C_PlayerMan.Team flagTeam;
	public bool isBlue;
	
	public S_GameMan gameMan;
	
	void Awake () {
		
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}

	}
	
	void OnServerInitialized () {
	
		enabled = true;
		
		gameMan = GameObject.FindGameObjectWithTag ("GameMan").GetComponent <S_GameMan> ();
		
	}
	
	void Update () {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
	
		isCarrier = carrier;
	
		if (Time.time - lastTouch > timeToReturn && !home && !carrier) {
			home = true;
			gameMan.FlagReturnedSelf (isBlue);
			networkView.RPC ("Home", RPCMode.All);
		}
		
		if (home) {
			transform.position = spawn.position;
			transform.rotation = spawn.rotation;
		}
		
		if (carrier) {
			home = false;
			transform.position = carrier.transform.position + addToHeight;
			transform.rotation = carrier.transform.rotation;
			lastTouch = Time.time;
		}
	
	}
	
	void OnTriggerEnter (Collider other) {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
	
		if (other.tag == "Tank") {
			C_PlayerMan otherType = other.transform.parent.GetComponent <C_PlayerMan> ();
			bool otherIsMy = otherType.IsMyTeam (isBlue);
			if (!carrier && !otherIsMy) {
				carrier = other.gameObject;
				other.gameObject.SendMessage ("GotFlag", SendMessageOptions.DontRequireReceiver);
				gameMan.FlagTaken (isBlue);
				networkView.RPC ("Taken", RPCMode.All, otherType.owner);
			} else if (!carrier && !home && otherIsMy) {
				home = true;
				gameMan.FlagReturned (isBlue);
				networkView.RPC ("Home", RPCMode.All);
			} else if (!carrier && home && otherIsMy) {
				if (otherFlag.carrier == other.gameObject) {
					otherFlag.home = true;
					gameMan.FlagCaptured (isBlue/*, otherFlag.isBlue*/, otherFlag.carrier.transform.parent.GetComponent <C_PlayerMan> ().owner);
					networkView.RPC ("Home", RPCMode.All);
					otherFlag.carrier = null;					
				}
			}
		
		}
	
	}
	
	/*void OnSerializeNetworkView (BitStream stream) {
	
		if (stream.isWriting) {
		
			stream.Serialize (ref home);
			stream.Serialize (ref isCarrier);
			stream.Serialize (ref carrierPlayer);
		
		}	
	
	}*/
	
}
