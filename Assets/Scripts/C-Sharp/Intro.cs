using UnityEngine;
using System.Collections;

public class Intro : MonoBehaviour {

	public GameObject connectionMan;
	
	void Awake () {
	
		DontDestroyOnLoad (transform.gameObject);
		connectionMan.GetComponent <ServerC> ().DontDestroy ();
	
	}
	
	void OnLevelWasLoaded (int level) {
	
		if (level == 1) {
		
			connectionMan.SetActive (true);
			Destroy (gameObject);
		
		}
	
	}
	
	void Start () {
	
		Application.LoadLevel ("main");
	
	}

}
