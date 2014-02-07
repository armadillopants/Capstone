using System.Collections;
using UnityEngine;

public class Fortification : MonoBehaviour {
	
	private Wave buildWave;
	private float timer = 60f;
	
	private MainPanelGUI mainPanel;
	private BuildPanelGUI buildPanel;
	private WeaponPanelGUI weaponPanel;
	private AbilityPanelGUI abilityPanel;
	private BuildUpgradeGUI buildUpgradePanel;
	
	private WeaponSelection selection;

	void Awake(){
		GameController.Instance.canShoot = false;
		GameController.Instance.canChangeWeapons = false;
		UIManager.Instance.displayUI = false;
		
		GameObject vendor = GameObject.Find("Vendor");
		
		weaponPanel = vendor.GetComponent<WeaponPanelGUI>();
		mainPanel = vendor.GetComponent<MainPanelGUI>();
		buildPanel = vendor.GetComponent<BuildPanelGUI>();
		abilityPanel = vendor.GetComponent<AbilityPanelGUI>();
		buildUpgradePanel = vendor.GetComponent<BuildUpgradeGUI>();
		
		selection = GameController.Instance.GetPlayer().GetComponentInChildren<WeaponSelection>();
		selection.changingWeapons = false;
		selection.drawWeapon = false;
	}
	
	public void StartFortifying(Wave wave){
		buildWave = wave;
	}
	
	void Update(){
		FortifyHandling();
	}
	
	void FortifyHandling(){
		buildWave.StopWave(); // Stops the wave
		
		if(buildWave.GetWaveNumber() == 2 && GameObject.Find("Tutorial").GetComponent<Tutorial>().key != ""){
			timer = 60f;
		} else {
			timer -= Time.deltaTime;
		}
		
		if(timer <= 0){
			UIManager.Instance.uiState = UIManager.UIState.NONE;
			UIManager.Instance.displayUI = true;
			GameObject.Find("GridContainer").GetComponent<GridSpawner>().DisableGrid();
			selection.UpdateWeaponsSlots();
			selection.SelectWeapon(selection.weaponSlots[0].GetComponent<BaseWeapon>().id);
			GameController.Instance.canShoot = true;
			GameController.Instance.canChangeWeapons = true;
			GameController.Instance.UpdateGraph();
			foreach(AmmoVendor vendor in GameObject.Find("Vendor").GetComponent<AmmoVendorContainer>().ammoVendors){
				vendor.Cancel();
			}
			Destroy(GameController.Instance.current);
			GameController.Instance.current = null;
			buildWave.BeginWave();
			Destroy(this);
		}
	}
	
	void OnGUI(){
		if(GameController.Instance.current == null){
			mainPanel.Draw(new Rect(Screen.width-512, Screen.height-(Screen.height-16), 512, 512), buildWave);
		}
		
		Rect timerRect = new Rect(100,50,100,50);
		GUI.BeginGroup(timerRect);
		
		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.MiddleCenter;
		style.normal.textColor = Color.white;
		style.font = UIManager.Instance.resourceFont;
		style.fontSize = 50;
		GUI.Label(new Rect(0, 0, timerRect.width, timerRect.height), GuiTime(timer), style);
		
		GUI.EndGroup();
		
		switch(UIManager.Instance.uiState){
		case UIManager.UIState.FORT_BUILD_SCREEN:
			DrawFortBuyScreen();
			break;
		case UIManager.UIState.FORT_WEAPON_SCREEN:
			DrawWeaponBuyScreen();
			break;
		case UIManager.UIState.FORT_ABILITY_SCREEN:
			DrawAbilityBuyScreen();
			break;
		case UIManager.UIState.FORT_UPGRADE_SCREEN:
			DrawFortUpgradeScreen();
			break;
		}
	}
	
	void DrawFortBuyScreen(){
		buildPanel.Draw(new Rect(Screen.width-750, Screen.height-(Screen.height-200), 400, 500));
	}
	
	void DrawWeaponBuyScreen(){
		weaponPanel.Draw(new Rect(Screen.width-750, Screen.height-(Screen.height-200), 400, 500));
	}
	
	void DrawAbilityBuyScreen(){
		abilityPanel.Draw(new Rect(Screen.width-750, Screen.height-(Screen.height-200), 400, 500));
	}

	void DrawFortUpgradeScreen(){
		buildUpgradePanel.Draw(new Rect((Screen.width/2f) - (200/2), (Screen.height/2f) - (300/2), 200, 300));
	}
	
	string GuiTime(float time){
		float guiTime = time;
		int seconds = (int)(guiTime % 60); // Creates 00 for seconds
		string text = ""; // For displaying the the timer in sec
	    text = string.Format("{0:00}", seconds);
	    return text;
	}
}
