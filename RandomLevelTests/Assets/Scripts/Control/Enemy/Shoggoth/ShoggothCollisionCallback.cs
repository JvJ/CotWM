using UnityEngine;
using System.Collections;

public class ShoggothCollisionCallback : ChildCallback {
	
	
	// Use this for initialization
	void Start () {
	
		//var c = GetComponent(typeof(Rigidbody)) as Rigidbody;
		
		//c.mo
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter(Collision collision){
		
		print("Collision entered!");
		
		var pc = parentControl as ShoggothControl;
		
		foreach (var c in collision.contacts){
			
			// Draw 'em up, thar!
			Debug.DrawLine(c.point, c.point+c.normal);
			
			// Only horizontal hits count, yo!
			if (c.normal.y == 0){
				if (c.otherCollider.gameObject.CompareTag("SNOW")
				    || c.otherCollider.gameObject.CompareTag("ROCK"))
				{
					// Reset the timer!
					pc.wanderCountDown = pc.wanderTimeOut;
				}
			}
		}
	}
}
