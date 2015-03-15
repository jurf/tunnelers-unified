using UnityEngine;
using System.Collections;

public class DiggerC : MonoBehaviour {
	
	//public Vector3 rotation1;
	//public Vector3 rotation2;
	public float horizontal;
	public float vertical;
	public bool a;
	public bool s;
	public bool d;
	public bool w;
	
	public Vector3[] rotations = new Vector3[4];
	public Quaternion[] rotationsQ = new Quaternion[4];
	
	public Quaternion oldRot;
	public Quaternion currentRot;
	public Quaternion newRot;
	public Quaternion rotRot;
	
	public bool rotating;
	public bool stopRotating;
	public float rotVariation = 2f;
	public float moveVariation = 45f;
	
	public float speed;
	public bool canMove;
	public bool isDigging;
	public float moveSpeed = 5f;
	public float digSpeed = 2f;
	
	public float timeSinceLastDig;
	public float digVariation;
	
	public float maxVelocityChange = 10f;

	// Use this for initialization
	void Start () {
	
		for (int r = 0; r < rotations.Length; r++) {
			rotationsQ[r] = Quaternion.Euler (rotations[r]);
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
		//Debug.Log (CalculateMiddle (rotation1,rotation2));
		if (!Network.isClient && !Network.isServer) {
		
			horizontal = Input.GetAxis ("Horizontal");
			vertical = Input.GetAxis ("Vertical");
			a = Input.GetKey (KeyCode.A);
			s = Input.GetKey (KeyCode.S);
			d = Input.GetKey (KeyCode.D);
			w = Input.GetKey (KeyCode.W);
			
			currentRot = transform.rotation;
			
			Movement ();
			
		} else if (Network.isClient && !Network.isServer) {
			enabled = false;
		} else if (!Network.isClient && Network.isServer) {
			
			PlayerTest axes = gameObject.GetComponent<PlayerTest>();
			
			horizontal = axes.horizontal;
			vertical = axes.vertical;
			a = axes.a;
			s = axes.s;
			d = axes.d;
			w = axes.w;
			
			currentRot = transform.rotation;
			
			Movement ();
		
		}
		
	}
	
	void Movement () {
		
		if (isDigging) {
			speed = digSpeed;
			//dirtShower.emit = true;
			if (Time.realtimeSinceStartup - timeSinceLastDig > digVariation) {
				isDigging = false;
			}
		} else {
			speed = moveSpeed;
			//dirtShower.emit = false;
			if (Time.realtimeSinceStartup - timeSinceLastDig < digVariation) {
				isDigging = true;
			}
		}

		/*
		if (horizontal < 0 && Mathf.Approximately (vertical,0)) {
			newRot = rotationsQ[0];
			//canMove = true;
		} else if (horizontal > 0 && Mathf.Approximately (vertical,0)) {
			newRot = rotationsQ[2];
			//canMove = true;
		} else if (vertical < 0 && Mathf.Approximately (horizontal,0)) {
			newRot = rotationsQ[1];
			//canMove = true;
		} else if (vertical > 0 && Mathf.Approximately (horizontal,0)) {
			newRot = rotationsQ[3];
			//canMove = true;
		} else if (horizontal < 0 && vertical < 0) {
			newRot = CalculateMiddle (rotationsQ[0],rotationsQ[1]);
			//canMove = true;
		} else if (horizontal < 0 && vertical > 0) {
			newRot = CalculateMiddle (rotationsQ[0],rotationsQ[3]);
			//canMove = true;
		} else if (horizontal > 0 && vertical < 0) {
			newRot = CalculateMiddle (rotationsQ[1],rotationsQ[2]);
			//canMove = true;
		} else if (horizontal > 0 && vertical > 0) {
			newRot = CalculateMiddle (rotationsQ[2],rotationsQ[3]);
			//canMove = true;
		//else if (Mathf.Approximately (horizontal,0) && Mathf.Approximately (vertical,0)) {
		}*//* else if (horizontal < 1 && horizontal > -1 && vertical < 1 && vertical > -1) { 
			canMove = false;
		}*/
		
		if (a && !d && !s && !w) {
			newRot = rotationsQ[0];
			//canMove = true;
		} else if (d && !a && !s && !w) {
			newRot = rotationsQ[2];
			//canMove = true;
		} else if (s && !a && !d && !w) {
			newRot = rotationsQ[1];
			//canMove = true;
		} else if (w && !a && !d && !s) {
			newRot = rotationsQ[3];
			//canMove = true;
		} else if (a && s && !d && !w) {
			newRot = CalculateMiddle (rotationsQ[0],rotationsQ[1]);
			//canMove = true;
		} else if (a && w && !d && !s) {
			newRot = CalculateMiddle (rotationsQ[0],rotationsQ[3]);
			//canMove = true;
		} else if (s && d && !a && !w) {
			newRot = CalculateMiddle (rotationsQ[1],rotationsQ[2]);
			//canMove = true;
		} else if (w && d && !a && !s) {
			newRot = CalculateMiddle (rotationsQ[2],rotationsQ[3]);
			//canMove = true;
		//else if (Mathf.Approximately (horizontal,0) && Mathf.Approximately (vertical,0)) {
		}
		
		if (!rotating && Quaternion.Angle (transform.rotation, newRot) > rotVariation) {
			StartCoroutine (Rotation(currentRot, newRot,0.3f));
			//StartCoroutine (RotationLerp (currentRot,newRot,0.3f));
			//Lerp (currentRot,newRot);
			
		}
		
		if (Quaternion.Angle (rotRot,newRot) > rotVariation && rotating && !stopRotating) {
			stopRotating = true;
		}
		
		//Debug.Log (Quaternion.Angle (oldRot,rotRot));
		
		//if (canMove) {
		float moveInput;
	
		if (Quaternion.Angle (currentRot,newRot) < rotVariation || Quaternion.Angle (oldRot,rotRot) < moveVariation) {
			moveInput = Mathf.Abs (vertical) + Mathf.Abs (horizontal);
			moveInput = Mathf.Clamp01 (moveInput);
		} else {
			moveInput = 0;
		}
		//rigidbody.MovePosition (transform.position + moveInput * transform.forward * moveSpeed * Time.smoothDeltaTime);
		//rigidbody.AddForce (transform.forward * moveInput * moveSpeed);
		//rigidbody.velocity = transform.forward * moveInput * speed;

		Vector3 targetVelocity = transform.forward * moveInput;
		//targetVelocity = transform.TransformDirection(targetVelocity);
		targetVelocity *= speed;
 
		// Apply a force that attempts to reach our target velocity
		Vector3 velocity = GetComponent<Rigidbody>().velocity;
		Vector3 velocityChange = (targetVelocity - velocity);
		velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
		velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
		velocityChange.y = 0;
		GetComponent<Rigidbody>().AddForce(velocityChange, ForceMode.VelocityChange);

		//}
	
		Debug.Log (GetComponent<Rigidbody>().velocity);
			
	}
	
	IEnumerator Rotation (Quaternion _from,Quaternion _to,float time) { 
		
		oldRot = _from;
		rotRot = _to;
		if (stopRotating) {
			rotating = false;
			stopRotating = false;
			yield break;
		}
		
		rotating = true;
		
		float timing = Quaternion.Angle (_from, _to) / (time * 1000);
		float rate = 1f/timing; 
		float t = 0f; 
	
		while (t < 1.0) {
			if (stopRotating/* || Quaternion.Angle (_to,newRot) > rotVariation*/) {
				rotating = false;
				stopRotating = false;
				yield break;	
			}

			t += Time.deltaTime * rate; 
			transform.rotation = Quaternion.Slerp (_from, _to, t);
			yield return 0;
		}
		
		rotating = false;

	}
	
	/*IEnumerator RotationLerp (Quaternion _from,Quaternion _to,float time) { 
		
		oldRot = _from;
		rotRot = _to;
		if (stopRotating) {
			rotating = false;
			stopRotating = false;
			yield break;
		}
		
		rotating = true;
		
		float timing = Quaternion.Angle (_from, _to) / (time * 1000);
		float rate = 1f/timing; 
		float t = 0f; 
	
		while (t < 1.0) {
			if (stopRotating) {
				rotating = false;
				stopRotating = false;
				yield break;	
			}

			t += Time.deltaTime * rate; 
			transform.rotation = Quaternion.Lerp (_from, _to, t);
			yield return 0;
		}
		
		rotating = false;

	}*/
	
	void Lerp (Quaternion _from,Quaternion _to) {
		transform.rotation = Quaternion.RotateTowards (_from,_to,speed   * Time.smoothDeltaTime);
	}
	
	public static Quaternion CalculateMiddle (Quaternion rot1, Quaternion rot2) {
		return Quaternion.Slerp (rot1, rot2, 0.5f);
	}
	
	
	public static float Inverse (float num) {
		return num * -1;
	}

	public void YouDug () {
		timeSinceLastDig = Time.realtimeSinceStartup;
	}
}
