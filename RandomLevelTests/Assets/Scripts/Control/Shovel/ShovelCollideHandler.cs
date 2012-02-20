using UnityEngine;
using System.Collections;

public class ShovelCollideHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter (Collision collision){
		
		print ("A COLLISION HAPPENS!");
		
		print(collision.collider.gameObject.name);
	}
	
	void OnTriggerEnter (Collider other){
		
		print ("A TRIGGER ENTERS!");
		
		print(other.gameObject.name);
	}
}
