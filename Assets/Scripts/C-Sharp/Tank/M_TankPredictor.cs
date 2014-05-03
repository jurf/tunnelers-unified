using UnityEngine;

[AddComponentMenu ("Mixed/Tank Predictor")]
[RequireComponent (typeof (NetworkView))]

public class M_TankPredictor : MonoBehaviour {

	public Transform observedTransform;
	public C_TankMan receiver; //Guy who is receiving data
	public C_PlayerMan parent;
	public float pingMargin = 0.5f; //ping top-margins
	
	float clientPing;
	M_NetState[] serverStateBuffer = new M_NetState[20];
	
	public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
	
		Vector3 pos = observedTransform.position;
		Quaternion rot = observedTransform.rotation;
		
		if (stream.isWriting) {
		
			//Debug.Log("Server is writing");
			stream.Serialize (ref pos);
			stream.Serialize (ref rot);
		
		} else {
		
			//This code takes care of the local client!
			stream.Serialize (ref pos);
			stream.Serialize (ref rot);
			receiver.serverPos = pos;
			receiver.serverRot = rot;
			//Smoothly correct clients position
			receiver.LerpToTarget ();
			
			//Take care of data for interpolating remote objects movements
			// Shift up the buffer
			//The loop is a bit complicated, but it's not my fault
			for (int i = serverStateBuffer.Length - 1; i >= 1; i--) {
				serverStateBuffer[i] = serverStateBuffer[i - 1];
			}
			
			//Override the first element with the latest server info
			serverStateBuffer[0] = new M_NetState ((float)info.timestamp, pos, rot);
			
		}
	}
	
	public void Update () {
	
		if ((Network.player == parent.GetOwner ()) || Network.isServer || (!Network.isServer && !Network.isClient)) {
			return; //This is only for remote peers, get off
		}
		//client side has !!only the server connected!!
		clientPing = (Network.GetAveragePing (Network.connections[0]) / 100) + pingMargin;
		float interpolationTime = (float)Network.time - clientPing;
		
		//ensure the buffer has at least one element:
		if (serverStateBuffer[0] == null) {
			serverStateBuffer[0] = new M_NetState (0f, transform.position, transform.rotation);
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
					M_NetState bestTarget = serverStateBuffer[Mathf.Max (i - 1, 0)];
					// The best playback state (closest current network time))
					M_NetState bestStart = serverStateBuffer[i];
					
					float timediff = bestTarget.timestamp - bestStart.timestamp;
					float lerpTime = 0f;
					// Increase the interpolation amount by growing ping
					// Reverse that for more smooth but less accurate positioning
					if (timediff > 0.0001f) {
						lerpTime = ((interpolationTime - bestStart.timestamp) / timediff);
					}
					
					transform.position = Vector3.Lerp (bestStart.pos, bestTarget.pos, lerpTime);
					
					transform.rotation = Quaternion.Slerp (bestStart.rot, bestTarget.rot, lerpTime);
					//Okay found our way through to lerp the positions, lets return here
					return;
					
				}
				
			}
			
		} /*so it appears there is no lag through latency.*/ else {
		
			M_NetState latest = serverStateBuffer[0];	
			transform.position = Vector3.Lerp (transform.position, latest.pos, 0.5f);
			transform.rotation = Quaternion.Slerp (transform.rotation, latest.rot, 0.5f);
			
		}
		
	}

}
