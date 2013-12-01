using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponSelection : MonoBehaviour {
	
	private Vector2 buttonSize = new Vector2(120, 40);
	
	public bool changingWeapons = false;
	private float slowmoTime = 2f;
	public List<GameObject> weaponSlots = new List<GameObject>();
	private WeaponManager manager;
	
	private BaseWeapon weapon;
	
	void Awake(){
		GameObject player = GameObject.FindWithTag(Globals.PLAYER);
		manager = player.GetComponentInChildren<WeaponManager>();
		weapon = GetComponentInChildren<BaseWeapon>();
		UpdateWeaponsSlots();
	}
	
	void Start(){
		// Choose first weapon
		SelectWeapon(weaponSlots[0].GetComponent<BaseWeapon>().id);
	}
	
	void Update(){
		if(weapon.isAutomatic){
			if(Input.GetButton("Fire1") && GameController.Instance.canShoot){
				BroadcastMessage("Fire");
				weapon.isFiring = true;
			} else {
				weapon.isFiring = false;
			}
		} else {
			if(Input.GetButtonDown("Fire1") && GameController.Instance.canShoot){
				BroadcastMessage("Fire");
				weapon.isFiring = true;
			} else {
				weapon.isFiring = false;
			}
		}
		
		if(GameController.Instance.canChangeWeapons){
			if(!changingWeapons && Input.GetKey(KeyCode.LeftShift) && UIManager.Instance.uiState != UIManager.UIState.PAUSE){
				changingWeapons = true;
				StartCoroutine("SlowMotion");
			}
			
			if(Input.GetKey(KeyCode.LeftShift) && UIManager.Instance.uiState != UIManager.UIState.PAUSE){
				changingWeapons = true;
				GameController.Instance.canShoot = false;
			} else {
				changingWeapons = false;
				GameController.Instance.canShoot = true;
			}
		}
	}
	
	private IEnumerator SlowMotion(){
		float startUp = Time.realtimeSinceStartup;
		// Set time to slow motion
		Time.timeScale = 0.1f;
		while(Time.realtimeSinceStartup - startUp < slowmoTime){
			// Break prematurely if the player is done before their time is up
			if(!changingWeapons){
				break;
			}
			yield return null;
		}
		
		// Set time back to normal
		Time.timeScale = 1.0f;
	}
	
	void OnGUI(){
		if(changingWeapons){
			// Display 4 GUI buttons: left, right, top, bottom
			if(weaponSlots[0] != null){
				if(GUI.Button(new Rect((Screen.width/2 - (buttonSize.x/2))-200, Screen.height/2 - (buttonSize.y/2), buttonSize.x, buttonSize.y), weaponSlots[0].name)){
					SelectWeapon(weaponSlots[0].GetComponent<BaseWeapon>().id);
				}
			}
			if(weaponSlots[1] != null){
				if(GUI.Button(new Rect((Screen.width/2 - (buttonSize.x/2))+200, Screen.height/2 - (buttonSize.y/2), buttonSize.x, buttonSize.y), weaponSlots[1].name)){
					SelectWeapon(weaponSlots[1].GetComponent<BaseWeapon>().id);
				}
			}
			if(weaponSlots[2] != null){
				if(GUI.Button(new Rect(Screen.width/2 - (buttonSize.x/2), (Screen.height/2 - (buttonSize.y/2))-200, buttonSize.x, buttonSize.y), weaponSlots[2].name)){
					SelectWeapon(weaponSlots[2].GetComponent<BaseWeapon>().id);
				}
			}
			if(weaponSlots[3] != null){
				if(GUI.Button(new Rect(Screen.width/2 - (buttonSize.x/2), (Screen.height/2 - (buttonSize.y/2))+200, buttonSize.x, buttonSize.y), weaponSlots[3].name)){
					SelectWeapon(weaponSlots[3].GetComponent<BaseWeapon>().id);
				}
			}
		}
		
		if(GameController.Instance.GetPlayer().GetComponent<PlayerMovement>() != null){
			GUI.Box(new Rect(Screen.width-200, Screen.height-100, 200, 30), "Ammo: " + weapon.bulletsLeft + " / " + weapon.clips);
		}
	}
	
	public void SelectWeapon(int index){
		for(int i=0; i<manager.allWeapons.Count; i++){
		// Activate the selected weapon
		if(i == index){
				manager.allWeapons[i].gameObject.SetActive(true);
				weapon = GetComponentInChildren<BaseWeapon>();
			} else {
				// Deactivate all other weapons
				if(manager.allWeapons[i] != null){
					manager.allWeapons[i].isReloading = false;
				}
				manager.allWeapons[i].gameObject.SetActive(false);
			}
		}
	}
	
	public void UpdateWeaponsSlots(){
		weaponSlots[0] = manager.equippedWeapons[0];
		weaponSlots[1] = manager.equippedWeapons[1];
		weaponSlots[2] = manager.equippedWeapons[2];
		weaponSlots[3] = manager.equippedWeapons[3];
	}
}
