using UnityEngine;
using System.Collections;

public class CharacterControl : MonoBehaviour {
	
	
	#region Tunable Parameters
	
	public float moveSpeed = 10f; 
	
	public float jumpSpeed = 20f;
	
	public float wallJumpSpeed = 20f;
	
	public float wallJumpAngle = 45;
	
	public float wallJumpTimeOut = 0.5f;
	
	public WrapMode walkingWrapMode = WrapMode.PingPong;
	
	public WrapMode whackingWrapMode = WrapMode.Default;
	
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
	
	// Use this for initialization
	void Start () {
		
		animation["Walking"].layer = 0;
		animation["Walking"].enabled = true;
		animation["Walking"].wrapMode = walkingWrapMode;
		
		animation["Whacking"].layer = 1;
		animation["Whacking"].enabled = true;
		animation["Whacking"].wrapMode = whackingWrapMode;
	}
	
	void Awake(){
		moveDirection = new Vector3(1.0f, 0.0f, 0.0f);
	}
	
	// Update is called once per frame
	void Update () {
		
		var moveDirection = Vector3.zero;
		float hAxis = Input.GetAxis("Horizontal");
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
			moveDirection.x += hAxis * moveSpeed;
		}
		else{
			
			moveDirection.x += v.x;
		}
		
		vSpeed += Physics.gravity.y * Time.deltaTime;
		
		if (hAxis == 0){
			transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
			animation.Stop("Walking");
		}
		else if (hAxis < 0){
			transform.rotation = Quaternion.Euler(new Vector3(0,90,0));
			animation.CrossFade("Walking");
		}
		else{
			transform.rotation = Quaternion.Euler(new Vector3(0,-90,0));
			animation.CrossFade("Walking");
		}
		
		// Handling the whacking and attacking!
		if (Input.GetButton("Whack")){
			animation.CrossFade("Whacking");
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
}