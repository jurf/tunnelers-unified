//
//  CFlagMan.cs is part of Tunnelers: Unified
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

[AddComponentMenu ("Network/Flag Man")]

public class CFlagMan : MonoBehaviour {

	// The server side
	public SFlagMan server;

	// Who's carrying us?
	GameObject carrier;
	public GameObject Carrier {
		get {
			return carrier;
		}
		set {
			// Once we know who it is, we need to move like them
			carrier = value;
			GetComponent <PosAsObjectC> ().theObject = value;
		}
	}

	// Are we home?
	bool home = true;
	public bool Home {
		get {
			return home;
		}
		set {
			home = value;
			// If we are going home, we need to get there
			if (value) {
				transform.position = server.spawn.position;
				transform.rotation = server.spawn.rotation;
			}
		}
	}
	
	void Awake () {

		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}

		// We need to ask what's the status on the server side, so we can act accordingly
		// TODO no need to ask, the server recieves an event when we connect
		// and can automatically send us the status
		GetComponent <NetworkView> ().RPC ("AnyoneHome", RPCMode.Server,Network.player);

	}
	
	void OnConnectedToServer () {

		// Time to go!
		enabled = true;
		
	}

	// Called by the server the first time we connect
	[RPC]
	void SetState (bool isHome, int carrierID) {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}

		// We're going to fake RPC calls in order to get into sync
		if (isHome)
			HomeTrue ();
		else
			HomeFalse ();

		// The ID is our own identification in PlayerMan, which is always larger than -1
		if (carrierID > -1)
			CarrierTrue (carrierID);
			
	}
	
	[RPC]
	public void CarrierTrue (int playerID) {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}

		// We don't get an instance, only an ID, so we have to make do with that
		Debug.Log ("Got a carrier, checking player index.", gameObject);
		// Get a list of all tanks in-game
		GameObject[] tanks = GameObject.FindGameObjectsWithTag ("Tank");
		// Report
		Debug.Log ("Found " + tanks.Length + " tanks. Checking each and every one of them.", gameObject);
		// Go through the list
		foreach (GameObject go in tanks) {
			// If the current tank is correct, assign it and exit
			if (go.transform.parent.gameObject.GetComponent <PlayerMan> ().id == playerID) {
				Debug.Log ("Found the one.");
				Carrier = go;
				return;
			}
			
		}
		// If we didn't find the tank, report it
		// This is bad
		Debug.LogError ("Didn't find suitable tank instance.", gameObject);
		
	}
	
	[RPC]
	public void CarrierFalse () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}

		Debug.Log ("No carrier anymore.", gameObject);
		// Disassign the carrier
		Carrier = null;
		
	}
	
	[RPC]
	public void HomeTrue () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
	
		Debug.Log ("Flag home.", gameObject);
		// Fly home!
		Home = true;
		
	}
	
	[RPC]
	public void HomeFalse () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
		
		Debug.Log ("Flag away.", gameObject);
		// We're no longer home
		Home = false;
		
	}
	
	//TODO watch http://www.youtube.com/watch?v=PaOe2Ru9KMk&t=43
		
}
