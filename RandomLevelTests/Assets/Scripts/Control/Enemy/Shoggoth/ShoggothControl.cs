using UnityEngine;
using System.Collections;

public class ShoggothControl : EntityControl{

	public Rigidbody rb;
	public Collider coll;
	
	/// <summary>
	/// How fast will the player go when he gets hit??
	/// </summary>
	public float playerWhackIntensity;
	
	// Use this for initialization
	void Start () {
		base.Start();
		
		rb.WakeUp();
	}
	
	
	public override void TakeDamage (Attack atk)
	{
		float dmg = atk.damageValue;
		
		if (atk.element == ElementType.ICE){
			dmg *= 2;
		}
		
		stats.DoDamage(dmg, false);
	}
	
	#region State Functions
	
	#region Wander Parameters
	
	/// <summary>
	/// Time-out for wandering.
	/// </summary>
	public float wanderTimeOut = 5;
	
	
	/// <summary>
	/// The amount of time left in the wandering
	/// </summary>
	public float wanderCountDown = 0;
	
	
	/// <summary>
	/// If -1, they will wander left.  Otherwise, they will wander right.
	/// </summary>
	public int wanderDirection;
	
	#endregion
	
	public override void Wander ()
	{

		//print("Something is happening");
		
		// Execute the wandering
		//controller.AddForce(new Vector3(wanderDirection * stats.speed, 0, 0));
		var v = new Vector3(wanderDirection * stats.speed * Time.deltaTime, 0, 0);
		rb.MovePosition(transform.position + v);
			//SimpleMove(new Vector3(wanderDirection * stats.speed, 0, 0));
		
		// Update the timer
		wanderCountDown -= Time.deltaTime;
		
		// Reset the time counter if necessary
		if (wanderCountDown <= 0){
			
			wanderDirection = Random.value - 0.5 < 0 ? -1 : 1;
			
			wanderCountDown = wanderTimeOut;
		}
		
		// Check if we can see the player
		//if (castToPlayer()){
		//	SwitchState(EntityState.CLOSE_IN);
		//}
	}
	
	#endregion
	
	#region Collision Handlers
	
	void OnColliderControllerHit (ControllerColliderHit hit){
		
		
		// Only horizontal hits count, yo!
		if (hit.normal.y == 0){
			if (hit.collider.gameObject.CompareTag("SNOW")
			    || hit.collider.gameObject.CompareTag("ROCK"))
			{
				// Reset the timer!
				wanderCountDown = wanderTimeOut;
			}
		}
	}
	
	void OnCollisionEnter(Collision collision){
		
		foreach (var c in collision.contacts){
			Debug.DrawLine(c.point, c.point + c.normal, Color.white, 1.0f);
			
			// Only horizontal hits count, yo!
			if (c.normal.y == 0){
				if (c.otherCollider.gameObject.CompareTag("SNOW")
				    || c.otherCollider.gameObject.CompareTag("ROCK"))
				{
					// Reset the timer!
					//wanderCountDown = wanderTimeOut;
					wanderDirection = -wanderDirection;
					return;
				}
			}
		}
	}
	
	#endregion
}
