using UnityEngine;
using System.Collections;

public class WaveController : MonoBehaviour {
	
	public bool isWaiting = false;
	private int waveNumber = 1;
	private float waitTime = 5.0f;
	private static Wave curWave;

	void Start(){
		if(waveNumber == 0){
			waveNumber++;
		}
	}
	
	void Update(){
		if(!isWaiting){
			curWave = gameObject.AddComponent<Wave>();
			isWaiting = true;
			curWave.StartWave(this, waveNumber);
		}
	}
	
	public void StartNextWave(){
		StartCoroutine("WaveCompleted");
	}
	
	private IEnumerator WaveCompleted(){
		yield return new WaitForSeconds(waitTime);
		isWaiting = false;
		waveNumber++;
	}
	
	void OnGUI(){
		GUILayout.Label("Wave Number: " + waveNumber);
	}
}
