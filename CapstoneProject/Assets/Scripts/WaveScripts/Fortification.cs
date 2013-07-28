using UnityEngine;
using System.Collections;

public class Fortification : MonoBehaviour {
	
	public enum FortState { MAIN_SCREEN, BUILD_SCREEN, UPGRADE_SCREEN, BUY_SCREEN, EQUIP_WEAPON_SCREEN };
	private FortState state = FortState.MAIN_SCREEN;
	private Wave buildWave;
	private WeaponSelection selection;
	private WeaponManager manager;
	private float infinity = Mathf.Infinity;
	private Rect mainScreen = new Rect(Screen.width-250, Screen.height-(Screen.height-16), 200, 500);
	private Rect displayScreen = new Rect(Screen.width/3, Screen.height/5, 500, 500);
	
	private WeaponVendor weaponVendor;
	private ItemVendor itemVendor;

	void Awake(){
		GameObject player = GameObject.FindWithTag("Player");
		selection = player.GetComponentInChildren<WeaponSelection>();
		selection.canShoot = false;
		manager = player.GetComponentInChildren<WeaponManager>();
		
		GameObject vendor = GameObject.Find("Vendor");
		weaponVendor = vendor.GetComponent<WeaponVendor>();
		itemVendor = vendor.GetComponent<ItemVendor>();
	}
	
	void Update(){
		selection.canShoot = false;
	}
	
	public void StartFortifying(Wave wave){
		buildWave = wave;
		StartCoroutine("FortifyHandling");
	}
	
	IEnumerator FortifyHandling(){
		buildWave.StopWave(); // Stops the wave
		// And displays the fortification screen
		yield return new WaitForSeconds(infinity); // Allows unlimited amount of time to select fortifications
	}
	
	void OnGUI(){
		DrawMainScreen();
		if(GameController.Instance.canDisplay){
			switch(state){
			case FortState.BUILD_SCREEN:
				DrawBuildScreen();
				break;
			case FortState.UPGRADE_SCREEN:
				DrawUpgradeScreen();
				break;
			case FortState.BUY_SCREEN:
				DrawBuyScreen();
				break;
			case FortState.EQUIP_WEAPON_SCREEN:
				DrawEquipWeaponScreen();
				break;
			}
		}
	}
	
	void DrawMainScreen(){
		GUI.BeginGroup(mainScreen);
		
		GUI.Box(new Rect(0, 0, mainScreen.width, mainScreen.height), "Fortifications");
		string[] fortNames = new string[5]{"Fortify", "Weapons", "Upgrades", "Equip Weapons", "Begin Wave"};
		for(int i=0; i<fortNames.Length; i++){
			string curFortName = fortNames[i];
			if(GUI.Button(new Rect(mainScreen.width/4, mainScreen.height/10+(i*100), 100, 50), curFortName)){
				GameController.Instance.canDisplay = true;
				if(curFortName == fortNames[0]){
					weaponVendor.Cancel();
					state = FortState.BUILD_SCREEN;
				} else if(curFortName == fortNames[1]){
					itemVendor.Cancel();
					state = FortState.BUY_SCREEN;
				} else if(curFortName == fortNames[2]){
					weaponVendor.Cancel();
					state = FortState.UPGRADE_SCREEN;
				} else if(curFortName == fortNames[3]){
					state = FortState.EQUIP_WEAPON_SCREEN;
				} else {
					selection.UpdateWeaponsSlots();
					selection.SelectWeapon(selection.weaponSlots[0].GetComponent<BaseWeapon>().id);
					selection.canShoot = true;
					SellableItemDisplayer display = GameObject.Find("ItemDisplayer").GetComponent<SellableItemDisplayer>();
					display.Purge();
					buildWave.BeginWave();
					Destroy(this);
				}
			}
		}
		
		GUI.EndGroup();
	}
	
	void DrawBuildScreen(){
		GUI.BeginGroup(displayScreen);
		
		GUI.Box(new Rect(0, 0, displayScreen.width, displayScreen.height), "Build Screen");
		itemVendor.Vendor(displayScreen.x, displayScreen.y);
		
		GUI.EndGroup();
	}
	
	void DrawBuyScreen(){
		GUI.BeginGroup(displayScreen);
		
		GUI.Box(new Rect(0, 0, displayScreen.width, displayScreen.height), "Weapons Screen");
		weaponVendor.Vendor(displayScreen.x, displayScreen.y);
		
		GUI.EndGroup();
	}
	
	void DrawUpgradeScreen(){
		GUI.BeginGroup(displayScreen);
		
		GUI.Box(new Rect(0, 0, displayScreen.width, displayScreen.height), "Upgrade Screen");
		weaponVendor.UpgradeVendor(displayScreen.x, displayScreen.y);
		
		GUI.EndGroup();
	}

	void DrawEquipWeaponScreen(){
		// Left
		if(manager.equippedWeapons[0]){
			GUI.Box(new Rect(Screen.width/3.0f,Screen.height/2.1f,120,40), manager.equippedWeapons[0].name);
		} else {
			GUI.Box(new Rect(Screen.width/3.0f,Screen.height/2.1f,120,40), "Rifle Slot");
		}
		for(int i=0; i<manager.rifleWeapons.Count; i++){
			if(GUI.Button(new Rect(Screen.width/3.0f, Screen.height/1.9f+(i*50), 120, 40), manager.rifleWeapons[i].name)){
				manager.equippedWeapons[0] = manager.rifleWeapons[i];
			}
		}
		
		// Right
		if(manager.equippedWeapons[1]){
			GUI.Box(new Rect(Screen.width/1.7f,Screen.height/2.1f,120,40), manager.equippedWeapons[1].name);
		} else {
			GUI.Box(new Rect(Screen.width/1.7f,Screen.height/2.1f,120,40), "Pistol Slot");
		}
		for(int i=0; i<manager.pistolWeapons.Count; i++){
			if(GUI.Button(new Rect(Screen.width/1.7f, Screen.height/2.3f-(i*50), 120, 40), manager.pistolWeapons[i].name)){
				manager.equippedWeapons[1] = manager.pistolWeapons[i];
			}
		}
		
		// Top
		if(manager.equippedWeapons[2]){
			GUI.Box(new Rect(Screen.width/2.17f,Screen.height/3.2f,120,40), manager.equippedWeapons[2].name);
		} else {
			GUI.Box(new Rect(Screen.width/2.17f,Screen.height/3.2f,120,40), "Launcher Slot");
		}
		for(int i=0; i<manager.launcherWeapons.Count; i++){
			if(GUI.Button(new Rect(Screen.width/2.17f, Screen.height/3.8f-(i*50), 120, 40), manager.launcherWeapons[i].name)){
				manager.equippedWeapons[2] = manager.launcherWeapons[i];
			}
		}
		
		// Bottom
		if(manager.equippedWeapons[3]){
			GUI.Box(new Rect(Screen.width/2.17f,Screen.height/1.6f,120,40), manager.equippedWeapons[3].name);
		} else {
			GUI.Box(new Rect(Screen.width/2.17f,Screen.height/1.6f,120,40), "Special Slot");
		}
		for(int i=0; i<manager.specialWeapons.Count; i++){
			if(GUI.Button(new Rect(Screen.width/2.17f, Screen.height/1.5f+(i*50), 120, 40), manager.specialWeapons[i].name)){
				manager.equippedWeapons[3] = manager.specialWeapons[i];
			}
		}
	}
}
