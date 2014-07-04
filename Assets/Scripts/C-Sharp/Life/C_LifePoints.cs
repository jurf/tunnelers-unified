using UnityEngine;

public class C_LifePoints : MonoBehaviour {
	
	public S_LifePoints sscript;
	
	void OnGUI () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
		
		if (Network.player != sscript.parent.GetComponent <PlayerMan> ().GetOwner ())
			return;
		
		GUILayout.BeginArea (new Rect (0, 0, Screen.width, Screen.height));
		GUILayout.BeginVertical ();
		
			GUILayout.FlexibleSpace ();
		
			GUILayout.Label ("Energy: " + sscript.EnergyPoints.ToString ());
			GUILayout.Label ("Shield: " + sscript.ShieldPoints.ToString ());
			
		GUILayout.EndVertical ();
		GUILayout.EndArea ();
		
	}
	
}
