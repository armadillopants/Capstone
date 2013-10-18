using UnityEngine;
using System.Collections;

public class EnemyWeapon : MonoBehaviour {
	
	private BaseWeapon gun;
	private Health health;
	public GameObject gunObject;
	private GameObject target;
	private float distance = 15f;
	
	private float coolDownTimer;
	private float coolDownLength = 5f;

	void Start(){
		gun = transform.GetComponentInChildren<BaseWeapon>();
		health = GetComponent<Health>();
	}
	
	void Update(){
		if(gunObject){
			target = GameController.Instance.FindNearestTarget(Globals.PLAYER, this.transform);
			
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
			
			if(health.curHealth < health.GetMaxHealth()/Random.Range(2,4)){
				gunObject.AddComponent<Rigidbody>().AddForce(new Vector3(2,0,2));
				gunObject.AddComponent<DestroyTimer>();
				gunObject = null;
			}
		}
	}
	
	IEnumerator Firing(){
		yield return new WaitForSeconds(3f);
		coolDownTimer = coolDownLength;
	}
}
