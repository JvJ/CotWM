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
		
        #endregion

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
    }
}
