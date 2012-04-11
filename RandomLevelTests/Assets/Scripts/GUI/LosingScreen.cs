using UnityEngine;
using System.Collections;

public class LosingScreen : MonoBehaviour {

	public Texture2D bgTex;
	
	public Rect menuButtonRect;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI(){
		GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height),  bgTex);
		
		if (GUI.Button(menuButtonRect, "Main Menu")){
			Application.LoadLevel("mainMenu");
		}
	}
}
