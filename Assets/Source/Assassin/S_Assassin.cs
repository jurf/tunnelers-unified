//
//  S_Assassin.cs is part of Tunnelers: Unified
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
[RequireComponent (typeof (C_Assassin))]

[AddComponentMenu ("Network/Assassin")]

public class S_Assassin : MonoBehaviour {


	public PlayerMan parent;
	public GameObject explosion;
	public float wait = 3f;
	
	public void Assassinate () {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
		
		Debug.Log ("Assassinating.", gameObject);
		
		GameObject [] flags = GameObject.FindGameObjectsWithTag ("Flag");
		
		foreach (GameObject flag in flags) {
		
			flag.GetComponent <S_FlagMan> ().DidICarryYou (parent.tank.gameObject);
				
		}
		
		Stun ();
		CreateDiversion ();
		Invoke ("Kill", wait);
		
	}
	
	void Stun () {
	
		networkView.RPC ("Freeze", RPCMode.All);
		parent.tank.gameObject.SetActive (false);
		parent.turret.gameObject.SetActive (false);
		
	}
	
	void CreateDiversion () {
	
		Instantiate (explosion, parent.tank.transform.position, parent.tank.transform.rotation);
		
	}
	
	void Kill () {
	
		GameObject.Find ("ManMan").GetComponent <S_NetMan> ().NetworkDestroy (gameObject);
	
	}

}
