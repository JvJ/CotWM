using System;
using System.Collections.Generic;

namespace LevelGen
{
	public enum MazeTileType
	{
		SPACE,
		WALL,
		XU,
		UU,
	};
	
	public class Maze
	{
		
		private MazeTileType[,] tiles;
		
		public MazeTileType this[int x, int y]{
			get{return tiles[x,y];}
			private set {tiles[x,y] = value;}
		}
		
		public int Width{
			get{return tiles.GetLength(0);}
		}
			
		public int Height{
			get{return tiles.GetLength(1);}
		}
		
		public Maze (int width, int height)
		{
			tiles = new MazeTileType[width, height];
		}
		
		#region Static Constructors
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// A <see cref="Room"/>
		/// </returns>
		public static Maze GrowingTree(int width, int height, int branchRate, bool nDiag, out coords StartLocation, out coords EndLocation){
			return GrowingTree(width, height, branchRate, nDiag, new System.Random(), out StartLocation, out EndLocation);
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
		public static Maze GrowingTree(int width, int height, int branchRate, bool nDiag, int seed, out coords StartLocation, out coords EndLocation){
			return GrowingTree(width, height, branchRate, nDiag, new System.Random(seed), out StartLocation, out EndLocation);
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
		public static Maze GrowingTree(int width, int height, int branchRate, bool nDiag, System.Random rand, out coords StartLocation, out coords EndLocation){
			
			// The return value
			Maze ret = new Maze(width, height);
			
			// Initialize the grid to contain UU tiles
			for (int x = 0; x < width; x++){
				for (int y = 0; y < height; y++){
					ret[x,y] = MazeTileType.UU;
				}
			}
			
			// List of coordinates of unexposed but undetermined cells
			var frontier = new List<coords>();
			
			// Functions
			
			#region Carve
			
			// This one makes the cell at (x,y) a space
			Action<int, int> carve
				= (int x, int y ) =>
			{
				var extra = new List<coords>();
				
				ret[x,y] = MazeTileType.SPACE;
				
				if (x > 0){
					if (ret[x-1,y] == MazeTileType.UU){
						ret[x-1,y] = MazeTileType.XU;
						extra.Add(new coords(x-1,y));
					}
				}
				if (x < width-1){
					if (ret[x+1,y] == MazeTileType.UU){
						ret[x+1,y] = MazeTileType.XU;
						extra.Add(new coords(x+1,y));
					}
				}
				if (y > 0){
					if (ret[x,y-1] == MazeTileType.UU){
						ret[x,y-1] = MazeTileType.XU;
						extra.Add(new coords(x,y-1));
					}
				}
				if (y < height - 1){
					if (ret[x,y+1] == MazeTileType.UU){
						ret[x,y+1] = MazeTileType.XU;
						extra.Add(new coords(x,y+1));
					}
				}
				
				// Add the shuffled list to the frontier
				frontier.AddRange(Misc.ShuffleList<coords>(extra, rand));
			};
			
			#endregion
			
			#region Harden
			Action<int,int> harden = 
				(int x, int y)=>
			{
				ret[x,y] = MazeTileType.WALL;
			};
			
			#endregion
			
			#region Check
			// Test cell at (x,y) : can this become a space?
			// True indicates it should become a space, false indicates
			// it should become a wall
			Func<int, int, bool, bool> check =
				(int x, int y, bool nodiagonals) =>
			{
				int edgeState = 0;
				
				if (x > 0){
					if (ret[x-1, y] == MazeTileType.SPACE){
						edgeState += 1;
					}
				}
				if (x < width - 1){
					if (ret[x+1, y] == MazeTileType.SPACE){
						edgeState += 2;
					}
				}
				if (y > 0){
					if (ret[x, y-1] == MazeTileType.SPACE){
						edgeState += 4;
					}
				}
				if (y < height - 1){
					if (ret[x,y+1] == MazeTileType.SPACE){
						edgeState += 8;
					}
				}
				
				// If this would make a diagonal connection, forbid it
				if (nodiagonals){
					if (edgeState == 1){
						if (x < width-1){
							if (y > 0){
								if (ret[x+1,y-1] == MazeTileType.SPACE){
									return false;
								}
							}
							if (y < height-1){
								if (ret[x+1,y+1] == MazeTileType.SPACE){
									return false;
								}
							}
						}
						return true;
					}
					if (edgeState == 2){
						if (x > 0){
							if (y > 0){
								if (ret[x-1, y-1] == MazeTileType.SPACE){
									return false;
								}
							}
							if (y < height-1){
								if (ret[x-1,y+1] == MazeTileType.SPACE){
									return false;
								}
							}
						}
						return true;
					}
					if (edgeState == 4){
						if (y < height-1){
							if (x > 0){
								if (ret[x-1,y+1] == MazeTileType.SPACE){
									return false;
								}
							}
							if (x < width-1){
								if (ret[x+1,y+1] == MazeTileType.SPACE){
									return false;
								}
							}
						}
						return true;
					}
					if (edgeState == 8){
						if (y > 0){
							if (x > 0){
								if (ret[x-1, y-1] == MazeTileType.SPACE){
									return false;
								}
							}
							if (x < width-1){
								if (ret[x+1,y-1] == MazeTileType.SPACE){
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
			
			#endregion
			
			// Now that the function definitions are over, the algorithm starts
			
			// Choose an original point at random and carve it out
			int xChoice = rand.Next(0, width);
			int yChoice = rand.Next(0, height);
			coords tempEnd;
			StartLocation = tempEnd = new coords(xChoice, yChoice);
			carve(xChoice, yChoice);
			
			while (frontier.Count > 0){
				// Select a random edge
				double pos = rand.NextDouble();
				pos = Math.Pow(pos, Math.Exp(-branchRate));
				
				int rIdx = (int)(pos * frontier.Count);
				coords choice = frontier[rIdx];
				
				if (check(choice.X, choice.Y, nDiag)){
					carve(choice.X, choice.Y);
					tempEnd = choice;
				}
				else{
					harden(choice.X, choice.Y);
				}
				
				frontier.RemoveAt(rIdx);
			}
			
			// Set unexposed cells to be walls
			for (int x = 0; x < width; x++){
				for (int y = 0; y < height; y++){
					if (ret[x,y] == MazeTileType.UU){
						ret[x,y] = MazeTileType.WALL;
					}
				}
			}
			
			EndLocation = tempEnd;
			
			return ret;
		}
		
		#endregion
		
		public override string ToString ()
		{
			string ret = "";
			
			for (int row = Height-1; row >= 0; row--){
				for (int col = 0; col < Width; col++){
					switch(this[col, row]){
					case MazeTileType.WALL:
						ret += "X";
						break;
					default:
						ret += " ";
						break;
					}
				}
				ret += "\n";
			}
			
			return ret;
		}
	}
}

