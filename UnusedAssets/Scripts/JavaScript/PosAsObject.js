var object : Transform;
var x : boolean;
var y : boolean;
var z : boolean;
var addPos : Vector3;


function Update () {

if (x)
	transform.position.x = object.position.x + addPos.x;
else 
	transform.position.x = addPos.x;

if (y)
	transform.position.y = object.position.y + addPos.y;
else 
	transform.position.y = addPos.y;

if (z)
	transform.position.z = object.position.z + addPos.z;
else 
	transform.position.z = addPos.z;

}