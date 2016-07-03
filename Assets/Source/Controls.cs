// vim:set ts=4 sw=4 sts=4 noet:
//
//  ControllerBase.cs is part of Tunnelers: Unified
//  <https://github.com/VacuumGames/tunnelers-unified/>.
//
//  Copyright (c) 2015-2016 Juraj Fiala <doctorjellyface@riseup.net>
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
using UnityEngine.Networking;

public class Controls: NetworkBehaviour {

	// Our controllers
	IMovable <sbyte> tankCtrl;
	IRotatable turretCtrl;

	// Store the input
	sbyte hMotion;
	sbyte vMotion;
	Vector2 mousePos;

	void Awake () {

		// Initialize references between objects
		tankCtrl = GetComponent <IMovable <sbyte>> ();
		turretCtrl = GetComponent <IRotatable> ();

		//NetworkServer.Spawn (tankCtrl.gameObject);

	}

	void Update () {

		// Stop if we aren't the local player
		// We don't want to move all the tanks
		if (!isLocalPlayer)
			return;

		// Read the input
		hMotion = (sbyte) Input.GetAxisRaw ("Horizontal");
		vMotion = (sbyte) Input.GetAxisRaw ("Vertical");
		mousePos = Input.mousePosition;

		// Pass it to the controllers
		tankCtrl.Move (hMotion, vMotion);
		turretCtrl.Rotate (mousePos);

	}

}
