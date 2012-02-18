using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {

	#region Script-Set parameters
	
	public GameObject targetObject = null;
	
	#endregion
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (targetObject != null){
			var p = targetObject.transform.position;
			transform.position = new Vector3(p.x, p.y, transform.position.z);
		}
	}
}
