using UnityEngine;
using System.Collections;

/// <summary>
/// Controls Xl'Grhthbtrg's tentacle aiming.
/// </summary>
public class XTentacleControl : MonoBehaviour {
	
	
	#region Public Params
	
	/// <summary>
	/// The blaster.
	/// </summary>
	public LightningBolt blaster;
	
	#endregion
	
	
	/// <summary>
	/// If greater than 0, the counterclockwise animation will be played.
	/// Clockwise played if less than 0.
	/// </summary>
	public float rotationRatio = 0;
	
	/// <summary>
	/// The end transform.
	/// </summary>
	public Transform endTransform;
	
	/// <summary>
	/// The relative end position.
	/// </summary>
	public Vector3 relativeEndPos = Vector3.zero;
	
	public float betaAngle = 0;
	
	public float alphaAngle = 0;
	
	public float rangle = 0;
	
	/// <summary>
	/// The time limit for blasting.
	/// </summary>
	private float timeLimit = 0;
	
	/// <summary>
	/// The current time for blasting.
	/// </summary>
	private float currentTime = 0;
	
	// Use this for initialization
	void Start () {
		
		// Set up the transforms and the relative end positions.
		// NOTE: This is assuming that the tentacle starts in the default position
		endTransform = transform.Find("Armature/Bone/Bone_1/Bone_2/Bone_3/Bone_4");
		relativeEndPos = endTransform.position - transform.position; 
		
		betaAngle = Mathf.Atan(relativeEndPos.y / relativeEndPos.x);
		
		animation["Still"].blendMode = AnimationBlendMode.Blend;
		animation["Still"].wrapMode = WrapMode.Loop;
		
		animation["CCW"].blendMode = AnimationBlendMode.Blend;
		animation["CCW"].wrapMode = WrapMode.Loop;
		
		animation["CW"].blendMode = AnimationBlendMode.Blend;
		animation["CW"].wrapMode = WrapMode.Loop;
	}
	
	// Update is called once per frame
	void Update () {
		
		// Which animation to use
		string anim = rotationRatio > 0 ? "CCW" : "CW";
		
		float r = Mathf.Abs(rotationRatio);
		
		// Here we blend the active animation with the still animation
		animation.Blend(anim, r);
		animation.Blend("Still", 1f-r);
		
	}
	
	/// <summary>
	/// Gets the rotation ratio of the tentacle with respect to a certain point.
	/// </summary>
	/// <param name='point'>
	/// Point.
	/// </param>
	public void ComputeRotationRatio(Vector3 point){
		
		/*float pointDX = point.x - transform.position.x;
		float pointDY = point.y - transform.position.y;
		
		// Alpha is the angle to the target point
		float alphaTan = pointDY / pointDX;
		
		float alpha = alphaAngle = Mathf.Atan(alphaTan);*/
		
		Vector3 offset = point - transform.position;
		
		Vector2 os2 = new Vector2(offset.x, offset.y);
		
		Vector2 ep = new Vector2(relativeEndPos.x, relativeEndPos.y);
		
		Vector3 cr = Vector3.Cross(new Vector3(os2.x, os2.y), new Vector3(ep.x, ep.y));
		
		float rot = rangle = Vector2.Angle(ep, os2) * Mathf.Sign(cr.z) * Mathf.Deg2Rad;
		
		// Clamp it!
		/*if (rot > Mathf.PI / 2f){
			rot = rangle = Mathf.PI / 2f;
		}
		if (rot < -Mathf.PI / 2f){
			rot = rangle = -Mathf.PI / 2f;
		}*/
		
		rotationRatio = Mathf.Sin(rot);
		
	}
	
	public void Blast(Transform target){
		blaster.target = target;
	}
	
	public void UnBlast(){
		blaster.target = blaster.transform;
	}
	
	
}
