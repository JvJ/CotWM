using UnityEngine;
using System.Collections;

using AssemblyCSharp;

public class ShadowmanControl : EntityControl {
	
	public DethmurderControl controller;
	
	private Attractor attr;
	
	// Use this for initialization
	void Start () {
		base.Start();
		
		// Shadowman-specific states
		this[EntityState.EXPANDING] = expand;
		this[EntityState.EXPELLING] = expelling;
		
		pController = player.GetComponent(typeof(CharacterController)) as CharacterController;
		
		attr = new Attractor(transform, suckSpeed);
	}
	
	/// <summary>
	/// Takes damage.
	/// TODO: Possible weakness to fire?
	/// </summary>
	/// <param name="atk">
	/// A <see cref="Attack"/>
	/// </param>
	public override void TakeDamage (Attack atk)
	{
		stats.DoDamage(atk.damageValue, false);
	}
	
	#region State Functions
	
	public override void SwitchState (EntityState s)
	{
		// LEFTOFF!
		if (currentState == EntityState.EXPANDING){
			controller.removeAttractor(attr);
		}
		
		base.SwitchState (s);
		
		// Add the attractor if necessary
		if ( s == EntityState.EXPANDING){
			controller.addAttractor(attr);
		}
	}
	
	#region Wandering Parameters
	
	public float wanderDistance;
	
	
	/// <summary>
	/// How far from the target before we pick a new one?
	/// </summary>
	public float breakOutDistance;
	
	private Vector3 currentTarget;
	
	private Color drawColor = Color.white;
	
	public bool reachedTarget = true;
	
	#endregion
	
	public override void Wander ()
	{
		
		if (Vector3.Distance(transform.position, currentTarget) < breakOutDistance){
			reachedTarget = true;
			drawColor = Color.white;
		}
		
		// Select a new target if the target has been reach-ed
		if (reachedTarget){
			
			// This selects a value on the unit circle
			Vector2 randTarget = Random.insideUnitCircle.normalized * wanderDistance;
			
			print("Selected new target with mag: "+randTarget.magnitude);
			
			currentTarget = new Vector3(randTarget.x, randTarget.y, 0);
			currentTarget += transform.position;
			
			reachedTarget = false;
		}
		
		// The usual update
		controller.Move((currentTarget - transform.position).normalized * stats.speed * Time.deltaTime);
		
		if (castToPlayer()){
			//SwitchState(EntityState.CLOSE_IN);
		}
		
		Debug.DrawLine(transform.position, currentTarget, drawColor);
	}
	
	#region Close In Parameters
	
	public float triggerRadius = 5.0f;
	
	#endregion
	
	public override void CloseIn ()
	{
		
		controller.Move((player.transform.position - transform.position).normalized * stats.speed * Time.deltaTime);
		
		if (Vector3.Distance(transform.position, player.transform.position) < triggerRadius){
			SwitchState(EntityState.EXPANDING);
		}
	}
	
	#region Expand Parameters
	
	CharacterController pController = null;
	
	public float suckSpeed = 10;
	
	#endregion
	
	public void expand(){
		
		/*if (pController != null){
			
			pController.Move((transform.position - player.transform.position).normalized * suckSpeed * Time.deltaTime);
			
			
			
		}*/
	}
	
	public void expelling(){
	}
	
	#endregion
	

	void OnControllerColliderHit(ControllerColliderHit hit){
	
		print("collision?");
		
		drawColor = Color.red;
		
		// Say we reached the target if we hit something
		if ((hit.gameObject.CompareTag("ROCK") || hit.gameObject.CompareTag("SNOW")) 
		    && currentState == EntityState.WANDER){
			
			print("It doesn't get in here!");
			
			drawColor = Color.green;
			
			currentTarget = (transform.position - hit.gameObject.transform.position).normalized * wanderDistance;
			
			currentTarget.z = 0;
			currentTarget += transform.position;
			
			
			
			//reachedTarget = false;
		}
	}
		
}
