using UnityEngine;

[AddComponentMenu ("Mixed/Tank Predictor")]
[RequireComponent (typeof (NetworkView))]

public class M_TankPredictor : MonoBehaviour {

	public Transform observedTransform;
	public C_TankMan receiver; //Guy who is receiving data
	public PlayerMan parent;
	public float pingMargin = 0.5f; //ping top-margins
	
	float clientPing;
	M_NetState[] serverStateBuffer = new M_NetState[20];
	
	public void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info) {
	
		Vector3 pos = observedTransform.position;
		Quaternion rot = observedTransform.rotation;
		
		if (stream.isWriting) {
		
		//	Debug.Log ("Server is writing. But wait, is server a server? " + Network.isServer);
			
			stream.Serialize (ref pos);
			stream.Serialize (ref rot);
		
		} else if (stream.isReading) {
		
		//	Debug.Log ("Client is reading. But wait, is client a client? " + Network.isClient);
		
			//This code takes care of the local client!
			stream.Serialize (ref pos);
			stream.Serialize (ref rot);
			receiver.serverPos = pos;
			receiver.serverRot = rot;
			//Smoothly correct clients position
			//receiver.LerpToTarget ();
			
			//Take care of data for interpolating remote objects movements
			// Shift up the buffer
			//The loop is a bit complicated, but it's not my fault
			for (int i = serverStateBuffer.Length - 1; i >= 1; i--) {
				serverStateBuffer[i] = serverStateBuffer[i - 1];
			}
			
			Debug.Log ("Recieved data. Timestamp: " + info.timestamp, gameObject);
			
			//Override the first element with the latest server info
			serverStateBuffer[0] = new M_NetState ((float)info.timestamp, pos, rot);
			
		}
	}
	
	void Update () {
	
		if ((Network.player == parent.GetOwner ()) || Network.isServer || (!Network.isServer && !Network.isClient)) {
			return; //This is only for remote peers, get off
		}
		
		Debug.Log ("Beginning prediction.", gameObject);
		
		//client side has !!only the server connected!!
		clientPing = (Network.GetAveragePing (Network.connections[0]) / 100f) + pingMargin;
		float interpolationTime = (float)Network.time - clientPing;
		
		Debug.Log ("The avarage ping is plus ping margin is: " + clientPing, gameObject);
		Debug.Log ("The interpolation time is: " + interpolationTime, gameObject);
		
		//ensure the buffer has at least one element:
		if (serverStateBuffer[0] == null) {
			Debug.Log ("ServerStateBuffer was empty.");
			serverStateBuffer[0] = new M_NetState (0f, transform.position, transform.rotation);
		}
		
		Debug.Log ("Trying interpolation.");
		//Try interpolation if possible. 
		//If the latest serverStateBuffer timestamp is smaller than the latency
		//we're not slow enough to really lag out and just extrapolate.
		if (serverStateBuffer[0].timestamp > interpolationTime) {
		
			Debug.Log ("The [0] timestamp is: " + serverStateBuffer[0].timestamp, gameObject);
			Debug.Log ("Was it bigger than iTime? " + (serverStateBuffer[0].timestamp > interpolationTime), gameObject);
			Debug.Log ("Going to check the buffer.", gameObject);
		
			for (int i = 0; i < serverStateBuffer.Length; i++) {
			
				if (serverStateBuffer[i] == null) {
					continue;
				}
				
				Debug.Log ("SSB [" + i + "] timeStamp is: " + serverStateBuffer[i].timestamp, gameObject);
				
				// Find the state which matches the interp. time or use last state
				if (serverStateBuffer[i].timestamp <= interpolationTime || i == serverStateBuffer.Length - 1) {
					
					
					Debug.Log ("It was smaller or equel iTime. It is perfect.", gameObject);
					
					// The state one frame newer than the best playback state
					M_NetState bestTarget = serverStateBuffer[Mathf.Max (i - 1, 0)];
					// The best playback state (closest current network time))
					M_NetState bestStart = serverStateBuffer[i];
					
					float timediff = bestTarget.timestamp - bestStart.timestamp;
					
					Debug.Log ("The time diff: " + timediff, gameObject);
					
					float lerpTime = 0f;
					// Increase the interpolation amount by growing ping
					// Reverse that for more smooth but less accurate positioning
					if (timediff > 0.0001f) {
						Debug.Log ("TimeDiff is bigger then little.", gameObject);
						lerpTime = ((interpolationTime - bestStart.timestamp) / timediff);
						Debug.Log ("New lerpTime: " + lerpTime, gameObject);
					}
					
					transform.position = Vector3.Lerp (bestStart.pos, bestTarget.pos, lerpTime);
					
					transform.rotation = Quaternion.Slerp (bestStart.rot, bestTarget.rot, lerpTime);
					
					Debug.Log ("Just lerped the pos & rot. Ending.", gameObject);
					//Okay found our way through to lerp the positions, lets return here
					return;
					
				}
				
			}
			
		} /*so it appears there is no lag through latency.*/ else {
		
			Debug.Log ("Interpolation isn't needed.", gameObject);
		
			M_NetState latest = serverStateBuffer[0];	
			transform.position = Vector3.Lerp (transform.position, latest.pos, 0.5f);
			transform.rotation = Quaternion.Slerp (transform.rotation, latest.rot, 0.5f);
			
		}
		
	}

}
