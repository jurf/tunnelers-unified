#pragma strict

@RequireComponent(NetworkView)
class PlayerManager extends MonoBehaviour {
    
    public var speed : float = 10;
    
    private var controller : CharacterController;
    
    private var horizontalMotion : float;
    private var verticalMotion : float;
    
    public function Start() {
        if (Network.isServer) {
            controller = GetComponent(CharacterController);
        }
    }
    
    public function Update() {
        if (Network.isClient) {
            return; //Get lost, this is the server-side!
        }
        //Debug.Log("Processing clients movement commands on server");
        controller.Move(Vector3(
        horizontalMotion * speed * Time.deltaTime, 
        0, 
        verticalMotion * speed * Time.deltaTime));
    }
    
    /**
     * The client calls this to notify the server about new motion data
     * @param	motion
     */
    @RPC
    public function updateClientMotion(hor : float, vert : float) : void {
        horizontalMotion = hor;
        verticalMotion = vert;
    }
}