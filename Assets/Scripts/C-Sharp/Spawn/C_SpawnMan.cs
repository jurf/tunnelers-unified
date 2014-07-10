using UnityEngine;

public class C_SpawnMan : MonoBehaviour {

	bool green; //is all loaded properly? set by client netman
	public bool Green {
	
		get {
			return green;
		}
		
		set {
			if (!green)
				green = value;
		}
	
	}
	
	public Rect teamSelectionRect;
	
	#region UnityMethods
	
		void Awake () {
		
			if (!Network.isClient || Network.isServer) {
				enabled = false;
				return;
			}
			
		}
		
		void ConnectedToServer () {
		
			enabled = true;
			
		}
		
		void OnGUI () {	
		
			if (!Network.isClient || Network.isServer) {
				enabled = false;
				return;
			}
			
			if (green) {
			
				teamSelectionRect.center = new Vector2 (Screen.width / 2, Screen.height / 2);
					
				teamSelectionRect = GUILayout.Window (0, teamSelectionRect, TeamSelection, "Team Selection");

			}
			
		}

	#endregion UnityMethods
		
	void TeamSelection (int windowID) {
           
//		int blueAmount;
        
		if (GUILayout.Button ("Blue")) {
			networkView.RPC ("RequestGameEntry", RPCMode.Server, Network.player, true, ServerC.name);
			enabled = false;
		}
        
		if (GUILayout.Button ("Red")) {
			networkView.RPC ("RequestGameEntry", RPCMode.Server, Network.player, false, ServerC.name);
			enabled = false;
		}

	}
	
}
