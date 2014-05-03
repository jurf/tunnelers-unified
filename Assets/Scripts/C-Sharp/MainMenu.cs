using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	
		[System.Serializable]
		public class Positions {
			
			public Rect background;
			public Rect backgroundText;
			
			public Rect main;
			
			public MainButtons[] mainButtons;
		
		}
		
		[System.Serializable]
		public class MainButtons {
			
			public Rect pos;
			
		}
		
		[System.Serializable]
		public class Source {
			
			public Texture2D background;
			public Rect backgroundText;
			
		}
		
		[System.Serializable]
		public class LocaleENG {
			
			public string[] main;
			
		}
		
		[System.Serializable]
		public class States {
			
			public enum MainState {
				
				none, play, community, options, credits, quit
			
			}
			
			public MainState mainState;
			
		}
	
	public Positions poses;
	
	public Source source;
	
	public LocaleENG lENG;
	
	public States states;
	
	public int lastHeight;
	public int lastWidth;
	
	void Start () {
		
		lastHeight = Screen.height;
		lastWidth = Screen.width;
		SetPoses ();
	
	}
	
	void Update () {
	
	}
	
	void OnGUI () {
		
		if (Screen.height != lastHeight || Screen.width != lastWidth) {
			SetPoses ();
			lastHeight = Screen.height;
			lastWidth = Screen.width;
		}
		
		ShowMainButtons ();	
		ShowBackground ();
	
	}
	
	public void SetPoses () {
		
		poses.main.center = new Vector2 (Screen.width / 2, Screen.height / 2);
		SetBackground ();
	
	}
	
	public void SetBackground () {
		
		float multiplierHeight = (float)Screen.height / source.background.height;
		float multiplierWidth = (float)Screen.width / source.background.width;
		
		float wouldBeWidth = multiplierHeight * source.background.width;
		float wouldBeHeight = multiplierWidth * source.background.height;
		
		if (Screen.width < wouldBeWidth) {			
			poses.background.height = Screen.height + 2;
			poses.background.width = wouldBeWidth + 2;
			poses.background.center = new Vector2 ((float)Screen.width / 2, (float)Screen.height / 2);
			poses.backgroundText = MultiplyRect (source.backgroundText,multiplierHeight);
		} else {			
			poses.background.width = Screen.width + 2;
			poses.background.height = wouldBeHeight + 2;
			poses.background.center = new Vector2 ((float)Screen.width / 2, (float)Screen.height / 2);
			poses.backgroundText = MultiplyRect (source.backgroundText,multiplierWidth);
		}
	
	}
	
	public void SetMain () {
	
		GUI.Box (poses.backgroundText, "something");
		
	}
		
	public void ShowMainButtons () {
		
//		Rect button = new Rect (0,0,poses.mainButtons.play.width,poses.mainB.play.height);
//		button.center = poses.main.center + poses.mainButtons.play.center;
//			
//		if (GUI.Button (button, "Play")) {}

	}
	
	public void ShowBackground ()  {
		
		GUIStyle backgroundStyle = new GUIStyle();
		backgroundStyle.normal.background = source.background;
		GUI.Label (poses.background,"",backgroundStyle);
		
	}
	
	public Rect MultiplyRect (Rect original, float multiplier) {		
		return new Rect (original.x * multiplier, original.y * multiplier, original.width * multiplier, original.height * multiplier);			
	}
		
}
