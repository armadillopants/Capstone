using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
	
	public enum UIState { PAUSE, WAVEWON, WAVELOST, GAMEOVER, NONE };
	public UIState uiState = UIState.NONE;
	public bool isPaused = false;
	private WeaponSelection selection;
	
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
	
	void Update(){
		if(isPaused){
			Time.timeScale = 0;
		} else {
			if(!selection.changingWeapons){
				Time.timeScale = 1;
			}
		}
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
		case UIState.WAVELOST:
			DrawWaveLostScreen();
			break;
		case UIState.GAMEOVER:
			DrawGameOverScreen();
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
	}
	
	void DrawWaveLostScreen(){
	}
	
	void DrawGameOverScreen(){
		if(GUI.Button(new Rect(Screen.width/2, Screen.height/2, 100, 50), "Restart")){
			Application.LoadLevel(Application.loadedLevel);
		}
	}
}
