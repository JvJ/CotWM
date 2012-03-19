using UnityEngine;
using System.Collections;

public class ShoggothControl : EntityControl{

	public CharacterController controller;
	public Collider coll;
	
	// Use this for initialization
	void Start () {
		base.Start();
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
	private float wanderCountDown = 0;
	
	
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
		controller.SimpleMove(new Vector3(wanderDirection * stats.speed, 0, 0));
		
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
	
	#endregion
}
