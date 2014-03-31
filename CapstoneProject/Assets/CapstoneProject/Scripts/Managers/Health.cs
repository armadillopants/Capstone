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
		
		if(gameObject.tag == Globals.SHIP || gameObject.tag == Globals.INTERACTABLE_ITEM){
			// Display nothing
		} else {
			Vector3 healthPos = Camera.main.WorldToScreenPoint(transform.position);
			healthDisplayRect = new Rect(healthPos.x, Screen.height-healthPos.y, 100, 10);
			
			if(displayHealthTimer > 0){
				displayHealthTimer -= Time.deltaTime;
				displayHealth = true;
			} else {
				displayHealth = false;
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
	
	void OnGUI(){
		if(displayHealth){
			GUI.BeginGroup(healthDisplayRect);
			
			Texture2D healthBar = new Texture2D(1, 1, TextureFormat.RGB24, false);
			healthBar.SetPixel(0, 0, healthColor);
			healthBar.Apply();
			
			Texture2D grayBar = new Texture2D(1, 1, TextureFormat.RGB24, false);
			grayBar.SetPixel(0, 0, Color.gray);
			grayBar.Apply();
			
			GUI.DrawTexture(new Rect(healthDisplayRect.width, 0, 
				-healthDisplayRect.width*maxHealth, healthDisplayRect.height), 
				grayBar, ScaleMode.StretchToFill);
			GUI.DrawTexture(new Rect(healthDisplayRect.width, 0, 
				-healthDisplayRect.width*curHealth/maxHealth, healthDisplayRect.height), 
				healthBar, ScaleMode.StretchToFill);
			
			GUI.EndGroup();
		}
	}
}
