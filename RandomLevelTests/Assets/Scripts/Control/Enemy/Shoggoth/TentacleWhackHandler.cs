using UnityEngine;
using System.Collections;

public class TentacleWhackHandler : MonoBehaviour {
	
	public ShoggothControl shoggoth;
	
	public bool okToHit = false;
	
	public float hitFloat = -1f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		var M = GetComponent(typeof(MeshRenderer)) as MeshRenderer;
		
		okToHit = hitFloat > 0;
		
		if (okToHit){
			M.material.color = Color.red;
		}
		else{
			M.material.color = Color.white;
		}
	}
	
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
	}
}
