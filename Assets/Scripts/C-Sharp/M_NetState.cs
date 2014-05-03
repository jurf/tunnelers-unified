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
