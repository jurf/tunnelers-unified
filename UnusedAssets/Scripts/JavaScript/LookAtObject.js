var target : Transform;
var speed : float;


function Update () {

//	var targetRotation = Quaternion.LookRotation(target);
//	transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
	
	transform.LookAt(target);

}