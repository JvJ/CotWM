using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class Stats : MonoBehaviour{
	
	public float HP;
	public float defense;
	public float speed;
	public float attack;
	
	public const float timeOut = 2;
	public const float pps = 20;
	
	
	class TimedNotification
	{
		public string msg;
		public Color clr;
		public float time;
		
		public TimedNotification(string message, Color color){
			msg = message;
			clr = color;
			time = 0;
		}
		
		public void advance(float deltaTime){
			time += deltaTime;
		}
		
		public bool isDone(){
			return time > timeOut;
		}
		
		public float vOffset{
			get{return time * pps;}
		}
	}
	
	// Damage notifications list
	List<TimedNotification> notifications = new List<TimedNotification>();
	
	public void DoDamage(float dmg, bool defIgnore)
	{
		if (!defIgnore){
			dmg -= defense;
		}
		
		notifications.Add(new TimedNotification(""+dmg, Color.white));
		HP -= dmg;
	}
	
	public void OnGUI()
	{
		Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
		//Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);
		
		List<TimedNotification> removals = new List<TimedNotification>();
		
		foreach(TimedNotification n in notifications){
			GUI.Label(
			              new Rect((int)(screenPos.x - 25),
			                       (int)((Camera.main.pixelHeight - screenPos.y) - 10 - n.vOffset),
			                       50,20),
			              n.msg);
			
			n.time += Time.deltaTime;
			
			// Check if it's done and add it to the removals
			if (n.isDone()){
				removals.Add(n);
			}
		}
		
		// Execute the removal
		foreach (TimedNotification n in removals){
			notifications.Remove(n);
		}
		
		//GUI.TextField(new Rect(0,70,100,20), "Dmg:"+lastDamage);
	}
}

/// <summary>
/// 
/// </summary>
public enum ElementType
{
	NONE,
	FIRE,
	LIGHTNING,
	ICE,
}

/// <summary>
/// 
/// </summary>
public class Attack{
	
	public float damageValue;
	
	public bool isContact;
	
	public ElementType element;
}