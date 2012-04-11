using UnityEngine;
using System.Collections;

public class WyrmControl : EntityControl {

	#region Editable Params
	
	public float minWanderTime = 1f;
	
	public float maxWanderTime = 2f;
		
	public float attackTime = 2f;
	
	#endregion
	
	private float currentWanderTimeOut;
	
	private float currentWanderTime;
	
	private float currentAttackTime;
	
	private float moveDirection;
	
	private Transform tempTarget;
	
	// Use this for initialization
	void Start () {
		base.Start();
		
		animation["WormWiggle"].wrapMode = WrapMode.Loop;
		
		currentWanderTime = maxWanderTime;
		
		currentWanderTimeOut = Random.Range(minWanderTime, maxWanderTime);
		
		currentAttackTime = 0f;
	}
	
	#region State Functions
	
	public override void TakeDamage (Attack atk)
	{
		stats.DoDamage(atk.damageValue, false);
	}
	
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
	
	private Vector3 attachPosition;
	public override void Attacking ()
	{
		base.Attacking ();
		
		transform.localPosition = attachPosition;
		
		currentAttackTime += Time.deltaTime;
		
		if (currentAttackTime >= attackTime){
			currentAttackTime = 0f;
			player.TakeDamage(new Attack{damageValue=stats.attack, element = ElementType.NONE, isContact = false});
		}
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
			/*
			// Set up the hinge's rigid body
			var rb = g.AddComponent(typeof(Rigidbody)) as Rigidbody;
			rb.useGravity = false;
			rb.isKinematic = true;
			
			var hj = g.AddComponent(typeof(HingeJoint)) as HingeJoint;
			
			hj.anchor = g.transform.position;
			
			
			rigidbody.useGravity = false;
			collider.isTrigger = true;
			rigidbody.velocity = Vector3.zero;
			*/
			
			var v = Random.insideUnitCircle;
			v.x = Mathf.Abs(v.x);
			attachPosition = new Vector3(v.x, v.y, 0);
			
			collider.isTrigger = true;
			
			SwitchState(EntityState.ATTACKING);
		}
		
	}
	
	public override void setScript (bool onOff)
	{
		base.setScript (onOff);
		
		animation.Play("WormWiggle");
		
		collider.isTrigger = !onOff;
		
		rigidbody.isKinematic = !onOff;
		rigidbody.useGravity = onOff;
	}
	
	#endregion
}
