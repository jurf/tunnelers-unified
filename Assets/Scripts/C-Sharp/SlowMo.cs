using UnityEngine;
using System.Collections;

public class SlowMo : MonoBehaviour {

	public float scale = 1f;
	
	void Update () {
	
		Time.timeScale = scale;
	
	}
}
