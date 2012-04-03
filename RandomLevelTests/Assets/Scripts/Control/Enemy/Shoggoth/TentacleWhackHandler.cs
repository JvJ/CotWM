using UnityEngine;
using System.Collections;

public class TentacleWhackHandler : CollideHandler {
	
	public ShoggothControl shoggoth;
	
	private MeshRenderer mRend = null;
	
	// Use this for initialization
	protected override void Start(){
		base.Start();
		mRend = GetComponent(typeof(MeshRenderer)) as MeshRenderer;
	}
	
	protected override void Update(){
		base.Update();
		
		if (hitFloat > 0){
			mRend.material.color = Color.red;
		}
		else{
			mRend.material.color = Color.green;
		}
	}
	
	protected override bool addCondition (Collider c)
	{
		return c.gameObject.CompareTag("PLAYER") || c.gameObject.CompareTag("PLAYERCHILD");
	}
	
	protected override void hitCollider (Collider c)
	{	
		var v = shoggoth.playerWhackNormal;

		v.z = 0;
			
		v.x *= shoggoth.wanderDirection;
			
		shoggoth.player.setForce( v.normalized * shoggoth.playerWhackIntensity );
			
		shoggoth.player.SwitchState(EntityState.GO_FLYING);
	}
	
	/*
	void OnTriggerEnter (Collider other){
		
		// Make sure it's the player
		// LEFTOFF: Make sure it still affects the player when they're on the ground
		// Add default time-out??
		if (okToHit && (other.gameObject.CompareTag("PLAYER") || other.gameObject.CompareTag("PLAYERCHILD"))){
			
			var v = shoggoth.playerWhackNormal;
			//var v = (other.transform.position - shoggoth.transform.position );
			v.z = 0;
			
			v.x *= shoggoth.wanderDirection;
			
			shoggoth.player.setForce( v.normalized * shoggoth.playerWhackIntensity );
			
			shoggoth.player.SwitchState(EntityState.GO_FLYING);
		}
	}*/
}
