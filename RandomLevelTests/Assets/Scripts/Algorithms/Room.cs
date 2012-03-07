using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LevelGen
{
	
	public class RoomFormatException : Exception{
		
		string msg;
		
		public RoomFormatException(string s){
			this.msg = s;
		}
		
		public override string ToString ()
		{
			return msg;
		}
	}
	
    /// <summary>
    /// A room is a grid of tiles.
    /// </summary>
    public class Room : ICloneable
    {

        /// <summary>
        /// Private field containing all the tiles.
        /// </summary>
        private Tile[,] tiles;

        /// <summary>
        /// Indexer into the grid.
        /// </summary>
        /// <param name="x">Across from left.</param>
        /// <param name="y">Down from top.</param>
        /// <returns>The tile!</returns>
        public Tile this[int x, int y]{
            get { return tiles[x, y]; }
            set { tiles[x, y] = value; }
        }

        /// <summary>
        /// Width across.
        /// </summary>
        public int Width
        {
            get { return tiles.GetLength(0); }
        }

        /// <summary>
        /// Height up.
        /// </summary>
        public int Height
        {
            get { return tiles.GetLength(1); }
        }

        /// <summary>
        /// Create a room of the specified size.  The default will be blank.
        /// </summary>
        /// <param name="width">Tiles across.</param>
        /// <param name="height">Tiles up.</param>
        public Room(int width, int height)
        {
            // Default value should be blank.
            tiles = new Tile[width, height];
        }
		

        #region Static Constructors
		
		/// <summary>
		/// Totally random room generation 
		/// </summary>
		/// <param name="width">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="height">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="Room"/>
		/// </returns>
        public static Room RandomRoom(int width, int height)
        {
            System.Random r = new System.Random();

            Room ret = new Room(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    TileType t = TileType.BLANK;

                    switch (r.Next() % 3)
                    {
                        case 0:
                            t = TileType.BLANK;
                            break;
                        case 1:
                            t = TileType.ROCK;
                            break;
                        case 2:
                            t = TileType.SNOW;
                            break;
                    }

                    ret[x, y] = new Tile(t);
                }
            }

            return ret;
        }
		
		/// <summary>
		/// Read the room from a string representation. 
		/// </summary>
		/// <param name="s">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="Room"/>
		/// </returns>
		public static Room FromString(string s){
			
			var lines = (from str in s.Split('\r','\n')
						where str.Length > 0
						select str).ToArray();
			
			int height = lines.Length;
			
			if (height == 0){
				throw new RoomFormatException("Zero lines in input.");
			}
			
			int width = lines[0].Length;
			
			Room ret = new Room(width, height);
			
			for (int y = height-1; y >= 0; y--){
				
				string currentLine = lines[height - y - 1];
				
				if (currentLine.Length != width){
					
					Debug.Log("Width: "+width+" doesn't match line: "+currentLine.Length);
					throw new RoomFormatException("Inconsistent line width ("+width+" vs. "+currentLine.Length+") at y = "+y);
				}
				
				for (int x = 0; x < width; x++){
					ret[x,y] = Tile.FromChar(currentLine[x]);
				}
			}
			
			return ret;
		}
		
		/// <summary>
		/// Crappy, naive random generation algorithm. 
		/// </summary>
		/// <param name="width">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="height">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="numRooms">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="Room"/>
		/// </returns>
		public static Room ChambersGen0(int width, int height, int numRooms){
			
			Room ret = new Room(width, height);
			for (int x = 0; x < width; x++){
				for (int y = 0; y < width; y++){
					ret.tiles[x,y] = new Tile(TileType.SNOW);
				}
			}
			
			System.Random r = new System.Random();
			
			for (int i = 0; i < numRooms; i++){
				
				// Randomly select the index
				ChamberType cIdx = (ChamberType)(r.Next() % (int)ChamberType.NumElements);
				
				Room rm = Chambers.Rooms(cIdx);
				
				int x = r.Next() % width;
				int y = r.Next() % height;
				
				ret.CopyRoom(rm, x, y);
			}
			
			return ret;
		}
		
		public static Room RoomFromMaze(Maze m){
			
			Room ret = new Room(m.Width, m.Height);
			
			for (int x = 0; x < ret.Width; x++){
				for (int y = 0; y < ret.Height; y++){
					switch(m[x,y]){
					case MazeTileType.WALL:
						ret[x,y] = new Tile(TileType.ROCK);
						break;
					case MazeTileType.SPACE:
						ret[x,y] = new Tile(TileType.BLANK);
						break;
					default:
						ret[x,y] = new Tile(TileType.BLANK);
						break;
					}
				}
			}
			
			return ret;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m">
		/// A <see cref="Maze"/>
		/// </param>
		/// <param name="rand">
		/// A <see cref="System.Random"/>
		/// </param>
		/// <returns>
		/// A <see cref="Room"/>
		/// </returns>
		public static Room ChambersFromMaze(Maze m, System.Random rand){
			
			Room ret = new Room(m.Width * Chambers.CHAMBER_WIDTH, m.Height  * Chambers.CHAMBER_HEIGHT);
			
			for (int x = 0; x < m.Width; x++){
				for (int y = 0; y < m.Height; y++){
					
					// TODO: Randomize this!!!
					Room r = Chambers.Rooms((ChamberType)(rand.Next() % (int)ChamberType.NumElements)).Clone() as Room;
					
					switch(m[x,y]){
					case MazeTileType.SPACE:
						
						// Handle the door carving
						// Left and Right
						if (x > 0){
							if (m[x-1,y] == MazeTileType.SPACE){
								Chambers.LeftDoor(r, TileType.SNOW);
							}
						}
						if (x < m.Width-1){
							if (m[x+1,y] == MazeTileType.SPACE){
								Chambers.RightDoor(r, TileType.SNOW);
							}
						}
						// Bottom and Top
						if (y > 0){
							if (m[x,y-1] == MazeTileType.SPACE){
								Chambers.BottomDoor(r, TileType.SNOW);
							}
						}
						if (y < m.Height-1){
							if (m[x,y+1] == MazeTileType.SPACE){
								Chambers.TopDoor(r, TileType.SNOW);
							}
						}
						
						ret.CopyRoom(r, x * Chambers.CHAMBER_WIDTH, y * Chambers.CHAMBER_HEIGHT);
						break;
					}
					
				}
			}
			
			return ret;
		}
		
        #endregion
		
		#region Methods
		
		public void CopyRoom(Room source, int x, int y){
			
			int xLim = Math.Min(Width, x + source.Width);
			int yLim = Math.Min(Height, y + source.Height);
			
			for (int xx = x; xx < xLim; xx++){
				for (int yy = y; yy < yLim; yy++){
					tiles[xx, yy] = source[xx - x, yy - y];
				}
			}
			
		}
		
        /// <summary>
        /// String representation of the grid.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string ret = "";

            for (int y = Height-1; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    ret += this[x, y];
                }
                ret += "\r\n";
            }

            return ret;
        }
		
		public object Clone ()
		{
			Room ret = new Room(0, 0);
			
			ret.tiles = (Tile[,]) tiles.Clone();
			
			return ret;
		}
		
		#endregion
    }
}
