using UnityEngine;
using System.Collections;
using LevelGen;

public class HUD : MonoBehaviour {

	#region Public Vars
	
	public float curScore = 0f;
	public float maxScore = 100f;// The maximium 
	public int tankScore=0;
	public Stats statsVar;
	public DethmurderControl player;
	
	public Texture2D whiteTexture;
	public Texture2D greenTexture;
	public Texture2D tanktexture;
	public Texture2D grayTexture;
	
	public Texture2D HPTank;
	public Texture2D MPTank;
	public Texture2D EmptyTank;
	
	public Vector2 HPTankOffset;
	public Vector2 MPTankOffset;
	
	public GUIStyle HPStyle;
	public Rect HPTextRect;
	
	public GUIStyle MPStyle;
	public Rect MPTextRect;
	
	public Rect mapRect;
	public Texture2D mapBG;
	public int borderWidth;
	private Texture2D mapTex;
	
	
	#endregion
	
	// Use this for initialization
	void Start () {
		
		// Initialize it!
		mapTex = new Texture2D(Chambers.CHAMBER_WIDTH * 3, Chambers.CHAMBER_HEIGHT * 3);
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI(){
		
		// Draw the HP and MP Tanks
		int totalHPTanks = ((int)player.stats.MaxHP) / ((int)player.stats.HPUnit);
		int numHPTanks = ((int) player.stats.HP) /((int) player.stats.HPUnit);
		int HPremainder = ((int) player.stats.HP) %((int) player.stats.HPUnit);
		
		int totalMPTanks = ((int)player.stats.MaxMP) / ((int)player.stats.MPUnit);
		int numMPTanks = ((int) player.stats.MP) /((int) player.stats.MPUnit);
		int MPremainder = ((int) player.stats.MP) %((int) player.stats.MPUnit);
		
		DrawMap();
		
		// Text Drawing
		GUI.Label(HPTextRect, ""+HPremainder, HPStyle);
		GUI.Label(MPTextRect, ""+MPremainder, MPStyle);
		
		// Tank Drawing
		Vector2 hpOffset = HPTankOffset;
		
		for (int i = 0; i < totalHPTanks; i++){
			
			if (i >= numHPTanks){
				GUI.DrawTexture(new Rect(hpOffset.x, hpOffset.y, EmptyTank.width, EmptyTank.height), EmptyTank);
			}
			else{
				GUI.DrawTexture(new Rect(hpOffset.x, hpOffset.y, HPTank.width, HPTank.height), HPTank);
			}
			
			hpOffset.x += HPTank.width;
		}
		
		Vector2 mpOffset = MPTankOffset;
		
		for (int i = 0; i < totalMPTanks; i++){
			
			if (i >= numMPTanks){
				GUI.DrawTexture(new Rect(mpOffset.x, mpOffset.y, EmptyTank.width, EmptyTank.height), EmptyTank);
			}
			else{
				GUI.DrawTexture(new Rect(mpOffset.x, mpOffset.y, MPTank.width, MPTank.height), MPTank);
			}
			
			mpOffset.x += MPTank.width;
		}
		
		
	}
	
	void DrawMap(){
		
		// Draw one red pixel!!
		coords idx = CubeGen.Singleton.TileIndex(new Vector2(player.transform.position.x, player.transform.position.y));
		coords ridx = CubeGen.Singleton.RoomIndex(new Vector2(player.transform.position.x, player.transform.position.y));
		
		// Get the current coordinates
		Room[,] rooms = new Room[3,3];
		
		var c = ridx;
		
		for (int x = -1; x <= 1; x++){
			for (int y = -1; y <= 1; y++){
				if ( c.X + x >= 0 && c.X + x < CubeGen.Singleton.gameMaze.Width
					&& c.Y + y >= 0 && c.Y + y < CubeGen.Singleton.gameMaze.Height){
					rooms[x+1, y+1] = CubeGen.Singleton.SubRoom(new coords(c.X + x, c.Y + y));
				}
			}
		}
		
		
		
		idx.X -= Chambers.CHAMBER_WIDTH * ridx.X;
		idx.Y -= Chambers.CHAMBER_HEIGHT * ridx.Y;
		
		for (int x = 0; x < 3; x++){
			for (int y = 0; y < 3; y++){
				
				Room currentRoom = rooms[x,y];
				
				// The pic!
				for (int xx = 0; xx < Chambers.CHAMBER_WIDTH; xx++){
					for (int yy = 0; yy < Chambers.CHAMBER_HEIGHT; yy++){
						
						// Pixel coords
						int xxx = x * Chambers.CHAMBER_WIDTH + xx;
						int yyy = y * Chambers.CHAMBER_HEIGHT +yy;
						
						
						if (currentRoom == null){
							mapTex.SetPixel(xxx, yyy, Color.black);
						}
						else{
							switch (currentRoom[xx,yy].Type){
							case TileType.BLANK:
								mapTex.SetPixel(xxx,yyy, Color.clear);
								break;
							case TileType.ROCK:
								mapTex.SetPixel(xxx,yyy, Color.gray);
								break;
							case TileType.SNOW:
								mapTex.SetPixel(xxx,yyy, Color.white);
								break;
							}
						}
					}
				}
					
			}
		}
		
		// Player position
		mapTex.SetPixel(idx.X + Chambers.CHAMBER_WIDTH, idx.Y + Chambers.CHAMBER_HEIGHT, Color.red);
		
		mapTex.Apply();
		
		GUI.DrawTexture(new Rect(Screen.width - mapRect.width, 0, mapRect.width, mapRect.height), mapBG);
		
		GUI.DrawTexture(new Rect(Screen.width - mapRect.width + borderWidth, borderWidth, mapRect.width - borderWidth*2, mapRect.height - borderWidth*2), mapTex);
		
	}
}
