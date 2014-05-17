using UnityEngine;
using System.Collections;

[RequireComponent (typeof (S_Warhead))]

public class S_WarheadLaser : MonoBehaviour {

	public S_Warhead warhead;
	public LineRenderer line;
	
	public float laserSpeed;
	public float coolDown;
	[SerializeField]
	private float time;
	public float waitForSec;
	
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
	
		if (warhead.left && time > coolDown) {
		
			Shoot ();
			networkView.RPC ("ShootLaser", RPCMode.Others);
			time = 0f;
		
		}
		
		if (time < coolDown)
			time += Time.deltaTime;
		else
			time = cooldown;
		
	}
	

	public void Shoot () {
		
		RayCastHit hit;
		if (Physics.Raycast (transform.position, transform.forward, out hit, range)) {
		
			if (hit.collider.tag == "Tank" || hit.collider.tag == "Turret") {
			
				hit2.collider.gameObject.GetComponent <LifePoints> ().ApplyDamage (damage);
				line.SetPosition (1, transform.localPosition + new Vector3 (0, 0, hit2.distance));
				Invoke ("ResetLaser", waitForSec);
				
			} else {
						
				line.SetPosition (1, transform.localPosition + new Vector3 (0, 0, hit.distance));
				Invoke ("ResetLaser", waitForSec);
				
			}
			
		} else {
		
			line.SetPosition (1, transform.localPosition + new Vector3 (0, 0, range));		
			Invoke ("ResetLaser", waitForSec);
		
		}
	}
	
	public void ResetLaser () {
	
		line.SetPosition (1, transform.localPosition);
				
	}	
	
}

