using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {
	
	public enum MenuState { INGAME, SETTINGS, MAINMENU };
	public MenuState menuState = MenuState.MAINMENU;
	public bool drawMainMenu = false;
	
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
			if(drawMainMenu){
				DrawMainMenu();
			}
			break;
		case MenuState.SETTINGS:
			break;
		}
	}
	
	void DrawMainMenu(){
		
	}
}
