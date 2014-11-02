//
//  MTankController.cs is part of Tunnelers: Unified
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

[AddComponentMenu("Mixed/Tank Controller")]
//[RequireComponent (typeof (Rigidbody))]

public class MTankController : MonoBehaviour {

	public static float rotVar = 2f;

	public static Vector3[] rotations = new Vector3[4] {new Vector3 (0,0,0), new Vector3 (0,90,0), new Vector3 (0,180,0), new Vector3 (0,270,0)};

	public static Quaternion[] rotationsQ = new Quaternion[4];

	static MTankController () {
		
		Debug.Log ("Run Constructor.");
		for (int r = 0; r < rotations.Length; r++) {
			rotationsQ[r] = Quaternion.Euler (rotations[r]);
		}
		
	}

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

	static bool GetNewRotationPhase (Transform tankTransform, Quaternion fromRotation, Quaternion toRotation, float speed, ref Quaternion lastSuccTo, ref Quaternion lastTo) {
		//TODO Rotate with force
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
	
	//Turrets
	static bool GetNewRotationPhase (Transform tankTransform, Quaternion fromRotation, Quaternion toRotation, float speed) {
		
		float timing = Quaternion.Angle (fromRotation, toRotation) / (speed * 2000f);
		float rate = 1f / timing;
		float t = 0f;
		t += Time.deltaTime * rate;
		
		tankTransform.rotation = Quaternion.Slerp (fromRotation, toRotation, t);
		
		return (Quaternion.Angle (fromRotation, toRotation) < rotVar);
		
	}

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

	static Quaternion CalculateMiddle (Quaternion rot1, Quaternion rot2) {
		return Quaternion.Slerp (rot1,rot2,0.5f);	
	}

	static public int GetHorizontalAxis (bool a, bool d) {		
		return System.Convert.ToInt32 (d) - System.Convert.ToInt32 (a);		
	}

	static public int GetVerticalAxis (bool s, bool w) {
		return System.Convert.ToInt32 (w) - System.Convert.ToInt32 (s);		
	}

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

	public Type type;
	public float moveSpeed = 5f;
	public float rotSpeed = 2.5f;
	public float maxVelocityChange = 10f;
	public float maxVelocityChangeStop = 2f;
	public Quaternion lastSuccRotation;
	public Quaternion lastRotation;
	public bool isMoving;

	public void Move (int h, int v) {
	
		MTankController.MoveMain (gameObject, h, v, moveSpeed, rotSpeed, maxVelocityChange, maxVelocityChangeStop, ref lastSuccRotation, ref lastRotation, ref isMoving);
	
	}

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

	public void Rotate (Quaternion toRotation) {
	
		MTankController.GetNewRotationPhase (transform, transform.rotation, toRotation, rotSpeed / 50f);
	
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
