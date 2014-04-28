using System.Collections.Generic;
using UnityEngine;

public class AbilityPanelGUI : MonoBehaviour {
	
	private int labelOffset = 10;
	
	// Label Placement
  	private int labelHeight = 30;//48;
	private int labelWidth = 360;
  	
	// Button Placement
	private int buyButtonWidth = 80;
	private int upgradeButtonWidth = 100;
	private int equipButtonWidth = 60;
	private int refillButtonWidth = 160;
  	private int buttonHeight = 20;//18;
	private int buttonColRefill = 100;
  	private int buttonColBuy = 200;
	private int buttonColUpgrade = 190;
  	private int buttonColEquip = 300;
	
	// Back ground
	public Texture2D backGround;
	
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
	
	private AbilityVendor abilityVendor;
	private List<GameObject> allAbilities = new List<GameObject>();
	private GameObject abilityHolder;
	private List<AbilityAmmoVendor> abilityAmmoVendorContainer;
	private bool useAbility;
	
	private GUIStyle abilityLabelStyle;
	private GUIStyle refillStyle;
	private GUIStyle descriptionStyle;
	private GUIStyle buttonStyle;
	private GUIStyle equipStyle;
	
	void Start(){
		Reset();
		
		abilityLabelStyle = new GUIStyle();
	  	abilityLabelStyle.alignment = TextAnchor.MiddleLeft;
		abilityLabelStyle.normal.textColor = Color.white;
		abilityLabelStyle.font = labelFont;
		abilityLabelStyle.contentOffset = new Vector2(labelOffset, 0);
		
		refillStyle = new GUIStyle();
		refillStyle.alignment = TextAnchor.MiddleCenter;
		refillStyle.normal.background = refillNormal;
		refillStyle.hover.background = refillHover;
		refillStyle.active.background = refillActive;
		refillStyle.normal.textColor = Color.white;
		refillStyle.hover.textColor = Color.white;
		refillStyle.active.textColor = Color.white;
		refillStyle.font = labelFont;
		
		descriptionStyle = new GUIStyle();
		descriptionStyle.alignment = TextAnchor.MiddleLeft;
		descriptionStyle.font = labelFont;
		descriptionStyle.fontSize = 15;
		descriptionStyle.wordWrap = true;
		descriptionStyle.normal.textColor = Color.white;
		
		buttonStyle = new GUIStyle();
		buttonStyle.font = labelFont;
		buttonStyle.fontSize = 10;
		buttonStyle.alignment = TextAnchor.MiddleCenter;
		buttonStyle.normal.textColor = Color.white;
		buttonStyle.hover.textColor = Color.white;
		buttonStyle.active.textColor = Color.white;
			
		equipStyle = new GUIStyle();
		equipStyle.font = labelFont;
		equipStyle.fontSize = 10;
		equipStyle.alignment = TextAnchor.MiddleCenter;
		equipStyle.normal.textColor = Color.white;
		equipStyle.hover.textColor = Color.white;
		equipStyle.active.textColor = Color.white;
	}
	
	public void Reset(){
		abilityVendor = GameObject.Find("Vendor").GetComponent<AbilityVendor>();
		allAbilities = abilityVendor.abilityVendor;
		abilityAmmoVendorContainer = GameObject.Find("Vendor").GetComponent<AbilityAmmoVendorContainer>().abilityAmmoVendor;
		abilityHolder = GameObject.Find("AbilityHolder");
		useAbility = true;
	}
	
	public void Draw(Rect drawRect){
		Rect drawArea = drawRect;
		
		int abilityRegionHeight = labelHeight + (4*labelHeight);
		
		GUI.BeginGroup(drawArea);
		
		GUI.DrawTexture(new Rect(0, 0, drawArea.width, drawArea.height), backGround);
	    for(int i=0; i<allAbilities.Count; i++){
			
			GUI.BeginGroup(new Rect(labelOffset, i * abilityRegionHeight, drawArea.width, abilityRegionHeight+labelOffset));
			
			GameObject curAbility = allAbilities[i];
			
			if(allAbilities[i].GetComponent<SellableItem>().purchased && abilityHolder.GetComponent(allAbilities[i].name) != null){
				abilityLabelStyle.normal.background = labelEquipped;
			} else if(allAbilities[i].GetComponent<SellableItem>().purchased && abilityHolder.GetComponent(allAbilities[i].name) == null){
				abilityLabelStyle.normal.background = labelOwned;
			} else {
				abilityLabelStyle.normal.background = labelLocked;
			}
	
	      	GUI.Label(new Rect(labelOffset, labelOffset, labelWidth, labelHeight), allAbilities[i].name, abilityLabelStyle);
			
			if(allAbilities[i].GetComponent<SellableItem>().purchased){
				// Handle Refill of weapon
				string refillType = allAbilities[i].name;
				Rect refillRect = new Rect(buttonColRefill+labelOffset+labelOffset, labelOffset*10, refillButtonWidth, buttonHeight);
				
				// Dispense the ammo
				if(refillType == allAbilities[0].name){
					if(GameController.Instance.GetResources() < abilityAmmoVendorContainer[0].ammoVendor.GetComponent<SellableItem>().cost && abilityAmmoVendorContainer[0].isDisplaying){
						abilityAmmoVendorContainer[0].Cancel();
					} else {
						abilityAmmoVendorContainer[0].SetWeapon(AbilitiesManager.Instance.orbitAbility);
						abilityAmmoVendorContainer[0].Vendor();
					}
				} else if(refillType == allAbilities[1].name){
					if(GameController.Instance.GetResources() < abilityAmmoVendorContainer[1].ammoVendor.GetComponent<SellableItem>().cost && abilityAmmoVendorContainer[1].isDisplaying){
						abilityAmmoVendorContainer[1].Cancel();
					} else {
						abilityAmmoVendorContainer[1].SetWeapon(AbilitiesManager.Instance.rockRainAbility);
						abilityAmmoVendorContainer[1].Vendor();
					}
				} else if(refillType == allAbilities[2].name){
					if(GameController.Instance.GetResources() < abilityAmmoVendorContainer[2].ammoVendor.GetComponent<SellableItem>().cost && abilityAmmoVendorContainer[2].isDisplaying){
						abilityAmmoVendorContainer[2].Cancel();
					} else {
						abilityAmmoVendorContainer[2].SetWeapon(AbilitiesManager.Instance.strikerAbility);
						abilityAmmoVendorContainer[2].Vendor();
					}
				}
				
				// If so, then purchase it
				if(GameController.Instance.GetResources() >= abilityAmmoVendorContainer[i].ammoVendor.GetComponent<SellableItem>().cost){
					if(GUI.Button(refillRect, "Refill Ammo: "+abilityAmmoVendorContainer[i].ammoVendor.GetComponent<SellableItem>().cost, refillStyle)){
						if(refillType == allAbilities[0].name){
							abilityAmmoVendorContainer[0].Purchase(abilityAmmoVendorContainer[0].ammoVendor);
						} else if(refillType == allAbilities[1].name){
							abilityAmmoVendorContainer[1].Purchase(abilityAmmoVendorContainer[1].ammoVendor);
						} else if(refillType == allAbilities[2].name){
							abilityAmmoVendorContainer[2].Purchase(abilityAmmoVendorContainer[2].ammoVendor);
						}
					}
				}
			}
			
			GUI.Label(new Rect(labelOffset, labelHeight+buttonHeight, labelWidth, labelHeight), allAbilities[i].GetComponent<SellableItem>().description, descriptionStyle);
			
			if(allAbilities[i].GetComponent<SellableItem>().purchased){
				buttonStyle.normal.background = upgradeNormal;
				buttonStyle.hover.background = upgradeHover;
				buttonStyle.active.background = upgradeActive;
				
				if(GameController.Instance.GetResources() >= allAbilities[i].GetComponent<SellableItem>().cost && allAbilities[i].GetComponent<SellableItem>().currentUpgrade <= 2){
					if(GUI.Button(new Rect(buttonColUpgrade, labelOffset+5, upgradeButtonWidth, buttonHeight), "UPGRADE: "+allAbilities[i].GetComponent<SellableItem>().cost, buttonStyle)){
						abilityVendor.Upgrade(allAbilities[i]);
						abilityAmmoVendorContainer[i].Cancel();
					}
				}
				
				equipStyle.normal.background = equipNormal;
				equipStyle.hover.background = equipHover;
				equipStyle.active.background = equipActive;
				if(GUI.Button(new Rect(buttonColEquip, labelOffset+5, equipButtonWidth, buttonHeight), "EQUIP", equipStyle)){
					if(curAbility == allAbilities[0]){
						if(abilityHolder.GetComponent(allAbilities[0].name) == null){
							abilityHolder.AddComponent(allAbilities[0].name);
							Destroy(abilityHolder.GetComponent(allAbilities[1].name));
							Destroy(abilityHolder.GetComponent(allAbilities[2].name));
						}
					} else if(curAbility == allAbilities[1]){
						if(abilityHolder.GetComponent(allAbilities[1].name) == null){
							abilityHolder.AddComponent(allAbilities[1].name);
							Destroy(abilityHolder.GetComponent(allAbilities[0].name));
							Destroy(abilityHolder.GetComponent(allAbilities[2].name));
						}
					} else {
						if(abilityHolder.GetComponent(allAbilities[2].name) == null){
							abilityHolder.AddComponent(allAbilities[2].name);
							Destroy(abilityHolder.GetComponent(allAbilities[0].name));
							Destroy(abilityHolder.GetComponent(allAbilities[1].name));
						}
					}
				}
        	} else {
				if(GameController.Instance.GetResources() >= allAbilities[i].GetComponent<SellableItem>().cost && GameController.Instance.GetWaveController().GetWaveNumber() >= 3){
					buttonStyle.normal.background = buyNormal;
					buttonStyle.hover.background = buyHover;
					buttonStyle.active.background = buyActive;
					
					if(GUI.Button(new Rect(buttonColBuy, labelOffset+5, buyButtonWidth, buttonHeight), "BUY: "+allAbilities[i].GetComponent<SellableItem>().cost, buttonStyle)){
						abilityVendor.Purchase(allAbilities[i]);
						if(useAbility){
							GameObject.Find("Tutorial").GetComponent<Tutorial>().SetKey("BoughtAbility");
							useAbility = false;
						}
					}
				}
        	}
			GUI.EndGroup();
	    }
		GUI.EndGroup();
	}
}
