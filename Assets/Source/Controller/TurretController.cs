// vim:set ts=4 sw=4 sts=4 noet:
//
//  TurretController.cs is part of Tunnelers: Unified
//  <https://github.com/VacuumGames/tunnelers-unified/>.
//
//  Copyright (c) 2014-2016 Juraj Fiala <doctorjellyface@riseup.net>
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

public class TurretController: ControllerBase, IRotatable {

	[Range (1, 10)]
	public float rotationSpeed = 2.5f;

	Transform tf;
	Transform tank;
	Vector3 initialPos;

	// Store the rotation we need to turn to
	Quaternion toRotation;

	void Awake () {

		// Initialize references between objects
		tf = transform.Find ("Turret");
		tank = transform.Find ("Tank");

		// Find out how high we are (no pun intended)
		initialPos = tf.position - tank.position;

	}

	void Update () {

		// Keep the turret over the tank
		tf.position = tank.position + initialPos;

	}

	public void Rotate (Vector2 mousePos) {

		// Calculate the rotation according to the mouse
		toRotation = RotateToMouse (mousePos);

		// Rotate to that position
		Quaternion fromRotation = tf.rotation;
		tf.rotation = AddTorque (fromRotation, toRotation, rotationSpeed);

	}

	Quaternion RotateToMouse (Vector2 mousePosition) {

		// Create a new plane to cast a raycast on
		var playerPlane = new Plane (Vector3.up, tf.position);

		// Cast the ray from the mouse position
		Ray ray = Camera.main.ScreenPointToRay (mousePosition);

		Quaternion toRot;

		float hitdist;
		// Intersect the ray with the plane, store the intersection distance
		if (playerPlane.Raycast (ray, out hitdist)) {
			// If we do hit the plane, get the position of the intersection
			Vector3 targetPoint = ray.GetPoint(hitdist);
			// Look at that the spot, calculated relative to us
			toRot = Quaternion.LookRotation (targetPoint - tf.position);
		} else {
			// If we didn't hit the plane, try not to panic
			toRot = Quaternion.identity;
		}

		// Return the calculated rotation
		return toRot;

	}

}
