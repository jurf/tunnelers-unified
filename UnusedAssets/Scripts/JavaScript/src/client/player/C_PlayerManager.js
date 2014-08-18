#pragma strict
@RequireComponent(NetworkView)
/**
 * Client-side PlayerMovement implementation, only send
 * motion data to the server.
 */
class C_PlayerManager extends MonoBehaviour {
    //That's actually not the owner but the player,
    //the server instantiated the prefab for, where this script is attached
    private var owner : NetworkPlayer;
    
    //Those are stored to only send RPCs to the server when the 
    //data actually changed.
    private var lastMotionH : float; //horizontal motion
    private var lastMotionV : float; //vertical motion
    
    public var speed : float = 10;
    
    private var controller : CharacterController;
    
    public function Start() {
        if (Network.isClient) {
            controller = GetComponent(CharacterController);
        }
    }
    
    @RPC
    function setOwner(player : NetworkPlayer) : void {
        Debug.Log("Setting the owner.");
        owner = player;
        if(player == Network.player){
            //So it just so happens that WE are the player in question,
            //which means we can enable this control again
            enabled=true;
        }
        else {
            //Disable a bunch of other things here that are not interesting:
            if (GetComponent(Camera)) {
                GetComponent(Camera).enabled = false;
            }
            
            if (GetComponent(AudioListener)) {
                GetComponent(AudioListener).enabled = false;
            }
            
            if (GetComponent(GUILayer)) {
                GetComponent(GUILayer).enabled = false;
            }
        }
    }
    
    @RPC
    function getOwner() : NetworkPlayer {
        return owner;
    }
    
    public function Awake() {
        //Disable this by default for now
        //Just to make sure no one can use this until we didn't
        //find the right player. (see setOwner())
        if (Network.isClient) {
            enabled = false;
        }
    }
    
    public function Update () {
	    if (Network.isServer) {
	        return; //get lost, this is the client side!
	    }
	    //Check if this update applies for the current client
	    if ((owner != null) && (Network.player == owner)) {
	        var motionH : float = Input.GetAxis("Horizontal");
	        var motionV : float = Input.GetAxis("Vertical");
	        networkView.RPC("updateClientMotion", RPCMode.Server, motionH, motionV);
	        //Simulate how we think the motion should come out
	        controller.Move(Vector3(
	        motionH * speed * Time.deltaTime, 
	        0, 
	        motionV * speed * Time.deltaTime));
	    }
	}
    
    public var positionErrorThreshold : float = 0.2;
	public var serverPos : Vector3;
	public var serverRot : Quaternion;
	
	public function lerpToTarget() {
	    var distance = Vector3.Distance(transform.position, serverPos);
	    
	        //only correct if the error margin (the distance) is too extreme
	    if (distance >= positionErrorThreshold) {
	        var lerp = ((1 / distance) * speed) / 100;
	        //Debug.Log("Lerp time: " + lerp);
	        transform.position = Vector3.Lerp(transform.position, serverPos, lerp);
	        transform.rotation = Quaternion.Slerp(transform.rotation, serverRot, lerp);
	    }
	}

}