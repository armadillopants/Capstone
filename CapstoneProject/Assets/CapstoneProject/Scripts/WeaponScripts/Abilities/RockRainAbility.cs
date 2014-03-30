using UnityEngine;

public class RockRainAbility : MonoBehaviour {
	
	private Rigidbody rock;
	private Transform player;
	private int spawnAmount = 8;
	private Vector3 pos;
	private int numRocks;
	private bool rocksSpawned = false;
	private float timer;
	private float maxTimer = 3f;

	void Start(){
		rock = (Rigidbody)Resources.Load("Rock", typeof(Rigidbody));
		player = GameObject.FindWithTag(Globals.PLAYER).transform;
		timer = maxTimer;
	}
	
	void Update(){
		if(AbilitiesManager.Instance.rockRainAbility.coolDown > 0){
			AbilitiesManager.Instance.rockRainAbility.coolDown -= Time.deltaTime;
			
			if(AbilitiesManager.Instance.rockRainAbility.coolDown <= 0){
				AbilitiesManager.Instance.beginAbility = true;
				AbilitiesManager.Instance.rockRainAbility.coolDown = 0;
			}
		}
		
		numRocks = GameObject.FindGameObjectsWithTag(Globals.ABILITY).Length;
		
		if(numRocks > 0){
			rocksSpawned = true;
		}
		
		if(rocksSpawned){
			timer -= Time.deltaTime;
			if(numRocks <= 0 && timer <= 0){
				player.GetComponent<AbilitiesManager>().SetCoolDown(AbilitiesManager.Instance.rockRainAbility);
				rocksSpawned = false;
				timer = maxTimer;
			}
		}
	}
	
	void BeginAbility(){
		AbilitiesManager.Instance.rockRainAbility.amount--;
		for(int i=0; i<spawnAmount; i++){
			pos = player.position + 
				new Vector3(Mathf.Cos(Random.Range(0,360)), 
							player.position.y+Random.Range(3,6), 
							Mathf.Sin(Random.Range(0,360)))*(Random.Range(5, 10));
			
			Rigidbody r =  (Rigidbody)Instantiate(rock, pos, Quaternion.identity);
			r.velocity = transform.TransformDirection(Vector3.down * 50f);
		}
	}
}
