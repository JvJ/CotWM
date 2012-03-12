using UnityEngine;
using System.Collections;

public class ShovelCollideHandler : MonoBehaviour {
	
	#region Tunable Parameters
	
	public GameObject owner;
	
	public AudioSource audio;
	
	EntityControl ec;
	
	#endregion
	
	public CharacterControl characterControl;
	
	// Use this for initialization
	void Start () {
		
		ec = owner.GetComponent(typeof(EntityControl)) as EntityControl;
		
		print("The ec is : "+ec);
		
		characterControl = owner.GetComponent(typeof(CharacterControl)) as CharacterControl;
		
		//print("It is : "+owner.GetComponent(typeof(MonoBehaviour)).GetType().ToString());
		
		print (characterControl.stats.speed);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnCollisionEnter (Collision collision){
		
	}
	
	void OnTriggerEnter (Collider other){
		
		
		
		if (other.gameObject != owner && characterControl.IsAttacking()){

		
			if (audio != null){
				audio.Play();
			}
			
			if (other.gameObject.name == "SNOW"){
			
				GameObject.Destroy(other.gameObject);
			
			}
	
			// Attempt to get an entity control object from the other entity
			EntityControl otherControl = other.gameObject.GetComponent(typeof(EntityControl)) as EntityControl;
			
			// If we did get one, let's do damage to it
			if (otherControl != null){
				otherControl.TakeDamage(new Attack{ damageValue = ec.stats.attack, isContact=false, element=ElementType.ICE});
			}
		}
	}
}
