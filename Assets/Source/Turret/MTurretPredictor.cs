//
//  MTurretPredictor.cs is part of Tunnelers: Unified
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
[RequireComponent (typeof (STurretMan))]
[RequireComponent (typeof (CTurretMan))]
[RequireComponent (typeof (IRotatable))]

[AddComponentMenu ("Network/Turret Man")]

public class MTurretPredictor : MonoBehaviour {

	public Transform observedTransform;
	public CTurretMan receiver; //Guy who is receiving data
	public PlayerMan parent;
	public float pingMargin = 0.5f; //ping top-margins
	
	float clientPing;
	MNetState[] serverStateBuffer = new MNetState[20];
	
	public void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info) {
		
		Quaternion rot = observedTransform.rotation;
		
		if (stream.isWriting) {
			
			//Debug.Log("Server is writing");
			stream.Serialize (ref rot);
			
		} else if (stream.isReading) {
			
			//This code takes care of the local client!
			stream.Serialize (ref rot);
			receiver.serverRot = rot;
			//Smoothly correct clients position
			//receiver.LerpToTarget ();
			
			//Take care of data for interpolating remote objects movements
			// Shift up the buffer
			//The loop is a bit complicated, but it's not my fault
			for (int i = serverStateBuffer.Length - 1; i >= 1; i--) {
				serverStateBuffer[i] = serverStateBuffer[i - 1];
			}
			
			//Override the first element with the latest server info
			serverStateBuffer[0] = new MNetState ((float)info.timestamp, Vector3.zero, rot);
			
		}
	}
	
	void Update () {
		
		if ((Network.player == parent.GetOwner ()) 
			|| Network.isServer || (!Network.isServer && !Network.isClient)) 
		{
			return; //This is only for remote peers, get off
		}
		
		//client side has !!only the server connected!!
		clientPing = (Network.GetAveragePing (Network.connections[0]) / 100f) + pingMargin;
		float interpolationTime = (float)Network.time - clientPing;
		
		//ensure the buffer has at least one element:
		if (serverStateBuffer[0] == null) {
			serverStateBuffer[0] = new MNetState (0f, Vector3.zero, transform.rotation);
		}
		
		//Try interpolation if possible. 
		//If the latest serverStateBuffer timestamp is smaller than the latency
		//we're not slow enough to really lag out and just extrapolate.
		if (serverStateBuffer[0].timestamp > interpolationTime) {
			
			for (int i = 0; i < serverStateBuffer.Length; i++) {
				
				if (serverStateBuffer[i] == null) {
					continue;
				}
				
				// Find the state which matches the interp. time or use last state
				if (serverStateBuffer[i].timestamp <= interpolationTime || i == serverStateBuffer.Length - 1) {
					
					// The state one frame newer than the best playback state
					MNetState bestTarget = serverStateBuffer[Mathf.Max (i - 1, 0)];
					// The best playback state (closest current network time))
					MNetState bestStart = serverStateBuffer[i];
					
					float timediff = bestTarget.timestamp - bestStart.timestamp;
					float lerpTime = 0f;
					// Increase the interpolation amount by growing ping
					// Reverse that for more smooth but less accurate positioning
					if (timediff > 0.0001f) {
						lerpTime = ((interpolationTime - bestStart.timestamp) / timediff);
					}
					
					transform.rotation = Quaternion.Slerp (bestStart.rot, bestTarget.rot, lerpTime);
					//Okay found our way through to lerp the positions, lets return here
					return;
					
				}
				
			}			
//		so it appears there is no lag through latency.
		} else {
			
			MNetState latest = serverStateBuffer[0];	
			transform.rotation = Quaternion.Slerp (transform.rotation, latest.rot, 0.5f);
			
		}
		
	}
}
