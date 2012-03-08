using UnityEngine;
using System.Collections;

public class CursorHit : MonoBehaviour {
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	private Vector2 mousePos;
	
	// Update is called once per frame
	void LateUpdate () {
		
		Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		RaycastHit hit;
		
		int mask = 0;
		mask |= 1 << LayerMask.NameToLayer("MousePlane");
			
		
		if (Physics.Raycast(cursorRay, out hit, Mathf.Infinity, mask)){
			transform.position = new Vector3(hit.point.x, hit.point.y, 0);
		}
	}
	
	void OnGUI(){
		GUI.Box(new Rect(0,0,200,50), new GUIContent("Mouse: "+Input.mousePosition));
	}
}
