//
//  SFlagMan.cs is part of Tunnelers: Unified
//  <https://github.com/VacuumGames/tunnelers-unified/>.
//
//  Copyright (c) 2014-2015 Juraj Fiala <doctorjellyface@riseup.net>
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

	// Are we at home?
	bool home = true;
	public bool Home {
		get {
			return home;
		}
		set {
			// Is the new value different to the current one?
			bool diff = home != value;
			// Now that we know, we can safely assign the new value
			home = value;

			// Are we going home?
			if (value) {
				// Yay!
				transform.position = spawn.position;
				transform.rotation = spawn.rotation;
				// If it's new, we need to tell everybody
				if (diff)
					netView.RPC ("HomeTrue", RPCMode.All);
				return;
			}
			// If we aren't going home and the value is different, that means we were taken
			if (diff)
				// We need to tell everybody this too
				netView.RPC ("HomeFalse", RPCMode.All);
		}
	}			

	// Home, sweet home
	public Transform spawn;

	// Who's carrying us?
	GameObject carrier;
	public GameObject Carrier {
		get {
			return carrier;
		}
		set {
			// Do we have a different carrier than the last one?
			bool diff = carrier != value;
			// Now that we know, we can safely assign the new value
			carrier = value;
			// We now need to move with our carrier
			GetComponent <PosAsObjectC> ().theObject = value;
			isCarrier = carrier;
			// If we now have a carrier
			if (value) {
				// And if they're different than before
				if (diff)
					// We need to tell everyone
					netView.RPC ("CarrierTrue", RPCMode.All, value.transform.parent.GetComponent <PlayerMan> ().id);
				return;
			}
			// If we don't have a carrier anymore
			if (diff)
				// Everyone needs to know too
				netView.RPC ("CarrierFalse", RPCMode.All);
		}
	}
	
	public bool isCarrier;
	// Time from last touch
	public float lastTouch;
	// Time to wait before we go back home if we are alone
	public float timeToReturn;
	// The flag from the other team
	public SFlagMan otherFlag;
	public Vector3 addToHeight;
	
//	public C_PlayerMan.Team flagTeam;
	public bool isBlue;

	// The network view instance on our gameobject
	NetworkView netView;

	// The game manager we need to report to
	public SGameMan gameMan;
	
	void Awake () {
	
		if (Network.isClient) {
			enabled = false;
			return;
		}

		netView = GetComponent <NetworkView> ();

	}
	
	void OnServerInitialized () {

		// We only need to wake up once the game has started
		enabled = true;
		// That also counts for finding the game manager
		gameMan = GameObject.FindGameObjectWithTag ("GameMan").GetComponent <SGameMan> ();
	
	}
	
	void Update () {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}

		// Go home if we waited long enough without a carrier
		if (Time.time - lastTouch > timeToReturn && !Home && !Carrier) {
			Home = true;
			gameMan.FlagReturnedSelf (isBlue);
		}

		// Check if we've got a carrier
		if (Carrier) {
			// If yes, we aren't home
			Home = false;
			// Countdown to porting home
			lastTouch = Time.time;
		}
		
	}
	
	void OnTriggerStay (Collider other) {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}

		// Is the other collider a tank?
		if (other.tag == "Tank") {
			
			PlayerMan otherType = other.transform.parent.GetComponent <PlayerMan> ();
			// Ask the tank if they're from our team
			bool otherIsMy = otherType.IsMyTeam (isBlue);

			// If they aren't, and we haven't got a carrier already
			if (!Carrier && !otherIsMy) {
				// We're taken!
				// Congratulations, you're now carrying us
				Carrier = other.gameObject;
				// We're leaving home
				Home = false;
				// Inform them that they've taken us
				other.gameObject.SendMessage ("GotFlag", SendMessageOptions.DontRequireReceiver);
				// Tell the game manager that we've been taken
				gameMan.FlagTaken (isBlue);

			// If they're ours, and no one is carrying us _and_ we're away from home
			} else if (!Carrier && !Home && otherIsMy) {
				// We're saved!
				// Go back home
				Home = true;
				// Tell the game manager we've been saved
				gameMan.FlagReturned (isBlue);
			
			// If we're home, and they're ours carrying the other flag
			} else if (!Carrier && Home && otherIsMy) {
				// We have captured the flag! Woo-hoo!
				// Double-check with the other flag if it's really them
				if (otherFlag.Carrier == other.gameObject) {
					// If yes, let's free the from their burden and return them home
					otherFlag.Carrier = null;
					otherFlag.Home = true;
					// Tell the game manager the wonderful news
					gameMan.FlagCaptured (isBlue);

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

		// Some newcomer has asked us what state we're in
		Debug.Log ("Sending flag state to the newly connected player.");
		
		if (Carrier)
			netView.RPC ("SetState", requester, Home, Carrier.transform.parent.GetComponent <PlayerMan> ().id);
		else
			netView.RPC ("SetState", requester, Home, -1);
		
	}

	// This should be "Am I Carrying You"
	public void DidICarryYou (GameObject me) {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}

		// We're asked if we're carried by some tank which is about to die

		// If yes, we'll free them from their burden and let them sleep
		if (me == Carrier) {
			Carrier = null;
		}
		
	}
	
}
