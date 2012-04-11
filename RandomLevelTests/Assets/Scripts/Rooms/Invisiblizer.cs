using UnityEngine;
using System.Collections;

public class Invisiblizer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void OnBecameVisible(){
		renderer.enabled = true;
	}
	
	public void OnBecameInvisible(){
		renderer.enabled = false;
	}
	
}
