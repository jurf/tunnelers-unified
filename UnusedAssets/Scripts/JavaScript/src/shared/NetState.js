class NetState {

    public var timestamp : float; //The time this state occured on the network
    public var pos : Vector3; //Position of the attached object at that time
    public var rot : Quaternion; //Rotation at that time
    
    function NetState() {
        timestamp = 0.0f;
        pos = Vector3.zero;
        rot = Quaternion.identity;
    }
    
    function NetState(time : float, pos : Vector3, rot : Quaternion) {
        timestamp = time;
        this.pos = pos;
        this.rot = rot;
    }
}