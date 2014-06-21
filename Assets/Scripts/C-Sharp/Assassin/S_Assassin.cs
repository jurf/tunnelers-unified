using UnityEngine;

public class S_Assassin : MonoBehaviour {


	public PlayerMan parent;
	public GameObject explosion;
	public float wait = 3f;
	
	public void Assassinate () {
	
		if (!Network.isServer || Network.isClient) {
			enabled = false;
			return;
		}
		
		Debug.Log ("Assassinating.", gameObject);
		Stun ();
		CreateDiversion ();
		Invoke ("Kill", wait);
		
	}
	
	void Stun () {
	
		networkView.RPC ("Freeze", RPCMode.All);
		parent.tank.gameObject.SetActive (false);
		parent.turret.gameObject.SetActive (false);
		
	}
	
	void CreateDiversion () {
	
		Instantiate (explosion, parent.tank.transform.position, parent.tank.transform.rotation);
		
	}
	
	void Kill () {
	
		GameObject.Find ("ManMan").GetComponent <S_NetMan> ().NetworkDestroy (gameObject);
	
	}

}
