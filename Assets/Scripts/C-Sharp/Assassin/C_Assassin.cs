//
//  C_Assassin.cs is part of Tunnelers: Unified
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
[RequireComponent (typeof (S_Assassin))]

[AddComponentMenu ("Network/Assassin")]

public class C_Assassin : MonoBehaviour {

	public S_Assassin sscript;
	
	[RPC]
	public void Freeze () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
		
		Debug.Log ("Freezing a tank.", gameObject);
	
		sscript.parent.tank.gameObject.SetActive (false);
		sscript.parent.turret.gameObject.SetActive (false);
		Instantiate (sscript.explosion, sscript.parent.tank.transform.position, sscript.parent.tank.transform.rotation);
		
	}

}
