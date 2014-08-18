//
//  M_NetState.cs is part of Tunnelers: Unified
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

public class M_NetState {

	public float timestamp; //The time this state occured on the network
	public Vector3 pos; //Position of the attached object at that time
	public Quaternion rot; //Rotation at that time
	
	public M_NetState () {
	
		timestamp = 0f;
		pos = Vector3.zero;
		rot = Quaternion.identity;
	
	}
	
	public M_NetState (float time, Vector3 pos, Quaternion rot) {
	
		timestamp = time;
		this.pos = pos;
		this.rot = rot;
	
	}
}
