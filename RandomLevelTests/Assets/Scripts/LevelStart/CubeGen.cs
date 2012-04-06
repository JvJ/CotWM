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
	
	public UnityEngine.Object snowPrefab = null;
	
	public UnityEngine.Object rockPrefab = null;
	
	public Room gameMap{
		get;
		private set;
	}
	
	public Maze gameMaze{
		get;
		private set;
	}
		
	#endregion
	
	#region Static bananas!
	
	public static CubeGen Singleton{
		get;
		private set;
	}
	
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
					//cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
					
					cube = GameObject.Instantiate(snowPrefab) as GameObject;
					
					
					cube.layer = cubeLayer;
					trans = cube.transform;
					trans.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
					pos = BottomLeft + new Vector2((x+0.5f)*cubeSize, (y+0.5f)*cubeSize);
					trans.position = new Vector3(pos.x, pos.y, 0f);
					
					print("Attempted to set position to : "+pos);
					print("Instead, got : "+trans.position);
					
					//cube.tag = "SNOW";
					cube.name = "SNOW";
					cube.tag = "SNOW";
					cube.layer = LayerMask.NameToLayer("Terrain");
					
					// Get the material component!
					mRend = (MeshRenderer)cube.GetComponent(typeof(MeshRenderer));
					mRend.material.color = new Color(1.0f, 1.0f, 1.0f);
					print( "Final mark!!");
					break;
					
				case TileType.ROCK:
					//cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
					
					cube = GameObject.Instantiate(rockPrefab) as GameObject;
					
					cube.layer = cubeLayer;
					trans = cube.transform;
					trans.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
					pos = BottomLeft + new Vector2((x+0.5f)*cubeSize, (y+0.5f)*cubeSize);
					trans.position = new Vector3(pos.x, pos.y, 0f);
					
					//cube.tag = "ROCK";
					cube.name = "ROCK";
					cube.tag = "ROCK";
					cube.layer = LayerMask.NameToLayer("Default");
					
					// Get the material component!
					mRend = (MeshRenderer)cube.GetComponent(typeof(MeshRenderer));
					mRend.material.color = new Color(0.0f, 0.0f, 0.0f);
					break;
				}
			}
		}
		
		// Add triggers
		for (int x = 0; x < gameMaze.Width; x++){
			for (int y = 0; y < gameMaze.Height; y++){
				var r = rectFromCoords(new coords(x,y));
				
				var g = new GameObject("Room ("+x+","+y+")");
				
				g.transform.position = new Vector3(r.center.x, r.center.y, 0);
				
				g.transform.localScale = new Vector3(r.width, r.height, cubeSize);
				
				var collider = g.AddComponent(typeof(BoxCollider)) as BoxCollider;
				
				collider.isTrigger = true;
			}
		}
	}
	
	// Use this for initialization
	void Start () {
	
		// :o How selfish!!!
		Singleton = this;
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
		
		gameMap = r;
		
		gameMaze = m;
		
		player.transform.position = new Vector3(
		                                        BottomLeft.x + ((start.X + 0.5f) * Chambers.CHAMBER_WIDTH * cubeSize),
		                                        BottomLeft.y + ((start.Y + 0.5f) * Chambers.CHAMBER_HEIGHT * cubeSize), 
		                                        0);
		
		firstEnemy.transform.position = new Vector3(
		                                        BottomLeft.x + ((start.X + 0.5f) * Chambers.CHAMBER_WIDTH * cubeSize),
		                                        BottomLeft.y + ((start.Y + 0.2f) * Chambers.CHAMBER_HEIGHT * cubeSize), 
		                                        0);
		
		
		MakeRoom(r);
		
		
		
		// Temporary!
		// Output the maze and the room to files
		
		/*System.IO.File.WriteAllText("mazeText.txt", m.ToString());
		System.IO.File.WriteAllText("levelText.txt", r.ToString());*/
	}
	
	// Update is called once per frame
	void Update () {
	
		// Debug rendering
		// Iterate through all the rooms and draw them!
		for (int x = 0; x < gameMaze.Width; x++){
			for (int y = 0; y < gameMaze.Height; y++){
				var r = rectFromCoords(new coords(x,y));
				
				var bl = new Vector3(r.xMin, r.yMin);
				var br = new Vector3(r.xMax, r.yMin);
				var tr = new Vector3(r.xMax, r.yMax);
				var tl = new Vector3(r.xMin, r.yMax);
				
				Debug.DrawLine(bl, br, Color.red, 0f, false);
				Debug.DrawLine(br, tr, Color.red, 0f, false);
				Debug.DrawLine(tr, tl, Color.red, 0f, false);
				Debug.DrawLine(tl, bl, Color.red, 0f, false);
			}
		}
		
	}
	
	#region Utility Methods
	
	/// <summary>
	/// Currents the room.
	/// </summary>
	/// <returns>
	/// A rectangle of the current room.  The "top" property should be used as
	/// the bottom!!!
	/// </returns>
	/// <param name='position'>
	/// Position.
	/// </param>
	public Rect CurrentRoom(Vector2 position){
		
		return rectFromCoords(RoomIndex(position));
	}
	
	public Rect rectFromCoords(coords c){
		return new Rect( Chambers.CHAMBER_WIDTH * cubeSize * c.X + BottomLeft.x, Chambers.CHAMBER_HEIGHT * cubeSize * c.Y + BottomLeft.y,
			Chambers.CHAMBER_WIDTH * cubeSize, Chambers.CHAMBER_HEIGHT * cubeSize);
	}
	
	public coords RoomIndex(Vector2 position){
		
		int windex = (int)((position.x - BottomLeft.x) / (Chambers.CHAMBER_WIDTH * cubeSize));
		
		int hindex = (int)((position.y - BottomLeft.y) / (Chambers.CHAMBER_HEIGHT * cubeSize));
		
		return new coords(windex, hindex);
	}
	
	public intRect SubRoomRect(coords position){
		return new intRect(position.X * Chambers.CHAMBER_WIDTH, position.Y * Chambers.CHAMBER_HEIGHT,
			Chambers.CHAMBER_WIDTH, Chambers.CHAMBER_HEIGHT); 
	}
	
	public Room SubRoom(coords position){
		return gameMap.Slice(SubRoomRect(position));
	}
	
	#endregion
}