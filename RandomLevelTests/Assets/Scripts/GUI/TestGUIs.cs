using UnityEngine;
using System.Collections;
using System;

public class TestGUIs : MonoBehaviour {
	
	private Texture2D tex;
	
	void Start(){
	
		tex = new Texture2D(256, 256);
		
		for ( int y = 0; y < tex.height; ++y) {
        for (int x = 0; x < tex.width; ++x) {
            var color = (x&y) != 0 ? Color.black : Color.red;
            tex.SetPixel (x, y, color);
        }
    }
		
		tex.Apply();
	}
	
	/// <summary>
	/// OK LETS TRY THIS LAMBDA EXPRESSION SHIT!
	/// </summary>
	void OnGUI(){
		
		//GUI.DrawTexture(new Rect(100, 0, 256, 256), tex);
		
		Func<bool> x =()=> GUI.Button(new Rect(0,200,100,20), "Sup bro!");
		
		if (x()){
			Time.timeScale = 1 - Time.timeScale;
			print("Awesome it worked");
		}
	}
}
