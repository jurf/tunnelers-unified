//
//  M_GameMan.cs is part of Tunnelers: Unified
//  <https://github.com/VacuumGames/Tunnelers-Unified/>.
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
[RequireComponent (typeof (S_GameMan))]
[RequireComponent (typeof (C_GameMan))]

[AddComponentMenu ("Network/Game Man")]

public class M_GameMan : MonoBehaviour {

	public S_GameMan observedGameMan;
	public C_GameMan recieverGameMan;

	public void OnSerializeNetworkView (BitStream stream) {
	
		int timeToEnd = (int)observedGameMan.timeToEnd;
		
		if (stream.isWriting) {
		
		//	Debug.Log("Server is writing");
			stream.Serialize (ref timeToEnd);
		
		} else {
		
			stream.Serialize (ref timeToEnd);

			recieverGameMan.serverTimeToEnd = timeToEnd;
			
		}
	}
	
}
