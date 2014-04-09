﻿using UnityEngine;
using System.Collections;

public class MainPanelGUI : MonoBehaviour {
	
	public Texture2D emblem;
	public Texture2D frameRight;
	public Texture2D frameBottom;
	public Texture2D resourceFrame;
	
	public Texture2D buildButtonNormal;
	public Texture2D buildButtonHover;
	public Texture2D buildButtonActive;
	
	public Texture2D weaponsButtonNormal;
	public Texture2D weaponsButtonHover;
	public Texture2D weaponsButtonActive;
	
	public Texture2D abilitiesButtonNormal;
	public Texture2D abilitiesButtonHover;
	public Texture2D abilitiesButtonActive;
	
	public Texture2D beginButtonNormal;
	public Texture2D beginButtonHover;
	public Texture2D beginButtonActive;
	
	public Texture2D buildTextHover;
	public Texture2D buildTextActive;
	
	public Texture2D weaponsTextHover;
	public Texture2D weaponsTextActive;
	
	public Texture2D abilitiesTextHover;
	public Texture2D abilitiesTextActive;
	
	public Texture2D beginTextHover;
	public Texture2D beginTextActive;
	
	public Texture2D backGround;
	
	public Texture2D buttonNormal;
	public Texture2D buttonHover;
	public Texture2D buttonActive;
	
	private WeaponSelection selection;
	private Tutorial tut;
	
	private int buttonWidth = 200;
	private int buttonHeight = 50;
	
	private int labelWidth = 250;
	private int labelHeight = 200;
	
	private int offsetWidth = 256;
	private int offsetHeight = 80;
	
	public Texture2D header;
	public Texture2D[] fortNames;
	
	public Font font;
	
	public float width, height, x, y;
	
	void Start(){
		Reset();
	}
	
	public void Reset(){
		selection = GameController.Instance.GetPlayer().GetComponentInChildren<WeaponSelection>();
		tut = GameObject.Find("Tutorial").GetComponent<Tutorial>();
	}
	
	public void Draw(Rect drawRect, Wave buildWave){
		
		GUI.DrawTexture(new Rect(Screen.width-110, Screen.height-(Screen.height-2), 90, 90), emblem);
		GUI.DrawTexture(new Rect(Screen.width-128, Screen.height-(Screen.height-65), 128, 365), frameRight);
		GUI.DrawTexture(new Rect(Screen.width-256, Screen.height-64, 256, 64), frameBottom);
		GUI.DrawTexture(new Rect(Screen.width-365, Screen.height-(Screen.height-1), 256, 128), resourceFrame);
		
		GUIStyle style = new GUIStyle();
		style.font = font;
		style.alignment = TextAnchor.MiddleCenter;
		style.normal.textColor = Color.cyan;
		style.fontSize = 25;
		GUI.Label(new Rect(Screen.width-260, Screen.height-(Screen.height-28), 0, 0), "RESOURCES", style);
		GUI.Label(new Rect(Screen.width-245, Screen.height-(Screen.height-75), 0, 0), GameController.Instance.GetResources().ToString(), style);
			
		GUIContent content = new GUIContent();
		
		GUIStyle buildButtonstyle = new GUIStyle();
		buildButtonstyle.normal.background = buildButtonNormal;
		buildButtonstyle.hover.background = buildButtonHover;
		buildButtonstyle.active.background = buildButtonActive;
		buildButtonstyle.alignment = TextAnchor.MiddleCenter;
		
		GUIStyle weaponButtonstyle = new GUIStyle();
		weaponButtonstyle.normal.background = weaponsButtonNormal;
		weaponButtonstyle.hover.background = weaponsButtonHover;
		weaponButtonstyle.active.background = weaponsButtonActive;
		weaponButtonstyle.alignment = TextAnchor.MiddleCenter;
		
		GUIStyle abilitiesButtonstyle = new GUIStyle();
		abilitiesButtonstyle.normal.background = abilitiesButtonNormal;
		abilitiesButtonstyle.hover.background = abilitiesButtonHover;
		abilitiesButtonstyle.active.background = abilitiesButtonActive;
		abilitiesButtonstyle.alignment = TextAnchor.MiddleCenter;
		
		GUIStyle beginButtonstyle = new GUIStyle();
		beginButtonstyle.normal.background = beginButtonNormal;
		beginButtonstyle.hover.background = beginButtonHover;
		beginButtonstyle.active.background = beginButtonActive;
		beginButtonstyle.alignment = TextAnchor.MiddleCenter;
		
		Rect buildRect = new Rect(Screen.width-100, Screen.height-(Screen.height-125), 64, 64);
		Rect weaponRect = new Rect(Screen.width-100, Screen.height-(Screen.height-205), 64, 64);
		Rect abilitiesRect = new Rect(Screen.width-100, Screen.height-(Screen.height-285), 64, 64);
		Rect beginRect = new Rect(Screen.width-100, Screen.height-(Screen.height-365), 64, 64);
		
		if(tut.key == "BuildScreen" || tut.key == ""){
			if(GUI.Button(buildRect, content, buildButtonstyle)){
				if(UIManager.Instance.uiState != UIManager.UIState.FORT_BUILD_SCREEN){
					UIManager.Instance.uiState = UIManager.UIState.FORT_BUILD_SCREEN;
				} else {
					UIManager.Instance.uiState = UIManager.UIState.NONE;
				}
			} 
		}
		if(tut.key == "WeaponScreen" || tut.key == ""){
			if(GUI.Button(weaponRect, content, weaponButtonstyle)){
				if(UIManager.Instance.uiState != UIManager.UIState.FORT_WEAPON_SCREEN){
					UIManager.Instance.uiState = UIManager.UIState.FORT_WEAPON_SCREEN;
				} else {
					UIManager.Instance.uiState = UIManager.UIState.NONE;
				}
			}
		}
		if(tut.key == "AbilityScreen" || tut.key == ""){
			if(GUI.Button(abilitiesRect, content, abilitiesButtonstyle)){
				if(UIManager.Instance.uiState != UIManager.UIState.FORT_ABILITY_SCREEN){
					UIManager.Instance.uiState = UIManager.UIState.FORT_ABILITY_SCREEN;
				} else {
					UIManager.Instance.uiState = UIManager.UIState.NONE;
				}
			}
		}
		if(tut.key == ""){
			if(GUI.Button(beginRect, content, beginButtonstyle)){
				UIManager.Instance.uiState = UIManager.UIState.NONE;
				UIManager.Instance.displayUI = true;
				GameObject.Find("GridContainer").GetComponent<GridSpawner>().DisableGrid();
				selection.UpdateWeaponsSlots();
				GameController.Instance.canShoot = true;
				GameController.Instance.canChangeWeapons = true;
				GameController.Instance.UpdateGraph();
				foreach(AmmoVendor vendor in GameObject.Find("Vendor").GetComponent<AmmoVendorContainer>().ammoVendors){
					vendor.Cancel();
				}
				Destroy(GameController.Instance.current);
				GameController.Instance.current = null;
				buildWave.BeginWave();
				Destroy(GameObject.Find("WaveController").GetComponent<Fortification>());
			}
		}
		
		if(UIManager.Instance.uiState != UIManager.UIState.FORT_BUILD_SCREEN){
			if(buildRect.Contains(Event.current.mousePosition)){
				GUI.DrawTexture(new Rect(Screen.width-360, Screen.height-(Screen.height-125), 256, 64), buildTextHover);
			}
		} else {
			GUI.DrawTexture(new Rect(Screen.width-360, Screen.height-(Screen.height-125), 256, 64), buildTextActive);
		}
		
		if(UIManager.Instance.uiState != UIManager.UIState.FORT_WEAPON_SCREEN){
			if(weaponRect.Contains(Event.current.mousePosition)){
				GUI.DrawTexture(new Rect(Screen.width-360, Screen.height-(Screen.height-205), 256, 64), weaponsTextHover);
			}
		} else {
			GUI.DrawTexture(new Rect(Screen.width-360, Screen.height-(Screen.height-205), 256, 64), weaponsTextActive);
		}
		
		if(UIManager.Instance.uiState != UIManager.UIState.FORT_ABILITY_SCREEN){
			if(abilitiesRect.Contains(Event.current.mousePosition)){
				GUI.DrawTexture(new Rect(Screen.width-360, Screen.height-(Screen.height-285), 256, 64), abilitiesTextHover);
			}
		} else {
			GUI.DrawTexture(new Rect(Screen.width-360, Screen.height-(Screen.height-285), 256, 64), abilitiesTextActive);
		}
		
		if(beginRect.Contains(Event.current.mousePosition)){
			GUI.DrawTexture(new Rect(Screen.width-360, Screen.height-(Screen.height-365), 256, 64), beginTextHover);
		}
		
		
		/*Rect drawArea = drawRect;
		
		GUI.BeginGroup(drawArea);
		
		GUI.DrawTexture(new Rect(0, 0, drawArea.width, drawArea.height), backGround);
		
		GUI.Label(new Rect(offsetWidth-50, offsetHeight-15, labelWidth, labelHeight), header);
		
		for(int i=0; i<fortNames.Length; i++){
			Texture2D curFortName = fortNames[i];
			
			GUI.BeginGroup(new Rect(0, i * offsetHeight+offsetHeight, drawArea.width, drawArea.height));
			
			GUIStyle style = new GUIStyle();
			style.normal.background = buttonNormal;
			style.hover.background = buttonHover;
			style.active.background = buttonActive;
			style.alignment = TextAnchor.MiddleCenter;
			
			if(GameController.Instance.GetWaveController().GetWaveNumber() == 2){
				Tutorial tut = GameObject.Find("Tutorial").GetComponent<Tutorial>();
				if(GUI.Button(new Rect(offsetWidth, offsetHeight, buttonWidth, buttonHeight), curFortName, style)){
					if(tut.key == "BuildScreen"){
						if(curFortName == fortNames[0] && UIManager.Instance.uiState != UIManager.UIState.FORT_BUILD_SCREEN){
							UIManager.Instance.uiState = UIManager.UIState.FORT_BUILD_SCREEN;
						}
					} else if(tut.key == "WeaponScreen"){
						if(curFortName == fortNames[1] && UIManager.Instance.uiState != UIManager.UIState.FORT_WEAPON_SCREEN){
							UIManager.Instance.uiState = UIManager.UIState.FORT_WEAPON_SCREEN;
						}
					} else if(tut.key == "AbilityScreen"){
						if(curFortName == fortNames[2] && UIManager.Instance.uiState != UIManager.UIState.FORT_ABILITY_SCREEN){
							UIManager.Instance.uiState = UIManager.UIState.FORT_ABILITY_SCREEN;
						}
					} else if(tut.key == ""){
						if(curFortName == fortNames[0] && UIManager.Instance.uiState != UIManager.UIState.FORT_BUILD_SCREEN){
							UIManager.Instance.uiState = UIManager.UIState.FORT_BUILD_SCREEN;
						} else if(curFortName == fortNames[1] && UIManager.Instance.uiState != UIManager.UIState.FORT_WEAPON_SCREEN){
							UIManager.Instance.uiState = UIManager.UIState.FORT_WEAPON_SCREEN;
						} else if(curFortName == fortNames[2] && UIManager.Instance.uiState != UIManager.UIState.FORT_ABILITY_SCREEN){
							UIManager.Instance.uiState = UIManager.UIState.FORT_ABILITY_SCREEN;
						} else if(curFortName == fortNames[3]){
							UIManager.Instance.uiState = UIManager.UIState.NONE;
							UIManager.Instance.displayUI = true;
							GameObject.Find("GridContainer").GetComponent<GridSpawner>().DisableGrid();
							selection.UpdateWeaponsSlots();
							//selection.SelectWeapon(selection.weaponSlots[0].GetComponent<BaseWeapon>().id);
							GameController.Instance.canShoot = true;
							GameController.Instance.canChangeWeapons = true;
							GameController.Instance.UpdateGraph();
							foreach(AmmoVendor vendor in GameObject.Find("Vendor").GetComponent<AmmoVendorContainer>().ammoVendors){
								vendor.Cancel();
							}
							foreach(AbilityAmmoVendor vendor in GameObject.Find("Vendor").GetComponent<AbilityAmmoVendorContainer>().abilityAmmoVendor){
								vendor.Cancel();
							}
							Destroy(GameController.Instance.current);
							GameController.Instance.current = null;
							buildWave.BeginWave();
							Destroy(GameObject.Find("WaveController").GetComponent<Fortification>());
						}
					}
				}
			} else {
				if(GUI.Button(new Rect(offsetWidth, offsetHeight, buttonWidth, buttonHeight), curFortName, style)){
					if(curFortName == fortNames[0] && UIManager.Instance.uiState != UIManager.UIState.FORT_BUILD_SCREEN){
						UIManager.Instance.uiState = UIManager.UIState.FORT_BUILD_SCREEN;
					} else if(curFortName == fortNames[1] && UIManager.Instance.uiState != UIManager.UIState.FORT_WEAPON_SCREEN){
						UIManager.Instance.uiState = UIManager.UIState.FORT_WEAPON_SCREEN;
					} else if(curFortName == fortNames[2] && UIManager.Instance.uiState != UIManager.UIState.FORT_ABILITY_SCREEN){
						UIManager.Instance.uiState = UIManager.UIState.FORT_ABILITY_SCREEN;
					} else if(curFortName == fortNames[3]){
						UIManager.Instance.uiState = UIManager.UIState.NONE;
						UIManager.Instance.displayUI = true;
						GameObject.Find("GridContainer").GetComponent<GridSpawner>().DisableGrid();
						selection.UpdateWeaponsSlots();
						//selection.SelectWeapon(selection.weaponSlots[0].GetComponent<BaseWeapon>().id);
						GameController.Instance.canShoot = true;
						GameController.Instance.canChangeWeapons = true;
						GameController.Instance.UpdateGraph();
						foreach(AmmoVendor vendor in GameObject.Find("Vendor").GetComponent<AmmoVendorContainer>().ammoVendors){
							vendor.Cancel();
						}
						Destroy(GameController.Instance.current);
						GameController.Instance.current = null;
						buildWave.BeginWave();
						Destroy(GameObject.Find("WaveController").GetComponent<Fortification>());
					}
				}
			}
			GUI.EndGroup();
		}
		GUI.EndGroup();*/
	}
}
