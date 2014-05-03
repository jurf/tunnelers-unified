static var mousepos : Vector3;
function Update () 
{
	var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
	var hit : RaycastHit;
	if (Physics.Raycast (ray, hit,1000))
	{
		mousepos = hit.point;
	}
}