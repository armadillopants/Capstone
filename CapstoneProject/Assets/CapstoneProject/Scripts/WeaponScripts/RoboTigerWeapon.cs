using System.Collections;
using UnityEngine;

public class RoboTigerWeapon : MonoBehaviour {
	
	private BaseWeapon gun;
	private Health health;
	public GameObject gunPrefab;
	public Transform spawnPoint;
	private GameObject gunObject;
	public float distance = 15f;
	
	private float coolDownTimer;
	private float coolDownLength = 5f;
	
	private bool dropGun = false;
	
	void Start(){
		health = GetComponent<Health>();
	}

	void OnEnable(){
		gunObject = (GameObject)Instantiate(gunPrefab, spawnPoint.position, spawnPoint.rotation);
		gunObject.transform.parent = spawnPoint;
		gun = GetComponentInChildren<BaseWeapon>();
	}
	
	void Update(){
		if(gunObject){
			GameObject target = GameController.Instance.FindNearestTarget(Globals.PLAYER, this.transform);
			gunObject.transform.rotation = Quaternion.Slerp(gunObject.transform.rotation, 
							Quaternion.LookRotation(target.transform.position-transform.position), 6f*Time.deltaTime);
			
			if(target){
				if(Vector3.Distance(target.transform.position, transform.position) < distance){
					if(coolDownTimer <= 0){
						coolDownTimer = 0;
						gun.Fire();
						StartCoroutine(Firing());
					}
				}
			}
			
			coolDownTimer -= Time.deltaTime;
			
			if(health.curHealth < health.GetMaxHealth()/Random.Range(2,4) && !dropGun){
				StopCoroutine("Firing");
				gunObject.AddComponent<BoxCollider>();
				gunObject.AddComponent<Rigidbody>().AddForce(new Vector3(transform.position.x+2,0,transform.position.z+2));
				gunObject.AddComponent<DestroyTimer>();
				gunObject.transform.parent = null;
				gunObject.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
				gunObject = null;
				dropGun = true;
			}
		}
	}
	
	IEnumerator Firing(){
		yield return new WaitForSeconds(coolDownLength);
		coolDownTimer = Random.Range(coolDownLength, coolDownLength*2);
	}
}
