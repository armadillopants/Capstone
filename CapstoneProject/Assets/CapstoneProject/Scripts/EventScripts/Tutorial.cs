using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {
	
	public GameObject WASD;
	public GameObject mouseLeft;
	public GameObject mouseRight;
	public GameObject leftShift;
	public Texture2D background;
	
	private Vector3 playerPos;
	private bool beginTutorial = false;
	private bool displaySatelliteInfo = true;
	private bool spawnRightMouse = false;
	private GameObject link = null;
	private float waitTime = 5f;
	public string key = "";
	private string curKey = "";
	
	void Update(){
		if(GameController.Instance.GetPlayer() && MenuManager.Instance.menuState == MenuManager.MenuState.INGAME){
			Transform playerTrans = GameController.Instance.GetPlayer();
			playerPos = playerTrans.position+new Vector3(0,1,0);
			
			if(GameController.Instance.GetPlayer().GetComponent<LocalInput>() != null 
				&& GameController.Instance.GetWaveController().GetWaveNumber() == 1 && !beginTutorial){
				
				StartCoroutine(BeginWASDLink());
				beginTutorial = true;
				UIManager.Instance.displayUI = true;
			}
			
			if(link != null){
				if(key == "Player"){
					link.transform.position = new Vector3(playerPos.x, 1, playerPos.z);
				} else if(key == "LeftClick" || key == "LeftShift" || key == "RightClickToReload"){
					link.transform.position = new Vector3(playerPos.x, 1, playerPos.z) - new Vector3(3,0,0);
				} else if(key == "RightClick"){
					link.transform.position = new Vector3(link.transform.position.x, 1, link.transform.position.z);
				}
			}
			
			if(displaySatelliteInfo){
				if(GameController.Instance.GetResources() >= 1000 
					&& GameController.Instance.GetWaveController().GetComponent<Fortification>() != null){
					
					curKey = "DisplaySatelliteInfo";
				}
			}
			
			if(UIManager.Instance.uiState == UIManager.UIState.GAMEOVER){
				curKey = "";
				key = "";
				StopAllCoroutines();
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
			} else if(curKey == "BoughtAbility"){
				DisplayAbilityUsage();
			} else if(curKey == "DisplayEnemyIncrease"){
				DisplayEnemyIncrease();
			} else if(curKey == "DisplaySatelliteInfo"){
				DisplaySatelliteInfo();
			}
		}
	}
	
	public void SetKey(string cur){
		curKey = cur;
	}
	
	public void ResetTutorial(){
		beginTutorial = false;
		StopAllCoroutines();
		Destroy(link);
		link = null;
		key = "";
		curKey = "";
		spawnRightMouse = false;
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
		key = "";
		yield return new WaitForSeconds(waitTime);
		key = "Player";
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
		StartCoroutine(BeginMouseRightLink());
	}
	
	IEnumerator BeginMouseRightLink(){
		key = "RightClickToReload";
		link = Spawn(mouseRight, playerPos, false);
		yield return new WaitForSeconds(waitTime);
		Destroy(GameObject.Find(link.name));
		key = "";
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
			StartCoroutine(IHateHacks());
			ClickRight();
		}
		
		if(Input.GetKey(KeyCode.Mouse1)){
			key = "BuildScreen";
		}
	}
	
	IEnumerator IHateHacks(){
		yield return new WaitForSeconds(0.1f);
		key = "ClickRight";
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
				waitTime = 10f;
				UIManager.Instance.uiState = UIManager.UIState.NONE;
			}
		} else {
			key = "AbilityScreen";
		}
	}
	
	void DisplayAbilityUsage(){
		waitTime -= Time.deltaTime;
		if(waitTime > 0){
			key = "BoughtAbility";
		} else {
			key = "";
			curKey = "";
			waitTime = 10f;
		}
	}
	
	void DisplayEnemyIncrease(){
		waitTime -= Time.deltaTime;
		if(waitTime < 0){
			key = "";
			curKey = "";
			waitTime = 10f;
		}
	}
	
	void DisplaySatelliteInfo(){
		if(UIManager.Instance.uiState == UIManager.UIState.FORT_BUILD_SCREEN){
			waitTime -= Time.deltaTime;
			if(waitTime <= 10f && waitTime >= 5f){
				key = "DisplaySatelliteInfo";
			} else if(waitTime >= 0f && waitTime < 5f){
				key = "DisplaySatelliteInfo2";
			} else {
				key = "";
				curKey = "";
				waitTime = 10f;
				displaySatelliteInfo = false;
			}
		}
	}
	
	void DrawScreen(string text){
		Rect rect = new Rect((Screen.width/2) - (900/2),(Screen.height-Screen.height)+80, 900, 80);
		
		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.MiddleCenter;
		style.normal.textColor = Color.yellow;
		style.font = UIManager.Instance.resourceFont;
		style.fontSize = 30;
		style.normal.background = background;
		
		GUI.BeginGroup(rect);
		
		GUI.Label(new Rect(0, 0, rect.width, rect.height), text, style);
		
		GUI.EndGroup();
	}
	
	void OnGUI(){
		if(key == "ProtectShip"){
			DrawScreen("PROTECT the SHIP at ALL COSTS");
		} else if(key == "BuildScreen"){
			DrawScreen("Click BUILD to access fortifications");
			curKey = key;
		} else if(key == "PurchaseFort"){
			DrawScreen("Purchase the BARRIER");
		} else if(key == "QandE"){
			DrawScreen("Q or E to rotate current fortification");
			curKey = key;
		} else if(key == "ClickLeft"){
			DrawScreen("LEFT CLICK to place current fortification");
			curKey = key;
		} else if(key == "ClickRight"){
			DrawScreen("RIGHT CLICK to cancel current fortification");
			curKey = key;
		} else if(key == "RightClick"){
			DrawScreen("RIGHT CLICK over fortification to UPGRADE it");
		} else if(key == "WeaponScreen"){
			DrawScreen("Click WEAPONS to access weaponry");
			curKey = key;
		} else if(key == "AbilityScreen"){
			DrawScreen("Click ABILITIES to access abilities");
			curKey = key;
		} else if(key == "BeginWaveScreen"){
			DrawScreen("Click BEGIN when ready to begin the next wave");
		} else if(key == "PurchaseAmmo"){
			DrawScreen("REFILL AMMO for equipped weapon slots");
		} else if(key == "PurchaseWeapon"){
			DrawScreen("BUY weapons, UPGRADE, and EQUIP them");
		} else if(key == "PurchaseAbility"){
			DrawScreen("BUY abilities for help in tight situations");
		} else if(key == "BoughtAbility"){
			DrawScreen("Press 'E' to use current ability");
			curKey = key;
		} else if(key == "Player"){
			DrawScreen("WASD to move Player");
		} else if(key == "LeftClick"){
			DrawScreen("LEFT CLICK to shoot");
		} else if(key == "LeftShift"){
			DrawScreen("Hold LEFT SHIFT to switch weapons");
		} else if(key == "RightClickToReload"){
			DrawScreen("RIGHT CLICK to reload weapon");
		} else if(curKey == "DisplayEnemyIncrease"){
			DrawScreen("Enemy HEALTH, SPEED, and ATTACK increased!");
		} else if(key == "DisplaySatelliteInfo"){
			DrawScreen("You can now purchase the Satellite Tower!");
		} else if(key == "DisplaySatelliteInfo2"){
			DrawScreen("Buy the Satellite Tower to call for rescue!");
		}
	}
}
