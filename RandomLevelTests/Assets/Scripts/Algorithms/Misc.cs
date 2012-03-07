
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
