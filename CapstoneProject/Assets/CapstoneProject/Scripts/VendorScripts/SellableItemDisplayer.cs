using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class SellableItemDisplayer : MonoBehaviour {
	private List<DisplayItem> itemsToDisplay = new List<DisplayItem>();
	private const int FONT_SIZE = 15;
	
	private GUIStyle labelStyle = new GUIStyle();
	private GUIContent labelContent = new GUIContent();
	
	private GUIStyle buttonStyle = new GUIStyle();
	private GUIContent buttonContent = new GUIContent();
	
	private GUIStyle equippedStyle = new GUIStyle();
	private GUIContent equippedContent = new GUIContent();
	
	private WeaponManager manager;
	public Texture2D equipped;
	public Texture2D owned;
	public Texture2D locked;
	
	public Texture2D buyNormal;
	public Texture2D buyHover;
	public Texture2D buyActive;
	
	public Texture2D upgradeNormal;
	public Texture2D upgradeHover;
	public Texture2D upgradeActive;
	
	public Texture2D equippedNormal;
	public Texture2D equippedHover;
	public Texture2D equippedActive;
	
	void Start(){
		manager = GameController.Instance.GetPlayer().GetComponentInChildren<WeaponManager>();
	}

	public void AddToDisplay(DisplayItem item){
		itemsToDisplay.Add(item);
	}
	
	public void RemoveFromDisplay(DisplayItem item){
		for(int i=0; i<itemsToDisplay.Count; i++){
			if(itemsToDisplay[i] == item){
				itemsToDisplay.RemoveAt(i);
				break;
			}
		}
	}
	
	public void Purge(){
		itemsToDisplay.Clear();
	}
	
	void OnGUI(){
		for(int i=0; i<itemsToDisplay.Count; i++){
			labelStyle.fontSize = FONT_SIZE;
			//labelStyle.font = UIManager.Instance.labelFont;
			labelStyle.normal.background = itemsToDisplay[i].weaponState;
			labelStyle.normal.textColor = Color.white;
			/*style.normal.background = itemsToDisplay[i].sellItem.preview;
			style.hover.background = itemsToDisplay[i].sellItem.preview;
			style.active.background = itemsToDisplay[i].sellItem.preview;
			style.normal.textColor = Color.white;
			style.hover.textColor = Color.yellow;
			style.active.textColor = Color.white;*/
			labelStyle.alignment = TextAnchor.MiddleLeft;
			// The location on the screen that we want to draw this item from (top-left corner)
			Vector2 guiLoc = new Vector2();
			
			if(manager.equippedWeapons.Contains(itemsToDisplay[i].item) && itemsToDisplay[i].sellItem.purchased){
				itemsToDisplay[i].weaponState = equipped;
			} else if(itemsToDisplay[i].sellItem.purchased && !manager.equippedWeapons.Contains(itemsToDisplay[i].item)){
				itemsToDisplay[i].weaponState = owned;
			} else {
				itemsToDisplay[i].weaponState = locked;
			}

			// If the item has a worldspace location to attach to, convert it to screenspace
			if(itemsToDisplay[i].hasWorldspace){
				Vector3 t = Camera.main.WorldToScreenPoint(itemsToDisplay[i].worldspaceLocation);
				guiLoc.x = t.x;
				guiLoc.y = Screen.height - t.y;
			}
			// Apply pixel-based offset
			guiLoc.x += itemsToDisplay[i].pixelOffset.x;
			guiLoc.y += itemsToDisplay[i].pixelOffset.y;
			
			/*content = new GUIContent("\n\n\n"+itemsToDisplay[i].sellItem.itemName+"\n"+itemsToDisplay[i].sellItem.description.Replace("NEWLINE", "\n")+
				"\nPrice: $"+itemsToDisplay[i].sellItem.cost.ToString(),itemsToDisplay[i].icon);*/
			
			labelContent = new GUIContent(itemsToDisplay[i].sellItem.itemName);
			labelStyle.contentOffset = new Vector2(5,0);
			
			GUI.Label(new Rect(guiLoc.x,guiLoc.y,itemsToDisplay[i].windowSize.x,itemsToDisplay[i].windowSize.y), labelContent, labelStyle);
			
			if(itemsToDisplay[i].sellItem.purchased){
				buttonStyle.fontSize = 10;
				//buttonStyle.font = UIManager.Instance.labelFont;
				buttonStyle.normal.background = upgradeNormal;
				buttonStyle.hover.background = upgradeHover;
				buttonStyle.active.background = upgradeActive;
				buttonStyle.normal.textColor = Color.white;
				buttonStyle.hover.textColor = Color.white;
				buttonStyle.active.textColor = Color.white;
				buttonStyle.alignment = TextAnchor.MiddleCenter;
				
				buttonContent = new GUIContent("UPGRADE");
							
				equippedStyle.fontSize = 10;
				//equippedStyle.font = UIManager.Instance.labelFont;
				equippedStyle.normal.background = equippedNormal;
				equippedStyle.hover.background = equippedHover;
				equippedStyle.active.background = equippedActive;
				equippedStyle.normal.textColor = Color.white;
				equippedStyle.hover.textColor = Color.white;
				equippedStyle.active.textColor = Color.white;
				equippedStyle.alignment = TextAnchor.MiddleCenter;
				
				equippedContent = new GUIContent("EQUIP");
				
				if(GUI.Button(new Rect(guiLoc.x+250, guiLoc.y, 70, 20), equippedContent, equippedStyle)){
					if(manager.rifleWeapons.Contains(itemsToDisplay[i].sellItem.gameObject)){
						manager.equippedWeapons[0] = itemsToDisplay[i].sellItem.gameObject;
					} else if(manager.pistolWeapons.Contains(itemsToDisplay[i].sellItem.gameObject)){
						manager.equippedWeapons[1] = itemsToDisplay[i].sellItem.gameObject;
					} else if(manager.launcherWeapons.Contains(itemsToDisplay[i].sellItem.gameObject)){
						manager.equippedWeapons[2] = itemsToDisplay[i].sellItem.gameObject;
					} else if(manager.specialWeapons.Contains(itemsToDisplay[i].sellItem.gameObject)){
						manager.equippedWeapons[3] = itemsToDisplay[i].sellItem.gameObject;
					}
				}
				
			} else {
				buttonStyle.fontSize = 10;
				//buttonStyle.font = UIManager.Instance.labelFont;
				buttonStyle.normal.background = buyNormal;
				buttonStyle.hover.background = buyHover;
				buttonStyle.active.background = buyActive;
				buttonStyle.normal.textColor = Color.white;
				buttonStyle.hover.textColor = Color.white;
				buttonStyle.active.textColor = Color.white;
				buttonStyle.alignment = TextAnchor.MiddleCenter;
				
				buttonContent = new GUIContent("BUY");
			}
			
			if(GUI.Button(new Rect(guiLoc.x+150, guiLoc.y, 70, 20), buttonContent, buttonStyle)){
				// Get Method
				MethodInfo method;
				try {
					if(itemsToDisplay[i].sellItem.purchased){
						method = itemsToDisplay[i].invokingType.GetMethod("Upgrade");
					} else {
						method = itemsToDisplay[i].invokingType.GetMethod("Purchase");
					}
					System.Object[] param = { itemsToDisplay[i].item };
					method.Invoke(itemsToDisplay[i].invokingObject, param);
					}
				catch(System.Exception e){
					Debug.LogError(e.Message + ": " + e.InnerException);
					Application.Quit();
				}
			}
		}
	}
}