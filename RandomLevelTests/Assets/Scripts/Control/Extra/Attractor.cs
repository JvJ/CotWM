using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class Attractor
	{
		
		/// <summary>
		/// Where do you get attracted to? 
		/// </summary>
		public Transform target{
			get;
			private set;
		}
		
		/// <summary>
		/// Determines attraction based on distance. 
		/// </summary>
		public float gConstant{
			get;
			private set;
		}
		
		public Attractor (Transform targ, float g)
		{
			target = targ;
			gConstant = g;
		}
	}
}

