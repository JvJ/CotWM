using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class Stats : MonoBehaviour{
	
	public float HP;
	public float MP;
	public float defense;
	public float speed;
	public float attack;
	public int HPUnit = 20;
	public int MPUnit = 20;
	
	GUIStyle style;
	
	public Transform rootTransform;
	
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
	
	void Start(){
		style = GUIVars.Singleton.gStyle;
	}
	
	public void DoDamage(float dmg, bool defIgnore)
	{
		if (!defIgnore){
			dmg -= defense;
		}
		
		dmg = Mathf.Clamp(dmg, 1.0f, float.PositiveInfinity);
		
		notifications.Add(new TimedNotification(""+dmg, Color.white));
		HP -= dmg;
	}
	
	public virtual void OnGUI()
	{
		Transform t = rootTransform == null ? transform : rootTransform;
		
		Vector3 screenPos = Camera.main.WorldToScreenPoint(t.position);
		
		List<TimedNotification> removals = new List<TimedNotification>();
		
		foreach(TimedNotification n in notifications){
			GUI.Label(
			              new Rect((int)(screenPos.x - 25),
			                       (int)((Camera.main.pixelHeight - screenPos.y) - 10 - n.vOffset),
			                       50,20),
			              n.msg, style);
			
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