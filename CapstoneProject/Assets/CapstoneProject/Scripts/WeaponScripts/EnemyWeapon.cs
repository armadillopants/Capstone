using UnityEngine;
using System.Collections;

public class EnemyWeapon : MonoBehaviour {
	
	private BaseWeapon gun;
	private Animation anim;
	private Enemy cyborg;
	private Health health;
	public GameObject gunObject;
	private GameObject target;
	public float distance = 15f;
	
	private float coolDownTimer;
	private float coolDownLength = 5f;
	
	private float tempSpeed = 0f;

	void Start(){
		gun = transform.GetComponentInChildren<BaseWeapon>();
		health = GetComponent<Health>();
		cyborg = GetComponent<Enemy>();
		tempSpeed = cyborg.speed;
		anim = cyborg.GetAnim();
	}
	
	void Update(){
		if(gunObject){
			target = GameController.Instance.FindNearestTarget(Globals.PLAYER, this.transform);
			
			if(target){
				if(Vector3.Distance(target.transform.position, transform.position) < distance){
					if(coolDownTimer <= 0){
						coolDownTimer = 0;
						cyborg.speed = 0;
						anim.CrossFade("Shoot", 0.2f);
						gun.Fire();
						StartCoroutine(Firing());
					}
				}
			}
			
			coolDownTimer -= Time.deltaTime;
			
			if(health.curHealth < health.GetMaxHealth()/Random.Range(2,4)){
				gunObject.AddComponent<Rigidbody>().AddForce(new Vector3(transform.position.x+2,0,transform.position.z+2));
				gunObject.AddComponent<DestroyTimer>();
				//gunObject.transform.parent = null;
				gunObject = null;
			}
		}
	}
	
	IEnumerator Firing(){
		yield return new WaitForSeconds(3f);
		coolDownTimer = Random.Range(coolDownLength, coolDownLength*2);
		cyborg.speed = tempSpeed;
		anim.CrossFade("Walk", 0.2f);
	}
}
