using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LevelGen
{
    public enum TileType
    {
        BLANK,
        SNOW,
        ROCK,
		/// <summary>
		///Exposed, Undetermined. 
		/// </summary>
		XU,
		/// <summary>
		///Unexposed, Undetermined 
		/// </summary>
		UU
    }

    public struct Tile
    {
		
		/// <summary>
		/// Get Tile Type from character.
		/// </summary>
		/// <param name="c">
		/// A <see cref="System.Char"/>
		/// </param>
		/// <returns>
		/// A <see cref="TileType"/>
		/// </returns>
		public static TileType TypeFromChar(char c){
			switch (c){
			case '*':
				return TileType.SNOW;
			case '@':
				return TileType.ROCK;
			case ',':
				return TileType.XU;
			case '?':
				return TileType.UU;
			case ' ':
			default:
				return TileType.BLANK;
			}
		}
		
		/// <summary>
		/// Get tile from character.
		/// </summary>
		/// <param name="c">
		/// A <see cref="System.Char"/>
		/// </param>
		/// <returns>
		/// A <see cref="Tile"/>
		/// </returns>
		public static Tile FromChar(char c){
			
			return new Tile(TypeFromChar(c));
		}
		
        public static char TileChar(TileType t)
        {
            switch (t)
            {
                case TileType.BLANK:
                    return ' ';
                case TileType.SNOW:
                    return '*';
                case TileType.ROCK:
                    return '@';
				case TileType.XU:
					return ',';
				case TileType.UU:
					return '?';
                default:
                    return '^';
            }
        }

        /// <summary>
        /// The type stored in this tile.
        /// </summary>
        public TileType Type {
            get { return type; }
        }
        private TileType type;

        /// <summary>
        /// Pretty self-explanatory.
        /// </summary>
        /// <param name="t">The tile type.</param>
        public Tile(TileType t)
        {
            type = t;
        }

        /// <summary>
        /// Basic conversion of a tile to a string using
        /// the character representation function.
        /// </summary>
        /// <returns>A single-character string.</returns>
        public override string ToString()
        {
            return ""+TileChar(type);
        }

        /// <summary>
        /// Just like ToString, but returns a single character.
        /// </summary>
        /// <returns>The char representation.</returns>
        public char ToChar()
        {
            return TileChar(type);
        }
    }
}
