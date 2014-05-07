using UnityEngine;

[AddComponentMenu ("Client/Tank Man")]

public class C_TankMan : MonoBehaviour {

    public C_PlayerMan parent;
    
    public int lastMotionH;
    public int lastMotionV;
    
    public float speed = 10f;
    
    public M_TankController controller;
           
    public float positionErrorThreshold = 0.2f;
	public Vector3 serverPos;
	public Quaternion serverRot;
    
    void Awake () {
    	if (!Network.isClient || Network.isServer) {
    		enabled = false;
    		return;
    	}
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
			
			lastMotionH = M_TankController.GetHorizontalAxis (a,d);
			lastMotionV = M_TankController.GetVerticalAxis (s, w);
	        
	        networkView.RPC ("UpdateClientMotion", RPCMode.Server, lastMotionH, lastMotionV);
	        //Simulate how we think the motion should come out	        
			controller.Move (lastMotionH, lastMotionV);
	    }
	    
	    LerpToTarget ();
	}
	
	public void LerpToTarget() {
	
	//	Debug.Log ("Lerping.");
	
	//TODO seperate pos lerp and rot slerp
	
	    float distance = Vector3.Distance (transform.position, serverPos);

	    if (distance >= positionErrorThreshold) {
	        float lerp = ((1f / distance) * speed * Time.deltaTime) / 100f;
	        transform.position = Vector3.Lerp (transform.position, serverPos, lerp);
	        transform.rotation = Quaternion.Slerp (transform.rotation, serverRot, lerp);
	    }
	
	}
}
