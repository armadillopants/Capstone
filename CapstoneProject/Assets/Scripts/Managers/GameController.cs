using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	
	private Health defendHealth;
	private Health playerHealth;
	private GameObject[] enemies;
	
	protected int amountOfResources = 10000;
	public bool canDisplay = true;
	
	#region Singleton
	
	private static GameController _instance;

	public static GameController Instance {
		get { return _instance; }
	}

	void Awake(){
		if(GameController.Instance != null){
			DestroyImmediate(gameObject);
			return;
		}
		_instance = this;
	}

	void OnApplicationQuit(){
		_instance = null;
	}
	
	#endregion

	void Start(){
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
	
	public int GetResources(){
		return amountOfResources;
	}
	
	public void AddResources(int amount){
		amountOfResources += amount;
	}
	
	public void DeleteResources(int amount){
		amountOfResources -= amount;
	}
	
	void SwitchUIState(UIManager.UIState state){
		UIManager.Instance.uiState = state;
	}
}
