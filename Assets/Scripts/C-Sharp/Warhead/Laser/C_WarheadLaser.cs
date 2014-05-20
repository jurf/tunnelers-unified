using UnityEngine;
using System.Collections;

public class C_WarheadLaser : MonoBehaviour {

	public S_WarheadLaser sscript;
	
	void Awake () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
		
	}
	
	[RPC]
	public void Shoot () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return false;
		}
		
		RayCastHit hit;
		if (Physics.Raycast (transform.position, transform.forward, out hit, sscript.range)) {
		
			if (hit.collider.tag == "Tank" || hit.collider.tag == "Turret") {
			
				sscript.line.SetPosition (1, new Vector3 (0, 0, hit.distance));
				Invoke ("ResetLaser", sscript.waitForSec);
				
			} else {
						
				sscript.line.SetPosition (1, new Vector3 (0, 0, hit.distance));
				Invoke ("ResetLaser", sscript.waitForSec);
				
			}
			
		} else {
		
			sscript.line.SetPosition (1, new Vector3 (0, 0, sscript.range));		
			Invoke ("ResetLaser", sscript.waitForSec);
		
		}
		
	}
	
	void ResetLaser () {
	
		line.SetPosition (1, Vector3.zero);
				
	}

}

