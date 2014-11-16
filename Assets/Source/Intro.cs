//
//  Intro.cs is part of Tunnelers: Unified
//  <https://github.com/VacuumGames/tunnelers-unified/>.
//
//  Copyright (c) 2014 Juraj Fiala<doctorjellyface@riseup.net>
//
//  Tunnelers: Unified is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  Tunnelers: Unified is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with Tunnelers: Unified.  If not, see <http://www.gnu.org/licenses/>.
//

using UnityEngine;
using System.Collections;

public class Intro : MonoBehaviour {

	public GameObject connectionMan;
	public CameraFade fader;
	
	public bool drawLogo = true;
	public Texture2D logo;
	public GUIStyle logogo;
	Rect logoRect;
	
	public bool drawCredits;
	public Texture2D credits;
	public GUIStyle creditits;
	Rect creditsRect;
	
	bool onMain;
	
	void Awake () {
	
		DontDestroyOnLoad (transform.gameObject);
		connectionMan.GetComponent <ServerC> ().DontDestroy ();
		
		logogo.normal.background = logo;
	
	}
	
	void Update () {
	
		if (Input.GetKeyDown (KeyCode.Space) && !onMain) {
			StopCoroutine ("Sequence");
			fader.SetScreenOverlayColor (Color.black);
			drawLogo = false;
			drawCredits = false;
			onMain = true;
			Application.LoadLevel ("main");
		}
	}
	
	void OnGUI () {
	
		if (drawLogo) {
		
			logoRect.width = logo.width;
			logoRect.height = logo.height;
			logoRect.center = new Vector2 (Screen.width / 2, Screen.height / 2);
		
			GUI.Box (logoRect, "", logogo);
			
		}
		
		if (drawCredits) {
		
			creditsRect.width = credits.width;
			creditsRect.height = credits.height;
			creditsRect.center = new Vector2 (Screen.width / 2, Screen.height / 2);
		
			GUI.Box (creditsRect, "", creditits);
			
		}
		
	}
	
	void OnLevelWasLoaded (int level) {
	
		if (level == 1) {
		
			onMain = true;		
			drawCredits = false;
			drawLogo = false;
		
			connectionMan.SetActive (true);
			
			StartCoroutine ("SelfDestruct");
		
		}
	
	}
	
	void Start () {
		
		StartCoroutine ("Sequence");
	
	}
	
	IEnumerator Sequence () {
	
		fader.SetScreenOverlayColor (Color.black);
		
		yield return new WaitForSeconds (1);
		
		//start
		
		fader.StartFade (Color.clear, 3);
		
		yield return new WaitForSeconds (5);
		
		//display logo
		
		fader.StartFade (Color.black, 3);
		
		yield return new WaitForSeconds (3);
		
		//black
		
		drawLogo = false;
		drawCredits = true;
		
		fader.StartFade (Color.clear, 3);
		
		yield return new WaitForSeconds (5);
		
		//display credit message
		
		fader.StartFade (Color.black, 3);
		
		yield return new WaitForSeconds (3);
		
		onMain = true;
		
		Application.LoadLevel ("main");
		
	}
	
	IEnumerator SelfDestruct () {
	
		fader.StartFade (Color.clear, 3);
		
		yield return new WaitForSeconds (3);
	
		Destroy (gameObject);
		
	}

}
