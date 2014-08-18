//
//  C_FlagMan.cs is part of Tunnelers: Unified
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
[RequireComponent (typeof (S_FlagMan))]

[AddComponentMenu ("Network/Flag Man")]

public class C_FlagMan : MonoBehaviour {
	
	public S_FlagMan server;
	
	public GameObject carrier;
	public GameObject Carrier {
		get {
			return carrier;
		}
		set {
			carrier = value;
			GetComponent <PosAsObjectC> ().theObject = value;
		}
	}
	
	public bool home = true;
	public bool Home {
		get {
			return home;
		}
		set {
			home = value;
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
		
		networkView.RPC ("AnyoneHome", RPCMode.Server,Network.player);
	
	}
	
	void OnConnectedToServer () {
	
		enabled = true;
		
	}
	
	[RPC]
	void SetState (bool isHome, int carrierID) {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
	
		if (isHome)
			HomeTrue ();
		else
			HomeFalse ();
		
		if (carrierID > -1)
			CarrierTrue (carrierID);
			
	}
	
	[RPC]
	public void CarrierTrue (int playerID) {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
	
		Debug.Log ("Got a carrier, checking player index.", gameObject);
	
		GameObject[] tanks = GameObject.FindGameObjectsWithTag ("Tank");
		
		Debug.Log ("Found " + tanks.Length + " tanks. Checking each and every one of them.", gameObject);
		
		foreach (GameObject go in tanks) {
		
			if (go.transform.parent.gameObject.GetComponent <PlayerMan> ().id == playerID) {
				Debug.Log ("Found the one.");
				Carrier = go;
				return;
			}
			
		}
		
		Debug.LogError ("Didn't find suitable tank instance.", gameObject);
		
	}
	
	[RPC]
	public void CarrierFalse () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
	
		Debug.Log ("No carrier anymore.", gameObject);

		Carrier = null;
		
	}
	
	[RPC]
	public void HomeTrue () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
	
		Debug.Log ("Flag home.", gameObject);
	
		Home = true;
		
	}
	
	[RPC]
	public void HomeFalse () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
		
		Debug.Log ("Flag away.", gameObject);
		
		Home = false;
		
	}
	
	//TODO watch http://www.youtube.com/watch?v=PaOe2Ru9KMk&t=43
		
}
