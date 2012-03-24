using UnityEngine;
using System.Collections.Generic;
using AssemblyCSharp;

public class DethmurderControl : EntityControl {
	
	#region Tunable Parameters
	
	public Transform aimTarget;
	
	//public float moveSpeed = 10f; 
	
	/// <summary>
	/// Ratio of speed to apply when moving.
	/// </summary>
	public float dashSpeed = 0f;
	
	public float jumpSpeed = 20f;
	
	public float wallJumpSpeed = 20f;
	
	public float wallJumpAngle = 45;
	
	public float wallJumpTimeOut = 0.5f;
	
	public WrapMode walkingWrapMode = WrapMode.PingPong;
	
	public WrapMode whackingWrapMode = WrapMode.Default;
	
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
		
		// TODO: Should this just be set in the editor?
		if (controller == null){
			controller = GetComponent(typeof(CharacterController)) as CharacterController;
		}
		
		// Setting up some more state functions
		this[EntityState.JUMPSTART] = JumpStart;
		this[EntityState.JUMPING] = Jumping;
		this[EntityState.GROUNDED] = Grounded;
		this[EntityState.GO_FLYING] = GoFlying;
		
		
		animation["Walking"].layer = 0;
		animation["Walking"].enabled = true;
		animation["Walking"].wrapMode = walkingWrapMode;
		
		animation["Whacking"].layer = 1;
		animation["Whacking"].enabled = true;
		animation["Whacking"].wrapMode = whackingWrapMode;
		animation["WhackingUp"].layer = 1;
		animation["WhackingUp"].enabled = true;
		animation["WhackingUp"].wrapMode = whackingWrapMode;
		
		attractors = new List<Attractor>();
		
		//moveDirection = new Vector3(1.0f, 0.0f, 0.0f);
	}
	
	void Awake(){
		moveDirection = new Vector3(1.0f, 0.0f, 0.0f);
	}
	
	#region Head and tail
	
	public override void Head ()
	{
		base.Head ();
		
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
	
	public float velocityMultiplier;
	
	private float jumpTimer = 0f;
	
	#endregion
	
	// A simple utility function for movement
	public void UpdateBasicMovement(){
		float hAxis = Input.GetAxis("Horizontal");
		float dashAxis = Input.GetAxis("Dash");
		
		// Handle the turning
		transform.rotation = Quaternion.Euler(new Vector3(0, cos < 0 ? 90 : -90,0));
		
		// Moving
		moveDirection.x += hAxis * stats.speed;
	}
	
	// Utility function for updating aim
	float sin;
	float cos;
	public void UpdateAim(){
		// Find the sine and cosine of the difference to the target
		float dx = aimTarget.position.x - transform.position.x;
		float dy = aimTarget.position.y - transform.position.y;
		float hyp = Mathf.Sqrt(dx * dx + dy * dy);
		
		sin = dy / hyp;
		cos = dx / hyp;
	}
	
	// Utility function for activating attacks
	public void UpdateAttacking(){
		
		if (Input.GetButton("Whack")
		    && !animation.IsPlaying("Whacking")
		    && !animation.IsPlaying("WhackingUp")
		    && !animation.IsPlaying("WhackingDown")){
			
			animation.Blend("Whacking", Mathf.Abs(cos));
			
			animation.Blend(sin > 0 ? "WhackingUp" : "WhackingDown", Mathf.Abs(sin));
			
		}
	}
	
	public void Grounded(){
		
		UpdateBasicMovement();
		
		// Always reset the vertical speed
		vSpeed = 0f;
		
		
		// Handle the attacking
		UpdateAttacking();
		
		if (Input.GetButton("Jump")){
			SwitchState(EntityState.JUMPSTART);
		}
		
		if (!controller.isGrounded){
			SwitchState(EntityState.JUMPING);
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
		else if (hit.normal.y < 0f){
			vSpeed = 0f;
		}
		else if (controller.isGrounded){
			wallJumpOK = false;
		}
		
		//print(hit.gameObject.name);
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
		stats.HP -= (atk.damageValue - stats.defense);
	
		//lastDamage = (atk.damageValue - stats.defense);
	}
	
	
	
	#endregion
}