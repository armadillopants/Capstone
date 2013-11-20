using UnityEngine;
using System.Collections.Generic;
using System;

public class BuildPanelGUI : MonoBehaviour {
	
	private Vector2 drawSize = new Vector2(350, 500);
	
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
  	private int buttonColOneX = 120;
  	private int buttonColTwoX = 220;
	
	private List<GameObject> allForts = new List<GameObject>();
	private ItemVendor itemVendor;
	
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
	
	void Start(){
		itemVendor = GameObject.Find("Vendor").GetComponent<ItemVendor>();
		allForts = itemVendor.itemVendor;
	}
	
	public void Draw(Rect drawRect){
		Rect drawArea = drawRect;
		
	    int weaponRegionHeight = headerHeight + (3 * weaponHeight);
		
		GUILayout.BeginArea(drawArea);
	    GUI.DrawTexture(new Rect(0, 0, drawArea.width, drawArea.height), backGround);
	    /*for(int i=0; i<types.Length; i++){
			
	      	GUILayout.BeginArea(new Rect(0, i * weaponRegionHeight, drawArea.width, weaponRegionHeight+labelOffset));
	
	      	GUIStyle headerStyle = new GUIStyle();
	      	headerStyle.alignment = TextAnchor.MiddleLeft;
			headerStyle.normal.background = header;
			headerStyle.normal.textColor = Color.white;
			headerStyle.font = headerFont;
			headerStyle.contentOffset = new Vector2(labelOffset, 0);
	
	      	GUI.Label(new Rect(labelOffset, labelOffset, headerWidth, headerHeight), "Structure", headerStyle);
	      
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
			
	      	for(int j=0; j<3; j++){
				
	        	GUI.Label(new Rect(labelOffset, (labelOffset+headerHeight)+j*weaponHeight, weaponWidth, weaponHeight), "Structure", weaponLabelStyle);
				buttonStyle.normal.background = buyNormal;
				buttonStyle.hover.background = buyHover;
				buttonStyle.active.background = buyActive;
					
	          	if(GUI.Button(new Rect(buttonColOneX, labelOffset+2+j*weaponHeight + headerHeight + 1, buttonWidth, buttonHeight), "BUY", buttonStyle)){
					//itemVendor.Purchase(type[j].gameObject);
				}
	      	}
			GUILayout.EndArea();
	    }*/
	    GUILayout.EndArea();
	}
}
