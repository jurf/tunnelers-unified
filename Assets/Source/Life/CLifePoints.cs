//
//  CLifePoints.cs is part of Tunnelers: Unified
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

[RequireComponent (typeof (NetworkView))]
[RequireComponent (typeof (SLifePoints))]

[AddComponentMenu ("Network/Life")]

public class CLifePoints : MonoBehaviour {
	
	public SLifePoints sscript;
	
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
			GUILayout.BeginHorizontal ();
			
				GUILayout.Label ("Energy: " + sscript.EnergyPoints, "box");
				GUILayout.FlexibleSpace ();
				
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			
				GUILayout.Label ("Shield: " + sscript.ShieldPoints, "box");
				GUILayout.FlexibleSpace ();
				
			GUILayout.EndHorizontal ();
			
		GUILayout.EndVertical ();
		GUILayout.EndArea ();
		
	}
	
}
