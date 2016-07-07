//
//  CameraMovement.cs is part of Tunnelers: Unified
//  <https://github.com/VacuumGames/tunnelers-unified/>.
//
//  Copyright (c) 2014-2016 Juraj Fiala<doctorjellyface@riseup.net>
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

public class CameraMovement: MonoBehaviour {

	public Transform target;
	public float moveBy = 5f;
	public float smoothTime = 0.2f;

	Vector3 velocity;
	Vector3 oldPos;

	void Update () {

		Vector3 zero = new Vector3 (target.position.x, transform.position.y, target.position.z);

		Vector2 center;
		center.x = Screen.width;
		center.y = Screen.height;
		center /= 2;

		Vector2 mousePos = (Vector2) Input.mousePosition - center;

		float percentX = mousePos.x / center.x;
		float percentY = mousePos.y / center.y;

		Vector3 newRelPos = new Vector3 (percentX * moveBy, 0, percentY * moveBy);

		Vector3 newPos = Vector3.SmoothDamp (oldPos, newRelPos, ref velocity, smoothTime);
		transform.position = zero + newPos;

		oldPos = newPos;
	}

}
