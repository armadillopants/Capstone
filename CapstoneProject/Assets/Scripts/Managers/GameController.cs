using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	
	private Health defendHealth;
	private Health playerHealth;
	private GameObject[] enemies;

	void Awake(){
		defendHealth = GameObject.FindWithTag("Defend").GetComponent<Health>();
		playerHealth = GameObject.FindWithTag("Player").GetComponent<Health>();
	}
	
	void Update(){
		if(MenuManager.Instance.menuState != MenuManager.MenuState.INGAME){
			return;
		}
		
		enemies = GameObject.FindGameObjectsWithTag("Enemy");
		if(playerHealth.IsDead()){
			foreach(GameObject enemy in enemies){
				Destroy(enemy);
			}
			SwitchUIState(UIManager.UIState.GAMEOVER);
		}
		
		if(playerHealth.IsDead() && defendHealth.IsDead()){
			foreach(GameObject enemy in enemies){
				Destroy(enemy);
			}
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
