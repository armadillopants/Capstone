using UnityEngine;
using System.Collections;

public class WaveController : MonoBehaviour {
	
	public bool isWaiting = false;
	private int waveNumber = 5;
	private float waitTime = 5.0f;
	private static Wave curWave;
	private bool displayGui;
	private bool canBeginWave = false;
	private float timer = 20f;

	void Start(){
		if(waveNumber == 0){
			waveNumber++;
		}
	}
	
	void Update(){
		if(MenuManager.Instance.menuState != MenuManager.MenuState.INGAME){
			displayGui = false;
			return;
		} else {
			timer -= Time.deltaTime;
			if(timer <= 0){
				timer = 0;
				displayGui = true;
				canBeginWave = true;
			}
		}
		if(canBeginWave){
			if(!isWaiting){
				curWave = gameObject.AddComponent<Wave>();
				isWaiting = true;
				curWave.StartWave(this, waveNumber);
			}
		}
	}
	
	public void StartNextWave(){
		UIManager.Instance.uiState = UIManager.UIState.WAVEWON;
		StartCoroutine("WaveCompleted");
	}
	
	private IEnumerator WaveCompleted(){
		yield return new WaitForSeconds(waitTime);
		isWaiting = false;
		waveNumber++;
	}
	
	void OnGUI(){
		if(displayGui){
			GUI.Box(new Rect(0, 300, 200, 20), "Wave Number: " + waveNumber);
		}
	}
}
