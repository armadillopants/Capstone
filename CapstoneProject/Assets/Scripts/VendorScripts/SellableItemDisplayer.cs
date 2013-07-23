using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class SellableItemDisplayer : MonoBehaviour {
	private List<DisplayItem> itemsToDisplay = new List<DisplayItem>();
	private const int FONT_SIZE = 15;
	private GUIStyle style = new GUIStyle();
	private GUIContent content = new GUIContent();

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
		if(GameController.Instance.canDisplay){
			for(int i=0; i<itemsToDisplay.Count; i++){
				style.fontSize = FONT_SIZE;
				style.normal.background = itemsToDisplay[i].sellItem.preview;
				style.hover.background = itemsToDisplay[i].sellItem.preview;
				style.active.background = itemsToDisplay[i].sellItem.preview;
				style.normal.textColor = Color.white;
				style.hover.textColor = Color.yellow;
				style.active.textColor = Color.white;
				style.alignment = TextAnchor.MiddleCenter;
				// The location on the screen that we want to draw this item from (top-left corner)
				Vector2 guiLoc = new Vector2();
	
				// If the item has a worldspace location to attach to, convert it to screenspace
				if(itemsToDisplay[i].hasWorldspace){
					Vector3 t = Camera.mainCamera.WorldToScreenPoint(itemsToDisplay[i].worldspaceLocation);
					guiLoc.x = t.x;
					guiLoc.y = Screen.height - t.y;
				}
				// Apply pixel-based offset
				guiLoc.x += itemsToDisplay[i].pixelOffset.x;
				guiLoc.y += itemsToDisplay[i].pixelOffset.y;
				
				content = new GUIContent(itemsToDisplay[i].sellItem.itemName+"\n"+itemsToDisplay[i].sellItem.description+
					"\nPrice $"+itemsToDisplay[i].sellItem.cost.ToString(),itemsToDisplay[i].icon);
	
				if(GUI.Button(new Rect(guiLoc.x,guiLoc.y,itemsToDisplay[i].windowSize.x,itemsToDisplay[i].windowSize.y),content,style)){
					// Get Method
					MethodInfo method;
					try {
						if(itemsToDisplay[i].upgrade){
							method = itemsToDisplay[i].invokingType.GetMethod("Upgrade");
						} else {
							method = itemsToDisplay[i].invokingType.GetMethod("Purchase");
						}
						System.Object[] param = { itemsToDisplay[i].item };
						method.Invoke(itemsToDisplay[i].invokingObject, param);
						}
					catch(System.Exception e){
						Debug.LogError (e.Message + ": " + e.InnerException);
						Application.Quit();
					}
				}
			}
		}
	}
}
