using UnityEngine;
using System.Collections;

public class Wave : MonoBehaviour {
	
	private int waveNumber;
	private const int WAVES_BETWEEN_FORTIFICATION = 1;
	private const int END_WAVE = 50;
	public bool beginWave = false;
	public bool endWave = false;
	private WaveController controller;
	private float waitTime = 5.0f;
	public float amountToSpawn; // Current number of enemies to spawn all together
	private int amountOfEnemiesToSpawnAtOnce = 15; // Max number of enemies to spawn at once
	private int enemiesSpawned = 0; // Enemies spawned this wave
	private Spawner spawner;

	void Awake(){
		spawner = GetComponentInChildren<Spawner>();
	}
	
	public int GetWaveNumber(){
		return waveNumber;
	}
	
	public void StartWave(WaveController wave, int waveNum){
		// Spawn amount based on wave number
		if(GameController.Instance.EndWave() != 0){
			if(waveNum == GameController.Instance.CurWave()+GameController.Instance.EndWave()){
				GameController.Instance.SpawnRescueShip();
			}
		}
		
		UIManager.Instance.uiState = UIManager.UIState.NONE;
		controller = wave;
		waveNumber = waveNum;
		beginWave = false;
    	StartCoroutine("WaveHandling");
  	}
	
	private IEnumerator WaveHandling(){
		if(waveNumber != 1 && waveNumber % WAVES_BETWEEN_FORTIFICATION == 0 && !GameController.Instance.GetShipHealth().IsDead){
			UIManager.Instance.uiState = UIManager.UIState.NONE;
			GameObject.Find("GridContainer").GetComponent<GridSpawner>().EnableGrid();
			GameController.Instance.TurnDragableOn();
			Fortification fort = gameObject.AddComponent<Fortification>();
			fort.StartFortifying(this);
			
			if(waveNumber == 2){
				Tutorial tut = GameObject.Find("Tutorial").GetComponent<Tutorial>();
				tut.key = "BuildScreen";
			}
		} else {
			BeginWave();
		}
		
		while(!beginWave){
      		yield return new WaitForSeconds(Time.deltaTime);
    	}
	}
	
	public void BeginWave(){
		UIManager.Instance.uiState = UIManager.UIState.NEXTWAVE;
		spawner.SetWaveData(this, waveNumber);
		StartCoroutine("BeginNewWave");
	}
	
	// Never call this function directly as it is to be called through BeginWave
	IEnumerator BeginNewWave(){
		yield return new WaitForSeconds(waitTime);
		UIManager.Instance.uiState = UIManager.UIState.NONE;
		GameController.Instance.canShoot = true;
		GameController.Instance.canChangeWeapons = true;
		beginWave = true;
	}
	
	public void StopWave(){
		beginWave = false;
	}
	
	void Update(){
		// If the wave is over, then start the next wave and destroy this instance
		// before any other unnecessary calculations are done
		if(endWave){
      		controller.StartNextWave();
			Destroy(this);
		}
		
		int numEnemies = GameObject.FindGameObjectsWithTag(Globals.ENEMY).Length;
		
		// Spawn enemies once the wave has started
		if(beginWave){
			if(amountToSpawn > 0){
				if(numEnemies < amountOfEnemiesToSpawnAtOnce && enemiesSpawned < amountToSpawn){
					spawner.SpawnEnemy();
					enemiesSpawned++;
				}
			}
			
			if(enemiesSpawned >= amountToSpawn && numEnemies <=0){
				endWave = true;
			}
		}
	}
}
