//
//  PlayerMan.cs is part of Tunnelers: Unified
//  <https://github.com/VacuumGames/Tunnelers-Unified/>.
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

[AddComponentMenu ("Network/Player Man")]

public class PlayerMan : MonoBehaviour {

	public NetworkPlayer owner;
	
	public int id;
	
	public C_TankMan tank;
	public C_TurretMan turret;
	public S_LifePoints life;
	
	public TextMesh name;
	
//	too complicated for now
/*
	public enum Team {
		
		Red,
		Blue,
		Green
		
	}
	
	static public Team GetTeam (int num) {
	
		return (Team) num;
	
	}
	
*/
	
	public enum Tank {
		
		DefaultTank
		
	}
	
//	public Team team;
	public bool team;
	public Tank tankType;
	
	public bool IsMyTeam (bool otherTeam) {
		
		return otherTeam == team ? true : false;
		
	}
	
	public bool IsMyBase (string teamTag) {
		
		/*	if (tag == team + "Base") {
			return true;
		}
		
		return false;	*/
		
		if (team && teamTag == "BlueBase")
			return true;
			
		if (!team && teamTag == "RedBase")
			return true;
			
		return false;
		
	}
	
	void Awake () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
	
		tank.enabled = false;
		turret.enabled = false;
	
	}
	
	[RPC]
	public void SetOwner (NetworkPlayer player) {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
		
		Debug.Log("Setting the owner.");
		owner = player;
		
		if (player == Network.player){
			//So it just so happens that WE are the player in question,
			//which means we can enable this control again
			enabled = true;
			tank.GetComponent <C_TankMan> ().enabled = true;
			turret.GetComponent <C_TurretMan> ().enabled = true;
			GameObject.Find ("CameraFollow").GetComponent <PosAsObjectC> ().theObject = tank.gameObject;
		} else {
			//Disable a bunch of other things here that are not interesting:
			if (GetComponent <Camera> ()) {
				GetComponent <Camera> ().enabled = false;
			}
			
			if (GetComponent <AudioListener> ()) {
				GetComponent <AudioListener> ().enabled = false;
			}
			
			if (GetComponent <GUILayer> ()) {
				GetComponent <GUILayer> ().enabled = false;
			}
		}
	}
	
	[RPC]
	public void SetID (int identificationNumber) {
	
		id = identificationNumber;
		
	}
	
	[RPC]
	public void SetName (string myNameBro) {
	
		name.text = myNameBro;
		
	}
	
	[RPC]
	public NetworkPlayer GetOwner () {
		return owner;
	}
}
