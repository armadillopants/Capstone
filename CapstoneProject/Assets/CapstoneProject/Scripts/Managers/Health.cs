using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
	
	public float curHealth;
	private float minHealth = 0f;
	private float maxHealth;
	public bool canTakeDamage = true;
	private bool isDead = false;
	public GameObject explosion;
	private bool isFortification = false;

	void Start(){
		if(gameObject.tag == Globals.FORTIFICATION){
			isFortification = true;
		}
	}
	
	void Update(){
	}
	
	public bool IsDead(){
		return isDead;
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
		if(curHealth == 0 && !isDead){
			Die();
		}
	}

	public void Die(){
		isDead = true;
		if(explosion){
			Instantiate(explosion, transform.position, Quaternion.identity);
		}
		if(!isFortification){
			Destroy(gameObject);
		}
		
		if(isFortification){
			GameController.Instance.UpdateGraphOnDestroyedObject(gameObject.collider.bounds, gameObject.collider, gameObject);
		}
	}
}
