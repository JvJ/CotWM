using UnityEngine;
using System.Collections;
using System;


public enum EntityState{
	
	STILL,
	WANDER,
	CLOSE_IN,
	ATTACKING,
	DEAD,
	
	// Dethmurder
	JUMPSTART,
	JUMPING,
	GROUNDED,
	WHACKING,
	GO_FLYING,
	CROUCHING,
	
	// Shadowman
	EXPANDING,
	EXPELLING,
	
	// Keep this here - it's important!
	NumElements
}

public enum WanderDirection{
	UP,
	DOWN,
	LEFT,
	RIGHT,
	NumElements
}

public class StateNotImplementedException : Exception{
	
	string message;
	public StateNotImplementedException(string msg){
		message = msg;
	}
	
	public override string Message {
		get {
			return base.Message + "\n" + message;
		}
	}
}

public class EntityControl : MonoBehaviour {

	
	
	#region Public Attributes
	
	public Stats stats;
	
	public EntityState currentState;
	
	public DethmurderControl player;
	
	#endregion
	
	#region Private and Protected Members
	
	protected Action[] StateFunctions;
	
	protected Action this[EntityState s]
	{
		get{return StateFunctions[(int)s];}
		set{StateFunctions[(int)s] = value;}
	}
	
	#endregion
	
	
	#region Inheritable Methods
	
	public virtual void TakeDamage(Attack atk)
	{
		throw new NotImplementedException();
	}
	
	
	#endregion
	
	
	// Use this for initialization
	// All subclasses should run this on startup.
	// Subclasses should also initialize delegates in this method.
	public virtual void Start () {
		StateFunctions = new Action[(int)EntityState.NumElements];
		
		this[EntityState.STILL] = this.Still;
		this[EntityState.WANDER] = this.Wander;
		this[EntityState.CLOSE_IN] = this.CloseIn;
		this[EntityState.ATTACKING] = this.Attacking;
		this[EntityState.DEAD] = this.Dead;
	}
	
	// Update is called once per frame
	// Subclasses should depend primarily on this implementation
	public virtual void Update () {
		RunStateFunc(currentState);
	}
	
	public virtual void SwitchState(EntityState s){
		currentState = s;
	}
	
	public void RunStateFunc(EntityState s){
		
		Head();
		
		var f = this[s];
		if (f == null){
			throw new StateNotImplementedException(""+s);
		}
		else{
			f();
		}
		
		Tail();
	}
	
	#region State Methods
	
	public virtual void Still(){
	}
	
	public virtual void Wander(){
	}
	
	public virtual void CloseIn(){
	}
	
	public virtual void Attacking(){
	}
	
	public virtual void Dead(){
		Destroy(gameObject);
	}
	
	/// <summary>
	/// This function precedes the update function.
	/// </summary>
	public virtual void Head(){
		if (stats.HP <= 0){
			SwitchState(EntityState.DEAD);
		}
	}
	
	/// <summary>
	/// This function always succeeds the update function.
	/// </summary>
	public virtual void Tail(){
	}
	
	#endregion
	
	#region Utility Methods
	
	public bool castToPlayer(){
		RaycastHit h;
		return castToPlayer(out h);
	}
	
	public bool castToPlayer(out RaycastHit hitInfo){
		
		// Do a ray-cast to the player
		Vector3 dPlayer = player.gameObject.transform.position - transform.position;
		
		RaycastHit info;
		
		// This is important!  It tells the enemies to ignore themselves when casting!
		int mask = 0;
		
		mask |= 1 << LayerMask.NameToLayer("Default");
		mask |= 1 << LayerMask.NameToLayer("Terrain");
		
		if (Physics.Raycast(new Ray(transform.position, dPlayer), out info, float.PositiveInfinity, mask)){
			hitInfo = info;
			//Debug.DrawLine(transform.position, info.point, Color.red, 1.0f);
			
			// TODO: This is just a work-around, but will probably work out alright!
			if ( Vector3.Distance (info.point, transform.position) >
			    Vector3.Distance(player.transform.position, transform.position)){
				return true;
			}
			
			if (info.collider.gameObject.CompareTag("PLAYER") ||
			    info.collider.gameObject.CompareTag("PLAYERCHILD")){
				return true;
			}
		}
		
		hitInfo = info;
		
		return false;
	}
	
	/// <summary>
	/// Set the tag of all descendant objects recursively.
	/// </summary>
	/// <param name="targ">
	/// A <see cref="GameObject"/>.  Start with this game object.
	/// </param>
	/// <param name="tag">
	/// A <see cref="System.String"/>.  What tag do you want to use?
	/// </param>
	public virtual void SetChildTags(GameObject targ, string tag){
		
		print("Setting tags for "+targ.name);
		
		if (transform == null){
			return;
		}
		
		foreach( Transform t in targ.transform){
			
			if (t.gameObject == null){
				continue;
			}
			
			print("Setting tags in loop for: "+t.gameObject.name);
			
			// Always set t's tag first, since we don't necessarily want
			// this game object to have the tag!
			t.gameObject.tag = tag;
			
			SetChildTags(t.gameObject, tag);
		}
	}
	
	/// <summary>
	/// Smooths the move when fading between 2 movement targets.
	/// </summary>
	/// <returns>
	/// The move.
	/// </returns>
	/// <param name='currentPos'>
	/// Current position.
	/// </param>
	/// <param name='dest1'>
	/// Dest1.
	/// </param>
	/// <param name='dest2'>
	/// Dest2.
	/// </param>
	/// <param name='current'>
	/// Current.
	/// </param>
	/// <param name='limit'>
	/// Limit.
	/// </param>
	public Vector3 smoothMove(Vector3 currentPos, Vector3 dest1, Vector3 dest2, float current, float limit){
		
		Vector3 v1 = (dest1 - currentPos).normalized;
		Vector3 v2 = (dest2 - currentPos).normalized;
		
		float w1 = Mathf.Lerp(0, limit, current);
		w1 = Mathf.Clamp01(w1);
		
		float w2 = 1 - w1;
		
		return ((v1 * w2 + v2 * w1) / 2f).normalized;
	}
	
	#endregion
}
