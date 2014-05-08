using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Spawner : MonoBehaviour {
	
	public static Spawner spawner;
	
	public class EnemyData {
		public XmlNode firstNode;
		public int amountToSpawn;
		public float health;
		public float moveSpeed;
		public float turnSpeed;
		public float attackSpeed;
		public float damageAmount;
	}
	
	private List<GameObject> enemiesToSpawn = new List<GameObject>();
	public GameObject cyborg;
	public GameObject cat;
	public GameObject tiger;
	public GameObject scavenger;
	public GameObject crusher;
	public GameObject worm;
	
	private GameObject[] spawnPoints;
	private Transform spawnPoint;
	private GameObject[] digPoints;
	private Transform digPoint;
	private bool canDig = false;
	
	private DayNightCycle cycle;
	
	private XmlDocument doc = new XmlDocument();
	
	private EnemyData cyborgData = new EnemyData();
	private EnemyData catData = new EnemyData();
	private EnemyData tigerData = new EnemyData();
	private EnemyData scavengerData = new EnemyData();
	private EnemyData crusherData = new EnemyData();
	private EnemyData wormData = new EnemyData();
	
	public GameObject hole;
	public GameObject explosion;
	private Dictionary<string, GameObject> baseInstantiator = new Dictionary<string, GameObject>();
	private MultiDictionary<string, GameObject> activeInstances = new MultiDictionary<string, GameObject>();

	void Awake(){
		spawner = this;
		spawnPoints = GameObject.FindGameObjectsWithTag(Globals.SPAWN_POINT);
		digPoints = GameObject.FindGameObjectsWithTag(Globals.DIG_POINT);
		cycle = GameObject.Find("Sun").GetComponent<DayNightCycle>();
		
		TextAsset asset = new TextAsset();
		asset = (TextAsset)Resources.Load("WaveData", typeof(TextAsset));
		doc.LoadXml(asset.text);
		//doc.Load(Application.dataPath + "/WaveData.xml");
	}
	
	public void Reset(){
		enemiesToSpawn.Clear();
	}
	
	public void SetWaveData(Wave wave, int waveNum){
		bool isDay = false;
		
		if(cycle.currentPhase == DayNightCycle.DayPhase.DAY || cycle.currentPhase == DayNightCycle.DayPhase.DAWN){
			cyborgData.firstNode = doc.SelectSingleNode("/WaveData/Wave/Wave"+waveNum + "/Day/Cyborg");
			cyborgData.amountToSpawn = int.Parse(cyborgData.firstNode.Attributes.GetNamedItem("amountToSpawn").Value);
			cyborgData.health = float.Parse(cyborgData.firstNode.Attributes.GetNamedItem("health").Value);
			cyborgData.moveSpeed = float.Parse(cyborgData.firstNode.Attributes.GetNamedItem("moveSpeed").Value);
			cyborgData.turnSpeed = float.Parse(cyborgData.firstNode.Attributes.GetNamedItem("turnSpeed").Value);
			cyborgData.attackSpeed = float.Parse(cyborgData.firstNode.Attributes.GetNamedItem("attackSpeed").Value);
			cyborgData.damageAmount = float.Parse(cyborgData.firstNode.Attributes.GetNamedItem("damageAmount").Value);
			
			catData.firstNode = doc.SelectSingleNode("/WaveData/Wave/Wave"+waveNum + "/Day/Cat");
			catData.amountToSpawn = int.Parse(catData.firstNode.Attributes.GetNamedItem("amountToSpawn").Value);
			catData.health = float.Parse(catData.firstNode.Attributes.GetNamedItem("health").Value);
			catData.moveSpeed = float.Parse(catData.firstNode.Attributes.GetNamedItem("moveSpeed").Value);
			catData.turnSpeed = float.Parse(catData.firstNode.Attributes.GetNamedItem("turnSpeed").Value);
			catData.attackSpeed = float.Parse(catData.firstNode.Attributes.GetNamedItem("attackSpeed").Value);
			catData.damageAmount = float.Parse(catData.firstNode.Attributes.GetNamedItem("damageAmount").Value);
			
			tigerData.firstNode = doc.SelectSingleNode("/WaveData/Wave/Wave"+waveNum + "/Day/Tiger");
			tigerData.amountToSpawn = int.Parse(tigerData.firstNode.Attributes.GetNamedItem("amountToSpawn").Value);
			tigerData.health = float.Parse(tigerData.firstNode.Attributes.GetNamedItem("health").Value);
			tigerData.moveSpeed = float.Parse(tigerData.firstNode.Attributes.GetNamedItem("moveSpeed").Value);
			tigerData.turnSpeed = float.Parse(tigerData.firstNode.Attributes.GetNamedItem("turnSpeed").Value);
			tigerData.attackSpeed = float.Parse(tigerData.firstNode.Attributes.GetNamedItem("attackSpeed").Value);
			tigerData.damageAmount = float.Parse(tigerData.firstNode.Attributes.GetNamedItem("damageAmount").Value);
			
			wave.amountToSpawn = cyborgData.amountToSpawn+catData.amountToSpawn+tigerData.amountToSpawn;
			isDay = true;
			canDig = false;
		}
		
		if(cycle.currentPhase == DayNightCycle.DayPhase.NIGHT || cycle.currentPhase == DayNightCycle.DayPhase.DUSK){
			scavengerData.firstNode = doc.SelectSingleNode("/WaveData/Wave/Wave"+waveNum + "/Night/Scavenger");
			scavengerData.amountToSpawn = int.Parse(scavengerData.firstNode.Attributes.GetNamedItem("amountToSpawn").Value);
			scavengerData.health = float.Parse(scavengerData.firstNode.Attributes.GetNamedItem("health").Value);
			scavengerData.moveSpeed = float.Parse(scavengerData.firstNode.Attributes.GetNamedItem("moveSpeed").Value);
			scavengerData.turnSpeed = float.Parse(scavengerData.firstNode.Attributes.GetNamedItem("turnSpeed").Value);
			scavengerData.attackSpeed = float.Parse(scavengerData.firstNode.Attributes.GetNamedItem("attackSpeed").Value);
			scavengerData.damageAmount = float.Parse(scavengerData.firstNode.Attributes.GetNamedItem("damageAmount").Value);
			
			crusherData.firstNode = doc.SelectSingleNode("/WaveData/Wave/Wave"+waveNum + "/Night/Crusher");
			crusherData.amountToSpawn = int.Parse(crusherData.firstNode.Attributes.GetNamedItem("amountToSpawn").Value);
			crusherData.health = float.Parse(crusherData.firstNode.Attributes.GetNamedItem("health").Value);
			crusherData.moveSpeed = float.Parse(crusherData.firstNode.Attributes.GetNamedItem("moveSpeed").Value);
			crusherData.turnSpeed = float.Parse(crusherData.firstNode.Attributes.GetNamedItem("turnSpeed").Value);
			crusherData.attackSpeed = float.Parse(crusherData.firstNode.Attributes.GetNamedItem("attackSpeed").Value);
			crusherData.damageAmount = float.Parse(crusherData.firstNode.Attributes.GetNamedItem("damageAmount").Value);
			
			wormData.firstNode = doc.SelectSingleNode("/WaveData/Wave/Wave"+waveNum + "/Night/Worm");
			wormData.amountToSpawn = int.Parse(wormData.firstNode.Attributes.GetNamedItem("amountToSpawn").Value);
			wormData.health = float.Parse(wormData.firstNode.Attributes.GetNamedItem("health").Value);
			wormData.moveSpeed = float.Parse(wormData.firstNode.Attributes.GetNamedItem("moveSpeed").Value);
			wormData.turnSpeed = float.Parse(wormData.firstNode.Attributes.GetNamedItem("turnSpeed").Value);
			wormData.attackSpeed = float.Parse(wormData.firstNode.Attributes.GetNamedItem("attackSpeed").Value);
			wormData.damageAmount = float.Parse(wormData.firstNode.Attributes.GetNamedItem("damageAmount").Value);

			wave.amountToSpawn = scavengerData.amountToSpawn+crusherData.amountToSpawn+wormData.amountToSpawn;
			canDig = true;
		}
		
		CalculateEnemiesToSpawn(isDay);
	}
	
	public void CalculateEnemiesToSpawn(bool isDayTime){
		if(isDayTime){
			for(int i=0; i<cyborgData.amountToSpawn; i++){
				AddEnemyType(cyborg.name, cyborg);
				enemiesToSpawn.Add(cyborg);
			}
			
			for(int i=0; i<catData.amountToSpawn; i++){
				AddEnemyType(cat.name, cat);
				enemiesToSpawn.Add(cat);
			}
			
			for(int i=0; i<tigerData.amountToSpawn; i++){
				AddEnemyType(tiger.name, tiger);
				enemiesToSpawn.Add(tiger);
			}
		} else {
			for(int i=0; i<scavengerData.amountToSpawn; i++){
				AddEnemyType(scavenger.name, scavenger);
				enemiesToSpawn.Add(scavenger);
			}
			
			for(int i=0; i<crusherData.amountToSpawn; i++){
				AddEnemyType(crusher.name, crusher);
				enemiesToSpawn.Add(crusher);
			}
			
			for(int i=0; i<wormData.amountToSpawn; i++){
				AddEnemyType(worm.name, worm);
				enemiesToSpawn.Add(worm);
			}
		}
	}
	
	public float SetEnemyHealth(string enemyName){
		switch(enemyName){
		case "Cyborg":
			return cyborgData.health;
		case "CyberCat":
			return catData.health;
		case "RoboTiger":
			return tigerData.health;
		case "Scavenger":
			return scavengerData.health;
		case "Crusher":
			return crusherData.health;
		case "Worm":
			return wormData.health;
		}
		
		return 0;
	}
	
	public float SetEnemyMoveSpeed(string enemyName){
		switch(enemyName){
		case "Cyborg":
			return cyborgData.moveSpeed;
		case "CyberCat":
			return catData.moveSpeed;
		case "RoboTiger":
			return tigerData.moveSpeed;
		case "Scavenger":
			return scavengerData.moveSpeed;
		case "Crusher":
			return crusherData.moveSpeed;
		case "Worm":
			return wormData.moveSpeed;
		}
		
		return 0;
	}
	
	public float SetEnemyTurnSpeed(string enemyName){
		switch(enemyName){
		case "Cyborg":
			return cyborgData.turnSpeed;
		case "CyberCat":
			return catData.turnSpeed;
		case "RoboTiger":
			return tigerData.turnSpeed;
		case "Scavenger":
			return scavengerData.turnSpeed;
		case "Crusher":
			return crusherData.turnSpeed;
		case "Worm":
			return wormData.turnSpeed;
		}
		
		return 0;
	}
	
	public float SetEnemyAttackSpeed(string enemyName){
		switch(enemyName){
		case "Cyborg":
			return cyborgData.attackSpeed;
		case "CyberCat":
			return catData.attackSpeed;
		case "RoboTiger":
			return tigerData.attackSpeed;
		case "Scavenger":
			return scavengerData.attackSpeed;
		case "Crusher":
			return crusherData.attackSpeed;
		case "Worm":
			return wormData.attackSpeed;
		}
		
		return 0;
	}
	
	public float SetEnemyDamageAmount(string enemyName){
		switch(enemyName){
		case "Cyborg":
			return cyborgData.damageAmount;
		case "CyberCat":
			return catData.damageAmount;
		case "RoboTiger":
			return tigerData.damageAmount;
		case "Scavenger":
			return scavengerData.damageAmount;
		case "Crusher":
			return crusherData.damageAmount;
		case "Worm":
			return wormData.damageAmount;
		}
		
		return 0;
	}
	
	public void SpawnEnemy(){
		
		GameObject enemy = enemiesToSpawn[Random.Range(0, enemiesToSpawn.Count)];
		
		spawnPoint = spawnPoints[Random.Range(0,spawnPoints.Length)].transform;
		
		Vector3 pos = new Vector3();
		Quaternion rot = new Quaternion();
		
		if(canDig && enemy.name != "Worm"){
			digPoint = digPoints[Random.Range(0,digPoints.Length)].transform;
			if(Random.Range(0, 100) <= 25){
				pos = digPoint.position;
				rot = Quaternion.Euler(-90f,0,0);
			} else {
				pos = spawnPoint.position + new Vector3(Mathf.Cos(Random.Range(0,360)), 
												0, 
												Mathf.Sin(Random.Range(0,360)))*(Random.Range(3,3));
				rot = Quaternion.identity;
			}
		} else {
			pos = spawnPoint.position + new Vector3(Mathf.Cos(Random.Range(0,360)), 
												0, 
												Mathf.Sin(Random.Range(0,360)))*(Random.Range(3,3));
			rot = Quaternion.identity;
		}
		
		GameObject enemyToSpawn = SpawnEnemy(enemy.name, pos, rot);//ObjectPool.Spawn(enemy, pos, rot);
		enemyToSpawn.name = enemy.name;
		if(enemy.name != "Worm"){
			if(canDig && pos == digPoint.position){
				enemyToSpawn.GetComponent<Enemy>().isUnderground = true;
				enemyToSpawn.AddComponent<DigOutOfGround>();
			}
		}
		enemiesToSpawn.Remove(enemy);
	}
	
	public void AddEnemyType(string enemyName, GameObject baseObject){
		if(!baseInstantiator.ContainsKey(enemyName)){
			baseInstantiator.Add(enemyName, baseObject);
		}
	}
	
	public GameObject SpawnEnemy(string enemyName, Vector3 pos, Quaternion rot){
		if(!baseInstantiator.ContainsKey(enemyName)){
			Debug.LogError("No definition for enemy type " + enemyName + " in Spawner.");
			return null;
		}
		
		GameObject instance = FindUnused(enemyName);
		if(instance != null){
			instance.SetActive(true);
			instance.transform.position = pos;
			instance.transform.rotation = rot;
			return instance;
		}
			
		instance = (GameObject)Instantiate(baseInstantiator[enemyName], pos, rot);
		activeInstances.Add(enemyName, instance);
		return instance;
	}
	
	private GameObject FindUnused(string enemyName){
		
		GameObject[] instances = activeInstances.GetValues(enemyName);
		
		if(instances != null){
			for(int i=0; i<instances.Length; i++){
				if(instances[i] != null){
					if(!instances[i].activeSelf){
						return instances[i];
					}
				}
			}
		}

		return null;
	}
}
