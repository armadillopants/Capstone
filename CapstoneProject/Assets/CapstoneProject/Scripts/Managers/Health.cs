using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour {
	
	public float curHealth;
	private float minHealth = 0f;
	public float maxHealth = 100f;
	public bool canTakeDamage = true;
	private bool isDead = false;
	public GameObject explosion;
	public float waitTime = 3f;
	
	private float regenSpeed = 0.5f;
	
	public AudioClip damageClip;
	public AudioClip deathClip;
	
	void Update(){
		
		if(gameObject.tag == Globals.PLAYER){
			if(curHealth < maxHealth){
				curHealth = Mathf.Min(maxHealth, curHealth+regenSpeed*Time.deltaTime);
			}
		}
	}
	
	public bool IsDead {
		get { return isDead; }
		set { isDead = value; }
	}
	
	public float GetMaxHealth(){
		return maxHealth;
	}
	
	public void ModifyHealth(float amount){
		maxHealth = amount;
		curHealth = maxHealth;
	}
	
	public void AddHealth(float howMuch){
		curHealth = Mathf.Min(maxHealth, curHealth+howMuch);
	}
	
	public void TakeDamage(float damage){
		if(canTakeDamage){
			curHealth = Mathf.Max(minHealth, curHealth-damage);
		}
		
		if(damageClip){
			audio.PlayOneShot(damageClip);
		}
		
		if(gameObject.tag == Globals.SHIP){
			if(curHealth % 5 == 0){
				gameObject.GetComponent<ShipDecay>().Release();
			}
		}
		
		if(curHealth == 0 && !isDead){
			Die();
		}
	}

	public void Die(){
		isDead = true;
		
		if(deathClip){
			audio.PlayOneShot(deathClip);
		}
		
		if(gameObject.tag == Globals.FORTIFICATION){
			StartCoroutine(BeginFortDestruction());
		} else if(gameObject.tag == Globals.ENEMY){
			GameController.Instance.AddResources(Mathf.RoundToInt(gameObject.GetComponent<Enemy>().AmountToGive()));
			gameObject.GetComponent<Collider>().enabled = false;
			StartCoroutine(BeginDeathSequence());
		} else if(gameObject.tag == Globals.SHIP){
			Debug.Log("Ship is dead");
		} else if(gameObject.tag == Globals.PLAYER){
			Destroy(gameObject.GetComponent<LocalInput>());
			Destroy(gameObject.GetComponent<PlayerMovement>());
			rigidbody.freezeRotation = true;
			//Destroy(gameObject.GetComponent<AnimationController>());
		} else {
			StartCoroutine(BeginDeathSequence());
		}
	}
	
	IEnumerator BeginDeathSequence(){
		yield return new WaitForSeconds(waitTime);
		if(explosion){
			Instantiate(explosion, transform.position, Quaternion.identity);
		}
		Destroy(gameObject);
	}
	
	IEnumerator BeginFortDestruction(){
		yield return new WaitForSeconds(waitTime);
		if(explosion){
			Instantiate(explosion, transform.position, Quaternion.identity);
		}
		GameController.Instance.UpdateGraphOnDestroyedObject(gameObject.collider, gameObject);
	}
}
