using UnityEngine;

[RequireComponent (typeof (NetworkView))]
[RequireComponent (typeof (C_FlagMan))]

public class S_FlagMan : MonoBehaviour {

	public bool home = true;
	public bool Home {
		get {
			return home;
		}
		set {
			bool diff = home != value;
			home = value;
			if (value) {
				transform.position = spawn.position;
				transform.rotation = spawn.rotation;
				if (diff)
					networkView.RPC ("HomeTrue", RPCMode.All);
				return;
			}
			if (diff)
				networkView.RPC ("HomeFalse", RPCMode.All);
		}
	}			

	public Transform spawn;
	public GameObject carrier;
	public GameObject Carrier {
		get {
			return carrier;
		}
		set {
			bool diff = carrier != value;
			carrier = value;
			GetComponent <PosAsObjectC> ().theObject = value;
			isCarrier = carrier;
			
			if (value) {
				if (diff)				
					networkView.RPC ("CarrierTrue", RPCMode.All, value.transform.parent.gameObject.GetComponent <PlayerMan> ().id);
				return;
			}
			if (diff)
				networkView.RPC ("CarrierFalse", RPCMode.All);
		}
	}
	
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

		if (Time.time - lastTouch > timeToReturn && !Home && !Carrier) {
			Home = true;
			gameMan.FlagReturnedSelf (isBlue);
		}
		
		if (Carrier) {
			Home = false;
			lastTouch = Time.time;
		}
		
	}
	
	void OnTriggerStay (Collider other) {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
	
		if (other.tag == "Tank") {
		
			PlayerMan otherType = other.transform.parent.GetComponent <PlayerMan> ();
			bool otherIsMy = otherType.IsMyTeam (isBlue);
			
			if (!Carrier && !otherIsMy) { //Flag taken
			
				Carrier = other.gameObject;
				Home = false;
				other.gameObject.SendMessage ("GotFlag", SendMessageOptions.DontRequireReceiver);
				gameMan.FlagTaken (isBlue);
				
			} else if (!Carrier && !Home && otherIsMy) { //Flag returned
			
				Home = true;
				gameMan.FlagReturned (isBlue);
				
			} else if (!Carrier && Home && otherIsMy) { //Flag captured
			
				if (otherFlag.Carrier == other.gameObject) {
					
					otherFlag.Carrier = null;
					otherFlag.Home = true;
					gameMan.FlagCaptured (isBlue/*, otherFlag.isBlue, otherFlag.Carrier.transform.parent.gameObject.GetComponent <PlayerMan> ().owner*/);	
									
				}
				
			}
		
		}
	
	}
	
	[RPC]
	public void AnyoneHome (NetworkPlayer requester) {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
		
		Debug.Log ("Sending flag state to the newly connected player.");
		
		if (Carrier)
			networkView.RPC ("SetState", requester, Home, Carrier.transform.parent.gameObject.GetComponent <PlayerMan> ().id);
		else
			networkView.RPC ("SetState", requester, Home, -1);
		
	}
	
	public void DidICarryYou (GameObject me) {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
	
		if (me == Carrier) {
		
			Carrier = null;
			
		}
		
	}
	
		
	
	/*void OnSerializeNetworkView (BitStream stream) {
	
		if (stream.isWriting) {
		
			stream.Serialize (ref Home);
			stream.Serialize (ref isCarrier);
		
		}	
	
	}*/
	
}
