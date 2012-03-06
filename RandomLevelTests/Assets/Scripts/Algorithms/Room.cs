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
    public class Room
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
				ChamberSize cIdx = (ChamberSize)(r.Next() % (int)ChamberSize.NumElements);
				
				Room rm = Chambers.Rooms(cIdx);
				
				int x = r.Next() % width;
				int y = r.Next() % height;
				
				ret.CopyRoom(rm, x, y);
			}
			
			return ret;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// A <see cref="Room"/>
		/// </returns>
		public static Room MazeRoom(int width, int height, int branchRate){
			return MazeRoom(width, height, branchRate, new System.Random());
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="seed">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="Room"/>
		/// </returns>
		public static Room MazeRoom(int width, int height, int branchRate, int seed){
			return MazeRoom(width, height, branchRate, new System.Random(seed));
		}
		
		/// <summary>
		/// Based on the Growing Tree algorithm.
		/// </summary>
		/// <param name="branchRate">
		/// Zero is unbiased, positive will make branches more frequent, negative will cause long passages.
		/// This controls the position in the list chosen: positive makes the start of the list more likely.
		/// Negative makes the end of the list more likely.
		/// Large negative values make the original point obvious.
		/// Try values between -10 and 10.
		/// </param>
		/// <param name="rand">
		/// A <see cref="System.Random"/>
		/// </param>
		/// <returns>
		/// A <see cref="Room"/>
		/// </returns>
		public static Room MazeRoom(int width, int height, int branchRate, System.Random rand){
			
			// The return value
			Room ret = new Room(width, height);
			
			// Initialize the grid to contain UU tiles
			for (int x = 0; x < width; x++){
				for (int y = 0; y < height; y++){
					ret.tiles[x,y] = new Tile(TileType.UU);
				}
			}
			
			// List of coordinates of unexposed but undetermined cells
			var frontier = new List<coords>();
			
			// Functions
			
			// This one makes the cell at (x,y) a space
			Action<int, int> carve
				= (int x, int y ) =>
			{
				var extra = new List<coords>();
				
				ret.tiles[x,y] = new Tile(TileType.BLANK);
				
				if (x > 0){
					if (ret.tiles[x-1,y].Type == TileType.UU){
						ret.tiles[x-1,y] = new Tile(TileType.XU);
						extra.Add(new coords(x-1,y));
					}
				}
				if (x < width-1){
					if (ret.tiles[x+1,y].Type == TileType.UU){
						ret.tiles[x+1,y] = new Tile(TileType.XU);
						extra.Add(new coords(x+1,y));
					}
				}
				if (y > 0){
					if (ret.tiles[x,y-1].Type == TileType.UU){
						ret.tiles[x,y-1] = new Tile(TileType.XU);
						extra.Add(new coords(x,y-1));
					}
				}
				if (y < height - 1){
					if (ret.tiles[x,y+1].Type == TileType.UU){
						ret.tiles[x,y+1] = new Tile(TileType.XU);
						extra.Add(new coords(x,y+1));
					}
				}
				
				// Add the shuffled list to the frontier
				frontier.AddRange(Misc.ShuffleList<coords>(extra, rand));
			};
			
			Action<int,int> harden = 
				(int x, int y)=>
			{
				ret.tiles[x,y] = new Tile(TileType.ROCK);
			};
			
			
			// Test cell at (x,y) : can this become a space?
			// True indicates it should become a space, false indicates
			// it should become a wall
			Func<int, int, bool, bool> check =
				(int x, int y, bool nodiagonals) =>
			{
				int edgeState = 0;
				
				if (x > 0){
					if (ret.tiles[x-1, y].Type == TileType.BLANK){
						edgeState += 1;
					}
				}
				if (x < width - 1){
					if (ret.tiles[x+1, y].Type == TileType.BLANK){
						edgeState += 2;
					}
				}
				if (y > 0){
					if (ret.tiles[x, y-1].Type == TileType.BLANK){
						edgeState += 4;
					}
				}
				if (y < height - 1){
					if (ret.tiles[x,y+1].Type == TileType.BLANK){
						edgeState += 8;
					}
				}
				
				// If this would make a diagonal connection, forbid it
				if (nodiagonals){
					if (edgeState == 1){
						if (x < width-1){
							if (y > 0){
								if (ret.tiles[x+1,y-1].Type == TileType.BLANK){
									return false;
								}
							}
							if (y < height-1){
								if (ret.tiles[x+1,y+1].Type == TileType.BLANK){
									return false;
								}
							}
						}
						return true;
					}
					if (edgeState == 2){
						if (x > 0){
							if (y > 0){
								if (ret.tiles[x-1, y-1].Type == TileType.BLANK){
									return false;
								}
							}
							if (y < height-1){
								if (ret.tiles[x-1,y+1].Type == TileType.BLANK){
									return false;
								}
							}
						}
						return true;
					}
					if (edgeState == 4){
						if (y < height-1){
							if (x > 0){
								if (ret.tiles[x-1,y+1].Type == TileType.BLANK){
									return false;
								}
							}
							if (x < width-1){
								if (ret.tiles[x+1,y+1].Type == TileType.BLANK){
									return false;
								}
							}
						}
						return true;
					}
					if (edgeState == 8){
						if (y > 0){
							if (x > 0){
								if (ret.tiles[x-1, y-1].Type == TileType.BLANK){
									return false;
								}
							}
							if (x < width-1){
								if (ret.tiles[x+1,y-1].Type == TileType.BLANK){
									return false;
								}
							}
						}
						return true;
					}
					
					return false;
				}
				else{
					if (edgeState == 1 || edgeState == 2 || edgeState == 4 || edgeState == 8){
						return true;
					}
					return false;
				}
				
			};
			
			// Now that the function definitions are over, the algorithm starts
			
			// Choose an original point at random and carve it out
			int xChoice = rand.Next(0, width);
			int yChoice = rand.Next(0, height);
			carve(xChoice, yChoice);
			
			while (frontier.Count > 0){
				// Select a random edge
				double pos = rand.NextDouble();
				pos = Math.Pow(pos, Math.Exp(-branchRate));
				
				int rIdx = (int)(pos * frontier.Count);
				coords choice = frontier[rIdx];
				
				if (check(choice.X, choice.Y, true)){
					carve(choice.X, choice.Y);
				}
				else{
					harden(choice.X, choice.Y);
				}
				
				frontier.RemoveAt(rIdx);
			}
			
			// Set unexposed cells to be walls
			for (int x = 0; x < width; x++){
				for (int y = 0; y < height; y++){
					if (ret.tiles[x,y].Type == TileType.UU){
						ret.tiles[x,y] = new Tile(TileType.ROCK);
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
		
		#endregion
    }
}
