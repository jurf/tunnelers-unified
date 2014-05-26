using UnityEngine;
using System.Collections;

[RequireComponent (typeof (NetworkView))]

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
			networkView.RPC ("Shoot", RPCMode.Others);
			time = 0f;
		
		}
		
		if (time < coolDown)
			time += Time.deltaTime;
		else
			time = coolDown;
		
	}
	

	void Shoot () {
		
		RaycastHit hit;
		if (Physics.Raycast (transform.position, transform.forward, out hit, range)) {
		
			if (hit.collider.tag == "Tank" || hit.collider.tag == "Turret") {
			
				hit.collider.gameObject.GetComponent <LifePoints> ().ApplyDamage (damage);
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
	
}

