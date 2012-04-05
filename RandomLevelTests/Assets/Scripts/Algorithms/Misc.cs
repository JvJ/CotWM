
using System.Collections.Generic;

/// <summary>
/// Some supplimentary miscellaneous classes. 
/// </summary>
namespace LevelGen{
	
	public struct coords
	{
		public int X, Y;
		
		public coords(int x, int y){
			X = x;
			Y = y;
		}
	}
	
	public struct intRect
	{
		public int X, Y, Width, Height;
		
		public int XMax{
			get {return X + Width;}
		}
		
		public int YMax{
			get {return Y + Height;}
		}
		
		public coords BottomLeft{
			get{ return new coords(X,Y);}
		}
		
		public coords BottomRight{
			get{ return new coords(X+Width,Y);}
		}
		
		public coords TopLeft{
			get{ return new coords(X,Y+Height);}
		}
		
		public coords TopRight{
			get{ return new coords(X+Width,Y+Height);}
		}
		
		public intRect(int x, int y, int w, int h){
			X = x;
			Y = y;
			Width = w;
			Height = h;
		}
	}
	
	public class Misc{
		
		public static List<E> ShuffleList<E>(List<E> inputList, System.Random r)
		{
			List<E> randomList = new List<E>();
			
			int randomIndex = 0;
			while (inputList.Count > 0)
			{
				randomIndex = r.Next(0, inputList.Count); //Choose a random object in the list
				randomList.Add(inputList[randomIndex]); //add it to the new, random list
				inputList.RemoveAt(randomIndex); //remove to avoid duplicates
			}
			
			return randomList; //return the new random list
		}
	}
	
}
