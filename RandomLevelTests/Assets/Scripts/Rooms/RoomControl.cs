using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LevelGen;

public class RoomControl : MonoBehaviour {
	
	
	
	public GameObject PlayerStart{
		get;
		set;
	}
	
	public List<EntityControl> Enemies{
		get;
		private set;
	}
	
	public List<GameObject> TerrainObjs{
		get;
		private set;	
	}
	
	public coords Position{
		get;
		set;
	}
	
	public DethmurderControl player;
	
	/// <summary>
	/// Adds the terrain and parents it to this object.
	/// </summary>
	/// <param name='o'>
	/// O.
	/// </param>
	public void AddTerrain(GameObject o){
		TerrainObjs.Add(o);
		o.transform.parent = this.transform;
	}
	
	public void AddEnemy(EntityControl o){
		Enemies.Add(o);
		o.transform.parent = transform;
	}
	
	/// <summary>
	/// Use this when creating new ones!.
	/// </summary>
	void InitLists(){
		if (TerrainObjs == null){
			TerrainObjs = new List<GameObject>();
		}
		if (Enemies == null){
			Enemies = new List<EntityControl>();
		}
	}
	
	void Awake() {
		if (TerrainObjs == null){
			TerrainObjs = new List<GameObject>();
		}
		if (Enemies == null){
			Enemies = new List<EntityControl>();
		}
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	public void WakeUpRoom(){
		
		foreach (var o in Enemies){
			if (o == null){continue;}
			o.SwitchState(o.defaultState);
			o.gameObject.SetActiveRecursively(true);
		}
		
		foreach (var o in TerrainObjs){
			if (o != null){
				o.SetActiveRecursively(true);
			}
		}
		/*
		foreach(EntityControl e in Enemies){
			e.SwitchState(e.defaultState);
			//e.turnOnMeshes();
			e.turnOnScript();
			e.enabled = true;
		}*/
		
		/*foreach(GameObject g in TerrainObjs){
			if (g != null && g.renderer != null){
				g.renderer.enabled = true;
			}
		}*/
	}
	
	public void FreezeRoom(){
		
		foreach (var o in Enemies){
			if (o == null){continue;}
			o.SwitchState(EntityState.STILL);
			o.gameObject.SetActiveRecursively(false);
		}
		
		foreach (var o in TerrainObjs){
			if (o != null){
				o.SetActiveRecursively(false);
			}
		}
		
		/*foreach(GameObject g in TerrainObjs){
			if (g != null && g.renderer != null){
				g.renderer.enabled = false;
			}
		}*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	/// <summary>
	/// Raises the trigger enter event.
	/// </summary>
	/// <param name='other'>
	/// Other.
	/// </param>
	void OnTriggerEnter (Collider other){
		
		if (other.gameObject.CompareTag("PLAYER")){
			player.CurrentRoom = this.Position;
			WakeUpRoom();
		}
		else{
			var c = other.gameObject.GetComponent(typeof(EntityControl)) as EntityControl;
			if (c != null){
				AddEnemy(c);
			}
		}
	}
	
	/// <summary>
	/// Raises the trigger exit event.
	/// </summary>
	/// <param name='other'>
	/// Other.
	/// </param>
	void OnTriggerExit(Collider other){
		
		if (other.gameObject.CompareTag("PLAYER")){
			FreezeRoom();
		}
		else{
			var c = other.gameObject.GetComponent(typeof(EntityControl)) as EntityControl;
			if (c != null){
				Enemies.RemoveAll((x)=>x==c);
			}
		}
	}
	
	void OnBecameVisible(){
		print(name+" became visible.");
	}
}
