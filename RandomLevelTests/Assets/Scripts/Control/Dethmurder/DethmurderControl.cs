using UnityEngine;
using System.Collections.Generic;
using AssemblyCSharp;

public class DethmurderControl : EntityControl {
	
	#region Tunable Parameters
	
	public GameObject aimTarget;
	
	//public float moveSpeed = 10f; 
	
	/// <summary>
	/// Ratio of speed to apply when moving.
	/// </summary>
	public float dashSpeed = 0f;
	
	public float wallJumpSpeed = 20f;
	
	public float wallJumpAngle = 45;
	
	public float wallJumpTimeOut = 0.5f;
	
	public WrapMode walkingWrapMode = WrapMode.PingPong;
	
	public WrapMode whackingWrapMode = WrapMode.Default;
	
	public float speedAnimMultiplier = 0.1f;
	
	public float whackSpeedMultiplier = 2.0f;
	
	#region Ability-related Variables
	
	public bool canDash = false;
	
	#endregion
	
	#endregion
	
	
	/// <summary>
	/// The direction in which the character moves (obviously)
	/// </summary>
	private Vector3 moveDirection;
	
	/// <summary>
	/// The currently-computed move vector.
	/// </summary>
	private Vector3 currentMoveVector = Vector3.zero;
	
	private float vSpeed = 0f;
	
	private CharacterController controller;
	
	private Color drawColor = Color.white;
	
	private bool wallJumpOK = false;
	
	private Vector3 jumpNormal = Vector3.zero;
	
	private float wallJumpTimeCounter = 0f;
	
	private bool wallJumpCounting = false;
	
	private int walkingAxis = 0;
	
	
	private List<Attractor> attractors;
	
	
	// Use this for initialization
	void Start () {
		base.Start();
		
		initVelocity = defaultInitVelocity;
		
		// TODO: Should this just be set in the editor?
		if (controller == null){
			controller = GetComponent(typeof(CharacterController)) as CharacterController;
		}
		
		// Setting up some more state functions
		this[EntityState.JUMPSTART] = JumpStart;
		this[EntityState.JUMPING] = Jumping;
		this[EntityState.GROUNDED] = Grounded;
		this[EntityState.GO_FLYING] = GoFlying;
		this[EntityState.CROUCHING] = Crouching;
		
		// Initialize the attractors
		attractors = new List<Attractor>();
		
		
		// Initialize animations
		animation["WalkCycle"].layer = 0;
		animation["WalkCycle"].enabled = true;
		animation["WalkCycle"].wrapMode = walkingWrapMode;
		
		var mixTransform = transform.Find("Armature/Hip/Spine_1/Spine_2");
		var mixTransform2 = transform.Find("Armature/Hip/ShovelBone");
		//var leftLegTransform = transform.Find("Armature/");
		var hipTransform = transform.Find("Armature/Hip");
		
		animation["WhackingForward_m_s2"].speed = whackSpeedMultiplier;
		animation["WhackingForward_m_s2"].layer = 1;
		animation["WhackingForward_m_s2"].enabled = true;
		animation["WhackingForward_m_s2"].wrapMode = whackingWrapMode;
		animation["WhackingForward_m_s2"].AddMixingTransform(mixTransform);
		animation["WhackingForward_m_s2"].AddMixingTransform(mixTransform2);
		//animation["WhackingForward_m_s2"].blendMode = AnimationBlendMode.Additive;
		
		animation["WhackingUp_m_s2"].speed = whackSpeedMultiplier;
		animation["WhackingUp_m_s2"].layer = 1;
		animation["WhackingUp_m_s2"].enabled = true;
		animation["WhackingUp_m_s2"].wrapMode = whackingWrapMode;
		animation["WhackingUp_m_s2"].AddMixingTransform(mixTransform);
		animation["WhackingUp_m_s2"].AddMixingTransform(mixTransform2);
		
		//animation["WhackingUp_m_s2"].blendMode = AnimationBlendMode.Additive;
		
		animation["WhackingDown_m_s2"].speed = whackSpeedMultiplier;
		animation["WhackingDown_m_s2"].layer = 1;
		animation["WhackingDown_m_s2"].enabled = true;
		animation["WhackingDown_m_s2"].wrapMode = whackingWrapMode;
		animation["WhackingDown_m_s2"].AddMixingTransform(mixTransform);
		animation["WhackingDown_m_s2"].AddMixingTransform(mixTransform2);
		//animation["WhackingDown_m_s2"].blendMode = AnimationBlendMode.Additive;
		
		animation["Crouch_m"].wrapMode = WrapMode.Clamp;
		animation["Crouch_m"].layer = 1;
		//animation["Crouch_m"].AddMixingTransform(hipTransform);
		animation["Crouch_m"].enabled = true;
		
		//moveDirection = new Vector3(1.0f, 0.0f, 0.0f);
	}
	
	void Awake(){
		moveDirection = new Vector3(1.0f, 0.0f, 0.0f);
		
		SetChildTags(gameObject, "PLAYERCHILD");
	}
	
	#region Head and tail
	
	public override void Head ()
	{
		base.Head ();
		
		// This may seem strange, but it has to do with stats
		animation["WalkCycle"].speed = stats.speed * speedAnimMultiplier;
		
		moveDirection = Vector3.zero;
		
		// Modify vertical speed
		vSpeed += Physics.gravity.y * Time.deltaTime;
		
		UpdateAim();
	}
	
	public override void Tail ()
	{
		base.Tail ();
		
		// Apply vertical speed
		moveDirection.y += vSpeed;
		
		// Apply velocity from attractors

		foreach (Attractor a in attractors){
			Vector3 dir = a.target.position - transform.position;
		
			float d2 = Vector3.Magnitude(dir);
			
			dir.Normalize();
			dir *= a.gConstant;
			
			moveDirection += (1f/d2) * dir;
		}
		
		controller.Move(moveDirection * Time.deltaTime);
	}
	
	#endregion
	
	#region State Methods
	
	#region JumpStart Parameters
	
	public float jumpLim;
	
	public float initVelocity;
	
	public float defaultInitVelocity;
	
	public float highInitVelocity;
	
	public float velocityMultiplier;
	
	private float jumpTimer = 0f;
	
	#endregion
	
	// A simple utility function for movement
	public void UpdateBasicMovement(){
		float hAxis = Input.GetAxis("Horizontal");
		float dashAxis = Input.GetAxis("Dash");
		
		// Handle the turning
		transform.rotation = Quaternion.Euler(new Vector3(0, cos < 0 ? -90 : 90,0));
		
		// Moving
		moveDirection.x += hAxis * stats.speed;
	}
	
	// Utility function for updating aim
	public float sin;
	public float cos;
	public float dx;
	public float dy;
	public float hyp;
	public void UpdateAim(){
		// Find the sine and cosine of the difference to the target
		dx = aimTarget.transform.position.x - transform.position.x;
		dy = aimTarget.transform.position.y - transform.position.y;
		hyp = Mathf.Sqrt(dx * dx + dy * dy);
		
		sin = dy / hyp;
		cos = dx / hyp;
	}
	
	// Utility function for activating attacks
	public void UpdateAttacking(){
		
		if (Input.GetButton("Whack")
		    && !animation.IsPlaying("WhackingForward_m_s2")
		    && !animation.IsPlaying("WhackingUp_m_s2")
		    && !animation.IsPlaying("WhackingDown_m_s2")){
			
			WhackThemWorms();
			
			animation.Stop("WalkCycle");
			
			animation.Blend("WhackingForward_m_s2", Mathf.Abs(cos));
			
			animation.Blend(sin > 0 ? "WhackingUp_m_s2" : "WhackingDown_m_s2", Mathf.Abs(sin));
			
		}
	}
	
	public void Grounded(){
		
		initVelocity = defaultInitVelocity;
		
		UpdateBasicMovement();
		
		if (moveDirection.x == 0){
			animation.CrossFade("DefaultPose");
		}
		else{
			
			
			// If the direction of motion does not equal the
			// direction of aiming, then reverse the animation!
			if (cos < 0 && moveDirection.x > 0
			    || cos > 0 && moveDirection.x < 0){
				animation["WalkCycle"].speed = - stats.speed * speedAnimMultiplier;
			}
			else{
				animation["WalkCycle"].speed = stats.speed * speedAnimMultiplier;
			}
			
			animation.CrossFade("WalkCycle");
		}
		
		// Always reset the vertical speed
		vSpeed = 0f;
		
		
		// Handle the attacking
		UpdateAttacking();
		
		if (Input.GetButton("Jump")){
			animation.CrossFade("Jumping");
			SwitchState(EntityState.JUMPSTART);
		}/*
		else if (!controller.isGrounded){
			animation.CrossFade("Jumping");
			SwitchState(EntityState.JUMPING);
		}*/
		
		if (!controller.isGrounded){
			SwitchState(EntityState.JUMPING);
		}
		
		// Handle crouching
		// LEFTOFF: Seriously, read about animation!
		if (Input.GetButton("Crouch")){
			animation.CrossFade("Crouch_m");
			SwitchState(EntityState.CROUCHING);
		}
	}
	
	public void Crouching(){
		
		initVelocity = highInitVelocity;
		
		UpdateAttacking();
		
		// Do the crouch
		if (!Input.GetButton("Crouch")){
			animation.CrossFade("UnCrouch_m");
			SwitchState(EntityState.GROUNDED);
		}
		if (Input.GetButton("Jump")){
			animation.CrossFade("Jumping");
			SwitchState(EntityState.JUMPSTART);
		}
		
	}
	
	public void JumpStart(){
		
		UpdateBasicMovement();
		
		UpdateAttacking();
		
		// Add to ther timer
		if (jumpTimer >= jumpLim){
			jumpTimer = 0f;
			SwitchState(EntityState.JUMPING);
			return;
		}
		
		jumpTimer += Time.deltaTime;
		
		// Do the lerp!
		vSpeed = initVelocity + Mathf.Lerp(0f, jumpLim, jumpTimer)* velocityMultiplier;
		
		// If, at any point, jump is not being pressed, switch state!
		if (!Input.GetButton("Jump")){
			jumpTimer = 0f;
			SwitchState(EntityState.JUMPING);
		}
	}
	
	public void Jumping(){
		
		UpdateBasicMovement();
		
		UpdateAttacking();
		
		// Gotta switch back if it's grounded
		if (controller.isGrounded){
			SwitchState(EntityState.GROUNDED);
		}
	}
	
	public void GoFlying(){
		controller.Move(flyingForce * Time.deltaTime);
	}
	
	/*public override void Wander ()
	{
		moveDirection = Vector3.zero;
		float hAxis = Input.GetAxis("Horizontal");
		float dashAxis = Input.GetAxis("Dash");
		var quat = Quaternion.AngleAxis(wallJumpAngle, Vector3.Cross(jumpNormal, Vector3.up));
		var v = quat * jumpNormal* wallJumpSpeed;
		
		
		
		// Counting the wall jump timer
		if (wallJumpCounting){
			wallJumpTimeCounter += Time.deltaTime;
			if (wallJumpTimeCounter > wallJumpTimeOut){
				wallJumpTimeCounter = 0f;
				wallJumpCounting = false;
			}
		}		
		
		if (controller.isGrounded){
			drawColor = Color.red;
			vSpeed = 0f;
			if (Input.GetButton("Jump")){
				print("JUMP BUTOTN!");
				vSpeed = jumpSpeed;
			}
		}
		else if (wallJumpOK){
			drawColor = Color.blue;
			
			if (Input.GetButton("Jump")){
				vSpeed = 0f;
				wallJumpOK = false;
				wallJumpTimeCounter = 0f;
				wallJumpCounting = true;
				vSpeed += v.y;
			}
		}
		else{
			drawColor = Color.green;
			
		}
		
		if (!wallJumpCounting){
			if (canDash){
				if (dashAxis != 0){
					animation.Blend("Dash");
					moveDirection.x += dashAxis * dashSpeed;
				}
			}
			moveDirection.x += hAxis * stats.speed;
		}
		else{
			
			moveDirection.x += v.x;
		}
		
		vSpeed += Physics.gravity.y * Time.deltaTime;
		
		if (hAxis == 0){
			//transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
			animation.Stop("Walking");
			//walkingAxis = 0;
		}
		else if (hAxis < 0){
			//transform.rotation = Quaternion.Euler(new Vector3(0,90,0));
			animation.CrossFade("Walking");
			walkingAxis = -1;
		}
		else{
			//transform.rotation = Quaternion.Euler(new Vector3(0,-90,0));
			animation.CrossFade("Walking");
			walkingAxis = 1;
		}
		
		bool hitting = false;
		// Handling the whacking and attacking!
		if (Input.GetButton("Whack")){
			hitting = true;
		}
			
		// Find the sine and cosine of the difference to the target
		float dx = aimTarget.position.x - transform.position.x;
		float dy = aimTarget.position.y - transform.position.y;
		float hyp = Mathf.Sqrt(dx * dx + dy * dy);
		
		float sin = dy / hyp;
		float cos = dx / hyp;
			
			
		if (cos > 0){
			transform.rotation = Quaternion.Euler(new Vector3(0,-90,0));
			walkingAxis = 1;
			
			if (hitting){
				animation.Blend("Whacking", cos);
				animation.Blend("WhackingBack", 0);
			}
		}
		else{
			transform.rotation = Quaternion.Euler(new Vector3(0,90,0));
			walkingAxis = -1;
			
			if (hitting){
				animation.Blend("Whacking", -cos);
				animation.Blend("WhackingBack", 0);
			}
		}
			
		if (sin > 0){
			if (hitting){
				animation.Blend("WhackingUp", sin);
				animation.Blend("WhackingDown", 0);
			}
		}
		else{
			if (hitting){
				animation.Blend("WhackingUp", 0);
				animation.Blend("WhackingDown", -sin);
			}
		}
		
		moveDirection.y += vSpeed;
		
		//print("moveDirection at end of Wander: "+moveDirection);
		
	}*/
	
	/// <summary>
	/// Called when a collider is encountered.
	/// </summary>
	void OnControllerColliderHit(ControllerColliderHit hit){
		
		Debug.DrawLine(hit.point, hit.point + hit.normal, drawColor, 0.1f);
		
		if (currentState == EntityState.GO_FLYING){
			SwitchState(EntityState.JUMPING);
		}
		
		if (hit.normal.y == 0f){
			jumpNormal = hit.normal;
			wallJumpOK = true;
		}
		else if (vSpeed > 0 && hit.normal.y < 0f){
			vSpeed = 0f;
		}
		else if (controller.isGrounded){
			wallJumpOK = false;
		}
		
		//print(hit.gameObject.name);
	}
	
	
	float max = 0f;
	float tMax = 0f;
	void OnGUI(){
		GUI.Box(new Rect(0,0, 200, 20), "Norm: "+animation["Crouch_m"].normalizedTime);
		GUI.Box(new Rect(0,20, 200, 20), "Time: "+animation["Crouch_m"].time);
		
		if (animation["Crouch_m"].normalizedTime > max){
			max = animation["Crouch_m"].normalizedTime;
		}
		
		if (animation["Crouch_m"].time > tMax){
			tMax = animation["Crouch_m"].time;
		}
		
		GUI.Box(new Rect(0,40, 200, 20), "Max: "+max);
		GUI.Box(new Rect(0,60, 200, 20), "tMax: "+tMax);
		GUI.Box(new Rect(0,80, 200, 20), "Length: "+animation["Crouch_m"].length);
	}
	
	#endregion
	
	#region External Interface
	
	public bool IsAttacking(){
		return animation.IsPlaying("Whacking");
	}
	
	public void addAttractor(Attractor a){
		attractors.Add(a);
	}
	
	public void removeAttractor(Attractor a){
		attractors.Remove(a);
	}
	
	/// <summary>
	/// Used to go flying!
	/// </summary>
	Vector3 flyingForce;
	
	/// <summary>
	/// Set force for when you... go flying!
	/// </summary>
	/// <param name="force">
	/// A <see cref="Vector3"/>
	/// </param>
	public void setForce(Vector3 force){
		flyingForce = force;
	}
	
	public void WhackThemWorms(){
		foreach (Transform t in transform){
			var wc = t.GetComponent(typeof(WyrmControl)) as WyrmControl;
			
			if (wc != null){
				wc.TakeDamage(new Attack{damageValue = stats.attack,element = ElementType.NONE, isContact = false});
			}
		}
	}
	
	#endregion
	
	#region EntityControl Methods
	
	/// <summary>
	/// Here's the basic damage-taking thing.
	/// </summary>
	/// <param name="atk">
	/// A <see cref="Attack"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Single"/>
	/// </returns>
	public override void TakeDamage (Attack atk)
	{
		
		stats.DoDamage(atk.damageValue, false);

	}
	
	
	#endregion
}