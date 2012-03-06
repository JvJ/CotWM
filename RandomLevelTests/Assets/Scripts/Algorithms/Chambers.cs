using System;
using System.Collections.Generic;

namespace LevelGen
{
	
	public enum ChamberSize
	{
		SIZE_1,
		SIZE_2,
		SIZE_3,
		SIZE_4,
		NumElements
	}
	
	public class Chambers
	{
		
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
		
		
		public static Room Rooms(ChamberSize idx)
		{
			return Singleton.rooms[(int)idx];
		}
		
		public Chambers ()
		{
			rooms = new Room[(int)ChamberSize.NumElements];
			
			rooms[(int)ChamberSize.SIZE_1] = Room.FromString
				(
				 "********************"+"\n"+
				 "*****          *****"+"\n"+
				 "****            ****"+"\n"+
				 "***              ***"+"\n"+
				 "**                **"+"\n"+
				 "*                  *"+"\n"+
				 "**                **"+"\n"+
				 "***              ***"+"\n"+
				 "****            ****"+"\n"+
				 "*****          *****"+"\n"+
				 "********************"+"\n"
				 );
			
			rooms[(int)ChamberSize.SIZE_2] = Room.FromString
				(
				 "******************************"+"\n"+
				 "*********             ********"+"\n"+
				 "******                  ******"+"\n"+
				 "****                      ****"+"\n"+
				 "***                        ***"+"\n"+
				 "**                          **"+"\n"+
				 "*                            *"+"\n"+
				 "*                            *"+"\n"+
				 "*                            *"+"\n"+
				 "**                          **"+"\n"+
				 "***                        ***"+"\n"+
				 "****                      ****"+"\n"+
				 "******                  ******"+"\n"+
				 "*********             ********"+"\n"+
				 "******************************"+"\n"
				 );
			
			rooms[(int)ChamberSize.SIZE_3] = Room.FromString
				(
				 "******************************"+"\n"+
				 "*********             ********"+"\n"+
				 "******                  ******"+"\n"+
				 "****                      ****"+"\n"+
				 "***                        ***"+"\n"+
				 "**                          **"+"\n"+
				 "*                            *"+"\n"+
				 "*                            *"+"\n"+
				 "*                            *"+"\n"+
				 "**                          **"+"\n"+
				 "***                        ***"+"\n"+
				 "****                      ****"+"\n"+
				 "******                  ******"+"\n"+
				 "*********             ********"+"\n"+
				 "******************************"+"\n"
				 );
			
			rooms[(int)ChamberSize.SIZE_4] = Room.FromString
				(
				 "******************************"+"\n"+
				 "*********             ********"+"\n"+
				 "******                  ******"+"\n"+
				 "****                      ****"+"\n"+
				 "***                        ***"+"\n"+
				 "**                          **"+"\n"+
				 "*                            *"+"\n"+
				 "*                            *"+"\n"+
				 "*                            *"+"\n"+
				 "**                          **"+"\n"+
				 "***                        ***"+"\n"+
				 "****                      ****"+"\n"+
				 "******                  ******"+"\n"+
				 "*********             ********"+"\n"+
				 "******************************"+"\n"
				 );
			
		}
	}
}

