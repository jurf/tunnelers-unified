//
//  PlayerMan.cs is part of Tunnelers: Unified
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

[AddComponentMenu ("Network/Player Man")]

public class Player: MonoBehaviour, IKillable <float> {

	// We need this position to create an explosion
	Transform tank;

	// The current player's name
	string nickname;
	public string Nickname {
		set {
			nickname = value;
			nameUI.text = nickname;
		}
	}

	// The UI element we display the name on
	public UnityEngine.UI.Text nameUI;

	public Team team;
	public Tank tankType;

	public GameObject explosion;

	public bool IsMyBase (string teamTag) {

		return team + "Base" == teamTag;

	}

	void Awake () {

		// We know it's one of our children
		tank = transform.Find ("Tank");

	}

	void DropFlags () {

		// Get a list of all flags on scene
		GameObject [] flags = GameObject.FindGameObjectsWithTag ("Flag");

		// Go thorugh the list
		foreach (GameObject flag in flags) {
			// Ask if we are the carrier
			var currentFlag = (IFlag) flag.GetComponent (typeof (IFlag));
			if (currentFlag.Carrier == tank.gameObject)
				currentFlag.Carrier = null;
		}

	}

	public void Kill () {

		// We need to drop flags before dissapearing to prevent confusion
		DropFlags ();

		// KABOOM
		Instantiate (explosion, transform.position, transform.rotation);

		// Commit suicide
		Destroy (gameObject);

	}

	public void Kill (float timeLeft) {

		Invoke ("Kill", timeLeft);

	}

}
