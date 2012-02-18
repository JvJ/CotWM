using UnityEngine;
using System.Collections;
using LevelGen;

public class CubeGen : MonoBehaviour {
	
	
	#region Level Creation Parameters
	
	public Vector2 BottomLeft = Vector2.zero;
	
	float cubeSize = 1.0f;
	
	public int width = 10;
	
	public int height = 10;
	
	public string textFile = "TestLevel1";
		
	#endregion
	
	/// <summary>
	/// Takes a room as a parameter and adds it to the stage.
	/// </summary>
	/// <param name="room">
	/// A <see cref="Room"/>
	/// </param>
	void MakeRoom(Room room){
		
		for (int x = 0; x < room.Width; x++){
			for (int y = 0; y < room.Height; y++){
				
				GameObject cube;
				Transform trans;
				Vector2 pos;
				MeshRenderer mRend;
				
				switch (room[x,y].Type){
				case TileType.SNOW:
					cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
					trans = cube.transform;
					pos = BottomLeft + new Vector2((x+0.5f)*cubeSize, (y+0.5f)*cubeSize);
					trans.position = new Vector3(pos.x, pos.y, 0f);
					
					// Get the material component!
					mRend = (MeshRenderer)cube.GetComponent(typeof(MeshRenderer));
					mRend.material.color = new Color(1.0f, 1.0f, 1.0f);
					break;
					
				case TileType.ROCK:
					cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
					trans = cube.transform;
					pos = BottomLeft + new Vector2((x+0.5f)*cubeSize, (y+0.5f)*cubeSize);
					trans.position = new Vector3(pos.x, pos.y, 0f);
					
					// Get the material component!
					mRend = (MeshRenderer)cube.GetComponent(typeof(MeshRenderer));
					mRend.material.color = new Color(0.0f, 0.0f, 0.0f);
					break;
				}
			}
		}
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	void Awake() {
		
		TextAsset t = Resources.Load(textFile) as TextAsset;
				
		Room r = Room.FromString(t.text); 
		
		MakeRoom(r);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}