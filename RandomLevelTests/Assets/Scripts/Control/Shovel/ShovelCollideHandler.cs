using UnityEngine;
using System.Collections;

public class ShovelCollideHandler : MonoBehaviour {
	
	#region Tunable Parameters
	
	public GameObject owner;
	
	#endregion
	
	public CharacterControl characterControl;
	
	// Use this for initialization
	void Start () {
		
		characterControl = owner.GetComponent(typeof(CharacterControl)) as CharacterControl;
		
		//print("It is : "+owner.GetComponent(typeof(MonoBehaviour)).GetType().ToString());
		
		print (characterControl.moveSpeed);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnCollisionEnter (Collision collision){
		
	}
	
	void OnTriggerEnter (Collider other){
		
		if (characterControl.IsAttacking() && other.gameObject.name == "SNOW"){
			
			GameObject.Destroy(other.gameObject);
			
		}
	}
}
