//
//  SFlagMan.cs is part of Tunnelers: Unified
//  <https://github.com/VacuumGames/tunnelers-unified/>.
//
//  Copyright (c) 2014 Juraj Fiala<doctorjellyface@riseup.net>
//
//  Tunnelers: Unified is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  Tunnelers: Unified is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with Tunnelers: Unified.  If not, see <http://www.gnu.org/licenses/>.
//

using UnityEngine;

[RequireComponent (typeof (NetworkView))]
[RequireComponent (typeof (CFlagMan))]

[AddComponentMenu ("Network/Flag Man")]

public class SFlagMan : MonoBehaviour {

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
	public SFlagMan otherFlag;
	public Vector3 addToHeight;
	
//	public C_PlayerMan.Team flagTeam;
	public bool isBlue;
	
	public SGameMan gameMan;
	
	void Awake () {
		
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}

	}
	
	void OnServerInitialized () {
	
		enabled = true;
		
		gameMan = GameObject.FindGameObjectWithTag ("GameMan").GetComponent <SGameMan> ();
		
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
