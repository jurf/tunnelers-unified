using UnityEngine;
using System.Collections;

[RequireComponent (typeof (NetworkView))]

public class S_WarheadLaser : MonoBehaviour {

	public S_Warhead warhead;
	public LineRenderer line;
	
	public float laserSpeed;
	public float coolDown = 5f;
	public bool cooled;
	
	public float time;
	public float waitForSec = 0.5f;
	
	public float range = 5f;
	
	public float damage;
	
	void Awake () {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
		
	}
	
	void Update () {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
	
		if (warhead.left && cooled) {
		
			Shoot ();
			networkView.RPC ("C_Shoot", RPCMode.Others);
			time = 0f;
		
		}
		
		if (time < coolDown) {
			cooled = false;
			time += Time.deltaTime;
		}
		
		if (time > coolDown) {
			cooled = true;
			time = coolDown;
		}
		
	}

	void Shoot () {
		
		RaycastHit hit;
		if (Physics.Raycast (transform.position, transform.forward, out hit, range)) {
		
			if (hit.collider.tag == "Tank" || hit.collider.tag == "Turret") {
			
				hit.collider.gameObject.transform.parent.GetComponent <S_LifePoints> ().ApplyDamage (damage);
				line.SetPosition (1, new Vector3 (0, 0, hit.distance));
				Invoke ("ResetLaser", waitForSec);
				
			} else {
						
				line.SetPosition (1, new Vector3 (0, 0, hit.distance));
				Invoke ("ResetLaser", waitForSec);
				
			}
			
		} else {
		
			line.SetPosition (1, new Vector3 (0, 0, range));		
			Invoke ("ResetLaser", waitForSec);
		
		}
	}
	
	void ResetLaser () {
	
		line.SetPosition (1, Vector3.zero);
				
	}
	
	public void OnSerializeNetworkView (BitStream stream) {
	
		int coolDownTime = 0;
	
		if (stream.isWriting) {
			coolDownTime = (int) time;
			stream.Serialize (ref cooled);
			stream.Serialize (ref coolDownTime);
		} else if (stream.isReading) {
			stream.Serialize (ref cooled);
			stream.Serialize (ref coolDownTime);
			time = (float) coolDownTime;
		}
	
	}
	
}

