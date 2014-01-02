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
	
	void DrawScreen(string text, int fontSize){
		Rect shipRect = new Rect((Screen.width/2) - (Screen.width/2), (Screen.height/2) - (600/2) - 100, Screen.width, 600);
		
		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.MiddleCenter;
		style.normal.textColor = Color.white;
		style.font = UIManager.Instance.resourceFont;
		style.fontSize = fontSize;
		
		GUI.BeginGroup(shipRect);
		
		GUI.Label(new Rect(0, 0, shipRect.width, shipRect.height), text, style);
		
		GUI.EndGroup();
	}
	
	void OnGUI(){
		if(key == "ProtectShip"){
			DrawScreen("PROTECT the SHIP at ALL COSTS", 50);
		} else if(key == "BuildScreen"){
			DrawScreen("Click BUILD to access fortifications", 30);
			StartCoroutine(WaitForBuildScreen());
		} else if(key == "QandE"){
			DrawScreen("Q or E to rotate current fortification", 30);
		} else if(key == "ClickLeft"){
			DrawScreen("LEFT CLICK to place current fortification", 30);
		} else if(key == "ClickRight"){
			DrawScreen("RIGHT CLICK to cancel current fortification", 30);
		} else if(key == "WeaponScreen"){
			DrawScreen("Click WEAPONS to access weaponry", 30);
		} else if(key == "AbilityScreen"){
			DrawScreen("Click ABILITIES to access abilities", 30);
		} else if(key == "BeginWaveScreen"){
			DrawScreen("Click BEGIN WAVE to begin the next wave", 30);
		}
	}
}
