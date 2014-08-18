//
//  M_TankController.cs is part of Tunnelers: Unified
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

[AddComponentMenu("Mixed/Tank Controller")]
//[RequireComponent (typeof (Rigidbody))]

/// <summary>
/// (Shared) Tank controller.
/// </summary>
public class M_TankController : MonoBehaviour {

	/// <summary>
	/// The rotation variable.
	/// </summary>
	public static float rotVar = 2f;
	
	/// <summary>
	/// The rotations.
	/// </summary>
	public static Vector3[] rotations = new Vector3[4] {new Vector3 (0,0,0), new Vector3 (0,90,0), new Vector3 (0,180,0), new Vector3 (0,270,0)};
	
	/// <summary>
	/// The rotations in Quaternion.
	/// </summary>
	public static Quaternion[] rotationsQ = new Quaternion[4];
	
	/// <summary>
	/// Initializes the <see cref="M_TankController"/> class.
	/// </summary>
	static M_TankController () {
		
		Debug.Log ("Run Constructor.");
		for (int r = 0; r < rotations.Length; r++) {
			rotationsQ[r] = Quaternion.Euler (rotations[r]);
		}
		
	}
	
	/// <summary>
	/// Gets the new rotation.
	/// </summary>
	/// <returns>The new rotation.</returns>
	/// <param name="horizontal">Horizontal axis.</param>
	/// <param name="vertical">Vertical axis.</param>
	/// <param name="last">Last rotation.</param>
	static Quaternion GetNewRotation (int horizontal, int vertical, Quaternion last) {
	
			if (horizontal == 1 && vertical == 0) {
				return rotationsQ [0];
			}
			if (horizontal == -1 && vertical == 0) {
				return rotationsQ [2];
			}
			if (vertical == -1 && horizontal == 0) {
				return rotationsQ [1];
			}
			if (vertical == 1 && horizontal == 0) {
				return rotationsQ [3];
			}
			if (horizontal == -1 && vertical == -1) {
				return CalculateMiddle (rotationsQ [2], rotationsQ [1]);
			}
			if (horizontal == -1 && vertical == 1) {
				return CalculateMiddle (rotationsQ [2], rotationsQ [3]);
			}
			if (horizontal == 1 && vertical == 1) {
				return CalculateMiddle (rotationsQ [0], rotationsQ [3]);
			}
			if (horizontal == 1 && vertical == -1) {
				return CalculateMiddle (rotationsQ [0], rotationsQ [1]);
			}
			return last;
		}
	
	/// <summary>
	/// Gets the new rotation phase.
	/// </summary>
	/// <returns><c>true</c>, if new rotation phase was gotten, <c>false</c> otherwise.</returns>
	/// <param name="tankTransform">Tank transform.</param>
	/// <param name="fromRotation">From rotation.</param>
	/// <param name="toRotation">To rotation.</param>
	/// <param name="speed">Speed.</param>
	/// <param name="lastSuccTo">Last successful to rotation.</param>
	/// <param name="lastTo">Last to rotation.</param>
	static bool GetNewRotationPhase (Transform tankTransform, Quaternion fromRotation, Quaternion toRotation, float speed, ref Quaternion lastSuccTo, ref Quaternion lastTo) {
		//MAYBEUPGRADE Rotate with force
		float timing = Quaternion.Angle (fromRotation, toRotation) / (speed * 120f);
		float rate = 1f / timing;
		float t = 0f;
		t += Time.deltaTime * rate;
		
		tankTransform.rotation = Quaternion.Slerp (fromRotation, toRotation, t);
//		Debug.Log (Quaternion.Angle (fromRotation, toRotation));
//		Debug.Log (Quaternion.Angle (fromRotation, toRotation) < 45 + rotVar);
		
		if (Quaternion.Angle (fromRotation, toRotation) < rotVar) {		
			lastSuccTo = toRotation;
		}
		
		lastTo = toRotation;
		
		return (Quaternion.Angle (fromRotation, toRotation) < rotVar);
	
	}
	
	/// <summary>
	/// Gets the new rotation phase (only for turrets).
	/// </summary>
	/// <returns><c>true</c>, if new rotation phase was gotten, <c>false</c> otherwise.</returns>
	/// <param name="tankTransform">Tank transform.</param>
	/// <param name="fromRotation">From rotation.</param>
	/// <param name="toRotation">To rotation.</param>
	/// <param name="speed">Speed.</param>
	static bool GetNewRotationPhase (Transform tankTransform, Quaternion fromRotation, Quaternion toRotation, float speed) {
		
		float timing = Quaternion.Angle (fromRotation, toRotation) / (speed * 2000f);
		float rate = 1f / timing;
		float t = 0f;
		t += Time.deltaTime * rate;
		
		tankTransform.rotation = Quaternion.Slerp (fromRotation, toRotation, t);
		
		return (Quaternion.Angle (fromRotation, toRotation) < rotVar);
		
	}
	
	/// <summary>
	/// Adds the force.
	/// </summary>
	/// <returns>The force.</returns>
	/// <param name="direction">Direction.</param>
	/// <param name="velocity">Velocity.</param>
	/// <param name="speed">Speed.</param>
	/// <param name="maximumVelocityChange">Maximum velocity change.</param>
	static public Vector3 AddForce (Vector3 direction, Vector3 velocity, float speed, float maximumVelocityChange) {
	
		Vector3 targetVelocity = direction;
		//targetVelocity = transform.TransformDirection(targetVelocity);
		targetVelocity *= speed;
		
		// Apply a force that attempts to reach our target velocity
		Vector3 velocityChange = (targetVelocity - velocity);
		velocityChange.x = Mathf.Clamp(velocityChange.x, -maximumVelocityChange, maximumVelocityChange);
		velocityChange.z = Mathf.Clamp(velocityChange.z, -maximumVelocityChange, maximumVelocityChange);
		velocityChange.y = 0;
		
		return velocityChange;
		//rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
	
	}
	
	/// <summary>
	/// Calculates the middle.
	/// </summary>
	/// <returns>The middle.</returns>
	/// <param name="rot1">Rotation 1.</param>
	/// <param name="rot2">Rotation 2.</param>
	static Quaternion CalculateMiddle (Quaternion rot1, Quaternion rot2) {
		return Quaternion.Slerp (rot1,rot2,0.5f);	
	}
	
	/// <summary>
	/// Gets the horizontal axis.
	/// </summary>
	/// <returns>The horizontal axis.</returns>
	/// <param name="a">"A" key.</param>
	/// <param name="d">"D" key.</param>
	static public int GetHorizontalAxis (bool a, bool d) {		
		return System.Convert.ToInt32 (d) - System.Convert.ToInt32 (a);		
	}
	
	/// <summary>
	/// Gets the vertical axis.
	/// </summary>
	/// <returns>The vertical axis.</returns>
	/// <param name="s">"S" key.</param>
	/// <param name="w">"W" key.</param>
	static public int GetVerticalAxis (bool s, bool w) {
		return System.Convert.ToInt32 (w) - System.Convert.ToInt32 (s);		
	}
	
	/// <summary>
	/// Main move method.
	/// </summary>
	/// <param name="tank">Tank.</param>
	/// <param name="h">The horizontal axis.</param>
	/// <param name="v">The vertical axis.</param>
	/// <param name="moveSpeed">Move speed.</param>
	/// <param name="maxVChange">Max Velocity change.</param>
	/// <param name="maxVChangeStop">Max stop velocity change.</param>
	/// <param name="lastSuccRot">Last succ rot.</param>
	/// <param name="lastRot">Last rot.</param>
	static public void MoveMain (GameObject tank, int h, int v, float mSpeed, float rSpeed, float maxVChange, float maxVChangeStop, ref Quaternion lastSuccRot, ref Quaternion lastRot, ref bool isItMoving) {		
		Quaternion newRot = GetNewRotation (h, v, lastRot);
	
		bool finRot = GetNewRotationPhase (tank.transform, tank.transform.rotation, newRot, rSpeed, ref lastSuccRot, ref lastRot);
		
		if (
			(finRot || (/*finRot && */Quaternion.Angle (lastSuccRot, lastRot) < 45f + rotVar))
			&& (h != 0 || v != 0)	
		) {
			isItMoving = true;
			tank.rigidbody.AddForce (AddForce (tank.transform.forward, tank.rigidbody.velocity, mSpeed, maxVChange), ForceMode.VelocityChange);
		
		} else {
			isItMoving = false;
			tank.rigidbody.AddForce (AddForce (tank.transform.forward, tank.rigidbody.velocity, 0f, maxVChangeStop), ForceMode.VelocityChange);
		
		}																																																				
	
	}
	
	public enum Type {
		Tank,
		Turret
	}
	
	/// <summary>
	/// The type of the controller needed.
	/// </summary>
	public Type type;
	
	/// <summary>
	/// The speed.
	/// </summary>
	public float moveSpeed = 5f;
	
	public float rotSpeed = 2.5f;
	
	/// <summary>
	/// The max velocity change.
	/// </summary>
	public float maxVelocityChange = 10f;
	
	/// <summary>
	/// The max stop velocity change.
	/// </summary>
	public float maxVelocityChangeStop = 2f;
	
	/// <summary>
	/// The last successful rotation.
	/// </summary>
	public Quaternion lastSuccRotation;
	
	/// <summary>
	/// The last rotation.
	/// </summary>
	public Quaternion lastRotation;
	
	/// <summary>
	/// Is the tank applying force?
	/// </summary>
	public bool isMoving;
	
	/// <summary>
	/// Move with the specified horizontal and vertical motion.
	/// </summary>
	/// <param name="h">The horizontal axis.</param>
	/// <param name="v">The vertical axis.</param>
	public void Move (int h, int v) {
	
		M_TankController.MoveMain (gameObject, h, v, moveSpeed, rotSpeed, maxVelocityChange, maxVelocityChangeStop, ref lastSuccRotation, ref lastRotation, ref isMoving);
	
	}
	
	/// <summary>
	/// Rotate the specified turret with the specified speed.
	/// </summary>
	/// <param name="turret">Turret.</param>
	/// <param name="speed">Speed.</param>
	public static Quaternion RotateToMouse (GameObject turret) {
		
		Plane playerPlane = new Plane (Vector3.up, turret.transform.position);
		
		// Generate a ray from the cursor position
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		float hitdist;
		// If the ray is parallel to the plane, Raycast will return false.
		if (playerPlane.Raycast (ray, out hitdist)) {
			// Get the point along the ray that hits the calculated distance.
			Vector3 targetPoint = ray.GetPoint(hitdist);
			
			// Determine the target rotation.  This is the rotation if the transform looks at the target point.
			Quaternion targetRotation = Quaternion.LookRotation (targetPoint - turret.transform.position);
			return targetRotation;
			// Smoothly rotate towards the target point.
			//transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
			//M_TankController.GetNewRotationPhase (turret.transform, turret.transform.rotation, targetRotation, speed);
			
		}
		
			return Quaternion.identity;
	
	}
	/// <summary>
	/// Local rotate method. called by the server and client
	/// scripts
	/// </summary>
	/// <param name="toRotation">To rotation.</param>
	public void Rotate (Quaternion toRotation) {
	
		M_TankController.GetNewRotationPhase (transform, transform.rotation, toRotation, rotSpeed / 50f);
	
	}
		
	void Update () {
		
		if (!Network.isClient && !Network.isServer) {
		
			if (type == Type.Tank) {
			
				bool a = Input.GetKey (KeyCode.A);
				bool d = Input.GetKey (KeyCode.D);
				bool s = Input.GetKey (KeyCode.S);
				bool w = Input.GetKey (KeyCode.W);
			
				int horizontal = GetHorizontalAxis (a,d);
				int vertical = GetVerticalAxis (s, w);
			
				Move (horizontal, vertical);
				
			} else {
			
				Rotate (RotateToMouse (gameObject));
			
			}
		}
		
	}
}
