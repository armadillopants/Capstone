using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
	
	public float curHealth;
	private float minHealth = 0f;
	public float maxHealth = 100f;
	public bool canTakeDamage = true;
	private bool isDead = false;
	public GameObject explosion;
	public float waitTime = 3f;
	private bool isFortification = false;
	private bool isEnemy = false;
	private bool isShip = false;
	private bool isPlayer = false;
	
	public GameObject shipHealth;
	private GameObject shipHealthBar;
	public GameObject shipHealthGrey;
	private GameObject shipHealthBarGrey;
	
	private GameObject ship;

	void Start(){
		if(gameObject.tag == Globals.FORTIFICATION){
			isFortification = true;
		} else if(gameObject.tag == Globals.SHIP){
			ship = GameObject.FindWithTag(Globals.SHIP);
			shipHealthBar = (GameObject)Instantiate(shipHealth, ship.transform.position+new Vector3(3,3,0), Quaternion.Euler(90,0,0));
			shipHealthBarGrey = (GameObject)Instantiate(shipHealthGrey, ship.transform.position+new Vector3(3,2.9f,0), Quaternion.Euler(90,0,0));
			shipHealthBar.renderer.enabled = false;
			shipHealthBarGrey.renderer.enabled = false;
			isShip = true;
		} else if(gameObject.tag == Globals.ENEMY){
			isEnemy = true;
		} else if(gameObject.tag == Globals.PLAYER){
			isPlayer = true;
		} else {
			Debug.Log("Uhhh, something");
		}
	}
	
	void Update(){
		if(shipHealthBar){
			shipHealthBar.transform.position = ship.transform.position+new Vector3(3,3,0);
			shipHealthBarGrey.transform.position = ship.transform.position+new Vector3(3,2.9f,0);
			
			Vector3 localScaleX = shipHealthBar.transform.localScale;
			localScaleX.x = curHealth*0.02f;
			shipHealthBar.transform.localScale = localScaleX;
			
			if(Vector3.Distance(ship.transform.position, GameObject.FindWithTag(Globals.PLAYER).transform.position) <= 10f && 
				GameController.Instance.GetPlayer().GetComponent<PlayerMovement>() != null){
				shipHealthBar.renderer.enabled = true;
				shipHealthBarGrey.renderer.enabled = true;
			} else {
				shipHealthBar.renderer.enabled = false;
				shipHealthBarGrey.renderer.enabled = false;
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
		
		if(isShip){
			gameObject.GetComponent<ShipDecay>().Release();
		}
		
		if(curHealth == 0 && !isDead){
			Die();
		}
	}

	public void Die(){
		isDead = true;
		
		if(isFortification){
			GameController.Instance.UpdateGraphOnDestroyedObject(gameObject.collider, gameObject);
		} else if(isEnemy){
			GameController.Instance.AddResources(Mathf.RoundToInt(gameObject.GetComponent<Enemy>().AmountToGive()));
			Destroy(gameObject.GetComponent<Collider>());
			StartCoroutine(BeginDeathSequence());
		} else if(isShip){
			Debug.Log("Ship is dead");
		} else if(isPlayer){
			Destroy(gameObject.GetComponent<LocalInput>());
			Destroy(gameObject.GetComponent<PlayerMovement>());
			Destroy(gameObject.GetComponent<AnimationController>());
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
}
