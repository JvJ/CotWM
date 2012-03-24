using UnityEngine;
using System.Collections;

public class FHeadmanControl : EntityControl {
	
	#region Editor Variables
	
	public CharacterController controller;
	
	//public float moveSpeed = 10;
	
	#endregion
	
	// Use this for initialization
	public override void Start () {
		base.Start();
	}
	
	public override void TakeDamage (Attack atk)
	{
		float dmg = atk.damageValue;
		
		// This guy is weak against ice
		if (atk.element == ElementType.ICE){
			dmg *= 2;
		}
		
		stats.DoDamage(dmg, false);
	}
	
	#region State Functions
	
	
	#region Wandering Parameters
	
	public float directionTimeOut;
	
	private float directionTimeCounter = 0;
	
	private WanderDirection wDir = WanderDirection.RIGHT;
	
	#endregion
	
	/// <summary>
	/// 
	/// </summary>
	public override void Wander ()
	{
		base.Wander ();
		
		// If we're timed-out, select a new direction.
		if (directionTimeCounter <= 0 || directionTimeCounter >= directionTimeOut){
			directionTimeCounter = 0;
			
			wDir = (WanderDirection)((int) (Random.value * (int)WanderDirection.NumElements));
			
			// Have to make sure we don't pick this one.  It's possible that the
			// RNG picks exactly 1.0.
			if (wDir == WanderDirection.NumElements){
				wDir = WanderDirection.RIGHT;
			}
		}
		
		Vector3 moveVector = Vector3.zero;
		
		switch(wDir){
		case WanderDirection.UP:
			moveVector.y = stats.speed * Time.deltaTime;
			break;
		case WanderDirection.DOWN:
			moveVector.y = -stats.speed * Time.deltaTime;
			break;
		case WanderDirection.LEFT:
			moveVector.x = -stats.speed * Time.deltaTime;
			transform.rotation = Quaternion.Euler(new Vector3(0,90,0));
			break;
		case WanderDirection.RIGHT:
			moveVector.x = stats.speed * Time.deltaTime;
			transform.rotation = Quaternion.Euler(new Vector3(0,-90,0));
			break;
		}
		
		controller.Move(moveVector);
		
		directionTimeCounter += Time.deltaTime;
		
		// Check for state transitions
		
		
	}
	
	/// <summary>
	/// 
	/// </summary>
	public override void CloseIn ()
	{
		base.CloseIn ();
		
		Vector3 directionToPlayer = (player.gameObject.transform.position - transform.position).normalized;
		
		// Turn to face the right (or left - LOL!) direction
		if (directionToPlayer.x < 0){
			transform.rotation = Quaternion.Euler(new Vector3(0,90,0));
		}
		else if (directionToPlayer.x > 0){
			transform.rotation = Quaternion.Euler(new Vector3(0,-90,0));
		}
		
		controller.Move(directionToPlayer * stats.speed * Time.deltaTime);
		
		RaycastHit info;
		
		// Raycast again to make sure we can still see the player
		// If not, then go back to wandering
		if (Physics.Raycast(new Ray(transform.position, directionToPlayer), out info)){
			if (info.collider.gameObject != player.gameObject){
				//TODO: Do this better with child objects...?
				SwitchState(EntityState.WANDER);
				directionTimeCounter = 0;
			}
		}
	}
	
	#endregion
}
