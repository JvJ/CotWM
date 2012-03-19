using UnityEngine;
using System.Collections;

public class TentacleInit : MonoBehaviour {

	// Use this for initialization
	void Start () {
		animation["Armature_001Action"].wrapMode = WrapMode.PingPong;
		animation["Armature_001Action"].time = Random.value;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
