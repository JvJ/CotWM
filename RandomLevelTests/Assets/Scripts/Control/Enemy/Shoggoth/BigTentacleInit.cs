using UnityEngine;
using System.Collections;

public class BigTentacleInit : MonoBehaviour {

	// Use this for initialization
	void Start () {
		animation["Wiggle"].wrapMode = WrapMode.PingPong;
		animation["Wiggle"].time = Random.value;
		animation["Smack_scripted"].speed = 4.0f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
