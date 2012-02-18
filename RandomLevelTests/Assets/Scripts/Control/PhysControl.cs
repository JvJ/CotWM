using UnityEngine;
using System.Collections;

public class PhysControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
		Rigidbody rb = GetComponent(typeof(Rigidbody)) as Rigidbody;
		
		rb.detectCollisions = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnControllerColliderHit(ControllerColliderHit hit){
		
		
		
		
	}
}
