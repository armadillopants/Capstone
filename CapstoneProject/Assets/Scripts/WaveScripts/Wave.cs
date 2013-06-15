using UnityEngine;
using System.Collections;

public class Wave : MonoBehaviour {
	
	private int waveNumber = 1;
	private const int WAVES_BETWEEN_FORTIFICATION = 5;
	public bool beginWave = false;
	public bool endWave = false;
	private WaveController controller;
	private float waitTime = 5.0f;
	public int numEnemies;
	private float spawnAmount;
	private Spawner spawner;

	void Awake(){
		spawner = GetComponentInChildren<Spawner>();
	}
	
	public void StartWave(WaveController wave, int waveNum) {
		// Spawn amount based on wave number
		spawnAmount = Mathf.FloorToInt(waveNum * 0.5f) + 6;
		
		UIManager.Instance.uiState = UIManager.UIState.NONE;
		controller = wave;
		waveNumber = waveNum;
		beginWave = false;
    	StartCoroutine("WaveHandling");
  	}
	
	private IEnumerator WaveHandling(){
		if(waveNumber != 0 && waveNumber % WAVES_BETWEEN_FORTIFICATION == 0){
			UIManager.Instance.uiState = UIManager.UIState.NONE;
			Fortification fort = gameObject.AddComponent<Fortification>();
			fort.StartFortifying(this);
		} else {
			BeginWave();
			UIManager.Instance.uiState = UIManager.UIState.NEXTWAVE;
		}
		
		while(!beginWave){
      		yield return new WaitForSeconds(Time.deltaTime);
    	}
	}
	
	public void BeginWave(){
		StartCoroutine("BeginNewWave");
	}
	
	// Never call this function directly as it is to be called through BeginWave
	IEnumerator BeginNewWave(){
		yield return new WaitForSeconds(waitTime);
		UIManager.Instance.uiState = UIManager.UIState.CURWAVE;
		beginWave = true;
	}
	
	public void StopWave(){
		beginWave = false;
	}
	
	void Update(){
		// Check to see if the waveNumber has been set (StartWave has been called)
		// and if so, and the current wave is neither the first nor a wave that
		// has a fortification phase to it (which would set the beginWave flag itself)
		// then start the wave.
		if(waveNumber != 0 && waveNumber % WAVES_BETWEEN_FORTIFICATION != 0){
			BeginWave();
		}
		// If the wave is over, then start the next wave and destroy this instance
		// before any other unnecessary calculations are done
		if(endWave){
      		controller.StartNextWave();
			Destroy(this);
		}
		
		// Spawn enemies here
		if(beginWave && spawnAmount > 0){
			spawner.SpawnEnemy();
			spawnAmount--;
		}
		
		numEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
		
		if(spawnAmount <= 0 && numEnemies <= 0){
			endWave = true;
		}
	}
}
