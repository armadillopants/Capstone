using UnityEngine;
using System.Collections;

public class EnemyWeapon : MonoBehaviour {
	
	private BaseWeapon[] guns;
	private Animation anim;
	private Enemy cyborg;
	private Health health;
	public GameObject gunObject;
	private GameObject target;
	private float distance = 15f;
	
	private float coolDownTimer;
	private float coolDownLength = 5f;

	void Start(){
		guns = transform.GetComponentsInChildren<BaseWeapon>();
		health = GetComponent<Health>();
		cyborg = GetComponent<Enemy>();
		anim = cyborg.GetAnim();
	}
	
	void Update(){
		if(gunObject){
			target = GameController.Instance.FindNearestTarget(Globals.PLAYER, this.transform);
			
			if(target){
				if(Vector3.Distance(target.transform.position, transform.position) < distance){
					if(coolDownTimer <= 0){
						coolDownTimer = 0;
						//cyborg.canMove = false;
						anim.CrossFade("Shoot", 0.2f);
						guns[0].Fire();
						guns[1].Fire();
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
		coolDownTimer = Random.Range(coolDownLength, coolDownLength*2);
		//cyborg.canMove = true;
		anim.CrossFade("Walk", 0.2f);
	}
}
