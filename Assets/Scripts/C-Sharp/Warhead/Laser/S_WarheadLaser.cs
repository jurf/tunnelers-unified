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
		
			StartCoroutine (LaserSequence ());
			networkView.RPC ("ShootLaser", RPCMode.Others);
			time = 0f;
		
		}
		
		time += Time.deltaTime;
	
	}
	
	IEnumerator LaserSequence () {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return false;
		}
		
		for (float i = 0.001f; i < range; i += laserSpeed * Time.deltaTime) {		
					
			RaycastHit hit;
			if (Physics.Raycast (transform.position, transform.forward, out hit, i)) {
				if (hit.collider.tag == "Tank") {
//					Debug.Log ("Hit tank.");
					hit.collider.gameObject.GetComponent <LifePoints> ().ApplyDamage (damage);
				
				} else {
				
					line.SetPosition (1, transform.localPosition + new Vector3 (0, 0, hit.distance));
//					Debug.Log ("Going to break, distance: " + hit.distance + ", Vector3.z: " + (transform.localPosition + new Vector3 (0,0,hit.distance)));
					yield return new WaitForSeconds (waitForSec);
					line.SetPosition (1, transform.localPosition);
//					Debug.LogWarning ("Just before yield break.");
					yield break;
					
//					break;
				
				}
			
			
			} else {
//				Debug.Log ("If else hit.");
				line.SetPosition (1, transform.localPosition + new Vector3 (0, 0, i));
			
			}
//			Debug.Log ("Yield returning.");
			yield return 0;
			
		}
//		Debug.Log ("End of loop.");
		
		RaycastHit hit2;
		if (Physics.Raycast (transform.position, transform.forward, out hit2, range)) {
			if (hit2.collider.tag == "Tank") {
//				Debug.Log ("Hit tank.");
				hit2.collider.gameObject.GetComponent <LifePoints> ().ApplyDamage (damage);
				line.SetPosition (1, transform.localPosition + new Vector3 (0, 0, hit2.distance));
				yield return new WaitForSeconds (waitForSec);
				line.SetPosition (1, transform.localPosition);
				yield break;
			} else {			
				line.SetPosition (1, transform.localPosition + new Vector3 (0, 0, hit2.distance));
//				Debug.Log ("Going to break, distance: " + hit.distance + ", Vector3.z: " + (transform.localPosition + new Vector3 (0,0,hit.distance)));
				yield return new WaitForSeconds (waitForSec);
				line.SetPosition (1, transform.localPosition);
//				Debug.LogWarning ("Just before yield break.");
				yield break;
				
				//					break;
				
			}
		} else {
		
		line.SetPosition (1, transform.localPosition + new Vector3 (0, 0, range));
		
		yield return new WaitForSeconds (waitForSec);
		
		line.SetPosition (1, transform.localPosition);
		
		yield break;
		
		}

	}	
	
}

