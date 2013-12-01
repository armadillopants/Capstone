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
	
	void Start(){
		itemVendor = GameObject.Find("Vendor").GetComponent<ItemVendor>();
	}
	
	public void Reset(){
		state = State.FORTINFO;
		curItem = itemVendor.upgradeItemVendor;
	}
	
	public void Draw(Rect drawRect){
		Rect drawArea = drawRect;
		
		GUIStyle style = new GUIStyle();
		style.normal.background = buttonNormal;
		style.hover.background = buttonHover;
		style.active.background = buttonActive;
		style.normal.textColor = Color.white;
		style.hover.textColor = Color.white;
		style.active.textColor = Color.white;
		style.alignment = TextAnchor.MiddleCenter;
		style.font = font;
		
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
		
		string[] fortInfo = new string[3]{"Upgrade", "Destroy", "Cancel"};
		for(int i=0; i<fortInfo.Length; i++){
			string curFortInfo = fortInfo[i];
			if(GUI.Button(new Rect(drawRect.width/4f, drawRect.height/6f+(i*60), 100, 50), curFortInfo, style)){
				if(curFortInfo == fortInfo[0]){
					state = State.UPGRADE;
				} else if(curFortInfo == fortInfo[1]){
					state = State.YESORNO;
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
		
		if(GameController.Instance.GetResources() >= curItem.GetComponent<SellableItem>().cost){
			if(GUI.Button(new Rect(drawRect.width/4f, drawRect.height/6f, 80, 20), "UPGRADE", style)){
				itemVendor.Upgrade(curItem);
			}
		}
		
		GUI.EndGroup();
	}

	void DrawYesOrNoScreen(Rect drawRect, GUIStyle style){
		
		GUI.BeginGroup(drawRect);
		
		GUI.DrawTexture(new Rect(0, 0, drawRect.width, drawRect.height), backGround);
		string[] fortInfo = new string[2]{"Yes", "No"};
		for(int i=0; i<fortInfo.Length; i++){
			string curFortInfo = fortInfo[i];
			if(GUI.Button(new Rect(drawRect.width/4f, drawRect.height/6f+(i*60), 100, 50), curFortInfo, style)){
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
