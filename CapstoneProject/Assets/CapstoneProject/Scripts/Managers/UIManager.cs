using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
	
	public enum UIState { PAUSE, 
		WAVEWON, WAVELOST, NEXTWAVE, 
		GAMEOVER, NONE, CURWAVE, 
		FORTINFO, YESORNO, BUILD_SCREEN, 
		UPGRADE_SCREEN, BUY_SCREEN, EQUIP_WEAPON_SCREEN, 
		FORT_UPGRADE_SCREEN };
	public UIState uiState = UIState.NONE;
	public bool isPaused = false;
	private WeaponSelection selection;
	
	private GameObject fortification;
	private Rect displayScreen = new Rect(Screen.width/2f - 50f, Screen.height/2f - 100f, 200, 300);
	
	#region Singleton
	
	private static UIManager _instance;

	public static UIManager Instance {
		get { return _instance; }
	}

	void Awake(){
		if(UIManager.Instance != null){
			DestroyImmediate(gameObject);
			return;
		}
		_instance = this;
		
		selection = GameObject.Find("WeaponSelection").GetComponent<WeaponSelection>();
	}

	void OnApplicationQuit(){
		_instance = null;
	}
	
	#endregion
	
	void Update(){
		if(isPaused){
			Time.timeScale = 0;
		} else {
			if(!selection.changingWeapons){
				Time.timeScale = 1;
			}
		}
	}
	
	public void SetFortification(GameObject fort){
		fortification = fort;
		uiState = UIState.FORTINFO;
	}
	
	void OnGUI(){
		switch(uiState){
		case UIState.PAUSE:
			isPaused = !isPaused;
			DrawPauseScreen();
			break;
		case UIState.WAVEWON:
			DrawWaveWonScreen();
			break;
		case UIState.NEXTWAVE:
			DrawNextWaveScreen();
			break;
		case UIState.CURWAVE:
			DrawCurWaveScreen();
			break;
		case UIState.WAVELOST:
			DrawWaveLostScreen();
			break;
		case UIState.GAMEOVER:
			DrawGameOverScreen();
			break;
		case UIState.FORTINFO:
			DrawFortInfo();
			break;
		case UIState.YESORNO:
			DrawYesOrNoScreen();
			break;
		}
	}
	
	void DrawPauseScreen(){
		if(GUI.Button(new Rect(Screen.width/2, Screen.height/2, 100, 50), "Resume")){
			isPaused = !isPaused;
			UIManager.Instance.uiState = UIState.NONE;
		}
	}
	
	void DrawWaveWonScreen(){
		GUI.Box(new Rect(Screen.width/3, Screen.height/5, 400, 300), "Wave Won");
	}
	
	void DrawWaveLostScreen(){
		GUI.Box(new Rect(Screen.width/3, Screen.height/5, 400, 300), "Wave Lost");
	}

	void DrawNextWaveScreen(){
		GUI.Box(new Rect(Screen.width/3, Screen.height/5, 400, 300), "Next Wave");
	}
	
	void DrawGameOverScreen(){
		if(GUI.Button(new Rect(Screen.width/3, Screen.height/5, 100, 50), "Restart")){
			//Application.LoadLevel(Application.loadedLevel);
			MenuManager.Instance.menuState = MenuManager.MenuState.ENDGAME;
			uiState = UIState.NONE;
		}
	}
	
	void DrawCurWaveScreen(){
		
	}

	void DrawYesOrNoScreen(){
		GUI.BeginGroup(displayScreen);
		
		GUI.Box(new Rect(0, 0, displayScreen.width, displayScreen.height), "ARE YOU SURE");
		string[] fortInfo = new string[2]{"Yes", "No"};
		for(int i=0; i<fortInfo.Length; i++){
			string curFortInfo = fortInfo[i];
			if(GUI.Button(new Rect(displayScreen.width/4f, displayScreen.height/6f+(i*60), 100, 50), curFortInfo)){
				if(curFortInfo == fortInfo[0]){
					// Destory the object and update graph
					Health fortHealth = fortification.GetComponent<Health>();
					if(fortHealth.curHealth == fortHealth.GetMaxHealth() && fortification.GetComponent<Dragable>() != null){
						GameController.Instance.AddResources(fortification.GetComponent<SellableItem>().cost);
					} else {
						GameController.Instance.AddResources(Mathf.RoundToInt(fortHealth.curHealth / 2));
					}
					GameController.Instance.UpdateGraphOnDestroyedObject(fortification.collider.bounds,
						fortification.collider,fortification.gameObject);
					fortification = null;
					uiState = UIState.NONE;
				} else if(curFortInfo == fortInfo[1]){
					uiState = UIState.FORTINFO;
				}
			}
		}
		
		GUI.EndGroup();
	}
	
	void DrawFortInfo(){
		GUI.BeginGroup(displayScreen);
		
		GUI.Box(new Rect(0, 0, displayScreen.width, displayScreen.height), "Fort Info");
		string[] fortInfo = new string[3]{"Upgrade", "Destroy", "Cancel"};
		for(int i=0; i<fortInfo.Length; i++){
			string curFortInfo = fortInfo[i];
			if(GUI.Button(new Rect(displayScreen.width/4f, displayScreen.height/6f+(i*60), 100, 50), curFortInfo)){
				if(curFortInfo == fortInfo[0]){
					uiState = UIState.FORT_UPGRADE_SCREEN;
				} else if(curFortInfo == fortInfo[1]){
					uiState = UIState.YESORNO;
				} else if(curFortInfo == fortInfo[2]){
					uiState = UIState.NONE;
				}
			}
		}
		
		GUI.EndGroup();
	}
}
