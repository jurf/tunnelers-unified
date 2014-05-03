var explosion : Transform;

function Update () {

transform.position = mouse.mousepos;

if (Input.GetMouseButtonDown (0))
{
Instantiate (explosion, transform.position, Quaternion.identity);
}


}