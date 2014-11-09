//
//  CTankMan.cs is part of Tunnelers: Unified
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
[RequireComponent (typeof (STankMan))]
[RequireComponent (typeof (MTankPredictor))]
[RequireComponent (typeof (IMovable <int>))]

[AddComponentMenu ("Network/Tank Man")]

public class CTankMan : MonoBehaviour {

    public PlayerMan parent;
    
    int lastMotionH;
    int lastMotionV;
    
	public IMovable <int> controller;
	public MTankPredictor predictor;

	public float speed = 10f;       
    public float positionErrorThreshold = 0.5f;
	public byte maxErrors = 3;

	byte positionErrors;

	public Vector3 serverPos;
	public Quaternion serverRot;
    
    void Awake () {

    	if (!Network.isClient || Network.isServer) {
    		enabled = false;
    		return;
    	}

		if (controller == null)
			controller = (IMovable <int>) GetComponent (typeof (IMovable <int>));

    }
    
    void Update () {
    
		if (Network.isServer || (!Network.isServer && !Network.isClient)) {
			enabled = false;
	        return;
	    }

	    if (parent.GetOwner () != null && Network.player == parent.GetOwner ()) {
		//	Debug.Log ("I am the owner.");
			bool a = Input.GetKey (KeyCode.A);
			bool d = Input.GetKey (KeyCode.D);
			bool s = Input.GetKey (KeyCode.S);
			bool w = Input.GetKey (KeyCode.W);
			
			lastMotionH = MTankController.GetHorizontalAxis (a,d);
			lastMotionV = MTankController.GetVerticalAxis (s, w);
	        
	        networkView.RPC ("UpdateClientMotion", RPCMode.Server, lastMotionH, lastMotionV);
	        //Simulate how we think the motion should come out	        
			controller.Move (lastMotionH, lastMotionV);
	    }
	    
	    LerpToTarget ();
	}
	
	public void LerpToTarget () {
	
	//	Debug.Log ("Lerping.");
	
		LerpPos ();
		LerpRot ();
	
	}
	
	void LerpPos () {
	
		/*
		float distance = Vector3.Distance (transform.position, serverPos);
	
		float timing = distance / speed;
		float rate = 1f / timing;
		float t = 0f;
		t += Time.deltaTime * rate;
	    	
	//	float lerp = ((1f / distance) * speed * Time.deltaTime) / 100f;
	    transform.position = Vector3.Lerp (transform.position, serverPos, t);
	    */

		float distance = Vector3.Distance (transform.position, serverPos);

		if (distance < predictor.maxDeltaPos)
			return;
	    
		if (distance < positionErrorThreshold) {
			positionErrors ++;
		}

		if (positionErrors < maxErrors) {
			transform.position = serverPos;
			positionErrors = 0;
			return;
		}

	    transform.position = Vector3.Lerp (transform.position, serverPos, Time.deltaTime * speed);
	}
	
	void LerpRot () {
		
		/*
		float distance = Quaternion.Angle (transform.rotation, serverRot);
	
		float timing = distance / (speed * 1000f);
		float rate = 1f / timing;
		float t = 0f;
		t += Time.deltaTime * rate;
		
		transform.rotation = Quaternion.Slerp (transform.rotation, serverRot, t);
		*/
		
		transform.rotation = Quaternion.Slerp (transform.rotation, serverRot, Time.deltaTime);
		
	}

}
