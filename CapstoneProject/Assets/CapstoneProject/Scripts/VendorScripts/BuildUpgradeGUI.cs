using System.Collections;
using UnityEngine;

public class BuildUpgradeGUI : MonoBehaviour {
	
	public enum State {FORTINFO, YESORNO, UPGRADE };
	private State state = State.FORTINFO;
	
	public Texture2D backGround;
	
	public Texture2D buttonNormal;
	public Texture2D buttonHover;
	public Texture2D buttonActive;
	
	public Font font;
	
	private ItemVendor itemVendor;
	private GameObject curItem;
	
	private GUIStyle style;
	private GUIStyle descriptionStyle;
	
	void Start(){
		itemVendor = GameObject.Find("Vendor").GetComponent<ItemVendor>();
		
		style = new GUIStyle();
		style.normal.background = buttonNormal;
		style.hover.background = buttonHover;
		style.active.background = buttonActive;
		style.normal.textColor = Color.white;
		style.hover.textColor = Color.white;
		style.active.textColor = Color.white;
		style.alignment = TextAnchor.MiddleCenter;
		style.font = font;
		
		descriptionStyle = new GUIStyle();
		descriptionStyle.alignment = TextAnchor.MiddleCenter;
		descriptionStyle.font = font;
		descriptionStyle.wordWrap = true;
		descriptionStyle.normal.textColor = Color.white;
	}
	
	public void Reset(bool reset){
		if(reset){
			state = State.FORTINFO;
		}
		curItem = itemVendor.upgradeItemVendor;
	}
	
	public void Draw(Rect drawRect){
		Rect drawArea = drawRect;
		
		switch(state){
		case State.FORTINFO:
			DrawFortInfo(drawArea, style);
			break;
		case State.YESORNO:
			DrawYesOrNoScreen(drawArea, style);
			break;
		case State.UPGRADE:
			DrawUpgradeScreen(drawArea, style);
			break;
		}

	}
	
	void DrawFortInfo(Rect drawRect, GUIStyle style){
		
		GUI.BeginGroup(drawRect);
		
		GUI.DrawTexture(new Rect(0, 0, drawRect.width, drawRect.height), backGround);
		
		string[] fortInfo = new string[3]{"UPGRADE", "DESTROY", "CANCEL"};
		for(int i=0; i<fortInfo.Length; i++){
			string curFortInfo = fortInfo[i];
			if(GUI.Button(new Rect(drawRect.width-150, drawRect.height-250+(i*60), 100, 50), curFortInfo, style)){
				if(curFortInfo == fortInfo[0]){
					state = State.UPGRADE;
				} else if(curFortInfo == fortInfo[1]){
					//state = State.YESORNO;
					DestroyFortification();
				} else if(curFortInfo == fortInfo[2]){
					UIManager.Instance.uiState = UIManager.UIState.NONE;
				}
			}
		}
		GUI.EndGroup();
	}

	void DrawUpgradeScreen(Rect drawRect, GUIStyle style){
		
		GUI.BeginGroup(drawRect);
		
		GUI.DrawTexture(new Rect(0, 0, drawRect.width, drawRect.height), backGround);
		
		XMLVendorReader vendorReader = GameObject.Find("XMLReader").GetComponent<XMLVendorReader>();
		SellableItem sellItem = null;
		if(curItem.GetComponent<SellableItem>().upgradedItem){
			sellItem = curItem.GetComponent<SellableItem>().upgradedItem.GetComponent<SellableItem>();
		}
		if(sellItem != null){
			if(sellItem.currentUpgrade <= 2){
				sellItem.cost = vendorReader.GetCurrentFortificationCost(sellItem.cost, sellItem.itemName, sellItem.currentUpgrade);
			}
			
			if(GameController.Instance.GetResources() >= sellItem.cost){
				if(GUI.Button(new Rect(drawRect.width-180, drawRect.height-250, 150, 50), "UPGRADE: "+sellItem.cost, style)){
					itemVendor.Upgrade(curItem);
				}
				
				GUI.Label(new Rect(drawRect.width-200, drawRect.height-200, 200, 100), 
					sellItem.GetComponent<SellableItem>().description, descriptionStyle);
			} else {
				GUI.Label(new Rect(drawRect.width-180, drawRect.height-250, 150, 50), "NOT ENOUGH FUNDS", style);
			}
		} else {
			GUI.Label(new Rect(drawRect.width-180, drawRect.height-250, 150, 50), "FULLY UPGRADED", style);
		}
		
		if(GUI.Button(new Rect(drawRect.width-150, drawRect.height-100, 100, 50), "BACK", style)){
			state = State.FORTINFO;
		}
		
		GUI.EndGroup();
	}
	
	void DestroyFortification(){
		// Destory the object and update graph
		Health fortHealth = UIManager.Instance.fortification.GetComponent<Health>();
		if(fortHealth.curHealth == fortHealth.GetMaxHealth() && UIManager.Instance.fortification.GetComponent<Dragable>() != null){
			GameController.Instance.AddResources(UIManager.Instance.fortification.GetComponent<SellableItem>().cost);
		} else {
			GameController.Instance.AddResources(Mathf.RoundToInt(fortHealth.curHealth / 2));
		}
		GameController.Instance.UpdateGraphOnDestroyedObject(UIManager.Instance.fortification.collider,UIManager.Instance.fortification.gameObject);
		UIManager.Instance.fortification = null;
		UIManager.Instance.uiState = UIManager.UIState.NONE;
	}

	void DrawYesOrNoScreen(Rect drawRect, GUIStyle style){
		
		GUI.BeginGroup(drawRect);
		
		GUI.DrawTexture(new Rect(0, 0, drawRect.width, drawRect.height), backGround);
		string[] fortInfo = new string[2]{"YES", "NO"};
		for(int i=0; i<fortInfo.Length; i++){
			string curFortInfo = fortInfo[i];
			if(GUI.Button(new Rect(drawRect.width-150, drawRect.height-250+(i*60), 100, 50), curFortInfo, style)){
				if(curFortInfo == fortInfo[0]){
					// Destory the object and update graph
					Health fortHealth = UIManager.Instance.fortification.GetComponent<Health>();
					if(fortHealth.curHealth == fortHealth.GetMaxHealth() && UIManager.Instance.fortification.GetComponent<Dragable>() != null){
						GameController.Instance.AddResources(UIManager.Instance.fortification.GetComponent<SellableItem>().cost);
					} else {
						GameController.Instance.AddResources(Mathf.RoundToInt(fortHealth.curHealth / 2));
					}
					GameController.Instance.UpdateGraphOnDestroyedObject(UIManager.Instance.fortification.collider,UIManager.Instance.fortification.gameObject);
					UIManager.Instance.fortification = null;
					UIManager.Instance.uiState = UIManager.UIState.NONE;
				} else if(curFortInfo == fortInfo[1]){
					state = State.FORTINFO;
				}
			}
		}
		GUI.EndGroup();
	}
}
