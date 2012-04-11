using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class XlGrhthbtrgControl : EntityControl {
	
	
	#region Public Params
	
	public XTentacleControl frontTopRightTentacle;
	public XTentacleControl frontTopLeftTentacle;
	
	public XTentacleControl frontBottomRightTentacle;
	public XTentacleControl frontBottomLeftTentacle;
	
	public XTentacleControl backTopRightTentacle;
	public XTentacleControl backTopLeftTentacle;
	
	public XTentacleControl backBottomRightTentacle;
	public XTentacleControl backBottomLeftTentacle;
	
	/// <summary>
	/// The target to aim the tentacles at!
	/// </summary>
	public Transform target;
	
	/// <summary>
	/// The blast time limit.
	/// </summary>
	public float blastTimeLimit = 0.1f;
	
	public AudioSource blast;
	
	/// <summary>
	/// Timeout for target movement.
	/// </summary>
	public float targetTimeout = 5.0f;
	private float currentTime = 0f;
	
	/// <summary>
	/// How far from the target before we stop moving.
	/// </summary>
	public float targetStopRadius = 0.2f;
	
	/// <summary>
	/// The attack time out.
	/// </summary>
	public float attackTimeOut = 2.0f;
	public float attackStartTime = 1.5f;
	public float attackSwitchTime = 1.9f;
	private float currentAttackTime = 0f;
	
	public Material safeMaterial;
	public Material dangerousMaterial;
	
	#endregion
	
	/// <summary>
	/// The tentacles.
	/// </summary>
	private XTentacleControl[] tentacles;
	private XTentacleControl[] fronts;
	private XTentacleControl currentMin;
	
	private bool currentTargetSelected = false;
	public Vector3 currentTarget;
	
	private bool prevTargetSelected = false;
	private Vector3 prevTarget;
	
	private bool rectSelected = false;
	public Rect roomRect;
	
	private Vector3 velocityVec;
	
	private CharacterController controller;
	
	/// <summary>
	/// Is the beam touching the player?.
	/// </summary>
	private bool beamTouchingPlayer;
	
	private Transform tempTarget;
	
	private bool alreadyAttacked;
	
	// Use this for initialization
	public override void Start(){
		base.Start();
		
		controller = GetComponent(typeof(CharacterController)) as CharacterController;
		
		// Set some amination variables
		animation["Gnash"].wrapMode = WrapMode.Loop;
		
		// Initialize them tentaclites!
		tentacles = new  XTentacleControl[]{
			frontTopRightTentacle,
			frontTopLeftTentacle,
			frontBottomRightTentacle,
			frontBottomLeftTentacle,
			backTopRightTentacle,
			backTopLeftTentacle,
			backBottomRightTentacle,
			backBottomLeftTentacle
		};
		
		fronts = new XTentacleControl[]{
			frontTopRightTentacle,
			frontTopLeftTentacle,
			frontBottomRightTentacle,
			frontBottomLeftTentacle,
		};
	}
	
	public override void TakeDamage (Attack atk)
	{
		stats.DoDamage(atk.damageValue, false);
	}
	
	#region State Helper Functions
	
	/// <summary>
	/// This head handles tentacle aiming(maybe...?).
	/// </summary>
	public override void Head ()
	{
		base.Head ();
		
		if (!rectSelected){
			rectSelected = true;
			roomRect = CubeGen.Singleton.CurrentRoom(new Vector2(transform.position.x, transform.position.y));
		}
		
		aimTentacles();
		
	}
	
	/// <summary>
	/// Helper function - Aims the tentacles.
	/// </summary>
	public void aimTentacles(){
		
		// Aim each one
		foreach (var t in tentacles){
			t.ComputeRotationRatio(target.transform.position);
		}
		
	}
	
	public void getMinTentacle(){
		
		XTentacleControl min = null;
		float minDist = float.PositiveInfinity;
		
		// First, Isolate the closest tentaclite
		foreach (var t in fronts){
			float d = Vector3.Distance(t.transform.position, target.position);
			if (d < minDist){
				minDist = d;
				min = t;
			}
		}
		
		currentMin = min;
	}
	
	/// <summary>
	/// Gets the temporary target position.
	/// </summary>
	/// <returns>
	/// The temporary target.
	/// </returns>
	public void getTemporaryTarget(){
		
		var g = new GameObject();
		
		// Set the position
		g.transform.position = target.position;
		
		g.layer = LayerMask.NameToLayer("Ignore Raycast");
		
		tempTarget = g.transform;
	}
	
	/// <summary>
	/// Repositions the target by raycasting.
	/// </summary>
	public void repositionTarget(){
		
		RaycastHit info;
		
		int mask = 0;
		mask |= 1 << LayerMask.NameToLayer("Default");
		mask |= 1 << LayerMask.NameToLayer("Terrain");
		
		Vector3 v1 = new Vector3(currentMin.endTransform.position.x, currentMin.endTransform.position.y, 0);
		
		Vector3 v2 = new Vector3(tempTarget.transform.position.x, tempTarget.transform.position.y, 0);
		
		if (Physics.Raycast(new Ray(v1,v2 - v1),
			out info,
			float.PositiveInfinity,
			mask)){
			
			tempTarget.position = info.point;
			
			Debug.DrawLine(v1, info.point, Color.cyan);
			
			// Do it!
			if (info.collider.gameObject.CompareTag("PLAYER") ||
				info.collider.gameObject.CompareTag("PLAYERCHILD")){
				beamTouchingPlayer = true;
			}
			else{
				beamTouchingPlayer = false;
			}
		}
		else{
			beamTouchingPlayer = false;
		}
	}
	
	/// <summary>
	/// This is where the actual attack goes down.
	/// </summary>
	public void attackPlayer(){
		
		if (blast != null){
			blast.Play();
		}
		
		// Change the material
		currentMin.blaster.pRend.material = dangerousMaterial;
		
		// Attack if possible
		if(beamTouchingPlayer){
			player.TakeDamage(new Attack{
				damageValue = stats.attack,
				element = ElementType.LIGHTNING,
				isContact = false
			});
		}
	}
	
	#endregion
	
	#region State Functions
	
	public override void Wander ()
	{
		// Function shortcut thingie with lambdas
		Func<Vector3> randVect =()=>
			new Vector3(
				UnityEngine.Random.Range(roomRect.x, roomRect.xMax),
				UnityEngine.Random.Range(roomRect.yMin, roomRect.yMax),
				0);
	
		// Select the target
		if (!prevTargetSelected){
			prevTargetSelected = true;
			currentTarget = randVect();
		}
		
		// Do the thing
		if (!currentTargetSelected){
			currentTime = 0;
			currentTargetSelected = true;
			prevTarget = currentTarget;
			currentTarget = randVect();
		}
		
		
		aimTentacles();
		getMinTentacle();
		
		// The attacking timer
		currentAttackTime += Time.deltaTime;
		if (currentAttackTime > attackStartTime){
			// TODO: WHAT THE HELL!
			getTemporaryTarget();
			currentMin.Blast(tempTarget);
			SwitchState(EntityState.ATTACKING);
		}
		
		// The timer
		currentTime += Time.deltaTime;
		currentTargetSelected = currentTime < targetTimeout;
		
		Debug.DrawLine(transform.position, currentTarget, Color.green);
		Debug.DrawLine(transform.position, transform.position + velocityVec * stats.speed );
		Debug.DrawLine(transform.position, prevTarget, Color.red);
		
		// Only do this if we are not within distance of the target
		if (Vector3.Distance(transform.position, currentTarget) > targetStopRadius){
			// Now that we have 2 targets, create a movment vector
			velocityVec = smoothMove(transform.position, prevTarget, currentTarget, currentTime, targetTimeout);
			velocityVec *= stats.speed * Time.deltaTime;
			
			// Move and don't apply gravity
			controller.Move(velocityVec);
		}
	}
	
	public override void Attacking ()
	{
		
		repositionTarget();
		
		Debug.DrawLine(currentMin.endTransform.position, tempTarget.position, Color.yellow);
		
		// Stop attacking, reset everything and switch state
		if (currentAttackTime > attackTimeOut){
			currentAttackTime = 0;
			currentMin.UnBlast();
			currentMin.blaster.pRend.material = safeMaterial;
			GameObject.Destroy(tempTarget.gameObject);
			tempTarget = null;
			alreadyAttacked = false;
			SwitchState(EntityState.WANDER);
		}
		else if (currentAttackTime > attackSwitchTime && !alreadyAttacked){
			alreadyAttacked = true;
			attackPlayer();
		}
		
		currentAttackTime += Time.deltaTime;
	}
	
	#endregion
	
	public override void setScript (bool onOff)
	{
		base.setScript (onOff);
		
		if (tentacles != null){
			foreach(XTentacleControl t in tentacles){
				t.enabled = false;
			}
		}
	}
}
