var digging : boolean;
var digSpeed : float;
var moveSpeed : float;
var currentSpeed : float;
var maxVelocityChange = 10.0;
var toRot : Vector3;
var newRot : Vector3;
var fromRot : Vector3;
var check : boolean;
var rotVariation : float;
var posVariation : float;
var targetRots : Vector3[];
var newRots : Vector3[];
var keys : float[];
var rotating : boolean;
var stopRotating : boolean;
var timeSinceLastDig : float;
var timeVariation : float;
var dirtShower : ParticleEmitter;

var timeScale : float = 1;

//player movement
function Update () {

	if (digging) {
		currentSpeed = digSpeed;
		dirtShower.emit = true;
		if (Time.realtimeSinceStartup - timeSinceLastDig > timeVariation) {
			digging = false;
		}
	} else {
		currentSpeed = moveSpeed;
		dirtShower.emit = false;
		if (Time.realtimeSinceStartup - timeSinceLastDig < timeVariation) {
			digging = true;
		}
	}
	
	
	Time.timeScale = timeScale;

	if(Input.GetKey(KeyCode.W)) {
		keys[0] = 1;
		newRots[0] = targetRots[0];
	} else {
		keys[0] = 0;
		newRots[0] = Vector3 (0,0,0);
	}

	if(Input.GetKey(KeyCode.D)) {
		keys[1] = 1;
		newRots[1] = targetRots[1];
	} else {
		keys[1] = 0;
		newRots[1] = Vector3 (0,0,0);
	}

	if(Input.GetKey(KeyCode.S)) {
		keys[2] = 1;
		newRots[2] = targetRots[2];
	} else {
		keys[2] = 0;
		newRots[2] = Vector3 (0,0,0);
	}

	if(Input.GetKey(KeyCode.A)) {
		keys[3] = 1;
		newRots[3] = targetRots[3];
	} else {
		keys[3] = 0;
		newRots[3] = Vector3 (0,0,0);
	}
	
	var totalKeys = keys[0] + keys[1] + keys[2] + keys[3];
	var totalRot = newRots[0] + newRots[1] + newRots[2] + newRots[3];
	
	if (keys[0] + keys[3] == 2 && totalRot == Vector3 (0, 270, 0)) {
		newRot = Vector3 (0, 315, 0); 
	} else if (totalKeys != 0) {
		newRot = totalRot / totalKeys;
	}/* else {
		newRot = totalRot;
	}*/
	
	var sa : float = keys[0] + keys[2]; 
	var ws : float = keys[1] + keys[3];
	
	if (sa < 2 && ws < 2) {
		if (Vector3.Distance (toRot, newRot) > posVariation && !rotating) {
			stopRotating = false;
			Rotation (transform.rotation, Quaternion.Euler(newRot), 0.3);
		} else if (Vector3.Distance (toRot, newRot) > posVariation && rotating) {
			stopRotating = true;
			Rotation (transform.rotation, Quaternion.Euler(newRot), 0.3);
		} else if (!rotating) {
			stopRotating = false;
		}
	}
	
	if (stopRotating) {
		rotating = false;
	}
	
	if (Quaternion.Angle(transform.rotation, Quaternion.Euler(newRot)) < rotVariation) {
		check = true;
		rotating = false;
	} else {
		check = false;
	}
	
	if (sa < 2 && ws < 2) {				
		if (check || Quaternion.Angle (Quaternion.Euler (fromRot), Quaternion.Euler (newRot)) < 47) {
			if (Input.GetKey(KeyCode.W) || Input.GetKey (KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey (KeyCode.D)) {
				transform.Translate(Vector3.forward * currentSpeed * Time.smoothDeltaTime);
				/*var targetVelocity = new Vector3(0, 0, 1);
				targetVelocity = transform.TransformDirection(targetVelocity);
				targetVelocity *= currentSpeed;
 
				// Apply a force that attempts to reach our target velocity
				var velocity = rigidbody.velocity;
				var velocityChange = (targetVelocity - velocity);
				velocityChange.x = 0;
				velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
				velocityChange.y = 0;
				rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);*/
			}	
		}
	}
}
		
		
/*
//W & S Foward/Backward
if(Input.GetKey(KeyCode.W))
{
transform.Translate(0,0,speed * Time.deltaTime,Space.Self);
//Debug.Log("Does W Work?");
}

if(Input.GetKey(KeyCode.S))
{
transform.Translate(0,0,-speed * Time.deltaTime,Space.Self);
//Debug.Log("Does S Work?");
}


//A & D side to side
if(Input.GetKey(KeyCode.A))
{
transform.Translate(-speed * Time.deltaTime,0,0,Space.Self);
//Debug.Log("Does A Work?");
}

if(Input.GetKey(KeyCode.D))
{
transform.Translate(speed * Time.deltaTime,0,0,Space.Self);
//Debug.Log("Does D Work?");
}

//Q and E keys rotate:


if(Input.GetKey(KeyCode.Q))
{
transform.Rotate(0,-rotateSpeed * Time.deltaTime,0,Space.Self);
//Debug.Log("Does Q Work?");
}

if(Input.GetKey(KeyCode.E))
{
transform.Rotate(0,rotateSpeed * Time.deltaTime,0,Space.Self);
//Debug.Log("Does E Work?");
}

}

@script RequireComponent(CharacterController)
*/
function Rotation (from : Quaternion, to : Quaternion, time : float) { 
	if (stopRotating) return;
	fromRot = from.eulerAngles;
	rotating = true;
	toRot = to.eulerAngles;
	
	var timing = Quaternion.Angle (from, to) / (time * 1000);
	var rate = 1.0/timing; 
	var t = 0.0; 

	while (t < 1.0) {
		if (stopRotating) return;
		t += Time.deltaTime * rate; 
		transform.rotation = Quaternion.Slerp(from, to, t);
		yield; 
	} 

}

function YouDug () {
	timeSinceLastDig = Time.realtimeSinceStartup;
}