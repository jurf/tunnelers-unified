var resolution : Vector2;
var mousePos : Vector2;
var newCameraPos : Vector3;
var moveBy : float;
var smoothTime : float;
private var velocity = Vector3.zero;

function Update () {
	
	resolution.x = Screen.width / 2;
	resolution.y = Screen.height / 2;
	mousePos = Input.mousePosition - resolution;
	
	var percentX = (100 / resolution.x) * mousePos.x;
	var percentY = (100 / resolution.y) * mousePos.y;
	
	newCameraPos = Vector3 ((percentX / 100) * moveBy, (percentY / 100) * moveBy, transform.localPosition.z);
	
	//transform.localPosition = newCameraPos;
	
	transform.localPosition = Vector3.SmoothDamp(transform.localPosition, newCameraPos, velocity, smoothTime);

}
	
	