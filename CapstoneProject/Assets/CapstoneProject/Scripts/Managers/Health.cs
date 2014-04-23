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
	
	private float displayHealthTimer;
	public float displayHealthTimerMax = 3f;
	public Color healthColor;
	private bool displayHealth = false;
	private Rect healthDisplayRect;
	public Texture2D damageTex;
	public Texture2D healthBarFrame;
	private Texture2D healthBar;
	private Texture2D grayBar;
	
	void Start(){
		healthBar = new Texture2D(1, 1, TextureFormat.RGB24, false);
		healthBar.SetPixel(0, 0, healthColor);
		healthBar.Apply();
		
		grayBar = new Texture2D(1, 1, TextureFormat.RGB24, false);
		grayBar.SetPixel(0, 0, Color.gray);
		grayBar.Apply();
	}
	
	void Update(){
		
		if(UIManager.Instance.uiState == UIManager.UIState.GAMEOVER){
			displayHealth = false;
			displayHealthTimer = 0;
		}
		
		if(gameObject.tag == Globals.PLAYER){
			if(curHealth < maxHealth){
				curHealth = Mathf.Min(maxHealth, curHealth+regenSpeed*Time.deltaTime);
			}
		}
		
		if(gameObject.tag != Globals.SHIP && gameObject.tag != Globals.INTERACTABLE_ITEM){
			Vector3 healthPos = Camera.main.WorldToScreenPoint(transform.position);
			healthDisplayRect = new Rect(healthPos.x, Screen.height-healthPos.y, 120, 20);
			
			if(displayHealthTimer > 0){
				displayHealthTimer -= Time.deltaTime;
				displayHealth = true;
			} else {
				displayHealth = false;
				displayHealthTimer = 0;
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
		if(isDead){
			return;
		}
		
		if(canTakeDamage){
			curHealth = Mathf.Max(minHealth, curHealth-damage);
		}
		
		displayHealthTimer = displayHealthTimerMax;
		
		if(damageClip){
			audio.PlayOneShot(damageClip);
		}
		
		if(gameObject.tag == Globals.SHIP){
			if(curHealth % 20 == 0){
				gameObject.GetComponent<ShipDecay>().Release();
			}
		}
		
		if(curHealth == 0 && !isDead){
			Die();
		}
	}

	public void Die(){
		isDead = true;
		displayHealth = false;
		displayHealthTimer = 0;
		
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
			rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			//Destroy(gameObject.GetComponent<AnimationController>());
		} else {
			StartCoroutine(BeginDeathSequence());
		}
	}
	
	IEnumerator BeginDeathSequence(){
		yield return new WaitForSeconds(waitTime);
		if(explosion){
			ObjectPool.Spawn(explosion, transform.position, Quaternion.identity);
		}
		gameObject.SetActive(false);
	}
	
	IEnumerator BeginFortDestruction(){
		yield return new WaitForSeconds(waitTime);
		if(explosion){
			ObjectPool.Spawn(explosion, transform.position, Quaternion.identity);
		}
		GameController.Instance.UpdateGraphOnDestroyedObject(gameObject.collider, gameObject);
	}
	
	void OnGUI(){
		if(displayHealth){
			GUI.BeginGroup(healthDisplayRect);
			
			GUI.DrawTexture(new Rect(100, 5, -100*maxHealth, 10), grayBar);
			GUI.DrawTexture(new Rect(100, 5, -100*curHealth/maxHealth, 10), healthBar);
			GUI.DrawTexture(new Rect(0, 0, healthDisplayRect.width, healthDisplayRect.height), healthBarFrame);
			
			GUI.EndGroup();
			
			if(gameObject.tag == Globals.PLAYER){
				float alpha = displayHealthTimer;
				if(alpha > 1f){
					alpha = 1f;
				}
				GUI.color = new Color(1.0f,1.0f,1.0f, (1f-(curHealth/maxHealth))*alpha);
				GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height), damageTex);
			}
		}
	}
}
