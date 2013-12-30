using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour {
	
	public enum UIState { PAUSE, 
		WAVEWON, WAVELOST, NEXTWAVE, 
		GAMEOVER, NONE, CURWAVE, FORT_BUILD_SCREEN,
		FORT_WEAPON_SCREEN, FORT_UPGRADE_SCREEN, 
		FORT_ABILITY_SCREEN, GAMEWON, CURRENT_FORT_INFO };
	public UIState uiState = UIState.NONE;
	
	public bool isPaused = false;
	private WeaponSelection selection;
	private bool fadeComplete = false;
	
	public GameObject fortification;
	
	private Texture2D playerHealthBar;
	private Texture2D shipHealthBar;
	private Texture2D grayBar;
	
	private Rect waveRect;
	private Rect playerHealthRect;
	private Rect shipHealthRect;
	private Rect resourceRect;
	private Rect waveNumberRect;
	private Rect fortDisplayRect;
	private bool displayFortData = false;
	private Health fortHealth;
	private BaseWeapon fortWeapon;
	
	public Texture2D resourceBackground;
	public Font resourceFont;
	
	public bool displayUI = true;
	
	private WaveController waveController;
	
	#region Singleton
	
	private static UIManager _instance;

	public static UIManager Instance {
		get { return _instance; }
	}

	void Awake(){
		if(UIManager.Instance != null){
			DestroyImmediate(gameObject);
			return;
		}
		_instance = this;
		
		selection = GameObject.Find("WeaponSelection").GetComponent<WeaponSelection>();
	}

	void OnApplicationQuit(){
		_instance = null;
	}
	
	#endregion
	
	public bool FadeCompleted {
		set { fadeComplete = value; }
	}
	
	void Start(){
		playerHealthBar = new Texture2D(1, 1, TextureFormat.RGB24, false);
		playerHealthBar.SetPixel(0, 0, Color.red);
		playerHealthBar.Apply();
		
		shipHealthBar = new Texture2D(1, 1, TextureFormat.RGB24, false);
		shipHealthBar.SetPixel(0, 0, Color.green);
		shipHealthBar.Apply();
		
		grayBar = new Texture2D(1, 1, TextureFormat.RGB24, false);
		grayBar.SetPixel(0, 0, Color.gray);
		grayBar.Apply();
		
		waveRect = new Rect((Screen.width/2) - (750/2), (Screen.height/2) - (600/2) - 100, 750, 600);
		waveNumberRect = new Rect(50, Screen.height-50, 150, 50);
		
		playerHealthRect = new Rect(Screen.width-135, Screen.height-25, 135, 25);
		resourceRect = new Rect((Screen.width/2)-(300/2), (Screen.height-Screen.height)+20, 300, 50);
		
		waveController = GameObject.Find("WaveController").GetComponent<WaveController>();
	}
	
	void Update(){
		if(isPaused){
			Time.timeScale = 0;
		} else {
			if(!selection.changingWeapons){
				Time.timeScale = 1;
			}
		}
		
		Vector3 shipPos = Camera.main.WorldToScreenPoint(GameController.Instance.GetShip().position);
		shipHealthRect = new Rect(shipPos.x, Screen.height - shipPos.y, 400, 20);
		
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit = new RaycastHit();
		if(Physics.Raycast(ray, out hit)){
			if(hit.transform.tag == Globals.FORTIFICATION && GameController.Instance.current == null){
				Vector3 fortPos = Camera.main.WorldToScreenPoint(hit.point);
				fortHealth = hit.transform.GetComponent<Health>();
				if(hit.transform.GetComponentInChildren<BaseWeapon>() != null){
					fortWeapon = hit.transform.GetComponentInChildren<BaseWeapon>();
					Logger.Log("Found weapon");
				}
				fortDisplayRect = new Rect(fortPos.x, Screen.height-fortPos.y, 50, 10);
				displayFortData = true;
			} else {
				displayFortData = false;
			}
		}
	}
	
	public void SetFortification(GameObject fort){
		fortification = fort;
		uiState = UIState.FORT_UPGRADE_SCREEN;
		BuildUpgradeGUI build = GameObject.Find("Vendor").GetComponent<BuildUpgradeGUI>();
		build.Reset();
	}
	
	void OnGUI(){
		switch(uiState){
		case UIState.PAUSE:
			DrawPauseScreen();
			break;
		case UIState.WAVEWON:
			DrawWaveWonScreen();
			break;
		case UIState.NEXTWAVE:
			DrawNextWaveScreen();
			break;
		case UIState.CURWAVE:
			DrawCurWaveScreen();
			break;
		case UIState.WAVELOST:
			DrawWaveLostScreen();
			break;
		case UIState.GAMEOVER:
			DrawGameOverScreen();
			break;
		case UIState.GAMEWON:
			DrawGameWonScreen();
			break;
		case UIState.CURRENT_FORT_INFO:
			DrawCurrentFortInfoScreen();
			break;
		}
		
		if(GameController.Instance.GetPlayer().GetComponent<PlayerMovement>() != null){
			if(displayUI){
				DrawPlayerHealth();
				DrawShipHealth();
				DrawResources();
				DrawCurWaveScreen();
				if(displayFortData){
					DrawFortDisplay();
				}
			}
		}
	}
	
	void DrawPauseScreen(){
		if(GUI.Button(new Rect((Screen.width/2) - (100/2), (Screen.height/2) - (50/2)-50, 100, 50), "Resume")){
			isPaused = !isPaused;
			UIManager.Instance.uiState = UIState.NONE;
		}
		if(GUI.Button(new Rect((Screen.width/2) - (100/2), (Screen.height/2) - (50/2), 100, 50), "Restart")){
			Application.LoadLevel(Application.loadedLevel);
		}
		if(GUI.Button(new Rect((Screen.width/2) - (100/2), (Screen.height/2) - (50/2)+50, 100, 50), "Quit")){
			Application.Quit();
		}
	}
	
	void DrawWaveWonScreen(){
				
		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.MiddleCenter;
		style.normal.textColor = Color.white;
		style.font = resourceFont;
		style.fontSize = 100;
		
		GUI.BeginGroup(waveRect);
		
		GUI.Label(new Rect(0, 0, waveRect.width, waveRect.height), "WAVE WON", style);
		
		GUI.EndGroup();
	}
	
	void DrawWaveLostScreen(){
				
		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.MiddleCenter;
		style.normal.textColor = Color.white;
		style.font = resourceFont;
		style.fontSize = 100;
		
		GUI.BeginGroup(waveRect);
		
		GUI.Label(new Rect(0, 0, waveRect.width, waveRect.height), "WAVE LOST", style);
		
		GUI.EndGroup();
	}

	void DrawNextWaveScreen(){
		
		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.MiddleCenter;
		style.normal.textColor = Color.white;
		style.font = resourceFont;
		style.fontSize = 100;
		
		GUI.BeginGroup(waveRect);
		
		GUI.Label(new Rect(0, 0, waveRect.width, waveRect.height), "WAVE "+waveController.GetWaveNumber(), style);
		
		GUI.EndGroup();
	}
	
	void DrawGameOverScreen(){
		if(fadeComplete){
			if(GUI.Button(new Rect((Screen.width/2f)-(100/2), (Screen.height/2)-(50/2), 100, 50), "Restart")){
				GameController.Instance.Reset();
				MenuManager.Instance.menuState = MenuManager.MenuState.ENDGAME;
				uiState = UIState.NONE;
			}
			if(GUI.Button(new Rect((Screen.width/2f)-(100/2), (Screen.height/2)-(50/2)+50, 100, 50), "Quit")){
				Application.Quit();
			}
		}
	}

	void DrawGameWonScreen(){
				
		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.MiddleCenter;
		style.normal.textColor = Color.white;
		style.font = resourceFont;
		style.fontSize = 100;
		
		GUI.BeginGroup(waveRect);
		
		GUI.Label(new Rect(0, 0, waveRect.width, waveRect.height), "GAME WON!", style);
		
		GUI.EndGroup();
	}
	
	void DrawCurWaveScreen(){
				
		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.MiddleCenter;
		style.normal.textColor = Color.white;
		style.font = resourceFont;
		style.fontSize = 50;
		
		GUI.BeginGroup(waveNumberRect);
		
		GUI.Label(new Rect(0, 0, waveNumberRect.width, waveNumberRect.height), waveController.GetWaveNumber().ToString(), style);
		
		GUI.EndGroup();
	}
	
	void DrawPlayerHealth(){
		GUI.BeginGroup(playerHealthRect);
		
		GUI.DrawTexture(new Rect(0, 0, 
			playerHealthRect.width*GameController.Instance.GetPlayerHealth().GetMaxHealth(), playerHealthRect.height), 
			grayBar, ScaleMode.StretchToFill);
		GUI.DrawTexture(new Rect(0, 0, 
			playerHealthRect.width*GameController.Instance.GetPlayerHealth().curHealth/GameController.Instance.GetPlayerHealth().GetMaxHealth(), playerHealthRect.height), 
			playerHealthBar, ScaleMode.StretchToFill);

		GUI.EndGroup();
	}
	
	void DrawShipHealth(){
		GUI.BeginGroup(shipHealthRect);
		
		GUI.DrawTexture(new Rect(0, 0, 
			shipHealthRect.width*GameController.Instance.GetShipHealth().GetMaxHealth(), shipHealthRect.height), 
			grayBar, ScaleMode.StretchToFill);
		GUI.DrawTexture(new Rect(0, 0, 
			shipHealthRect.width*GameController.Instance.GetShipHealth().curHealth/GameController.Instance.GetShipHealth().GetMaxHealth(), shipHealthRect.height), 
			shipHealthBar, ScaleMode.StretchToFill);
		
		GUI.EndGroup();
	}
	
	void DrawFortDisplay(){
		GUI.BeginGroup(fortDisplayRect);
		
		GUI.DrawTexture(new Rect(0, 0, 
			fortDisplayRect.width*fortHealth.GetMaxHealth(), fortDisplayRect.height), 
			grayBar, ScaleMode.StretchToFill);
		GUI.DrawTexture(new Rect(0, 0, 
			fortDisplayRect.width*fortHealth.curHealth/fortHealth.GetMaxHealth(), fortDisplayRect.height), 
			playerHealthBar, ScaleMode.StretchToFill);
		
		/*GUIStyle style = new GUIStyle();
		style.normal.background = resourceBackground;
		style.normal.textColor = Color.white;
		style.font = resourceFont;
		style.alignment = TextAnchor.MiddleCenter;
		
		GUI.Label(new Rect(0, 0, fortDisplayRect.width, fortDisplayRect.height), 
							"Ammo: "+fortWeapon.bulletsLeft+"/"+fortWeapon.clips, style);*/
		
		GUI.EndGroup();
	}
	
	void DrawResources(){
		GUI.BeginGroup(resourceRect);
		
		GUIStyle style = new GUIStyle();
		style.normal.background = resourceBackground;
		style.font = resourceFont;
		style.alignment = TextAnchor.MiddleCenter;
		GUI.Label(new Rect(0, 0, resourceRect.width, resourceRect.height), "RESOURCES: " + GameController.Instance.GetResources(), style);
		
		GUI.EndGroup();
	}

	void DrawCurrentFortInfoScreen(){
		GUILayout.Label("LEFT CLICK to place current fortification");
		GUILayout.Label("RIGHT CLICK to cancel current fortification");
		GUILayout.Label("Q and E to rotate current fortification");
	}
	
	public IEnumerator FadeComplete(){
		yield return new WaitForSeconds(8f);
		fadeComplete = true;
	}
	
	public IEnumerator Fade(){
		float duration = 1f;
		GameObject fade = new GameObject();
		fade.AddComponent(typeof(GUITexture));
		fade.guiTexture.pixelInset = new Rect(0, 0, Screen.width, Screen.height);
		Texture2D tex = new Texture2D(1, 1);
		tex.SetPixel(0, 0, Color.black);
		tex.Apply();
		fade.guiTexture.texture = tex;
		
  	    float alpha = 0f;
		Color temp = fade.guiTexture.color;
  		while(alpha < 1.0f && uiState == UIState.GAMEOVER){
	    	alpha += Time.deltaTime / (duration*10f);
	    	temp.a = alpha;
			fade.guiTexture.color = temp;
	    	yield return new WaitForSeconds(Time.deltaTime);
  		}
		while(uiState == UIState.GAMEOVER){
			yield return new WaitForSeconds(Time.deltaTime);
		}
		while(alpha > 0f && uiState != UIState.GAMEOVER){
	    	alpha -= Time.deltaTime / (duration*0.05f);
	    	temp.a = Mathf.Max(alpha, 0f);
			fade.guiTexture.color = temp;
	    	yield return new WaitForSeconds(Time.deltaTime);
  		}
		Destroy(fade);
	}
}
