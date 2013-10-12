using UnityEngine;
using System.Collections;

public class WaveController : MonoBehaviour {
	
	public bool isWaiting = false;
	private int waveNumber = 1;
	private float waitTime = 5.0f;
	private static Wave curWave;
	public bool canBeginWave = false;

	void Start(){
		if(waveNumber == 0){
			waveNumber++;
		}
	}
	
	public void ResetWave(int wave){
		waveNumber = wave;
		isWaiting = false;
		canBeginWave = false;
	}
	
	void Update(){
		if(canBeginWave){
			if(!isWaiting){
				curWave = gameObject.AddComponent<Wave>();
				curWave.StartWave(this, waveNumber);
				isWaiting = true;
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
	
	public IEnumerator BeginFirstWave(){
		yield return new WaitForSeconds(waitTime);
		canBeginWave = true;
	}
	
	void OnGUI(){
		if(canBeginWave){
			GUI.Box(new Rect(0, 300, 200, 20), "Wave Number: " + waveNumber);
		}
	}
}
