using UnityEngine;
using System.Collections.Generic;
using System;

public class BuildPanelGUI : MonoBehaviour {
	
  	private int labelOffset = 10;
	
	// Label Placement
  	private int labelHeight = 48;
	private int labelWidth = 330;
  	
	// Button Placement
	private int buttonWidth = 80;
  	private int buttonHeight = 24;
  	private int buttonPosX = 200;
	
	private List<GameObject> allForts = new List<GameObject>();
	private ItemVendor itemVendor;
	
	// Background image
	public Texture2D backGround;
	
	// Label
	public Texture2D labelCanBuy;
	public Texture2D labelCantBuy;
	public Font labelFont;
	
	// Buy
	public Texture2D buyNormal;
	public Texture2D buyHover;
	public Texture2D buyActive;
	
	void Start(){
		itemVendor = GameObject.Find("Vendor").GetComponent<ItemVendor>();
		allForts = itemVendor.itemVendor;
	}
	
	public void Draw(Rect drawRect){
		Rect drawArea = drawRect;
		
		GUI.BeginGroup(drawArea);
	    GUI.DrawTexture(new Rect(0, 0, drawArea.width, drawArea.height), backGround);
	    for(int i=0; i<allForts.Count; i++){
			
	      	GUIStyle itemLabelStyle = new GUIStyle();
	      	itemLabelStyle.alignment = TextAnchor.MiddleLeft;
			if(GameController.Instance.GetResources() >= allForts[i].GetComponent<SellableItem>().cost){
				itemLabelStyle.normal.background = labelCanBuy;
			} else {
				itemLabelStyle.normal.background = labelCantBuy;
			}
			itemLabelStyle.normal.textColor = Color.white;
			itemLabelStyle.font = labelFont;
			itemLabelStyle.contentOffset = new Vector2(labelOffset, 0);
	
	      	GUI.Label(new Rect(labelOffset, labelOffset+i*labelHeight + labelHeight, labelWidth, labelHeight), allForts[i].name, itemLabelStyle);
			
			GUIStyle buttonStyle = new GUIStyle();
			buttonStyle.font = labelFont;
			buttonStyle.fontSize = 10;
			buttonStyle.alignment = TextAnchor.MiddleCenter;
			buttonStyle.normal.background = buyNormal;
			buttonStyle.hover.background = buyHover;
			buttonStyle.active.background = buyActive;
			buttonStyle.normal.textColor = Color.white;
			buttonStyle.hover.textColor = Color.white;
			buttonStyle.active.textColor = Color.white;
			
			if(GameController.Instance.GetResources() >= allForts[i].GetComponent<SellableItem>().cost){
				if(GUI.Button(new Rect(buttonPosX, labelOffset+i*labelHeight + labelHeight+(buttonHeight/2), buttonWidth, buttonHeight), "BUY", buttonStyle)){
					itemVendor.Purchase(allForts[i]);
				}
			}
	    }
	    GUI.EndGroup();
	}
}
