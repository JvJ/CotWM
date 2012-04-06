using UnityEngine;
using System.Collections;

public class WyrmControl : EntityControl {

	#region Editable Params
	
	public float minWanderTime = 1f;
	
	public float maxWanderTime = 2f;
		
	#endregion
	
	private float currentWanderTimeOut;
	
	private float currentWanderTime;
	
	private float moveDirection;
	
	private Transform tempTarget;
	
	// Use this for initialization
	void Start () {
		base.Start();
		
		animation["WormWiggle"].wrapMode = WrapMode.Loop;
		
		currentWanderTime = maxWanderTime;
		
		currentWanderTimeOut = Random.Range(minWanderTime, maxWanderTime);
	}
	
	#region State Functions
	
	
	/// <summary>
	/// Randomly move back and forth.
	/// </summary>
	public override void Wander ()
	{
		// Select a direction to move and a time-out
		if (currentWanderTime >= currentWanderTimeOut){
			
			// Randomly select -1 or +1
			moveDirection = Mathf.Sign(Random.Range(-1, 1));
			
			// Set the rotation
			transform.rotation = Quaternion.Euler(new Vector3(0, moveDirection < 0 ? -90 : 90,0));
			
			currentWanderTimeOut = Random.Range(minWanderTime, maxWanderTime);
			
			currentWanderTime = 0f;
		}
		
		// Move in the direction
		rigidbody.MovePosition(transform.position + new Vector3(moveDirection * stats.speed * Time.deltaTime, 0, 0));
		
		// Run the timer
		currentWanderTime += Time.deltaTime;
	}
	
	public override void Attacking ()
	{
		base.Attacking ();
	}
	
	/// <summary>
	/// Detect collisions with the player.
	/// </summary>
	/// <param name='collision'>
	/// Collision.
	/// </param>
	public void OnCollisionEnter(Collision collision){
		
		// If it's the player, create a new game object, parent it to the player,
		// and attach this object to it
		if (currentState == EntityState.WANDER && 
			(collision.gameObject.CompareTag("PLAYER") ||
			collision.gameObject.CompareTag("PLAYERCHILD"))){
			
			var g = new GameObject("WyrmTarget");
			
			// Get a contact point
			g.transform.position = collision.contacts[0].point;
			
			// Parent it to the player transform
			transform.parent = g.transform.parent = player.transform;
			
			// Set up the hinge's rigid body
			var rb = g.AddComponent(typeof(Rigidbody)) as Rigidbody;
			rb.useGravity = false;
			rb.isKinematic = true;
			
			var hj = g.AddComponent(typeof(HingeJoint)) as HingeJoint;
			
			
			rigidbody.useGravity = false;
			collider.isTrigger = true;
			rigidbody.velocity = Vector3.zero;
			
			
			
			SwitchState(EntityState.ATTACKING);
		}
		
	}
	
	
	#endregion
}
