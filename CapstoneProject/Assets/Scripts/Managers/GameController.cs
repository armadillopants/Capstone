using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	
	private Health defendHealth;
	private Health playerHealth;

	void Awake(){
		defendHealth = GameObject.FindWithTag("Defend").GetComponent<Health>();
		playerHealth = GameObject.FindWithTag("Player").GetComponent<Health>();
	}
	
	void Update(){
		if(MenuManager.Instance.menuState != MenuManager.MenuState.INGAME){
			return;
		}
		if(playerHealth.IsDead()){
			SwitchUIState(UIManager.UIState.GAMEOVER);
		}
		
		if(playerHealth.IsDead() && defendHealth.IsDead()){
			SwitchUIState(UIManager.UIState.GAMEOVER);
		}
		
		if(Input.GetKeyDown(KeyCode.Escape) && UIManager.Instance.uiState != UIManager.UIState.GAMEOVER){
			UIManager.Instance.isPaused = true;
			SwitchUIState(UIManager.UIState.PAUSE);
		}
	}
	
	void SwitchUIState(UIManager.UIState state){
		UIManager.Instance.uiState = state;
	}
}
