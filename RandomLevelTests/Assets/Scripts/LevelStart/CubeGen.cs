using UnityEngine;
using System.Collections;
using LevelGen;

public class CubeGen : MonoBehaviour {
	
	
	#region Level Creation Parameters
	
	public Vector2 BottomLeft = Vector2.zero;
	
	public float cubeSize = 1.0f;
	
	public int cubeLayer = 0;
	
	public int width = 100;
	
	public int height = 100;
	
	public int numRooms = 100;
	
	public int branchRate = 0;
	
	public bool noDiagonals = true;
	
	public string textFile = "TestLevel1";
	
	public GameObject player = null;
	
	public GameObject firstEnemy = null;
		
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
					cube.layer = cubeLayer;
					trans = cube.transform;
					trans.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
					pos = BottomLeft + new Vector2((x+0.5f)*cubeSize, (y+0.5f)*cubeSize);
					trans.position = new Vector3(pos.x, pos.y, 0f);
					
					//cube.tag = "SNOW";
					cube.name = "SNOW";
					cube.tag = "SNOW";
					cube.layer = LayerMask.NameToLayer("Terrain");
					
					// Get the material component!
					mRend = (MeshRenderer)cube.GetComponent(typeof(MeshRenderer));
					mRend.material.color = new Color(1.0f, 1.0f, 1.0f);
					break;
					
				case TileType.ROCK:
					cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
					cube.layer = cubeLayer;
					trans = cube.transform;
					trans.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
					pos = BottomLeft + new Vector2((x+0.5f)*cubeSize, (y+0.5f)*cubeSize);
					trans.position = new Vector3(pos.x, pos.y, 0f);
					
					//cube.tag = "ROCK";
					cube.name = "ROCK";
					cube.tag = "ROCK";
					cube.layer = LayerMask.NameToLayer("Terrain");
					
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
				
		//Room r = Room.FromString(t.text); 
		
		//r.CopyRoom(Chambers.Rooms(ChamberSize.SIZE_1),0,0);
		
		//r.CopyRoom(Chambers.Rooms(ChamberSize.SIZE_2),20,0);
		
		//Room r = Room.ChambersGen0(width, height, numRooms);
		
		coords start = new coords();
		
		Maze m = Maze.GrowingTree(width, height, branchRate, noDiagonals, out start);
		
		Room r = Room.ChambersFromMaze(m, new System.Random());
		
		player.transform.position = new Vector3(
		                                        BottomLeft.x + ((start.X + 0.5f) * Chambers.CHAMBER_WIDTH * cubeSize),
		                                        BottomLeft.y + ((start.Y + 0.5f) * Chambers.CHAMBER_HEIGHT * cubeSize), 
		                                        0);
		
		firstEnemy.transform.position = new Vector3(
		                                        BottomLeft.x + ((start.X + 0.5f) * Chambers.CHAMBER_WIDTH * cubeSize),
		                                        BottomLeft.y + ((start.Y + 0.2f) * Chambers.CHAMBER_HEIGHT * cubeSize), 
		                                        0);
		
		// Temporary!
		// Output the maze and the room to files
		
		System.IO.File.WriteAllText("mazeText.txt", m.ToString());
		System.IO.File.WriteAllText("levelText.txt", r.ToString());
		
		
		MakeRoom(r);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}