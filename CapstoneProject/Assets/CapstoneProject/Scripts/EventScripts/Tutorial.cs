using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {
	
	public GameObject WASD;
	public GameObject mouseLeft;
	public GameObject mouseRight;
	public GameObject leftShift;
	
	private Vector3 playerPos;
	public bool beginTutorial = false;
	private bool spawnRightMouse = false;
	private GameObject link = null;
	private float waitTime = 5f;
	public string key = "";
	private string curKey = "";
	
	void Update(){
		if(GameController.Instance.GetPlayer()){
			Transform playerTrans = GameController.Instance.GetPlayer();
			playerPos = playerTrans.position+new Vector3(0,1,0);
			
			if(GameController.Instance.GetPlayer().GetComponent<LocalInput>() != null && GameController.Instance.GetWaveController().GetWaveNumber() == 1 && !beginTutorial){
				StartCoroutine(BeginWASDLink());
				beginTutorial = true;
			}
			
			if(link != null){
				if(key == "Player"){
					link.transform.position = new Vector3(playerPos.x, 1, playerPos.z);
				} else if(key == "LeftClick" || key == "LeftShift"){
					link.transform.position = new Vector3(playerPos.x, 1, playerPos.z) - new Vector3(3,0,0);
				} else if(key == "RightClick"){
					link.transform.position = new Vector3(link.transform.position.x, 1, link.transform.position.z);
				}
			}
			
			if(curKey == "BuildScreen"){
				WaitForBuildScreen();
			} else if(curKey == "QandE"){
				QandE();
			} else if(curKey == "ClickLeft"){
				ClickLeft();
			} else if(curKey == "ClickRight"){
				ClickRight();
			} else if(curKey == "FortPlacement"){
				WaitForFortPlacement();
			} else if(curKey == "DestroyMouseRightLink"){
				DestroyMouseRightLink();
			} else if(curKey == "WeaponScreen"){
				WaitForWeaponScreen();
			} else if(curKey == "AbilityScreen"){
				WaitForAbilityScreen();
			}
		}
	}
	
	public void ResetTutorial(){
		beginTutorial = false;
	}
	
	GameObject Spawn(GameObject g, Vector3 pos, bool spawned){
		if(!spawned){
			spawned = true;
			return (GameObject)Instantiate(g, pos, Quaternion.Euler(90,0,0));
		}
		
		return null;
	}
			
	IEnumerator BeginWASDLink(){
		key = "ProtectShip";
		yield return new WaitForSeconds(waitTime);
		key = "Player";
		yield return new WaitForSeconds(waitTime);
		link = Spawn(WASD, playerPos, false);
		yield return new WaitForSeconds(waitTime);
		Destroy(GameObject.Find(link.name));
		link = null;
		StartCoroutine(BeginMouseLeftLink());
	}
	
	IEnumerator BeginMouseLeftLink(){
		key = "LeftClick";
		link = Spawn(mouseLeft, playerPos, false);
		yield return new WaitForSeconds(waitTime);
		Destroy(GameObject.Find(link.name));
		link = null;
		StartCoroutine(BeginLeftShiftLink());
	}
	
	IEnumerator BeginLeftShiftLink(){
		key = "LeftShift";
		link = Spawn(leftShift, playerPos, false);
		yield return new WaitForSeconds(waitTime);
		Destroy(GameObject.Find(link.name));
		link = null;
	}
	
	void WaitForBuildScreen(){
		if(UIManager.Instance.uiState == UIManager.UIState.FORT_BUILD_SCREEN){
			key = "PurchaseFort";
		} else {
			if(GameObject.FindWithTag(Globals.FORTIFICATION)){
				key = "QandE";
			} else {
				key = "BuildScreen";
			}
		}
	}
	
	void QandE(){
		if(Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E)){
			key = "ClickLeft";
			ClickLeft();
		}
		
		if(Input.GetKey(KeyCode.Mouse1)){
			key = "BuildScreen";
		}
	}
	
	void ClickLeft(){
		if(Input.GetKey(KeyCode.Mouse0)){
			key = "ClickRight";
			ClickRight();
		}
		
		if(Input.GetKey(KeyCode.Mouse1)){
			key = "BuildScreen";
		}
	}
	
	void ClickRight(){
		if(Input.GetKey(KeyCode.Mouse1)){
			curKey = "FortPlacement";
			key = "";
		}
	}
	
	void WaitForFortPlacement(){
		if(GameObject.FindWithTag(Globals.FORTIFICATION).GetComponent<Dragable>().canUpdate){
			if(!spawnRightMouse){
				BeginMouseRightLinks();
				spawnRightMouse = true;
			}
		}
	}
	
	void BeginMouseRightLinks(){
		if(GameObject.FindWithTag(Globals.FORTIFICATION)){
			key = "RightClick";
			link = Spawn(mouseRight, GameObject.FindWithTag(Globals.FORTIFICATION).transform.position-new Vector3(2,0,0), false);
			curKey = "DestroyMouseRightLink";
		}
	}
	
	void DestroyMouseRightLink(){
		waitTime -= Time.deltaTime;
		if(waitTime <= 0){
			Destroy(GameObject.Find(link.name));
			link = null;
			key = "WeaponScreen";
			waitTime = 15f;
		}
	}
	
	void WaitForWeaponScreen(){
		if(UIManager.Instance.uiState == UIManager.UIState.FORT_WEAPON_SCREEN){
			waitTime -= Time.deltaTime;
			if(waitTime >= 10f){
				key = "PurchaseAmmo";
			} else if(waitTime >= 5f && waitTime < 10f){
				key = "PurchaseWeapon"; 
			} else if(waitTime >= 0f && waitTime < 5f) {
				key = "AbilityScreen";
				waitTime = 15f;
			}
		} else {
			key = "WeaponScreen";
		}
	}
	
	void WaitForAbilityScreen(){
		if(UIManager.Instance.uiState == UIManager.UIState.FORT_ABILITY_SCREEN){
			waitTime -= Time.deltaTime;
			if(waitTime >= 10f){
				key = "PurchaseAbility";
			} else if(waitTime >= 5f && waitTime < 10f){
				key = "BeginWaveScreen";
			} else if(waitTime >=0f && waitTime < 5f){
				key = "";
				curKey = "";
			}
		} else {
			key = "AbilityScreen";
		}
	}
	
	void DrawScreen(string text, int fontSize){
		Rect shipRect = new Rect((Screen.width/2) - (900/2), /*(Screen.height/2) - (80/2) - 250*/(Screen.height-Screen.height)+80, 900, 80);
		
		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.MiddleCenter;
		style.normal.textColor = Color.white;
		style.font = UIManager.Instance.resourceFont;
		style.fontSize = fontSize;
		style.normal.background = UIManager.Instance.resourceBackground;
		
		GUI.BeginGroup(shipRect);
		
		GUI.Label(new Rect(0, 0, shipRect.width, shipRect.height), text, style);
		
		GUI.EndGroup();
	}
	
	void OnGUI(){
		if(key == "ProtectShip"){
			DrawScreen("PROTECT the SHIP at ALL COSTS", 30);
		} else if(key == "BuildScreen"){
			DrawScreen("Click BUILD to access fortifications", 30);
			curKey = key;
		} else if(key == "PurchaseFort"){
			DrawScreen("Purchase the BARRIER", 30);
		} else if(key == "QandE"){
			DrawScreen("Q or E to rotate current fortification", 30);
			curKey = key;
		} else if(key == "ClickLeft"){
			DrawScreen("LEFT CLICK to place current fortification", 30);
			curKey = key;
		} else if(key == "ClickRight"){
			DrawScreen("RIGHT CLICK to cancel current fortification", 30);
			curKey = key;
		} else if(key == "RightClick"){
			DrawScreen("RIGHT CLICK over fortification to UPGRADE it", 30);
		} else if(key == "WeaponScreen"){
			DrawScreen("Click WEAPONS to access weaponry", 30);
			curKey = key;
		} else if(key == "AbilityScreen"){
			DrawScreen("Click ABILITIES to access abilities", 30);
			curKey = key;
		} else if(key == "BeginWaveScreen"){
			DrawScreen("Click BEGIN to begin the next wave", 30);
		} else if(key == "PurchaseAmmo"){
			DrawScreen("REFILL AMMO for equipped weapon slots", 30);
		} else if(key == "PurchaseWeapon"){
			DrawScreen("BUY weapons, UPGRADE, and EQUIP them", 30);
		} else if(key == "PurchaseAbility"){
			DrawScreen("BUY abilities for help in tight situations", 30);
		}
	}
}
