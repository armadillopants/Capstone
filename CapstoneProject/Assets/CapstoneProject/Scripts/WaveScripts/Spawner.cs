using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Spawner : MonoBehaviour {
	
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

	void Start(){
		spawnPoints = GameObject.FindGameObjectsWithTag(Globals.SPAWN_POINT);
		digPoints = GameObject.FindGameObjectsWithTag(Globals.DIG_POINT);
		cycle = GameObject.Find("Sun").GetComponent<DayNightCycle>();
		
		TextAsset asset = new TextAsset();
		asset = (TextAsset)Resources.Load("WaveData", typeof(TextAsset));
		doc.LoadXml(asset.text);
		//doc.Load(Application.dataPath + "/WaveData.xml");
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

			wave.amountToSpawn = scavengerData.amountToSpawn+crusherData.amountToSpawn;
			canDig = true;
		}
		
		CalculateEnemiesToSpawn(isDay);
	}
	
	public void CalculateEnemiesToSpawn(bool isDayTime){
		if(isDayTime){
			for(int i=0; i<cyborgData.amountToSpawn; i++){
				enemiesToSpawn.Add(cyborg);
			}
			
			for(int i=0; i<catData.amountToSpawn; i++){
				enemiesToSpawn.Add(cat);
			}
			
			for(int i=0; i<tigerData.amountToSpawn; i++){
				enemiesToSpawn.Add(tiger);
			}
		} else {
			for(int i=0; i<scavengerData.amountToSpawn; i++){
				enemiesToSpawn.Add(scavenger);
			}
			
			for(int i=0; i<crusherData.amountToSpawn; i++){
				enemiesToSpawn.Add(crusher);
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
		}
		
		return 0;
	}
	
	public void SpawnEnemy(){
		
		GameObject enemy = enemiesToSpawn[Random.Range(0, enemiesToSpawn.Count)];
		
		spawnPoint = spawnPoints[Random.Range(0,spawnPoints.Length)].transform;
		
		Vector3 pos = new Vector3();
		Quaternion rot = new Quaternion();
		
		if(canDig){
			digPoint = digPoints[Random.Range(0,digPoints.Length)].transform;
			if(Random.Range(0, 100) <= 30f){
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
		
		GameObject enemyToSpawn = (GameObject)Instantiate(enemy, pos, rot);
		if(canDig && pos == digPoint.position){
			enemyToSpawn.GetComponent<Enemy>().isUnderground = true;
			enemyToSpawn.AddComponent<DigOutOfGround>();
		}
		enemyToSpawn.name = enemy.name;
		enemiesToSpawn.Remove(enemy);
	}
}
