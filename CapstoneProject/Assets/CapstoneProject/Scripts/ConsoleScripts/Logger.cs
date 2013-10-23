using UnityEngine;
using System.Collections.Generic;

public class Logger : MonoBehaviour {
	
	private static Logger logger;
	private const int MAX_MESSAGES = 8;
	private List<string> messages = new List<string>();
	private Rect screenRect = new Rect(10, Screen.height-500, 500, 200);
	
	void Awake(){
		logger = this;
	}
	
	void OnGUI(){
		GUILayout.BeginArea(screenRect);
		for(int i=0; i<messages.Count; i++){
			GUILayout.BeginHorizontal();
			GUILayout.Label(messages[i]);
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
	}
	
	void AddMessage(string msg){
		messages.Add(msg);
		if(messages.Count > MAX_MESSAGES){
			messages.RemoveAt(0);
		}
	}
	
	public static void Log(string msg){
		if(logger){
			logger.AddMessage(msg);
		}
	}
}
