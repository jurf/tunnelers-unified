//
//  C_TurretMan.cs is part of Tunnelers: Unified
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
[RequireComponent (typeof (S_TurretMan))]
[RequireComponent (typeof (M_TurretPredictor))]
[RequireComponent (typeof (M_TankController))]

[AddComponentMenu ("Network/Turret Man")]

public class C_TurretMan : MonoBehaviour {
	
	//Those are stored to only send RPCs to the server when the 
	//data actually changed.
	float lastYRotation;
	
	public float speed = 10f;
	/*public float maxVelocityChange = 10f;
    public float maxVelocitychangeStop = 2f;
    Quaternion lastSuccRotation;
    Quaternion lastRotation;*/
	
	public M_TankController controller;
	public PlayerMan parent;
	
	void Awake () {
	
		if (!Network.isClient || Network.isServer) {
			enabled = false;
			return;
		}
		
	}
	
	/// <summary>
	///Get the M_TankController instance from the current GameObject. 
	/// </summary>
	void Start () {
		if (Network.isClient) {
			controller = GetComponent<M_TankController>();
//			parent = transform.parent.gameObject.GetComponent <C_PlayerMan> ();
		}
	}
	
	/// <summary>
	///Calculate new rotation, send it to the server and try to simulate how it will work out. 
	/// </summary>
	void Update () {
		
		if (Network.isServer || (!Network.isServer && !Network.isClient)) {
			enabled = false;
			return; //get lost, this is the client side!
		}
		
		//Check if this update applies for the current client
		if (parent.GetOwner () != null && Network.player == parent.GetOwner ()) {
			
			Quaternion toRot = M_TankController.RotateToMouse (gameObject);
			
			networkView.RPC ("UpdateClientRotation", RPCMode.Server, toRot.eulerAngles.y);
			controller.Rotate (toRot);
		}
		
		LerpToTarget ();
	}
	
	public float positionErrorThreshold = 0.2f;
	public Quaternion serverRot;
	
	/// <summary>
	///Smoothly lerps to the server position and rotation.
	/// </summary>
	public void LerpToTarget () {
		
		float distance = Quaternion.Angle (transform.rotation, serverRot);
		
		//only correct if the error margin (the distance) is too extreme
		if (distance >= positionErrorThreshold) {
			float lerp = ((1f / distance) * speed * Time.deltaTime) / 100f;
			//Debug.Log("Lerp time: " + lerp);
			transform.rotation = Quaternion.Slerp (transform.rotation, serverRot, lerp);
		}
		
	}
}
