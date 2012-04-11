using UnityEngine;
using System.Collections;
using LevelGen;
using System.Linq;

public class Configs : MonoBehaviour{
	
	public class ConfigFormatException : UnityException{
		string msg;
		
		public ConfigFormatException(string s){
			this.msg = s;
		}
		
		public override string ToString ()
		{
			return msg;
		}
		
	}
	
	#region Prefabs
	
	public DethmurderControl player;
	
	public Object ShoggothPrefab;
	public Object WyrmPrefab;
	public Object XlGrhthbtrgPrefab;
	public Object SnowPrefab;
	
	public int numWyrms = 0;
	
	#endregion
	
	
	public enum ObjectType{
		PLAYER_START,
		SHOGGOTH,
		WYRM,
		XLGRHTHBTRG,
		SNOWMAN,
		SHADOWMAN,
		F_HEADMAN,
		SPACE,
		SNOW,
		NumElements
	}
	
	#region Character Conversions
	
	public static char ObjectToChar(ObjectType o){
		switch (o){
		case ObjectType.PLAYER_START:
			return '$';
		case ObjectType.SHOGGOTH:
			return 'h';
		case ObjectType.WYRM:
			return 'w';
		case ObjectType.XLGRHTHBTRG:
			return 'X';
		case ObjectType.SNOWMAN:
			return 'n';
		case ObjectType.SHADOWMAN:
			return 's';
		case ObjectType.F_HEADMAN:
			return 'f';
		case ObjectType.SNOW:
			return '*';
		case ObjectType.SPACE:
			return ' ';
		default:
			return ' ';
		}
	}
	
	public static ObjectType CharToObject(char c){
		switch (c){
		case '$':
			return ObjectType.PLAYER_START;
		case 'h':
			return ObjectType.SHOGGOTH;
		case 'w':
			return ObjectType.WYRM;
		case 'X':
			return ObjectType.XLGRHTHBTRG;
		case 'n':
			return ObjectType.SNOWMAN;
		case 's':
			return ObjectType.SHADOWMAN;
		case 'f':
			return ObjectType.F_HEADMAN;
		case '*':
			return ObjectType.SNOW;
		case ' ':
			return ObjectType.SPACE;
		default:
			return ObjectType.SPACE;
		}
	}
	
	#endregion
	
	
	/// <summary>
	/// A single configuration map.
	/// </summary>
	public class Config{
		
		private ObjectType[,] grid;
		
		public ObjectType this[int x, int y]{
			get{return grid[x,y];}
			set{grid[x,y] = value;}
		}
		
		public int Width{
			get{ return grid.GetLength(0);}
		}
		
		public int Height{
			get{ return grid.GetLength(1);}
		}
		
		public Config(int width, int height){
			grid = new ObjectType[width, height];
		}
		
		public static Config ConfigFromString (string s)
		{
			var lines = (from str in s.Split('\r','\n')
						where str.Length > 0
						select str).ToArray();
			
			int height = lines.Length;
			
			if (height == 0){
				throw new ConfigFormatException("Zero lines in input.");
			}
			
			int width = lines[0].Length;
			
			Config ret = new Config(width, height);
			
			for (int y = height-1; y >= 0; y--){
				
				string currentLine = lines[height - y - 1];
				
				if (currentLine.Length != width){
					
					Debug.Log("Width: "+width+" doesn't match line: "+currentLine.Length);
					throw new ConfigFormatException("Inconsistent line width ("+width+" vs. "+currentLine.Length+") at y = "+y);
				}
				
				for (int x = 0; x < width; x++){
					ret[x,y] = Configs.CharToObject(currentLine[x]);
				}
			}
			
			return ret;
		}
		
		public override string ToString ()
		{
			string ret = "";
			
			for (int y = Height - 1; y >= 0; y--){
				for (int x = 0; x < Width; x++){
					ret += Configs.ObjectToChar(this[x,y]);
				}
				ret += '\n';
			}
			
			return ret;
		}
		
	}
	
	
	public static Configs Singleton{
		get;
		private set;
	}
	
	private Object[] prefabs;
	
	/// <summary>
	/// A list of prefabs indexed by object type.
	/// </summary>
	/// <param name='o'>
	/// O.
	/// </param>
	public Object this[ObjectType o]{
		get{return prefabs[(int)o];}
		set{prefabs[(int)o] = value;}
	}
	
	private Config[][] configurations;
	
	/// <summary>
	/// Gets an array of configurations by Chamber type.
	/// </summary>
	/// <param name='c'>
	/// C.
	/// </param>
	public Config[] this[ChamberType c]{
		get{return configurations[(int)c];}
		set{configurations[(int)c] = value;}
	}
	
	
	
	// Initialization before Start()
	void Awake(){
		
		Singleton = this;
		
		prefabs = new Object[(int)ObjectType.NumElements];
		
		this[ObjectType.PLAYER_START] = new GameObject("PStart");
		this[ObjectType.SHOGGOTH] = ShoggothPrefab;
		this[ObjectType.XLGRHTHBTRG] = XlGrhthbtrgPrefab;
		this[ObjectType.WYRM] = WyrmPrefab;
		this[ObjectType.SNOW] = SnowPrefab;
		
		configurations = new Config[(int)ChamberType.NumElements][];
		
		#region Setting Configs
		
		this[ChamberType.TYPE_1] = new Config[]{
			
			/*Config.ConfigFromString(
			"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@                      @@@@"+"\n"+
"@@@            f           @@@"+"\n"+
"@@      f            f      @@"+"\n"+
"@                            @"+"\n"+
"@             $@             @"+"\n"+
"@       f     @@@    f       @"+"\n"+
"@@                          @@"+"\n"+
"@@@     @@@@       @@@@    @@@"+"\n"+
"@@@@           f          @@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),*//*
 Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@                      @@@@"+"\n"+
"@@@                        @@@"+"\n"+
"@@                          @@"+"\n"+
"@              s             @"+"\n"+
"@              @             @"+"\n"+
"@             @@@            @"+"\n"+
"@@        s         s       @@"+"\n"+
"@@@     @@@@       @@@@    @@@"+"\n"+
"@@@@                      @@@@"+"\n"+
"@@@@@@$        s        @@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),*/

			/*Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@           f          @@@@"+"\n"+
"@@@                        @@@"+"\n"+
"@@                          @@"+"\n"+
"@        f           f       @"+"\n"+
"@             $@             @"+"\n"+
"@             @@@            @"+"\n"+
"@@       n           n      @@"+"\n"+
"@@@     @@@@       @@@@    @@@"+"\n"+
"@@@@                      @@@@"+"\n"+
"@@@@@@    f        f    @@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),*/
 
			Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@                      @@@@"+"\n"+
"@@@                        @@@"+"\n"+
"@@                          @@"+"\n"+
"@              $             @"+"\n"+
"@              @             @"+"\n"+
"@             @@@            @"+"\n"+
"@@         w        w       @@"+"\n"+
"@@@     @@@@   w   @@@@    @@@"+"\n"+
"@@@@         w   w        @@@@"+"\n"+
"@@@@@@ w   w   w   w   w@@@@@@"+"\n"+
"@@@@@@@@@w           w@@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),
 
			Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@                      @@@@"+"\n"+
"@@@                        @@@"+"\n"+
"@@                          @@"+"\n"+
"@              $             @"+"\n"+
"@              @             @"+"\n"+
"@             @@@            @"+"\n"+
"@@                          @@"+"\n"+
"@@@     @@@@       @@@@    @@@"+"\n"+
"@@@@                      @@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@@@@@@      h      @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),

 
			Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@                      @@@@"+"\n"+
"@@@                        @@@"+"\n"+
"@@                          @@"+"\n"+
"@              $             @"+"\n"+
"@              @             @"+"\n"+
"@             @@@            @"+"\n"+
"@@       h           h      @@"+"\n"+
"@@@     @@@@       @@@@    @@@"+"\n"+
"@@@@                      @@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),
 
			Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@                      @@@@"+"\n"+
"@@@                        @@@"+"\n"+
"@@                          @@"+"\n"+
"@        f     s     f       @"+"\n"+
"@        f     @     f       @"+"\n"+
"@        f    @@@    f       @"+"\n"+
"@@$                  h      @@"+"\n"+
"@@@     @@@@       @@@@    @@@"+"\n"+
"@@@@                      @@@@"+"\n"+
"@@@@@@wwww          wwww@@@@@@"+"\n"+
"@@@@@@@@@      s      @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@")
			
		};
		
		this[ChamberType.TYPE_2] = new Config[]{
			
			Config.ConfigFromString(
			"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@                      @@@@"+"\n"+
"@@@                        @@@"+"\n"+
"@@                          @@"+"\n"+
"@                            @"+"\n"+
"@                            @"+"\n"+
"@              $             @"+"\n"+
"@@             @@@          @@"+"\n"+
"@@@      w  @@@@  h        @@@"+"\n"+
"@@@@ w w @@@@@@       @@@@@@@@"+"\n"+
"@@@@@@@@@@       @@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),
 
				/*Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@                      @@@@"+"\n"+
"@@@                        @@@"+"\n"+
"@@                          @@"+"\n"+
"@    f f f     f f    f f    @"+"\n"+
"@    f f f     f f    f f    @"+"\n"+
"@    f f f     f f    f f    @"+"\n"+
"@@             @@@          @@"+"\n"+
"@@@         @@@@           @@@"+"\n"+
"@@@@  $  @@@@@@       @@@@@@@@"+"\n"+
"@@@@@@@@@@       @@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),*/
 
				Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@                      @@@@"+"\n"+
"@@@                        @@@"+"\n"+
"@@                          @@"+"\n"+
"@                            @"+"\n"+
"@                            @"+"\n"+
"@              $             @"+"\n"+
"@@             @@@          @@"+"\n"+
"@@@         @@@@           @@@"+"\n"+
"@@@@  h  @@@@@@     h @@@@@@@@"+"\n"+
"@@@@@@@@@@       @@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),
 /*
				Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@                      @@@@"+"\n"+
"@@@                        @@@"+"\n"+
"@@                          @@"+"\n"+
"@                            @"+"\n"+
"@                            @"+"\n"+
"@               n            @"+"\n"+
"@@          $  @@@          @@"+"\n"+
"@@@         @@@@           @@@"+"\n"+
"@@@@  n  @@@@@@     n @@@@@@@@"+"\n"+
"@@@@@@@@@@       @@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@  n          @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),*/
 
				Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@                      @@@@"+"\n"+
"@@@                        @@@"+"\n"+
"@@                          @@"+"\n"+
"@                            @"+"\n"+
"@                            @"+"\n"+
"@               $            @"+"\n"+
"@@             @@@          @@"+"\n"+
"@@@         @@@@  w w   w w@@@"+"\n"+
"@@@@w w w@@@@@@ w w  w@@@@@@@@"+"\n"+
"@@@@@@@@@@w w w w@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@w           w@@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),
 
				/*Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@                      @@@@"+"\n"+
"@@@                        @@@"+"\n"+
"@@                          @@"+"\n"+
"@                            @"+"\n"+
"@                            @"+"\n"+
"@               n            @"+"\n"+
"@@             @@@          @@"+"\n"+
"@@@         @@@@         $ @@@"+"\n"+
"@@@@  s  @@@@@@     s @@@@@@@@"+"\n"+
"@@@@@@@@@@       @@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@   s      s  @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@")*/
		};
		
		this[ChamberType.TYPE_3] = new Config[]{
			
			Config.ConfigFromString(
			"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@       @      @       @@@@"+"\n"+
"@@@        @      @        @@@"+"\n"+
"@@         @      @         @@"+"\n"+
"@          @      @          @"+"\n"+
"@          @  $   @          @"+"\n"+
"@          ********          @"+"\n"+
"@@         ********         @@"+"\n"+
"@@@        @@@@@@@@        @@@"+"\n"+
"@@@@    @@@@      @@@@    @@@@"+"\n"+
"@@@@@@w w w  w w w w w w@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),
 
				/*Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@       @      @       @@@@"+"\n"+
"@@@        @      @        @@@"+"\n"+
"@@         @      @         @@"+"\n"+
"@          @      @          @"+"\n"+
"@          @   $  @          @"+"\n"+
"@          ********          @"+"\n"+
"@@         ********         @@"+"\n"+
"@@@       n@@@@@@@@n       @@@"+"\n"+
"@@@@    @@@@      @@@@    @@@@"+"\n"+
"@@@@@@wwwwwwwwwwwwwwwwww@@@@@@"+"\n"+
"@@@@@@@@@wwwwwwwwwwwww@@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),*/
 
				Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@       @      @       @@@@"+"\n"+
"@@@        @      @        @@@"+"\n"+
"@@         @      @         @@"+"\n"+
"@          @      @          @"+"\n"+
"@          @  h   @          @"+"\n"+
"@          ********          @"+"\n"+
"@@         ********         @@"+"\n"+
"@@@      $ @@@@@@@@        @@@"+"\n"+
"@@@@    @@@@      @@@@    @@@@"+"\n"+
"@@@@@@s                s@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),
 
				Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@       @      @       @@@@"+"\n"+
"@@@        @      @        @@@"+"\n"+
"@@         @      @         @@"+"\n"+
"@          @      @          @"+"\n"+
"@          @  h   @          @"+"\n"+
"@          ********          @"+"\n"+
"@@         ********         @@"+"\n"+
"@@@      $ @@@@@@@@        @@@"+"\n"+
"@@@@    @@@@      @@@@    @@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@@@@@@      h      @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),
 
				/*Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@       @      @       @@@@"+"\n"+
"@@@        @      @        @@@"+"\n"+
"@@         @      @         @@"+"\n"+
"@          @      @          @"+"\n"+
"@          @  nn  @          @"+"\n"+
"@          ********          @"+"\n"+
"@@         ********         @@"+"\n"+
"@@@      $ @@@@@@@@        @@@"+"\n"+
"@@@@    @@@@      @@@@    @@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),*/
 
				/*Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@       @      @       @@@@"+"\n"+
"@@@        @      @        @@@"+"\n"+
"@@         @      @         @@"+"\n"+
"@     f    @      @     f    @"+"\n"+
"@     f    @      @     f    @"+"\n"+
"@     f    ********     f    @"+"\n"+
"@@         ********         @@"+"\n"+
"@@@      $ @@@@@@@@        @@@"+"\n"+
"@@@@    @@@@      @@@@    @@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@@@@@@      n      @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),*/
 
				/*Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@     n      n     @@@@@@"+"\n"+
"@@@@       @      @       @@@@"+"\n"+
"@@@        @      @        @@@"+"\n"+
"@@         @      @         @@"+"\n"+
"@          @      @          @"+"\n"+
"@          @   s  @          @"+"\n"+
"@          ********          @"+"\n"+
"@@         ********         @@"+"\n"+
"@@@      $ @@@@@@@@        @@@"+"\n"+
"@@@@    @@@@      @@@@    @@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@@@@@@      s      @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),*/
 
				/*Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@  f f f@      @f f f  @@@@"+"\n"+
"@@@  f    f@      @f    f  @@@"+"\n"+
"@@         @      @         @@"+"\n"+
"@          @      @          @"+"\n"+
"@          @   $  @          @"+"\n"+
"@          ********          @"+"\n"+
"@@         ********         @@"+"\n"+
"@@@     n  @@@@@@@@  n     @@@"+"\n"+
"@@@@    @@@@      @@@@    @@@@"+"\n"+
"@@@@@@wwwwwwwwwwwwwwwwww@@@@@@"+"\n"+
"@@@@@@@@@wwwwwwwwwwwww@@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),*/
 
				Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@  f f f@      @f f f  @@@@"+"\n"+
"@@@  f    f@      @f    f  @@@"+"\n"+
"@@         @      @         @@"+"\n"+
"@          @      @          @"+"\n"+
"@          @  h   @          @"+"\n"+
"@          ********          @"+"\n"+
"@@         ********         @@"+"\n"+
"@@@        @@@@@@@@  $     @@@"+"\n"+
"@@@@    @@@@      @@@@    @@@@"+"\n"+
"@@@@@@ w     w  w      w@@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@")
			
		};
		
		this[ChamberType.TYPE_4] = new Config[]{
			
			Config.ConfigFromString(
			"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@  @*******@  @@@@@@@@"+"\n"+
"@@@@@@     @*******@    @@@@@@"+"\n"+
"@@@@       @*******@      @@@@"+"\n"+
"@@@        @*******@       @@@"+"\n"+
"@@@        @*******@       @@@"+"\n"+
"@  @       @*******@      @  @"+"\n"+
"@   @      *********     @   @"+"\n"+
"@    @  s  *********  s @    @"+"\n"+
"@@ $  ****@@@@@@@@@@@***     @"+"\n"+
"@@@*******@@@@@@@@@@@******@@@"+"\n"+
"@@@@******@@@@@@@@@@@*****@@@@"+"\n"+
"@@@@@@w     w     w   w @@@@@@"+"\n"+
"@@@@@@@@@w w       w w@@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),
 
			/*Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@@  @*******@  @@@@@@@@"+"\n"+
"@@@@@@     @*******@    @@@@@@"+"\n"+
"@@@@       @*******@      @@@@"+"\n"+
"@@@        @*******@       @@@"+"\n"+
"@@@        @*******@       @@@"+"\n"+
"@  @       @*******@      @  @"+"\n"+
"@   @      *********     @   @"+"\n"+
"@    @ $   *********    @    @"+"\n"+
"@@    ****@@@@@@@@@@@***     @"+"\n"+
"@@@*******@@@@@@@@@@@******@@@"+"\n"+
"@@@@******@@@@@@@@@@@*****@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@@@@@@             @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@")*/
			
		};
		
		this[ChamberType.TYPE_5] = new Config[]{
			
			Config.ConfigFromString(
			"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@     *  *     @@@@@@@@"+"\n"+
"@@@@@@       * w*       @@@@@@"+"\n"+
"@@@@         *  *         @@@@"+"\n"+
"@@@          *  *          @@@"+"\n"+
"@@           *w *           @@"+"\n"+
"@            *  *            @"+"\n"+
"@            *  *            @"+"\n"+
"@            * w*            @"+"\n"+
"@@           *w *           @@"+"\n"+
"@@@          *  *          @@@"+"\n"+
"@@@@         *w *         @@@@"+"\n"+
"@@@@@@$      *  *       @@@@@@"+"\n"+
"@@@@@@@@     *  *     @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),
 
				Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@              @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@                      @@@@"+"\n"+
"@@@                        @@@"+"\n"+
"@@             $            @@"+"\n"+
"@ ************************** @"+"\n"+
"@ *  w       w   w       w * @"+"\n"+
"@ ************************** @"+"\n"+
"@@                          @@"+"\n"+
"@@@                        @@@"+"\n"+
"@@@@                      @@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@@@@@              @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),
 
				/*Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@              @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@    f      f      f   @@@@"+"\n"+
"@@@                        @@@"+"\n"+
"@@      f      f      f     @@"+"\n"+
"@                            @"+"\n"+
"@       f      f      f      @"+"\n"+
"@                            @"+"\n"+
"@@      f      f      f     @@"+"\n"+
"@@@                        @@@"+"\n"+
"@@@@ $  f      f      f   @@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@@@@@              @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),
				
				Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@              @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@                      @@@@"+"\n"+
"@@@           s            @@@"+"\n"+
"@@                          @@"+"\n"+
"@                            @"+"\n"+
"@    s        s        s     @"+"\n"+
"@                            @"+"\n"+
"@@                          @@"+"\n"+
"@@@           s            @@@"+"\n"+
"@@@@ $                    @@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@@@@@              @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),*/
 
				Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@              @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@                      @@@@"+"\n"+
"@@@                        @@@"+"\n"+
"@@                          @@"+"\n"+
"@                            @"+"\n"+
"@                            @"+"\n"+
"@                            @"+"\n"+
"@@                          @@"+"\n"+
"@@@                        @@@"+"\n"+
"@@@@ $        h h         @@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@@@@@   h       h  @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),
				
				/*Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@              @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@                      @@@@"+"\n"+
"@@@                        @@@"+"\n"+
"@@                          @@"+"\n"+
"@                            @"+"\n"+
"@                            @"+"\n"+
"@                            @"+"\n"+
"@@                          @@"+"\n"+
"@@@                        @@@"+"\n"+
"@@@@ $                    @@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@@@@@   n n   n n  @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"),*/
 
				
			
		};
		
		this[ChamberType.TYPE_BOSS] = new Config[]{
		Config.ConfigFromString(
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
"@@@@@@@@              @@@@@@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@                      @@@@"+"\n"+
"@@@            $           @@@"+"\n"+
"@@          *******         @@"+"\n"+
"@           *     *          @"+"\n"+
"@           *  X  *          @"+"\n"+
"@           *     *          @"+"\n"+
"@@          *******         @@"+"\n"+
"@@@                        @@@"+"\n"+
"@@@@                      @@@@"+"\n"+
"@@@@@@                  @@@@@@"+"\n"+
"@@@@@@@@              @@@@@@@@"+"\n"+
"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@")
		};
		
		#endregion
		
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	/// <summary>
	/// Overlay the specified roomControl, room and config onto the room control.
	/// </summary>
	/// <param name='roomControl'>
	/// Room control.
	/// </param>
	/// <param name='room'>
	/// Room.
	/// </param>
	/// <param name='config'>
	/// Config.
	/// </param>
	public void Overlay(RoomControl roomControl, Room room, Config config){
		
		if (config.Width != room.Width || config.Height != room.Height){
			throw new ConfigFormatException("Room and config dimensions do not match!");
		}
		
		for( int x = 0; x < room.Width; x++){
			for(int y = 0; y < room.Height; y++){
				
				// Only add the overlay if it's not already occupied by the room
				switch(room[x,y].Type){
				case TileType.BLANK:
					
					var currentConf = config[x,y];
					var currentPre = this[currentConf];
					
					if (currentPre != null){
					
						// Instantiate whatever the prefab is!
						var o = GameObject.Instantiate(currentPre) as GameObject;
						
						o.transform.position = CubeGen.Singleton.PositionFromIdx(roomControl.Position, new coords(x,y));
						
						switch (currentConf){
						case ObjectType.SNOW:
							o.transform.localScale = new Vector3( CubeGen.Singleton.cubeSize, CubeGen.Singleton.cubeSize, CubeGen.Singleton.cubeSize);
							var mRend = o.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
							mRend.material.color = new Color(1f,1f,1f);
							o.tag = "SNOW";
							o.layer = LayerMask.NameToLayer("Terrain");
							roomControl.AddTerrain(o);
							break;
						case ObjectType.PLAYER_START:
							roomControl.PlayerStart = o;
							break;
						case ObjectType.XLGRHTHBTRG:
							var xc = o.GetComponent(typeof(XlGrhthbtrgControl)) as XlGrhthbtrgControl;
							xc.player = player;
							xc.currentState = EntityState.STILL;
							xc.target = player.transform.Find("Armature/Hip");
							o.layer = LayerMask.NameToLayer("Enemy");
							roomControl.AddEnemy(xc);
							break;
						case ObjectType.WYRM:
							numWyrms++;
							var wc = o.GetComponent(typeof(WyrmControl)) as WyrmControl;
							wc.player = player;
							wc.currentState = EntityState.STILL;
							o.layer = LayerMask.NameToLayer("Wyrm");
							roomControl.AddEnemy(wc);
							break;
						case ObjectType.SHOGGOTH:
							var sc = o.GetComponent(typeof(ShoggothControl)) as ShoggothControl;
							sc.player = player;
							sc.currentState = EntityState.STILL;
							o.layer = LayerMask.NameToLayer("Enemy");
							roomControl.AddEnemy(sc);
							break;
						}
					}
					break;
				default:
					break;
				}
			}
		}
		
	}
}