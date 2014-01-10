using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {
	
	public GameObject WASD;
	public GameObject mouseLeft;
	public GameObject mouseRight;
	public GameObject leftShift;
	public Texture2D arrow;
	
	private WaveController waveController;
	private Vector3 playerPos;
	private bool beginTutorial = false;
	private bool spawnRightMouse = false;
	private GameObject link = null;
	private float waitTime = 5f;
	public string key = "";
	
	void Start(){
		waveController = GameObject.Find("WaveController").GetComponent<WaveController>();
	}
	
	void Update(){
		Transform playerTrans = GameController.Instance.GetPlayer();
		playerPos = playerTrans.position+new Vector3(0,1,0);
		
		if(GameController.Instance.GetPlayer().GetComponent<LocalInput>() != null && waveController.GetWaveNumber() == 1 && !beginTutorial){
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
	
	IEnumerator WaitForBuildScreen(){
		if(UIManager.Instance.uiState == UIManager.UIState.FORT_BUILD_SCREEN){
			yield return new WaitForSeconds(1f);
			if(GameObject.FindWithTag(Globals.FORTIFICATION)){
				key = "QandE";
				yield return new WaitForSeconds(waitTime);
				key = "ClickLeft";
				yield return new WaitForSeconds(waitTime);
				key = "ClickRight";
				yield return new WaitForSeconds(waitTime);
				StartCoroutine(WaitForFortPlacement());
			} else {
				key = "PurchaseFort";
				StartCoroutine(WaitForBuildScreen());
			}
		} else {
			yield return new WaitForSeconds(Time.deltaTime);
		}
	}
	
	IEnumerator WaitForFortPlacement(){
		if(GameObject.FindWithTag(Globals.FORTIFICATION).GetComponent<Dragable>().canUpdate){
			yield return new WaitForSeconds(1f);
			if(!spawnRightMouse){
				StartCoroutine(BeginMouseRightLink());
				spawnRightMouse = true;
			}
		} else {
			yield return new WaitForSeconds(Time.deltaTime);
		}
	}
	
	IEnumerator BeginMouseRightLink(){
		if(GameObject.FindWithTag(Globals.FORTIFICATION)){
			key = "RightClick";
			link = Spawn(mouseRight, GameObject.FindWithTag(Globals.FORTIFICATION).transform.position-new Vector3(2,0,0), false);
			yield return new WaitForSeconds(waitTime);
			Destroy(GameObject.Find(link.name));
			link = null;
			key = "WeaponScreen";
		} else {
			yield return new WaitForSeconds(Time.deltaTime);
		}
	}
	
	IEnumerator WaitForWeaponScreen(){
		if(UIManager.Instance.uiState == UIManager.UIState.FORT_WEAPON_SCREEN){
			yield return new WaitForSeconds(1f);
			key = "PurchaseAmmo";
			yield return new WaitForSeconds(waitTime);
			key = "PurchaseWeapon";
			yield return new WaitForSeconds(waitTime);
			key = "AbilityScreen";
		} else {
			yield return new WaitForSeconds(Time.deltaTime);
		}
	}
	
	IEnumerator WaitForAbilityScreen(){
		if(UIManager.Instance.uiState == UIManager.UIState.FORT_ABILITY_SCREEN){
			yield return new WaitForSeconds(1f);
			key = "PurchaseAbility";
			yield return new WaitForSeconds(waitTime);
			key = "BeginWaveScreen";
			yield return new WaitForSeconds(waitTime);
			key = "";
		} else {
			yield return new WaitForSeconds(Time.deltaTime);
		}
	}
	
	void DrawScreen(string text, int fontSize){
		Rect shipRect = new Rect((Screen.width/2) - (900/2), (Screen.height/2) - (80/2) - 250, 900, 80);
		
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
			StartCoroutine(WaitForBuildScreen());
		} else if(key == "PurchaseFort"){
			DrawScreen("Purchase the BARRIER", 30);
		} else if(key == "QandE"){
			DrawScreen("Q or E to rotate current fortification", 30);
		} else if(key == "ClickLeft"){
			DrawScreen("LEFT CLICK to place current fortification", 30);
		} else if(key == "ClickRight"){
			DrawScreen("RIGHT CLICK to cancel current fortification", 30);
		} else if(key == "WeaponScreen"){
			DrawScreen("Click WEAPONS to access weaponry", 30);
			StartCoroutine(WaitForWeaponScreen());
		} else if(key == "AbilityScreen"){
			DrawScreen("Click ABILITIES to access abilities", 30);
			StartCoroutine(WaitForAbilityScreen());
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
