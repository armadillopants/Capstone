using UnityEngine;
using System.Collections;

public class WaveController : MonoBehaviour {
	
	public bool isWaiting = false;
	private int waveNumber = 2;
	private float waitTime = 5.0f;
	private static Wave curWave;
	public bool canBeginWave = false;
	
	public TextMesh curWaveText;
	public TextMesh nextWaveText;
	public TextMesh nextWaveNumberText;
	public TextMesh waveWonOrLostText;
	public TextMesh curWaveNumberText;

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
	
	public int GetWaveNumber(){
		return waveNumber;
	}
	
	void Update(){
		if(canBeginWave){
			curWaveText.text = "WAVE";
			
			if(!isWaiting){
				curWave = gameObject.AddComponent<Wave>();
				curWave.StartWave(this, waveNumber);
				isWaiting = true;
			}
		}
	}
	
	public void StartNextWave(){
		//UIManager.Instance.uiState = UIManager.UIState.WAVEWON;
		waveWonOrLostText.text = "WAVE WON";
		StartCoroutine("WaveCompleted");
	}
	
	private IEnumerator WaveCompleted(){
		yield return new WaitForSeconds(waitTime);
		waveWonOrLostText.text = "";
		isWaiting = false;
		waveNumber++;
	}
	
	public IEnumerator BeginFirstWave(){
		yield return new WaitForSeconds(waitTime);
		canBeginWave = true;
	}
	
	void OnGUI(){
		if(canBeginWave){
			//GUI.Label(new Rect(10, Screen.height-20, 200, 20), "Wave Number: " + waveNumber);
		}
	}
}
