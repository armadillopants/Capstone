using UnityEngine;
using System.Collections;

public class Wave : MonoBehaviour {
	
	private int waveNumber;
	private const int WAVES_BETWEEN_FORTIFICATION = 5;
	private const int END_WAVE = 50;
	public bool beginWave = false;
	public bool endWave = false;
	private WaveController controller;
	private float waitTime = 5.0f;
	public int numEnemies; // Number of enemies on screen
	private float amountToSpawn; // Current number of enemies to spawn all together
	private int amountOfEnemiesToSpawnAtOnce = 15; // Max number of enemies to spawn at once
	private int enemiesSpawned = 0; // Enemies spawned this wave
	private Spawner spawner;
	
	public int waveIncrementor = 6;

	void Start(){
		spawner = GetComponentInChildren<Spawner>();
	}
	
	public void StartWave(WaveController wave, int waveNum){
		// Spawn amount based on wave number
		if(GameController.Instance.EndWave() != 0){
			if(waveNum == GameController.Instance.CurWave()+GameController.Instance.EndWave()){
				amountToSpawn = Mathf.Infinity;
				GameController.Instance.SpawnRescueShip();
			} else {
				amountToSpawn = Mathf.FloorToInt(waveNum * 0.5f) + waveIncrementor;
			}
		} else {
			amountToSpawn = Mathf.FloorToInt(waveNum * 0.5f) + waveIncrementor;
		}
		/*if(waveNum == END_WAVE){
			amountToSpawn = Mathf.Infinity;
			GameController.Instance.SpawnRescueShip();
		} else {
			amountToSpawn = Mathf.FloorToInt(waveNum * 0.5f) + waveIncrementor;
		}*/
		
		UIManager.Instance.uiState = UIManager.UIState.NONE;
		controller = wave;
		waveNumber = waveNum;
		beginWave = false;
    	StartCoroutine("WaveHandling");
  	}
	
	private IEnumerator WaveHandling(){
		if((waveNumber != 0 && waveNumber % WAVES_BETWEEN_FORTIFICATION == 0 && !GameController.Instance.GetShipHealth().IsDead) || (waveNumber == 2)){
			UIManager.Instance.uiState = UIManager.UIState.NONE;
			GameObject.Find("GridContainer").GetComponent<GridSpawner>().EnableGrid();
			GameController.Instance.TurnDragableOn();
			//GameController.Instance.AddDynamicObstacleToFortifications();
			Fortification fort = gameObject.AddComponent<Fortification>();
			fort.StartFortifying(this);
			
			if(waveNumber == 2){
				Tutorial tut = GameObject.Find("Tutorial").GetComponent<Tutorial>();
				tut.key = "Arrow";
			}
			
			controller.curWaveNumberText.text = waveNumber.ToString();
		} else {
			BeginWave();
		}
		
		while(!beginWave){
      		yield return new WaitForSeconds(Time.deltaTime);
    	}
	}
	
	public void BeginWave(){
		//UIManager.Instance.uiState = UIManager.UIState.NEXTWAVE;
		controller.nextWaveText.text = "WAVE";
		controller.nextWaveNumberText.text = waveNumber.ToString();
		StartCoroutine("BeginNewWave");
	}
	
	// Never call this function directly as it is to be called through BeginWave
	IEnumerator BeginNewWave(){
		yield return new WaitForSeconds(waitTime);
		controller.nextWaveText.text = "";
		controller.nextWaveNumberText.text = "";
		controller.curWaveNumberText.text = waveNumber.ToString();
		//UIManager.Instance.uiState = UIManager.UIState.CURWAVE;
		GameController.Instance.canShoot = true;
		GameController.Instance.canChangeWeapons = true;
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
		/*if(waveNumber != 0 && waveNumber % WAVES_BETWEEN_FORTIFICATION != 0){
			BeginWave();
		}*/
		// If the wave is over, then start the next wave and destroy this instance
		// before any other unnecessary calculations are done
		if(endWave){
      		controller.StartNextWave();
			Destroy(this);
		}
		
		numEnemies = GameObject.FindGameObjectsWithTag(Globals.ENEMY).Length;
		
		// Spawn enemies here
		if(beginWave && amountToSpawn > 0){
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
