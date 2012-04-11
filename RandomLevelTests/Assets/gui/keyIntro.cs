using UnityEngine;
using System.Collections;

public class keyIntro : MonoBehaviour {

	public Texture2D bgTex;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnGUI() {
		/////what do we awant here?
		string title="key intro page";
		GUI.Label( new Rect(Screen.width/2-150,10,300,30),title);
		Event e = Event.current;
        if (e.isKey){
         //   Debug.Log("Detected a keyboard event!");
			Application.LoadLevel("TestScene");
		}
		
		GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), bgTex);
	}
}
