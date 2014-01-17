using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelection : MonoBehaviour {
	
	private Vector2 buttonSize = new Vector2(120, 40);
	
	public bool changingWeapons = false;
	public bool drawWeapon = false;
	private float slowmoTime = 2f;
	public List<GameObject> weaponSlots = new List<GameObject>();
	private WeaponManager manager;
	private BaseWeapon weapon;
	public Texture2D weaponEquipped;
	public Texture2D weaponUnEquipped;
	private Rect[] weaponDisplayRect = new Rect[4];
	
	void Awake(){
		GameObject player = GameObject.FindWithTag(Globals.PLAYER);
		manager = player.GetComponentInChildren<WeaponManager>();
		weaponDisplayRect[0] = new Rect((Screen.width/2 - (buttonSize.x/2))-200, Screen.height/2 - (buttonSize.y/2), buttonSize.x, buttonSize.y);
		weaponDisplayRect[1] = new Rect((Screen.width/2 - (buttonSize.x/2))+200, Screen.height/2 - (buttonSize.y/2), buttonSize.x, buttonSize.y);
		weaponDisplayRect[2] = new Rect(Screen.width/2 - (buttonSize.x/2), (Screen.height/2 - (buttonSize.y/2))-200, buttonSize.x, buttonSize.y);
		weaponDisplayRect[3] = new Rect(Screen.width/2 - (buttonSize.x/2), (Screen.height/2 - (buttonSize.y/2))+200, buttonSize.x, buttonSize.y);
		UpdateWeaponsSlots();
	}
	
	void Start(){
		drawWeapon = true;
		// Choose first weapon
		SelectWeapon(weaponSlots[0].GetComponent<BaseWeapon>().id);
		weapon = GetComponentInChildren<BaseWeapon>();
	}
	
	void Update(){
		if(weapon.isAutomatic){
			if(Input.GetButton("Fire1") && GameController.Instance.canShoot){
				weapon.isFiring = true;
				BroadcastMessage("Fire");
			} else {
				weapon.isFiring = false;
			}
		} else {
			if(Input.GetButtonDown("Fire1") && GameController.Instance.canShoot){
				weapon.isFiring = true;
				BroadcastMessage("Fire");
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
			
			if(drawWeapon){
				StartCoroutine("DrawWeapon");
			}
		}
	}
	
	private IEnumerator DrawWeapon(){
		yield return new WaitForSeconds(2f);
		drawWeapon = false;
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
			for(int i=0; i<weaponSlots.Count; i++){
				GUIStyle selectedWeaponStyle = new GUIStyle();
				selectedWeaponStyle.font = UIManager.Instance.resourceFont;
				selectedWeaponStyle.fontSize = 12;
				selectedWeaponStyle.alignment = TextAnchor.MiddleCenter;
				selectedWeaponStyle.normal.textColor = Color.white;
				if(weaponSlots[i] != null){
					if(weaponSlots[i].gameObject.activeSelf){
						selectedWeaponStyle.normal.background = weaponEquipped;
					} else {
						selectedWeaponStyle.normal.background = weaponUnEquipped;
					}
					
					// Display 4 GUI buttons: left, right, top, bottom
					if(GUI.Button(weaponDisplayRect[i], weaponSlots[i].name, selectedWeaponStyle)){
						SelectWeapon(weaponSlots[i].GetComponent<BaseWeapon>().id);
						drawWeapon = true;
					}
				}
			}
		}
		
		if(GameController.Instance.GetPlayer().GetComponent<PlayerMovement>() != null){
			if(UIManager.Instance.displayUI){
			GUI.Box(new Rect(Screen.width-200, Screen.height-100, 200, 30), "Ammo: " + weapon.bulletsLeft + " / " + weapon.clips);
			}
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
