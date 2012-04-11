using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CollideHandler : MonoBehaviour {
	
	#region Public Params
	
	public float hitFloat = 0f;
	
	#endregion
	
	protected HashSet<Collider> enteredList;
	protected HashSet<Collider> hitList;
	
	protected virtual void Awake(){
		enteredList = new  HashSet<Collider>();
		hitList = new HashSet<Collider>();
	}
	
	// Use this for initialization
	protected virtual void Start () {
		
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		
		// Find all colliders that will be moved.
		var movedColliders =
			(from c in enteredList
			where moveCondition(c)
			select c).ToList();
		
		var moveBackColliders = 
			(from c in hitList
				where moveBackCondition(c)
				select c).ToList();
		
		// For each collider that will be moved,
		// move it and hit it.
		foreach(Collider c in movedColliders){
			enteredList.Remove(c);
			hitCollider(c);
			hitList.Add(c);
		}
		
		foreach (Collider c in moveBackColliders){
			hitList.Remove(c);
			enteredList.Add(c);
		}
	}
	
	/// <summary>
	/// The condition to add the thing to the thing.
	/// </summary>
	/// <returns>
	/// The condition.
	/// </returns>
	/// <param name='c'>
	/// If set to <c>true</c> c.
	/// </param>
	/// <exception cref='System.NotImplementedException'>
	/// Is thrown when the not implemented exception.
	/// </exception>
	protected virtual bool addCondition(Collider c){
		throw new System.NotImplementedException();
	}
	
	/// <summary>
	/// The condition required for moving a collider from the
	/// entered list to the hit list.
	/// </summary>
	/// <returns>
	/// By default, hitFloat > 0.
	/// </returns>
	/// <param name='c'>
	/// The collider in question.  Doesn't matter by default.
	/// </param>
	protected virtual bool moveCondition(Collider c){
		return hitFloat > 0;
	}
	
	/// <summary>
	/// Condition to move something back from the hit list.
	/// </summary>
	/// <returns>
	/// By default, it's just !moveCondition(c).
	/// </returns>
	/// <param name='c'>
	/// The collider in question.  Doesn't matter by default.
	/// </param>
	protected virtual bool moveBackCondition(Collider c){
		return !moveCondition(c);
	}
	
	/// <summary>
	/// Called when the collider is actually hit.
	/// </summary>
	/// <param name='c'>
	/// The collider getting hit.
	/// </param>
	protected virtual void hitCollider(Collider c){
		throw new System.NotImplementedException();
	}
	
	/// <summary>
	/// Raises the trigger enter event.
	/// </summary>
	/// <param name='other'>
	/// Other.
	/// </param>
	void OnTriggerEnter (Collider other){
		
		if (addCondition(other)){
			enteredList.Add(other);
		}
	}
	
	/// <summary>
	/// Raises the trigger exit event.
	/// </summary>
	/// <param name='other'>
	/// Other.
	/// </param>
	void OnTriggerExit(Collider other){
		
		enteredList.Remove(other);
		hitList.Remove(other);
	}
}
