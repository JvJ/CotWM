using UnityEngine;
using System.Collections;
using System;

public class TestGUIs : MonoBehaviour {

	
	/// <summary>
	/// OK LETS TRY THIS LAMBDA EXPRESSION SHIT!
	/// </summary>
	void OnGUI(){
		
		Func<bool> x =()=> GUI.Button(new Rect(0,200,100,20), "Sup bro!");
		
		if (x()){
			print("Awesome it worked");
		}
	}
}
