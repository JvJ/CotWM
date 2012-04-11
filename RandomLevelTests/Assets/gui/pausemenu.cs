using UnityEngine;
using System.Collections;

public class pausemenu : MonoBehaviour {
	public int boxHight=300;
	public int boxwidth=50;
	public int xposition=Screen.width/2-300;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnGUI() {
    	if ( GUI.Button(new Rect(xposition, 20,boxHight,boxwidth), "quick save")){
			Application.LoadLevel ("quicksave");
		}
		//GUI.Button(new Rect(xposition, 20+50+10, boxHight, boxwidth), "options");
		GUI.Button(new Rect(xposition, 20+50+10+50+10, boxHight, boxwidth), "exit");
		
    }
}
