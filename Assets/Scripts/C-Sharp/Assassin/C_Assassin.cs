using UnityEngine;

public class C_Assassin : MonoBehaviour {

	public S_Assassin sscript;
	
	[RPC]
	public void Freeze () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
		
		Debug.Log ("Freezing a tank.", gameObject);
	
		sscript.parent.tank.gameObject.SetActive (false);
		sscript.parent.turret.gameObject.SetActive (false);
		Instantiate (sscript.explosion, sscript.parent.tank.transform.position, sscript.parent.tank.transform.rotation);
		
	}

}
