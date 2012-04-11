using UnityEngine;
using System.Collections;

public class ShoggothControl : EntityControl{

	#region Editor Variables
	
	public Rigidbody rb;
	public Collider coll;
	
	public GameObject leftTentacle, rightTentacle;
	
	public TentacleWhackHandler leftWhackHandler, rightWhackHandler;
	
	public float attackRadius;
	
	public Vector3 playerWhackNormal;
	
	/// <summary>
	/// How fast will the player go when he gets hit??
	/// </summary>
	public float playerWhackIntensity;
		
	#endregion
	
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
	
	/// <summary>
	/// Start the basic animations on the tentacles.
	/// </summary>
	//public void StartTentacleAnimations(){
		
	//}
	
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
	
	public void UpdateBasicMovement(){
		
		var v = new Vector3(wanderDirection * stats.speed * Time.deltaTime, 0, 0);
		rb.MovePosition(transform.position + v);
	}
	
	public override void Wander ()
	{
		UpdateBasicMovement();
		
		// Update the timer
		wanderCountDown -= Time.deltaTime;
		
		// Reset the time counter if necessary
		if (wanderCountDown <= 0){
			
			wanderDirection = Random.value - 0.5 < 0 ? -1 : 1;
			
			wanderCountDown = wanderTimeOut;
		}
		
		// See player
		if (castToPlayer()){
			wanderCountDown = wanderTimeOut;
			SwitchState(EntityState.CLOSE_IN);
		}
	}
	
	public override void CloseIn ()
	{
		wanderDirection = (int)Mathf.Sign(player.transform.position.x - transform.position.x);
		
		UpdateBasicMovement();
		
		// Lost sight of player
		RaycastHit rcInfo;
		if (!castToPlayer(out rcInfo)){
			
			Debug.DrawLine(transform.position, rcInfo.point, Color.red, 1.0f);
			
			SwitchState(EntityState.WANDER);
		}
		// In range
		Debug.DrawLine(transform.position, transform.position + new Vector3(0, attackRadius, 0), Color.blue);
		if (Vector3.Distance(transform.position, player.transform.position) < attackRadius){
			SwitchState(EntityState.ATTACKING);
		}
	}
	
	public override void Attacking ()
	{
		wanderDirection = (int)Mathf.Sign(player.transform.position.x - transform.position.x);
		
		UpdateBasicMovement();
		
		// Initiate smacking!
		
		// Which tentacle can I use?
		var tent = wanderDirection < 0 ? leftTentacle : rightTentacle;
		
		if (!tent.animation.IsPlaying("Smack_scripted")){
			tent.animation.CrossFade("Smack_scripted");
			tent.animation.PlayQueued("Wiggle");
		}
		
		// Out of range
		if (Vector3.Distance(transform.position, player.transform.position) > attackRadius){
			SwitchState(EntityState.CLOSE_IN);
		}
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
	
	public override void setScript (bool onOff)
	{
		base.setScript (onOff);
		
		leftWhackHandler.enabled = onOff;
		rightWhackHandler.enabled = onOff;
	}
}