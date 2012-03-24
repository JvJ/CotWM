using UnityEngine;
using System.Collections;

public class TentacleWhackHandler : MonoBehaviour {
	
	public ShoggothControl shoggoth;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter (Collider other){
		
		// Make sure it's the player
		// LEFTOFF: Make sure it still affects the player when they're on the ground
		// Add default time-out??
		if (other.gameObject == shoggoth.player.gameObject){
			
			shoggoth.player.setForce( (other.transform.position
			             - shoggoth.transform.position
			             ).normalized
			            * shoggoth.playerWhackIntensity );
			
			shoggoth.player.SwitchState(EntityState.GO_FLYING);
		}
	}
}
