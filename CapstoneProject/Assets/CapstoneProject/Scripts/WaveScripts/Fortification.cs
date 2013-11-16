using UnityEngine;
using System.Collections;

public class Fortification : MonoBehaviour {
	
	private Wave buildWave;
	private WeaponSelection selection;
	private WeaponManager manager;
	private float infinity = Mathf.Infinity;
	private Rect mainScreen = new Rect(Screen.width-250, Screen.height-(Screen.height-16), 200, 500);
	private Rect displayScreen = new Rect(Screen.width/3, Screen.height/5, 600, 600);
	
	private WeaponVendor weaponVendor;
	private ItemVendor itemVendor;
	private AmmoVendorContainer ammoContainer;

	void Awake(){
		GameObject player = GameController.Instance.GetPlayer().gameObject;
		selection = player.GetComponentInChildren<WeaponSelection>();
		GameController.Instance.canShoot = false;
		GameController.Instance.canChangeWeapons = false;
		manager = player.GetComponentInChildren<WeaponManager>();
		
		GameObject vendor = GameObject.Find("Vendor");
		weaponVendor = vendor.GetComponent<WeaponVendor>();
		itemVendor = vendor.GetComponent<ItemVendor>();
		ammoContainer = vendor.GetComponent<AmmoVendorContainer>();
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
		switch(UIManager.Instance.uiState){
		case UIManager.UIState.BUILD_SCREEN:
			DrawFortBuyScreen();
			break;
		case UIManager.UIState.BUY_SCREEN:
			DrawWeaponBuyScreen();
			break;
		case UIManager.UIState.UPGRADE_SCREEN:
			DrawWeaponUpgradeScreen();
			break;
		case UIManager.UIState.EQUIP_WEAPON_SCREEN:
			DrawEquipWeaponScreen();
			break;
		case UIManager.UIState.FORT_UPGRADE_SCREEN:
			DrawFortUpgradeScreen();
			break;
		}
	}
	
	void DrawMainScreen(){
		GUI.BeginGroup(mainScreen);
		
		GUI.Box(new Rect(0, 0, mainScreen.width, mainScreen.height), "Fortifications");
		string[] fortNames = new string[5]{"Fortify", "Weapons", "Upgrades", "Equip Weapons", "Begin Wave"};
		for(int i=0; i<fortNames.Length; i++){
			string curFortName = fortNames[i];
			if(GUI.Button(new Rect(mainScreen.width/4, mainScreen.height/10+(i*100), 100, 50), curFortName)){
				if(curFortName == fortNames[0] && UIManager.Instance.uiState != UIManager.UIState.BUILD_SCREEN){
					weaponVendor.Cancel();
					weaponVendor.CancelUpgrades();
					UIManager.Instance.uiState = UIManager.UIState.BUILD_SCREEN;
				} else if(curFortName == fortNames[1] && UIManager.Instance.uiState != UIManager.UIState.BUY_SCREEN){
					itemVendor.Cancel();
					weaponVendor.CancelUpgrades();
					UIManager.Instance.uiState = UIManager.UIState.BUY_SCREEN;
				} else if(curFortName == fortNames[2] && UIManager.Instance.uiState != UIManager.UIState.UPGRADE_SCREEN){
					itemVendor.Cancel();
					weaponVendor.Cancel();
					UIManager.Instance.uiState = UIManager.UIState.UPGRADE_SCREEN;
				} else if(curFortName == fortNames[3] && UIManager.Instance.uiState != UIManager.UIState.EQUIP_WEAPON_SCREEN){
					itemVendor.Cancel();
					weaponVendor.Cancel();
					weaponVendor.CancelUpgrades();
					UIManager.Instance.uiState = UIManager.UIState.EQUIP_WEAPON_SCREEN;
				} else if(curFortName == fortNames[4]){
					UIManager.Instance.uiState = UIManager.UIState.NONE;
					GameObject.Find("GridContainer").GetComponent<GridSpawner>().DisableGrid();
					selection.UpdateWeaponsSlots();
					selection.SelectWeapon(selection.weaponSlots[0].GetComponent<BaseWeapon>().id);
					GameController.Instance.canShoot = true;
					GameController.Instance.canChangeWeapons = true;
					SellableItemDisplayer display = GameObject.Find("ItemDisplayer").GetComponent<SellableItemDisplayer>();
					display.Purge();
					GameController.Instance.UpdateGraph();
					buildWave.BeginWave();
					Destroy(this);
				}
			}
		}
		
		GUI.EndGroup();
	}
	
	void DrawFortBuyScreen(){
		GUI.BeginGroup(displayScreen);
		
		GUI.Box(new Rect(0, 0, displayScreen.width, displayScreen.height), "Build Screen");
		itemVendor.Vendor(displayScreen.x, displayScreen.y);
		
		GUI.EndGroup();
	}
	
	void DrawWeaponBuyScreen(){
		GUI.BeginGroup(displayScreen);
		
		GUI.Box(new Rect(0, 0, displayScreen.width, displayScreen.height), "Weapons Screen");
		weaponVendor.Vendor(displayScreen.x, displayScreen.y);
		
		GUI.EndGroup();
	}
	
	void DrawWeaponUpgradeScreen(){
		GUI.BeginGroup(displayScreen);
		
		GUI.Box(new Rect(0, 0, displayScreen.width, displayScreen.height), "Upgrade Screen");
		weaponVendor.UpgradeVendor(displayScreen.x, displayScreen.y);
		
		GUI.EndGroup();
	}

	void DrawFortUpgradeScreen(){
		GUI.BeginGroup(displayScreen);
		
		GUI.Box(new Rect(0, 0, displayScreen.width, displayScreen.height), "Upgrade Screen");
		itemVendor.UpgradeVendor(displayScreen.x, displayScreen.y);
		
		GUI.EndGroup();
	}

	void DrawEquipWeaponScreen(){
		// Left
		if(manager.equippedWeapons[0]){
			GUI.Box(new Rect(Screen.width/3.0f,Screen.height/2.1f,120,40), manager.equippedWeapons[0].name);
			ammoContainer.ammoVendors[0].SetWeapon(manager.equippedWeapons[0]);
			ammoContainer.ammoVendors[0].Vendor(Screen.width/4.6f,Screen.height/2.1f);
		} else {
			GUI.Box(new Rect(Screen.width/3.0f,Screen.height/2.1f,120,40), "Rifle Slot");
		}
		for(int i=0; i<manager.rifleWeapons.Count; i++){
			if(GUI.Button(new Rect(Screen.width/3.0f, Screen.height/1.9f+(i*50), 120, 40), manager.rifleWeapons[i].name)){
				manager.equippedWeapons[0] = manager.rifleWeapons[i];
				ammoContainer.ammoVendors[0].Cancel();
			}
		}
		
		// Right
		if(manager.equippedWeapons[1]){
			GUI.Box(new Rect(Screen.width/1.7f,Screen.height/2.1f,120,40), manager.equippedWeapons[1].name);
			ammoContainer.ammoVendors[1].SetWeapon(manager.equippedWeapons[1]);
			ammoContainer.ammoVendors[1].Vendor(Screen.width/1.5f,Screen.height/2.1f);
		} else {
			GUI.Box(new Rect(Screen.width/1.7f,Screen.height/2.1f,120,40), "Pistol Slot");
		}
		for(int i=0; i<manager.pistolWeapons.Count; i++){
			if(GUI.Button(new Rect(Screen.width/1.7f, Screen.height/2.3f-(i*50), 120, 40), manager.pistolWeapons[i].name)){
				manager.equippedWeapons[1] = manager.pistolWeapons[i];
				ammoContainer.ammoVendors[1].Cancel();
			}
		}
		
		// Top
		if(manager.equippedWeapons[2]){
			GUI.Box(new Rect(Screen.width/2.17f,Screen.height/3.2f,120,40), manager.equippedWeapons[2].name);
			ammoContainer.ammoVendors[2].SetWeapon(manager.equippedWeapons[2]);
			ammoContainer.ammoVendors[2].Vendor(Screen.width/3.0f,Screen.height/3.2f);
		} else {
			GUI.Box(new Rect(Screen.width/2.17f,Screen.height/3.2f,120,40), "Launcher Slot");
		}
		for(int i=0; i<manager.launcherWeapons.Count; i++){
			if(GUI.Button(new Rect(Screen.width/2.17f, Screen.height/3.8f-(i*50), 120, 40), manager.launcherWeapons[i].name)){
				manager.equippedWeapons[2] = manager.launcherWeapons[i];
				ammoContainer.ammoVendors[2].Cancel();
			}
		}
		
		// Bottom
		if(manager.equippedWeapons[3]){
			GUI.Box(new Rect(Screen.width/2.17f,Screen.height/1.6f,120,40), manager.equippedWeapons[3].name);
			ammoContainer.ammoVendors[3].SetWeapon(manager.equippedWeapons[3]);
			ammoContainer.ammoVendors[3].Vendor(Screen.width/1.9f,Screen.height/1.6f);
		} else {
			GUI.Box(new Rect(Screen.width/2.17f,Screen.height/1.6f,120,40), "Special Slot");
		}
		for(int i=0; i<manager.specialWeapons.Count; i++){
			if(GUI.Button(new Rect(Screen.width/2.17f, Screen.height/1.5f+(i*50), 120, 40), manager.specialWeapons[i].name)){
				manager.equippedWeapons[3] = manager.specialWeapons[i];
				ammoContainer.ammoVendors[3].Cancel();
			}
		}
	}
}
