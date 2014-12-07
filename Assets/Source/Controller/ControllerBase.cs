//
//  ControllerBase.cs is part of Tunnelers: Unified
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

public class ControllerBase: MonoBehaviour	{

	public static float rotVar = 2f;

	public static Quaternion [] rotations = new Quaternion [] {
		new Quaternion (0f, 0f, 0f, 1f),
		new Quaternion (0f, 0.7071068f, 0f, 0.7071068f),
		new Quaternion (0f, 1f, 0f, -4.371139e-08f),
		new Quaternion (0f, 0.7071068f, 0f, -0.7071068f)
	};
	
	public static Quaternion GetNewRotation (int horizontal, int vertical, Quaternion lastRot) {

		if (horizontal == 1 && vertical == 0) {
			return rotations [0];
		}
		if (horizontal == -1 && vertical == 0) {
			return rotations [2];
		}
		if (vertical == -1 && horizontal == 0) {
			return rotations [1];
		}
		if (vertical == 1 && horizontal == 0) {
			return rotations [3];
		}
		if (horizontal == -1 && vertical == -1) {
			return CalculateMiddle (rotations [2], rotations [1]);
		}
		if (horizontal == -1 && vertical == 1) {
			return CalculateMiddle (rotations [2], rotations [3]);
		}
		if (horizontal == 1 && vertical == 1) {
			return CalculateMiddle (rotations [0], rotations [3]);
		}
		if (horizontal == 1 && vertical == -1) {
			return CalculateMiddle (rotations [0], rotations [1]);
		}

		return lastRot;

	}

	public static Quaternion CalculateMiddle (Quaternion rot1, Quaternion rot2) {
		return Quaternion.Slerp (rot1, rot2, 0.5f);
	}

	public static Vector3 AddForce (Vector3 direction, Vector3 velocity, float speed, float maximumVelocityChange) {

		Vector3 targetVelocity = direction;
		targetVelocity *= speed;

		Vector3 velocityChange = (targetVelocity - velocity);
		velocityChange.x = Mathf.Clamp (velocityChange.x, -maximumVelocityChange, maximumVelocityChange);
		velocityChange.z = Mathf.Clamp (velocityChange.z, -maximumVelocityChange, maximumVelocityChange);
		velocityChange.y = 0;

		return velocityChange;

	}

	public static Quaternion AddTorque (Quaternion fromRotation, Quaternion toRotation, float speed) {

		//TODO Rotate with force
		float timing = Quaternion.Angle (fromRotation, toRotation) / (speed * 120f);
		float rate = 1f / timing;
		float t = 0f;
		t += Time.deltaTime * rate;

		return Quaternion.Slerp (fromRotation, toRotation, t);

	}

}