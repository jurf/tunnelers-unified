var startGO : Transform;
var rocket : Transform;
var explosion : Transform;
var rocketSpeed : float;
var delay : float;
var coolDown : float;
var rocketDistance : float;
var waitForExplode : float;
var canFire : boolean = true;
var rayLength : float;

function Update () {

	if (Input.GetMouseButton (0)) {
		if (canFire) {
			ShootRocket ();
			Wait ();
			canFire = false;
		}	
	}

}

function ShootRocket () {

	var startPoint = startGO.position;
	var rocket = Instantiate (rocket, startPoint, transform.rotation);
	
	rocket.rigidbody.AddForce (rocket.forward * rocketSpeed);
	rocket.GetComponent (RocketIdentifier).from = gameObject;
	
	while (Vector3.Distance (startPoint, rocket.transform.position) < rocketDistance) {
		if (rocket.tag != "RocketTroll")
			return;
		
		var hit : RaycastHit;
		if (Physics.Raycast (rocket.position, rocket.forward, hit, rayLength)) {
			//Explode (rocket);
			rocket.tag = "ExplodeRocket";
			return;
		} else {
			yield;
		}
	}
	
	rocket.tag = "ExplodeRocket";
	//Explode (rocket);

	return;
	
}

function Explode (rocket : Transform) {
	
	rocket.particleEmitter.emit = false;
	rocket.renderer.enabled = false;
	Instantiate (explosion, rocket.position, rocket.rotation);
	
}

function Wait () {

	yield WaitForSeconds (coolDown);
	canFire = true;
	
}
	