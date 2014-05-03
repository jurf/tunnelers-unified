using UnityEngine;
using System.Collections;

public class Toggle : MonoBehaviour {

	private bool toggleBool = true;
	public bool toggleBool2 = true;
	
	void OnGUI () {
		toggleBool = GUI.Toggle (new Rect (25, 25, 100, 30), toggleBool, "Toggle");
		toggleBool2 = GUILayout.Toggle (toggleBool2, "toggle2");
	}
}
