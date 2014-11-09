//
//  TurretController.cs is part of Tunnelers: Unified
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

public class TurretController: ControllerBase, IRotatable {

	public float rotationSpeed = 2.5f;

	public Quaternion RotateToMouse (Vector2 mousePosition) {

		var playerPlane = new Plane (Vector3.up, transform.position);

		Ray ray = Camera.main.ScreenPointToRay (mousePosition);

		Quaternion fromRot = transform.rotation;
		Quaternion toRot;

		float hitdist;
		if (playerPlane.Raycast (ray, out hitdist)) {
			Vector3 targetPoint = ray.GetPoint(hitdist);
			toRot = Quaternion.LookRotation (targetPoint - transform.position);
		} else {
			toRot = Quaternion.identity;
		}

		return AddTorque (fromRot, toRot, rotationSpeed);

	}

	public void Rotate (Quaternion toRot) {

		Quaternion fromRot = transform.rotation;
		transform.rotation = AddTorque (fromRot, toRot, rotationSpeed);

	}

}