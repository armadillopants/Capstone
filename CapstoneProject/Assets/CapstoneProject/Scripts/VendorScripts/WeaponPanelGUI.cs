using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPanelGUI : MonoBehaviour {
	
  	private int labelOffset = 10;
	
	// Header Placement
  	private int headerHeight = 48;
	private int headerWidth = 330;
	
	// Weapon Placement
	private int weaponWidth = 330;
 	private int weaponHeight = 24;
  	
	// Button Placement
	private int buttonWidth = 80;
  	private int buttonHeight = 18;
	private int refillButtonX = 120;
  	private int buttonColOneX = 170;
  	private int buttonColTwoX = 250;

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
		weaponVendor = GameObject.Find("Vendor").GetComponent<WeaponVendor>();
		ammoVendorContainer = GameObject.Find("Vendor").GetComponent<AmmoVendorContainer>().ammoVendors;
	}
	
	public void Reset(){
	    weaponManager = GameController.Instance.GetPlayer().GetComponentInChildren<WeaponManager>();
	    weapons = weaponManager.allWeapons;
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
			
			GUI.BeginGroup(new Rect(labelOffset*2, i * weaponRegionHeight, drawArea.width, weaponRegionHeight+labelOffset));
			
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
				string refillType = types[i];
				Rect refillRect = new Rect(refillButtonX+labelOffset+labelOffset, labelOffset+labelOffset, buttonWidth*2f, buttonHeight*1.5f);
				
				// Dispense the ammo
				if(refillType == types[0]){
					if(weaponManager.equippedWeapons[1]){
						if(GameController.Instance.GetResources() < ammoVendorContainer[0].ammoVendor.GetComponent<SellableItem>().cost && ammoVendorContainer[0].isDisplaying){
							ammoVendorContainer[0].Cancel();
						} else {
							ammoVendorContainer[0].SetWeapon(weaponManager.equippedWeapons[1]);
							ammoVendorContainer[0].Vendor();
						}
					}
				} else if(refillType == types[1]){
					if(weaponManager.equippedWeapons[0]){
						if(GameController.Instance.GetResources() < ammoVendorContainer[1].ammoVendor.GetComponent<SellableItem>().cost && ammoVendorContainer[1].isDisplaying){
							ammoVendorContainer[1].Cancel();
						} else {
							ammoVendorContainer[1].SetWeapon(weaponManager.equippedWeapons[0]);
							ammoVendorContainer[1].Vendor();
						}
					}
				} else if(refillType == types[2]){
					if(weaponManager.equippedWeapons[2]){
						if(GameController.Instance.GetResources() < ammoVendorContainer[2].ammoVendor.GetComponent<SellableItem>().cost && ammoVendorContainer[2].isDisplaying){
							ammoVendorContainer[2].Cancel();
						} else {
							ammoVendorContainer[2].SetWeapon(weaponManager.equippedWeapons[2]);
							ammoVendorContainer[2].Vendor();
						}
					}
				} else {
					if(weaponManager.equippedWeapons[3]){
						if(GameController.Instance.GetResources() < ammoVendorContainer[0].ammoVendor.GetComponent<SellableItem>().cost && ammoVendorContainer[3].isDisplaying){
							ammoVendorContainer[3].Cancel();
						} else {
							ammoVendorContainer[3].SetWeapon(weaponManager.equippedWeapons[3]);
							ammoVendorContainer[3].Vendor();
						}
					}
				}
				
				// If so, then purchase it
				if(GUI.Button(refillRect, "Refill Ammo: "+ammoVendorContainer[i].ammoVendor.GetComponent<SellableItem>().cost, refillStyle)){
					if(refillType == types[0]){
						if(weaponManager.equippedWeapons[1]){
							ammoVendorContainer[0].Purchase(ammoVendorContainer[0].ammoVendor);
						}
					} else if(refillType == types[1]){
						if(weaponManager.equippedWeapons[0]){
							ammoVendorContainer[1].Purchase(ammoVendorContainer[1].ammoVendor);
						}
					} else if(refillType == types[2]){
						if(weaponManager.equippedWeapons[2]){
							ammoVendorContainer[2].Purchase(ammoVendorContainer[2].ammoVendor);
						}
					} else {
						if(weaponManager.equippedWeapons[3]){
							ammoVendorContainer[3].Purchase(ammoVendorContainer[3].ammoVendor);
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
					
					if(GameController.Instance.GetResources() > type[j].GetComponent<SellableItem>().cost && type[j].GetComponent<SellableItem>().currentUpgrade < 3){
	          			if(GUI.Button(new Rect(buttonColOneX, labelOffset+2+j*weaponHeight + headerHeight + 1, buttonWidth, buttonHeight), "UPGRADE: "+type[j].GetComponent<SellableItem>().cost.ToString(), buttonStyle)){
							weaponVendor.Upgrade(type[j].gameObject);
							ammoVendorContainer[i].Cancel();
						}
					}
					
					equipStyle.normal.background = equipNormal;
					equipStyle.hover.background = equipHover;
					equipStyle.active.background = equipActive;
					if(GUI.Button(new Rect(buttonColTwoX, labelOffset+2+j*weaponHeight + headerHeight + 1, buttonWidth, buttonHeight), "EQUIP", equipStyle)){
						if(weaponManager.rifleWeapons.Contains(type[j].gameObject)){
							weaponManager.equippedWeapons[0] = type[j].gameObject;
							ammoVendorContainer[1].Cancel();
						} else if(weaponManager.pistolWeapons.Contains(type[j].gameObject)){
							weaponManager.equippedWeapons[1] = type[j].gameObject;
							ammoVendorContainer[0].Cancel();
						} else if(weaponManager.launcherWeapons.Contains(type[j].gameObject)){
							weaponManager.equippedWeapons[2] = type[j].gameObject;
							ammoVendorContainer[2].Cancel();
						} else if(weaponManager.specialWeapons.Contains(type[j].gameObject)){
							weaponManager.equippedWeapons[3] = type[j].gameObject;
							ammoVendorContainer[3].Cancel();
						}
					}
	        	} else {
					if(GameController.Instance.GetResources() > type[j].GetComponent<SellableItem>().cost){
						buttonStyle.normal.background = buyNormal;
						buttonStyle.hover.background = buyHover;
						buttonStyle.active.background = buyActive;
						
		          		if(GUI.Button(new Rect(buttonColOneX, labelOffset+2+j*weaponHeight + headerHeight + 1, buttonWidth, buttonHeight), "BUY: "+type[j].GetComponent<SellableItem>().cost.ToString(), buttonStyle)){
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