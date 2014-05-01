using System.Collections.Generic;
using UnityEngine;

public class AbilityPanelGUI : MonoBehaviour {
	
	private int labelOffset = 10;
	
	// Label Placement
  	private int labelHeight = 30;
	private int labelWidth = 360;
  	
	// Button Placement
	private int buyButtonWidth = 80;
	private int upgradeButtonWidth = 100;
	private int equipButtonWidth = 60;
  	private int buttonHeight = 20;
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
	
	private AbilityVendor abilityVendor;
	private List<GameObject> allAbilities = new List<GameObject>();
	private GameObject abilityHolder;
	private bool useAbility;
	
	private GUIStyle abilityLabelStyle;
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
			
			GUI.Label(new Rect(labelOffset, labelHeight+buttonHeight, labelWidth, labelHeight), allAbilities[i].GetComponent<SellableItem>().description, descriptionStyle);
			
			if(allAbilities[i].GetComponent<SellableItem>().purchased){
				buttonStyle.normal.background = upgradeNormal;
				buttonStyle.hover.background = upgradeHover;
				buttonStyle.active.background = upgradeActive;
				
				if(GameController.Instance.GetResources() >= allAbilities[i].GetComponent<SellableItem>().cost && allAbilities[i].GetComponent<SellableItem>().currentUpgrade <= 2){
					if(GUI.Button(new Rect(buttonColUpgrade, labelOffset+5, upgradeButtonWidth, buttonHeight), "UPGRADE: "+allAbilities[i].GetComponent<SellableItem>().cost, buttonStyle)){
						abilityVendor.Upgrade(allAbilities[i]);
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
