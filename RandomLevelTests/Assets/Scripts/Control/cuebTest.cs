using UnityEngine;
using System.Collections;

public class cuebTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetButton("Whack")){
			animation.Play("OpenCloak");
		}
		/*else if (Input.GetButton("Jump")){
			animation.Play("TurnRight");
		}*/
	}
}
