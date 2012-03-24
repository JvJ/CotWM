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
		
		// Do a ray-cast to the player
		Vector3 dPlayer = player.gameObject.transform.position - transform.position;
		
		RaycastHit info;
		
		if (Physics.Raycast(new Ray(transform.position, dPlayer), out info)){
			if (info.collider.gameObject == player.gameObject){
				return true;
			}
		}
		
		return false;
	}
	
	#endregion
}
