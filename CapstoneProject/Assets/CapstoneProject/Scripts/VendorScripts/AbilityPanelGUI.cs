using UnityEngine;
using System.Collections.Generic;

public class AbilityPanelGUI : MonoBehaviour {
	
	private int labelOffset = 10;
	
	// Label Placement
  	private int labelHeight = 48;
	private int labelWidth = 330;
  	
	// Button Placement
	private int buttonWidth = 80;
  	private int buttonHeight = 18;
	private int refillButtonX = 120;
  	private int buttonColOneX = 170;
  	private int buttonColTwoX = 250;
	
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
	private AbilitiesManager abilityManager;
	private List<GameObject> allAbilities = new List<GameObject>();
	private GameObject abilityHolder;
	
	void Start(){
		abilityVendor = GameObject.Find("Vendor").GetComponent<AbilityVendor>();
		allAbilities = abilityVendor.abilityVendor;
		
		abilityManager = GameController.Instance.GetPlayer().GetComponent<AbilitiesManager>();
		
		abilityHolder = GameObject.Find("AbilityHolder");
	}
	
	public void Draw(Rect drawRect){
		Rect drawArea = drawRect;
		
		GUI.BeginGroup(drawArea);
		
		GUI.DrawTexture(new Rect(0, 0, drawArea.width, drawArea.height), backGround);
	    for(int i=0; i<allAbilities.Count; i++){
			
			GameObject curAbility = allAbilities[i];
			
	      	GUIStyle abilityLabelStyle = new GUIStyle();
	      	abilityLabelStyle.alignment = TextAnchor.MiddleLeft;
			abilityLabelStyle.normal.textColor = Color.white;
			abilityLabelStyle.font = labelFont;
			abilityLabelStyle.contentOffset = new Vector2(labelOffset, 0);
			
			if(allAbilities[i].GetComponent<SellableItem>().purchased && abilityHolder.GetComponent(allAbilities[i].name) != null){
				abilityLabelStyle.normal.background = labelEquipped;
			} else if(allAbilities[i].GetComponent<SellableItem>().purchased && abilityHolder.GetComponent(allAbilities[i].name) == null){
				abilityLabelStyle.normal.background = labelOwned;
			} else {
				abilityLabelStyle.normal.background = labelLocked;
			}
	
	      	GUI.Label(new Rect(labelOffset, labelOffset+i*labelHeight + labelHeight+labelHeight, labelWidth, labelHeight), allAbilities[i].name, abilityLabelStyle);
			
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
			
			if(allAbilities[i].GetComponent<SellableItem>().purchased){
				buttonStyle.normal.background = upgradeNormal;
				buttonStyle.hover.background = upgradeHover;
				buttonStyle.active.background = upgradeActive;
				
				if(GameController.Instance.GetResources() > allAbilities[i].GetComponent<SellableItem>().cost){
					if(GUI.Button(new Rect(buttonColOneX, labelOffset+i*labelHeight + labelHeight+labelHeight+(buttonHeight/2), buttonWidth, buttonHeight), "UPGRADE", buttonStyle)){
						abilityVendor.Upgrade(allAbilities[i]);
					}
				}
				
				equipStyle.normal.background = equipNormal;
				equipStyle.hover.background = equipHover;
				equipStyle.active.background = equipActive;
				if(GUI.Button(new Rect(buttonColTwoX, labelOffset+i*labelHeight + labelHeight+labelHeight+(buttonHeight/2), buttonWidth, buttonHeight), "EQUIP", buttonStyle)){
					if(curAbility == allAbilities[0]){
						if(abilityHolder.GetComponent(allAbilities[0].name) == null){
							abilityHolder.AddComponent(allAbilities[0].name);
							abilityManager.SetAbility(allAbilities[0].name);
							Destroy(abilityHolder.GetComponent(allAbilities[1].name));
							Destroy(abilityHolder.GetComponent(allAbilities[2].name));
						}
					} else if(curAbility == allAbilities[1]){
						if(abilityHolder.GetComponent(allAbilities[1].name) == null){
							abilityHolder.AddComponent(allAbilities[1].name);
							abilityManager.SetAbility(allAbilities[1].name);
							Destroy(abilityHolder.GetComponent(allAbilities[0].name));
							Destroy(abilityHolder.GetComponent(allAbilities[2].name));
						}
					} else {
						if(abilityHolder.GetComponent(allAbilities[2].name) == null){
							abilityHolder.AddComponent(allAbilities[2].name);
							abilityManager.SetAbility(allAbilities[2].name);
							Destroy(abilityHolder.GetComponent(allAbilities[0].name));
							Destroy(abilityHolder.GetComponent(allAbilities[1].name));
						}
					}
				}
        	} else {
				if(GameController.Instance.GetResources() >= allAbilities[i].GetComponent<SellableItem>().cost){
					buttonStyle.normal.background = buyNormal;
					buttonStyle.hover.background = buyHover;
					buttonStyle.active.background = buyActive;
					
					if(GUI.Button(new Rect(buttonColOneX, labelOffset+i*labelHeight + labelHeight+labelHeight+(buttonHeight/2), buttonWidth, buttonHeight), "BUY", buttonStyle)){
						abilityVendor.Purchase(allAbilities[i]);
					}
				}
        	}
	    }
		
		GUI.EndGroup();
	}
}
