using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
	
	public float curHealth;
	private float minHealth = 0f;
	private float maxHealth;
	public bool canTakeDamage = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
		if(curHealth == 0){
			
		}
	}
}
