//
//  PlayerTest.cs is part of Tunnelers: Unified
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

public class PlayerTest : MonoBehaviour {

    //public float speed = 10f;
	public float horizontal;
	public float vertical;
	
	public bool a;
	public bool s;
	public bool d;
	public bool w;
 
    void Update () {

        if (GetComponent<NetworkView>().isMine) {
			InputMovement();
		}

    }
 
    void InputMovement () {
		
		//Debug.Log (Input.GetAxis ("Horizontal"));
		horizontal = Input.GetAxis ("Horizontal");
		vertical = Input.GetAxis ("Vertical");
		
		a = Input.GetKey (KeyCode.A);
		s = Input.GetKey (KeyCode.S);
		d = Input.GetKey (KeyCode.D);
		w = Input.GetKey (KeyCode.W);
        
//		if (Input.GetKey(KeyCode.W))
//            rigidbody.MovePosition(rigidbody.position + Vector3.forward * speed * Time.deltaTime);
// 
//        if (Input.GetKey(KeyCode.S))
//            rigidbody.MovePosition(rigidbody.position - Vector3.forward * speed * Time.deltaTime);
// 
//        if (Input.GetKey(KeyCode.D))
//            rigidbody.MovePosition(rigidbody.position + Vector3.right * speed * Time.deltaTime);
// 
//        if (Input.GetKey(KeyCode.A))
//            rigidbody.MovePosition(rigidbody.position - Vector3.right * speed * Time.deltaTime);
    }
}
