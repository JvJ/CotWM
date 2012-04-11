using UnityEngine;
using System.Collections;


//int 

public class mainmenu : MonoBehaviour {
	
	public Rect newGameBox;
	
	public Rect exitBox;
	
	public Texture2D bgTex;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnGUI() {
    	
		GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), bgTex);
		
		if ( GUI.Button(newGameBox, "New Game")){
			Application.LoadLevel ("intro");
		}
		//GUI.Button(new Rect(xposition, 20+50+10, boxHight, boxwidth), "options");
		if (GUI.Button(exitBox, "Exit")){
			
			//print("**************************************");
			Application.Quit();
		}
		
    }
	
}
