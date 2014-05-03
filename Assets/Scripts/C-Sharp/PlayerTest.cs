using UnityEngine;
using System.Collections;

public class PlayerTest : MonoBehaviour
{
    //public float speed = 10f;
	public float horizontal;
	public float vertical;
	
	public bool a;
	public bool s;
	public bool d;
	public bool w;
 
    void Update() {
        if (networkView.isMine) {
			InputMovement();
		}
    }
 
    void InputMovement() {
		
		//Debug.Log (Input.GetAxis ("Horizontal"));
		horizontal = Input.GetAxis ("Horizontal");
		vertical = Input.GetAxis ("Vertical");
		
		a = Input.GetKey (KeyCode.A);
		s = Input.GetKey (KeyCode.S);
		d = Input.GetKey (KeyCode.D);
		w = Input.GetKey (KeyCode.W);
        
//		if (Input.GetKey(KeyCode.W))
//            rigidbody.MovePosition(rigidbody.position + Vector3.forward * speed * Time.deltaTime);
// 
//        if (Input.GetKey(KeyCode.S))
//            rigidbody.MovePosition(rigidbody.position - Vector3.forward * speed * Time.deltaTime);
// 
//        if (Input.GetKey(KeyCode.D))
//            rigidbody.MovePosition(rigidbody.position + Vector3.right * speed * Time.deltaTime);
// 
//        if (Input.GetKey(KeyCode.A))
//            rigidbody.MovePosition(rigidbody.position - Vector3.right * speed * Time.deltaTime);
    }
}
