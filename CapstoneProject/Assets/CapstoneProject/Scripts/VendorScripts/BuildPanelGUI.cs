﻿using System.Collections.Generic;
using UnityEngine;

public class BuildPanelGUI : MonoBehaviour {
	
  	private int labelOffset = 10;
	
	// Label Placement
  	private int labelHeight = 30;
	private int labelWidth = 340;
  	
	// Button Placement
	private int buttonWidth = 80;
  	private int buttonHeight = 20;
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
	
	private GUIStyle itemLabelStyle;
	private GUIStyle descriptionStyle;
	private GUIStyle buttonStyle;
	
	void Start(){
		Reset();
		
		itemLabelStyle = new GUIStyle();
      	itemLabelStyle.alignment = TextAnchor.MiddleLeft;
		itemLabelStyle.normal.textColor = Color.white;
		itemLabelStyle.font = labelFont;
		itemLabelStyle.contentOffset = new Vector2(labelOffset, 0);
		
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
		buttonStyle.normal.background = buyNormal;
		buttonStyle.hover.background = buyHover;
		buttonStyle.active.background = buyActive;
		buttonStyle.normal.textColor = Color.white;
		buttonStyle.hover.textColor = Color.white;
		buttonStyle.active.textColor = Color.white;
	}
	
	public void Reset(){
		itemVendor = GameObject.Find("Vendor").GetComponent<ItemVendor>();
		allForts = itemVendor.itemVendor;
		for(int i=0; i<allForts.Count; i++){
			if(allForts[i].name == "SatelliteTower"){
				allForts[i].GetComponent<SellableItem>().soldOut = false;
			}
		}
	}
	
	public void Draw(Rect drawRect){
		Rect drawArea = drawRect;
		int regionHeight = labelHeight + (labelOffset*3);
		
		if(!UIManager.Instance.isPaused){
			GUI.BeginGroup(drawArea);
		    GUI.DrawTexture(new Rect(0, 0, drawArea.width, drawArea.height), backGround);
		    for(int i=0; i<allForts.Count; i++){
				
				GUI.BeginGroup(new Rect(labelOffset*2, i*regionHeight+labelOffset, drawArea.width, regionHeight*labelOffset));
	
				if(GameController.Instance.GetResources() >= allForts[i].GetComponent<SellableItem>().cost){
					itemLabelStyle.normal.background = labelCanBuy;
				} else {
					itemLabelStyle.normal.background = labelCantBuy;
				}
				
				GUI.Label(new Rect(labelOffset, 0, labelWidth, labelHeight), allForts[i].GetComponent<SellableItem>().itemName, itemLabelStyle);
				
				GUI.Label(new Rect(labelOffset, buttonHeight+labelOffset, labelWidth, labelHeight), allForts[i].GetComponent<SellableItem>().description, descriptionStyle);
				
				if(GameController.Instance.GetResources() >= allForts[i].GetComponent<SellableItem>().cost && !allForts[i].GetComponent<SellableItem>().soldOut && GameController.Instance.GetWaveController().GetWaveNumber() >= 3){
					if(GUI.Button(new Rect(buttonPosX, labelOffset/2, buttonWidth, buttonHeight), "BUY: "+allForts[i].GetComponent<SellableItem>().cost, buttonStyle)){
						itemVendor.Purchase(allForts[i]);
						if(allForts[i].name == "SatelliteTower"){
							allForts[i].GetComponent<SellableItem>().soldOut = true;
						}
					}
				} else if(GameController.Instance.GetResources() >= allForts[i].GetComponent<SellableItem>().cost && !allForts[i].GetComponent<SellableItem>().soldOut && GameController.Instance.GetWaveController().GetWaveNumber() < 3){
					if(allForts[i].name.Contains("Barrier")){
						if(GUI.Button(new Rect(buttonPosX, labelOffset/2, buttonWidth, buttonHeight), "BUY: "+allForts[i].GetComponent<SellableItem>().cost, buttonStyle)){
							itemVendor.Purchase(allForts[i]);
						}
					}
				}
				GUI.EndGroup();
		    }
		    GUI.EndGroup();
		}
	}
}
