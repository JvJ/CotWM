using System;
using System.Collections.Generic;

namespace LevelGen
{
	
	public enum ChamberType
	{
		//TYPE_1,
		//TYPE_2,
		//TYPE_3,
		//TYPE_4,
		TYPE_5,
		NumElements
	}
	
	/// <summary>
	/// Exception thrown if any chamber is the wrong size. 
	/// </summary>
	public class ChamberSizeException : Exception{
	}
	
	public class Chambers
	{
		
		public const int CHAMBER_WIDTH = 30;
		public const int CHAMBER_HEIGHT = 15;
		
		public const int LR_DOOR_WIDTH = 2;
		public const int LR_DOOR_HEIGHT = 3;
		public const int LR_DOOR_Y = 6;
		
		public const int TB_DOOR_WIDTH = 5;
		public const int TB_DOOR_HEIGHT = 2;
		public const int TB_DOOR_X = 13;
		
		private static Chambers singleton;
		
		public static Chambers Singleton
		{
			get
			{
				if (singleton == null){
					singleton = new Chambers();
				}
				
				return singleton;
			}
		}
		
		
		private Room[] rooms;
		
		
		public static Room Rooms(ChamberType idx)
		{
			return Singleton.rooms[(int)idx];
		}
		
		public static void LeftDoor(Room r, TileType fillType)
		{
			for (int x = 0; x < LR_DOOR_WIDTH; x++){
				for (int y = LR_DOOR_Y; y < LR_DOOR_Y + LR_DOOR_HEIGHT; y++){
					r[x,y] = new Tile(fillType);
				}
			}
		}
		
		public static void RightDoor(Room r, TileType fillType)
		{
			for (int x = r.Width - LR_DOOR_WIDTH; x < r.Width; x++){
				for (int y = LR_DOOR_Y; y < LR_DOOR_Y + LR_DOOR_HEIGHT; y++){
					r[x,y] = new Tile(fillType);
				}
			}
		}
		
		public static void TopDoor(Room r, TileType fillType)
		{
			for (int x = TB_DOOR_X; x < TB_DOOR_X + TB_DOOR_WIDTH; x++){
				for (int y = r.Height - TB_DOOR_HEIGHT; y < r.Height; y++){
					r[x,y] = new Tile(fillType);
				}
			}
		}
		
		public static void BottomDoor(Room r, TileType fillType)
		{
			for (int x = TB_DOOR_X; x < TB_DOOR_X + TB_DOOR_WIDTH; x++){
				for (int y = 0; y < TB_DOOR_HEIGHT; y++){
					r[x,y] = new Tile(fillType);
				}
			}
		}
		
		public Chambers ()
		{
			rooms = new Room[(int)ChamberType.NumElements];
			
		/*	rooms[(int)ChamberType.TYPE_1] = Room.FromString

				(

				 "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
				 "@@@@@@@@@             @@@@@@@@"+"\n"+
				 "@@@@@@                  @@@@@@"+"\n"+
				 "@@@@                      @@@@"+"\n"+
				 "@@@                        @@@"+"\n"+
				 "@@                          @@"+"\n"+
				 "@                            @"+"\n"+
				 "@              @             @"+"\n"+
				 "@             @@@            @"+"\n"+
				 "@@                          @@"+"\n"+
				 "@@@     @@@@       @@@@    @@@"+"\n"+
				 "@@@@                      @@@@"+"\n"+
				 "@@@@@@                  @@@@@@"+"\n"+
				 "@@@@@@@@@             @@@@@@@@"+"\n"+
				 "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"

				 );

			

			rooms[(int)ChamberType.TYPE_2] = Room.FromString

				(

				 "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
				 "@@@@@@@@@             @@@@@@@@"+"\n"+
				 "@@@@@@                  @@@@@@"+"\n"+
				 "@@@@                      @@@@"+"\n"+
				 "@@@                        @@@"+"\n"+
				 "@@                          @@"+"\n"+
				 "@                            @"+"\n"+
				 "@                            @"+"\n"+
				 "@                            @"+"\n"+
				 "@@             @@@          @@"+"\n"+
				 "@@@         @@@@           @@@"+"\n"+
				 "@@@@     @@@@@@       @@@@@@@@"+"\n"+
				 "@@@@@@@@@@       @@@@@@@@@@@@@"+"\n"+
				 "@@@@@@@@@             @@@@@@@@"+"\n"+
				 "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"

				 );

			

			rooms[(int)ChamberType.TYPE_3] = Room.FromString

				(

				 "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
				 "@@@@@@@@@             @@@@@@@@"+"\n"+
				 "@@@@@@                  @@@@@@"+"\n"+
				 "@@@@       @      @       @@@@"+"\n"+
				 "@@@        @      @        @@@"+"\n"+
				 "@@         @      @         @@"+"\n"+
				 "@          @      @          @"+"\n"+
				 "@          @      @          @"+"\n"+
				 "@          ********          @"+"\n"+
				 "@@         ********         @@"+"\n"+
				 "@@@        @@@@@@@@        @@@"+"\n"+
				 "@@@@    @@@@      @@@@    @@@@"+"\n"+
				 "@@@@@@                  @@@@@@"+"\n"+
				 "@@@@@@@@@             @@@@@@@@"+"\n"+
				 "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"

				 );

			

			rooms[(int)ChamberType.TYPE_4] = Room.FromString

				(

				 "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
				 "@@@@@@@@@  @*******@  @@@@@@@@"+"\n"+
				 "@@@@@@     @*******@    @@@@@@"+"\n"+
				 "@@@@       @*******@      @@@@"+"\n"+
				 "@@@        @*******@       @@@"+"\n"+
				 "@@@        @*******@       @@@"+"\n"+
				 "@  @       @*******@      @  @"+"\n"+
				 "@   @      *********     @   @"+"\n"+
				 "@    @     *********    @    @"+"\n"+
				 "@@    ****@@@@@@@@@@@***     @"+"\n"+
				 "@@@*******@@@@@@@@@@@******@@@"+"\n"+
				 "@@@@******@@@@@@@@@@@*****@@@@"+"\n"+
				 "@@@@@@                  @@@@@@"+"\n"+
				 "@@@@@@@@@             @@@@@@@@"+"\n"+
				 "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"

				 );*/
			
			rooms[(int)ChamberType.TYPE_5] = Room.FromString

				(

				 "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"+
				 "@@@@@@@@@             @@@@@@@@"+"\n"+
				 "@@@@@@                  @@@@@@"+"\n"+
				 "@@@@                      @@@@"+"\n"+
				 "@@@                        @@@"+"\n"+
				 "@@                          @@"+"\n"+
				 "@                            @"+"\n"+
				 "@                            @"+"\n"+
				 "@                            @"+"\n"+
				 "@@                          @@"+"\n"+
				 "@@@                        @@@"+"\n"+
				 "@@@@                      @@@@"+"\n"+
				 "@@@@@@                  @@@@@@"+"\n"+
				 "@@@@@@@@@             @@@@@@@@"+"\n"+
				 "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"+"\n"

				 );
			
			// Let's do a crazy exception and consistency check
			foreach(Room r in rooms){
				if (r.Width != CHAMBER_WIDTH || r.Height != CHAMBER_HEIGHT){
					throw new ChamberSizeException();
				}
			}
		}
	}
}

