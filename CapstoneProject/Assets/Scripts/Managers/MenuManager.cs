using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {
	
	public enum MenuState { INGAME, SETTINGS, EXIT, MAINMENU };
	public MenuState menuState = MenuState.MAINMENU;
	
	#region Singleton
	
	private static MenuManager _instance;

	public static MenuManager Instance {
		get { return _instance; }
	}

	void Awake(){
		if(MenuManager.Instance != null){
			DestroyImmediate(gameObject);
			return;
		}
		_instance = this;
	}

	void OnApplicationQuit(){
		_instance = null;
	}
	
	#endregion
	
	void OnGUI(){
		switch(menuState){
		case MenuState.MAINMENU:
			break;
		case MenuState.SETTINGS:
			break;
		}
	}
}
