using UnityEngine;
using System.Collections;

public class DethmurderGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI(){
		GUI.Button( new Rect(0,0,200,100), "Click Me!");
	}
}