using UnityEngine;
using System.Collections;

public class ShovelCollideHandler : CollideHandler {
	
	#region Tunable Parameters
	
	public GameObject owner;
	
	public AudioSource shovelAudio;
	
	public MeshRenderer mRend = null;
	
	EntityControl ec;
	
	#endregion
	
	public DethmurderControl characterControl;
	
	// Use this for initialization
	protected override void Start () {
		
		base.Start();
		
		ec = owner.GetComponent(typeof(EntityControl)) as EntityControl;
		
		print (characterControl.stats.speed);
	}
	
	protected override void Update ()
	{
		base.Update ();
		
		if (hitFloat > 0){
			mRend.material.color = Color.red;
		}
		else{
			mRend.material.color = Color.green;
		}
	}
	
	protected override bool addCondition (Collider c)
	{
		return !c.gameObject.CompareTag("PLAYER") && !c.gameObject.CompareTag("PLAYERCHILD");
	}
	
	protected override void hitCollider (Collider c)
	{
		if (c == null || c.gameObject == null){
			return;
		}
		
		if (shovelAudio != null){
			shovelAudio.Play();
		}
		
		
		
		// If it is snow, destroy it
		if (c.gameObject.CompareTag("SNOW")){			
				GameObject.Destroy(c.gameObject);
		}
		// Otherwise, try to get an entityControl and send an attack to it.
		else{
	
			// Attempt to get an entity control object from the other entity
			EntityControl otherControl = c.gameObject.GetComponent(typeof(EntityControl)) as EntityControl;
			
			// If we did get one, let's do damage to it
			if (otherControl != null){
				otherControl.TakeDamage(new Attack{ damageValue = ec.stats.attack, isContact=false, element=ElementType.ICE});
			}
		}
	}
	
	
	#region Old Code
	
	/*
	// Update is called once per frame
	void Update () {
		
		// Check if we've already entered a collision before
		// the box turned red!
		if (lastCollider != null){
			if (hitFloat > 0){
				if (!triggered){
					HandleHit();
				}
				triggered = true;
			}
		}
		
		if (mRend != null){
			if (hitFloat <= 0){
				mRend.material.color = Color.green;
			}
			else{
				mRend.material.color = Color.red;
			}
		}		
	}
	
	void OnCollisionEnter (Collision collision){
		
	}
	
	/// <summary>
	/// Raises the trigger enter event.
	/// </summary>
	/// <param name='other'>
	/// Other.
	/// </param>
	void OnTriggerEnter (Collider other){
		
		if (lastCollider != null){
			if (!lastCollider.gameObject.CompareTag("PLAYER") && !lastCollider.gameObject.CompareTag("PLAYERCHILD") && hitFloat > 0){
				lastCollider = other;
			}
			else{
				lastCollider = null;
			}
		}
		
		triggered = false;
	}
	
	/// <summary>
	/// Raises the trigger exit event.
	/// </summary>
	/// <param name='other'>
	/// Other.
	/// </param>
	void OnTriggerExit (Collider other){
		
		if (other == lastCollider){
			lastCollider = null;
		}
		
		triggered = false;
	}
	
	/// <summary>
	/// Handles the hit.
	/// </summary>
	void HandleHit(){
		
		//if (!lastCollider.gameObject.CompareTag("PLAYER") && !lastCollider.gameObject.CompareTag("PLAYERCHILD") && hitFloat > 0){
		
			if (shovelAudio != null){
				shovelAudio.Play();
			}
			
			if (lastCollider.gameObject.CompareTag("SNOW")){
			
				GameObject.Destroy(lastCollider.gameObject);
			
			}
	
			// Attempt to get an entity control object from the other entity
			EntityControl otherControl = lastCollider.gameObject.GetComponent(typeof(EntityControl)) as EntityControl;
			
			
			// If we did get one, let's do damage to it
			if (otherControl != null){
				otherControl.TakeDamage(new Attack{ damageValue = ec.stats.attack, isContact=false, element=ElementType.ICE});
			}
		//}
		
	}
		 */
	
	#endregion
}
