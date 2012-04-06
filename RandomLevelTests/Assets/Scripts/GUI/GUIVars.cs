using UnityEngine;
using System.Collections;

public class GUIVars : MonoBehaviour {
	
	/// <summary>
	/// Gets or sets the singleton.
	/// </summary>
	/// <value>
	/// The singleton.
	/// </value>
	public static GUIVars Singleton{
		get;
		private set;
	}
	
	public GUIStyle gStyle;
	
	// Called before start
	void Awake(){
		Singleton = this;
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
