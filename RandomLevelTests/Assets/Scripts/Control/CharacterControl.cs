using UnityEngine;
using System.Collections;

public class CharacterControl : EntityControl {
	
	
	#region Tunable Parameters
	
	public Transform aimTarget;
	
	public float moveSpeed = 10f; 
	
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
	
	
	// Use this for initialization
	void Start () {
		
		animation["Walking"].layer = 0;
		animation["Walking"].enabled = true;
		animation["Walking"].wrapMode = walkingWrapMode;
		
		animation["Whacking"].layer = 1;
		animation["Whacking"].enabled = true;
		animation["Whacking"].wrapMode = whackingWrapMode;
		animation["WhackingUp"].layer = 1;
		animation["WhackingUp"].enabled = true;
		animation["WhackingUp"].wrapMode = whackingWrapMode;
	}
	
	void Awake(){
		moveDirection = new Vector3(1.0f, 0.0f, 0.0f);
	}
	
	// Update is called once per frame
	void Update () {
		
		var moveDirection = Vector3.zero;
		float hAxis = Input.GetAxis("Horizontal");
		float dashAxis = Input.GetAxis("Dash");
		var quat = Quaternion.AngleAxis(wallJumpAngle, Vector3.Cross(jumpNormal, Vector3.up));
		var v = quat * jumpNormal* wallJumpSpeed;
		
		if (controller == null){
			controller = GetComponent(typeof(CharacterController)) as CharacterController;
		}
		
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
			moveDirection.x += hAxis * moveSpeed;
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
		
		controller.Move(moveDirection * Time.deltaTime);
	}
	
	/// <summary>
	/// Called when a collider is encountered.
	/// </summary>
	void OnControllerColliderHit(ControllerColliderHit hit){
		
		Debug.DrawLine(hit.point, hit.point + hit.normal, drawColor, 0.1f);
		
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
	
	void OnCollisionEnter (Collision collision){
		print(collision.gameObject.name);
	}
	
	#region External Interface
	
	public bool IsAttacking(){
		return animation.IsPlaying("Whacking");
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
	public override float TakeDamage (Attack atk)
	{
		stats.HP -= (atk.damageValue - stats.defense);
	
		lastDamage = (atk.damageValue - stats.defense);
		
		return (atk.damageValue - stats.defense);
	}
	
	
	
	#endregion
	
	private float lastDamage = 0f;
	
	public void OnGUI(){
		GUI.TextField(new Rect(0, 50, 100, 20), "Dmg: "+lastDamage);
	}
	
}