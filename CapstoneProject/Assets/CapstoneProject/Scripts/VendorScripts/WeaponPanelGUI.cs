using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPanelGUI : MonoBehaviour {
	
  	private int labelOffset = 10;
	
	// Header Placement
  	private int headerHeight = 48;
	private int headerWidth = 360;//330;
	
	// Weapon Placement
	private int weaponWidth = 360;//330;
 	private int weaponHeight = 24;
  	
	// Button Placement
	private int buyButtonWidth = 100;
	private int upgradeButtonWidth = 120;//80;
	private int refillButtonWidth = 80;
	private int equipButtonWidth = 60;
  	private int buttonHeight = 18;
	private int buttonColRefill = 160;//120;
	private int buttonColBuy = 190;
  	private int buttonColUpgrade = 180;//170;
  	private int buttonColEquip = 300;//250;

  	private List<BaseWeapon> weapons;
	private WeaponManager weaponManager;
	private WeaponVendor weaponVendor;
	private List<AmmoVendor> ammoVendorContainer;
	
	// Background image
	public Texture2D backGround;
	
	// Header
	public Texture2D header;
	public Font headerFont;
	
	// Label
	public Texture2D labelEquipped;
	public Texture2D labelOwned;
	public Texture2D labelLocked;
	public Font labelFont;
	
	// Buy
	public Texture2D buyNormal;
	public Texture2D buyHover;
	public Texture2D buyActive;
	
	// Upgrade
	public Texture2D upgradeNormal;
	public Texture2D upgradeHover;
	public Texture2D upgradeActive;
	
	// Equip
	public Texture2D equipNormal;
	public Texture2D equipHover;
	public Texture2D equipActive;
	
	// Refill
	public Texture2D refillNormal;
	public Texture2D refillHover;
	public Texture2D refillActive;
	
	void Start(){
		Reset();
	}
	
	public void Reset(){
	    weaponManager = GameController.Instance.GetPlayer().GetComponentInChildren<WeaponManager>();
	    weapons = weaponManager.allWeapons;
		weaponVendor = GameObject.Find("Vendor").GetComponent<WeaponVendor>();
		ammoVendorContainer = GameObject.Find("Vendor").GetComponent<AmmoVendorContainer>().ammoVendors;
	}
	
	public void Draw(Rect drawRect){
		Rect drawArea = drawRect;
	    string[] types = Enum.GetNames(typeof(WeaponType));
	    int weaponRegionHeight = headerHeight + (3 * weaponHeight);
		
		GUI.BeginGroup(drawArea);
	    GUI.DrawTexture(new Rect(0, 0, drawArea.width, drawArea.height), backGround);
	    for(int i=0; i<types.Length; i++){
	      	List<BaseWeapon> type = new List<BaseWeapon>();
	      	type = weapons.FindAll(x => (int)x.weaponType == i);
			
			GUI.BeginGroup(new Rect(labelOffset, i * weaponRegionHeight, drawArea.width, weaponRegionHeight+labelOffset));
			
	      	GUIStyle headerStyle = new GUIStyle();
	      	headerStyle.alignment = TextAnchor.MiddleLeft;
			headerStyle.normal.background = header;
			headerStyle.normal.textColor = Color.white;
			headerStyle.font = headerFont;
			headerStyle.contentOffset = new Vector2(labelOffset, 0);
			
			GUIStyle refillStyle = new GUIStyle();
			refillStyle.alignment = TextAnchor.MiddleCenter;
			refillStyle.normal.background = refillNormal;
			refillStyle.hover.background = refillHover;
			refillStyle.active.background = refillActive;
			refillStyle.normal.textColor = Color.white;
			refillStyle.hover.textColor = Color.white;
			refillStyle.active.textColor = Color.white;
			refillStyle.font = labelFont;
	
	      	GUI.Label(new Rect(labelOffset, labelOffset, headerWidth, headerHeight), types[i], headerStyle);
			
			if(weaponManager.equippedWeapons[i]){
				// Handle Refill of weapon
				Rect refillRect = new Rect(buttonColRefill+labelOffset+labelOffset, labelOffset+labelOffset, refillButtonWidth*2f, buttonHeight*1.5f);
				
				// Dispense the ammo
				if(weaponManager.equippedWeapons[i]){
					if(GameController.Instance.GetResources() < ammoVendorContainer[i].ammoVendor.GetComponent<SellableItem>().cost && ammoVendorContainer[i].isDisplaying){
						ammoVendorContainer[i].Cancel();
					} else {
						ammoVendorContainer[i].SetWeapon(weaponManager.equippedWeapons[i]);
						ammoVendorContainer[i].Vendor();
					}
				}
				
				// If so, then purchase it
				if(GameController.Instance.GetResources() >= ammoVendorContainer[i].ammoVendor.GetComponent<SellableItem>().cost){
					if(GUI.Button(refillRect, "Refill Ammo: "+ammoVendorContainer[i].ammoVendor.GetComponent<SellableItem>().cost, refillStyle)){
						if(weaponManager.equippedWeapons[i]){
							ammoVendorContainer[i].Purchase(ammoVendorContainer[i].ammoVendor);
						}
					}
				}
			}
			
	      	GUIStyle weaponLabelStyle = new GUIStyle();
			weaponLabelStyle.font = labelFont;
			weaponLabelStyle.normal.textColor = Color.white;
			weaponLabelStyle.contentOffset = new Vector2(labelOffset, 0);
			
			GUIStyle buttonStyle = new GUIStyle();
			buttonStyle.font = labelFont;
			buttonStyle.fontSize = 10;
			buttonStyle.alignment = TextAnchor.MiddleCenter;
			buttonStyle.normal.textColor = Color.white;
			buttonStyle.hover.textColor = Color.white;
			buttonStyle.active.textColor = Color.white;
			
			GUIStyle equipStyle = new GUIStyle();
			equipStyle.font = labelFont;
			equipStyle.fontSize = 10;
			equipStyle.alignment = TextAnchor.MiddleCenter;
			equipStyle.normal.textColor = Color.white;
			equipStyle.hover.textColor = Color.white;
			equipStyle.active.textColor = Color.white;
			
	      	for(int j=0; j<3; j++){
				if(weaponManager.equippedWeapons.Contains(type[j].gameObject) && type[j].GetComponent<SellableItem>().purchased){
					weaponLabelStyle.normal.background = labelEquipped;
				} else if(type[j].GetComponent<SellableItem>().purchased && !weaponManager.equippedWeapons.Contains(type[j].gameObject)){
					weaponLabelStyle.normal.background = labelOwned;
				} else {
					weaponLabelStyle.normal.background = labelLocked;
				}
				
	        	GUI.Label(new Rect(labelOffset, (labelOffset+headerHeight)+j*weaponHeight, weaponWidth, weaponHeight), type[j].name, weaponLabelStyle);
	        	if(type[j].GetComponent<SellableItem>().purchased){
					buttonStyle.normal.background = upgradeNormal;
					buttonStyle.hover.background = upgradeHover;
					buttonStyle.active.background = upgradeActive;
					
					if(GameController.Instance.GetResources() >= type[j].GetComponent<SellableItem>().cost && type[j].GetComponent<SellableItem>().currentUpgrade < 3){
	          			if(GUI.Button(new Rect(buttonColUpgrade, labelOffset+2+j*weaponHeight + headerHeight + 1, upgradeButtonWidth, buttonHeight), "UPGRADE: "+type[j].GetComponent<SellableItem>().cost.ToString(), buttonStyle)){
							weaponVendor.Upgrade(type[j].gameObject);
							ammoVendorContainer[i].Cancel();
						}
					}
					
					equipStyle.normal.background = equipNormal;
					equipStyle.hover.background = equipHover;
					equipStyle.active.background = equipActive;
					if(GUI.Button(new Rect(buttonColEquip, labelOffset+2+j*weaponHeight + headerHeight + 1, equipButtonWidth, buttonHeight), "EQUIP", equipStyle)){
						 if(weaponManager.pistolWeapons.Contains(type[j].gameObject)){
							weaponManager.equippedWeapons[0] = type[j].gameObject;
							ammoVendorContainer[0].Cancel();
						} else if(weaponManager.rifleWeapons.Contains(type[j].gameObject)){
							weaponManager.equippedWeapons[1] = type[j].gameObject;
							ammoVendorContainer[1].Cancel();
						} else if(weaponManager.launcherWeapons.Contains(type[j].gameObject)){
							weaponManager.equippedWeapons[2] = type[j].gameObject;
							ammoVendorContainer[2].Cancel();
						} else if(weaponManager.specialWeapons.Contains(type[j].gameObject)){
							weaponManager.equippedWeapons[3] = type[j].gameObject;
							ammoVendorContainer[3].Cancel();
						}
					}
	        	} else {
					if(GameController.Instance.GetResources() >= type[j].GetComponent<SellableItem>().cost){
						buttonStyle.normal.background = buyNormal;
						buttonStyle.hover.background = buyHover;
						buttonStyle.active.background = buyActive;
						
		          		if(GUI.Button(new Rect(buttonColBuy, labelOffset+2+j*weaponHeight + headerHeight + 1, buyButtonWidth, buttonHeight), "BUY: "+type[j].GetComponent<SellableItem>().cost.ToString(), buttonStyle)){
							weaponVendor.Purchase(type[j].gameObject);
						}
					}
	        	}
	      	}
			GUI.EndGroup();
	    }
		GUI.EndGroup();
	}
}