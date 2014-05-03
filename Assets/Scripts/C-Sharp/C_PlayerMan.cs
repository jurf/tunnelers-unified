using UnityEngine;
using System.Collections;

[AddComponentMenu ("Client/Player Man")]

public class PlayerType : MonoBehaviour {

	public enum Team {
	
		Red,
		Blue,
		Green
	
	}
	
	public enum Tank {
	
		DefaultTank
	
	}
	
	public Team team;
	public Tank tank;
	
	
	// Use this for initialization
		void Start () {
	
	}
	
		// Update is called once per frame
	void Update () {
	
	}
	
	public bool IsMyTeam (Team otherTeam) {
	
		if (otherTeam == team) {
			return true;
		} else {
			return false;
		}
	
	}
	
	public bool IsMyBase (string tag) {
	
		if (tag == team + "Base") {
			return true;
		} else {
			return false;
		}
	
	}
	
}

