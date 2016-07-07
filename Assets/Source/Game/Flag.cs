//
//  Flag.cs is part of Tunnelers: Unified
//  <https://github.com/VacuumGames/tunnelers-unified/>
//
//  Copyright (c) 2015 Juraj Fiala <doctorjellyface@riseup.net>
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

public class Flag : MonoBehaviour, IFlag {
	
	// Home, sweet home
	public Transform spawn;
	// Our team
	Team playerTeam;
	public Team PlayerTeam;
	// Time to wait before we go back home if we are alone
	public float timeToReturn;

	// Are we at home?
	bool home = true;
	public bool Home {
		get {
			return home;
		}
		set {
			home = value;
			// Are we going home?
			if (value) {
				// Yay!
				transform.position = spawn.position;
				transform.rotation = spawn.rotation;
			}
		}
	}
	
	// Time from last touch
	float lastTouch;
	// Who's carrying us?
	GameObject carrier;
	public GameObject Carrier {
		get {
			return carrier;
		}
		set {
			bool diff = carrier != value;
			carrier = value;
			if (carrier)
				// If we've get a carrier, we aren't home
				Home = false;
			// Check against diff to avoid false touches
			else if (diff)
				// If not, save the time as the last touch
				lastTouch = Time.time;

			// We now need to move with our carrier
			//moveWith.TheObject = value.transform;
		}
	}
	
	// The flag from the other team
	IFlag otherFlag;
	// The game manager we need to report to
	IGame game;
	// A componenet that will make us follow our carrier
	//IMoveWith moveWith;

	void Awake () {

		// Find other flag
		otherFlag = (IFlag) GameObject.FindGameObjectWithTag ("Flag").GetComponent (typeof (IFlag));

		// Set the component that's supposed to move us
		//moveWith = (IMoveWith) GetComponent (typeof (IMoveWith)); 

		// Find the game component

	}

	void Update () {

		// Go home if we waited long enough without a carrier
		if (!Home && !Carrier && Time.time - lastTouch > timeToReturn) {
			Home = true;
			game.FlagReturnedSelf (playerTeam);
		}

	}

	void OnTriggerStay (Component other) {

		// Is the other collider a tank?
		if (other.tag != "Tank")
			return;

		// Access the component which holds the team variable
		var otherType = (IPlayer) other.transform.parent.GetComponent (typeof (IPlayer));
		// Ask the tank if they're from our team
		bool otherIsOur = otherType.playerTeam == playerTeam;

		// If they aren't, and we haven't got a carrier already
		if (!Carrier && !otherIsOur) {
			// We're taken! Congratulations, you're now carrying us
			Carrier = other.gameObject;
			// Inform them that they've taken us
			other.gameObject.SendMessage ("GotFlag", SendMessageOptions.DontRequireReceiver);
			// Tell the game manager that we've been taken
			game.FlagTaken (playerTeam);

		// If they're ours, and no one is carrying us and we're away from home
		} else if (!Carrier && !Home && otherIsOur) {
			// We're saved! Go back home
			Home = true;
			// Tell the game manager we've been saved
			game.FlagReturned (playerTeam);

		// If we're home, and they're ours carrying the other flag
		} else if (!Carrier && Home && otherIsOur) {
			// Double-check with the other flag if it's really them
			if (otherFlag.Carrier == other.gameObject) {
				// If yes, let's free the from their burden and return them home
				otherFlag.Carrier = null;
				otherFlag.Home = true;
				// Tell the game manager the wonderful news
				game.FlagCaptured (playerTeam);

			}
		}

	}
}
