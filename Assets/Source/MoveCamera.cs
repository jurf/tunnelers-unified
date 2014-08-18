//
//  MoveCamera.cs is part of Tunnelers: Unified
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

public class MoveCamera : MonoBehaviour {

	Vector2 resolution;
	Vector2 mousePos;
	Vector3 newCameraPos;
	public float moveBy = 5f;
	public float smoothTime = 0.2f;
	Vector3 velocity;

	void Update () {

		resolution.x = Screen.width / 2;
		resolution.y = Screen.height / 2;
		mousePos = (Vector2) Input.mousePosition - resolution;

		float percentX = (100 / resolution.x) * mousePos.x;
		float percentY = (100 / resolution.y) * mousePos.y;

		newCameraPos = new Vector3 ((percentX / 100) * moveBy, (percentY / 100) * moveBy, transform.localPosition.z);

		//transform.localPosition = newCameraPos;

		transform.localPosition = Vector3.SmoothDamp (transform.localPosition, newCameraPos, ref velocity, smoothTime);

	}

}