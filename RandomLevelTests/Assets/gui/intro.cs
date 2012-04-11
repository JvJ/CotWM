using UnityEngine;
using System.Collections;

public class intro : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnGUI() {
		/////what do we awant here?
		GUI.Label( new Rect(Screen.width/2-150,10,300,30),title);
		Event e = Event.current;
        if (e.isKey){
         //   Debug.Log("Detected a keyboard event!");
			Application.LoadLevel ("keyIntro");
		
		
		}
	
	
	}
}
