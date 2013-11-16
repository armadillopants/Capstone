using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {
	
	public List<GameObject> dayEnemies = new List<GameObject>();
	public List<GameObject> nightEnemies = new List<GameObject>();
	private GameObject enemy;
	private Vector3 pos;
	
	private GameObject[] spawnPoints;
	private Transform spawnPoint;
	
	private DayNightCycle cycle;

	void Start(){
		spawnPoints = GameObject.FindGameObjectsWithTag(Globals.SPAWN_POINT);
		cycle = GameObject.Find("Sun").GetComponent<DayNightCycle>();
	}
	
	void Update(){
	}
	
	public void SpawnEnemy(){
		if(cycle.currentPhase == DayNightCycle.DayPhase.DAY || cycle.currentPhase == DayNightCycle.DayPhase.DAWN){
			for(int i=0; i<dayEnemies.Count; i++){
				enemy = dayEnemies[Random.Range(0, dayEnemies.Count)];
			}
		}
		
		if(cycle.currentPhase == DayNightCycle.DayPhase.NIGHT || cycle.currentPhase == DayNightCycle.DayPhase.DUSK){
			for(int i=0; i<nightEnemies.Count; i++){
				enemy = nightEnemies[Random.Range(0, nightEnemies.Count)];
			}
		}
		
		spawnPoint = spawnPoints[Random.Range(0,spawnPoints.Length)].transform;
		pos = spawnPoint.position + new Vector3(Mathf.Cos(Random.Range(0,360)), 
												0, 
												Mathf.Sin(Random.Range(0,360)))*(Random.Range(3,3));
		Instantiate(enemy, pos, Quaternion.identity);
	}
}
