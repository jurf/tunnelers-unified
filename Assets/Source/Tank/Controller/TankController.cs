// vim:set ts=4 sw=4 sts=4 noet:
//
//  TankController.cs is part of Tunnelers: Unified
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

public class TankController: ControllerBase, IMovable <sbyte> {

	[Range (1, 15)]
	public float moveSpeed = 5f;
	[Range (1, 10)]
	public float rotationSpeed = 2.5f;
	[Range (1, 15)]
	public float maxVelocityChange = 10f;
	[Range (1, 10)]
	public float maxStopVelocityChange = 2f;

	Quaternion lastSuccRotation;
	Quaternion lastRotation;
	Transform tf;
	Rigidbody rb;

	bool isMoving;
	public bool IsMoving {
		get {
			return isMoving;
		}
	}

	void Awake () {

		// Initialize references between objects
		tf = transform.Find ("Tank");
		rb = tf.GetComponent <Rigidbody> ();
	
	}

	public void Move (sbyte h, sbyte v) {

		Quaternion fromRot = tf.rotation;
		Quaternion toRot = GetNewRotation (h, v, lastRotation);
		lastRotation = toRot;

		Quaternion newRotation = AddTorque (fromRot, toRot, rotationSpeed);

		bool isFinal = false;
		if (Quaternion.Angle (fromRot, toRot) < rotVar) {
			lastSuccRotation = newRotation;
			isFinal = true;
		}

		tf.rotation = newRotation;

		bool isVarSmallEnough = Quaternion.Angle (lastSuccRotation, lastRotation) < 45f + rotVar;
		bool canMove = isFinal || (isMoving && isVarSmallEnough);

		if ((h != 0 || v != 0) && canMove) {
			isMoving = true;
			Vector3 newForce = AddForce (tf.forward, rb.velocity, moveSpeed, maxVelocityChange);
			rb.AddForce (newForce, ForceMode.VelocityChange);
		} else {
			isMoving = false;
			Vector3 stopForce = AddForce (tf.forward, rb.velocity, 0f, maxStopVelocityChange);
			rb.AddForce (stopForce, ForceMode.VelocityChange);
		}

	}

}
