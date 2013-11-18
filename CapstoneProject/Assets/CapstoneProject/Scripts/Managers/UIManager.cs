using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
	
	public enum UIState { PAUSE, 
		WAVEWON, WAVELOST, NEXTWAVE, 
		GAMEOVER, NONE, CURWAVE, 
		FORTINFO, YESORNO, BUILD_SCREEN, 
		UPGRADE_SCREEN, BUY_SCREEN, EQUIP_WEAPON_SCREEN, 
		FORT_UPGRADE_SCREEN, GAMEWON };
	public UIState uiState = UIState.NONE;
	
	public bool isPaused = false;
	private WeaponSelection selection;
	private bool fadeComplete = false;
	
	private GameObject fortification;
	private Rect displayScreen = new Rect(Screen.width/2f - 50f, Screen.height/2f - 100f, 200, 300);
	
	private Texture2D playerHealthBar;
	private Texture2D shipHealthBar;
	private Texture2D grayBar;
	
	private Rect playerHealthRect;
	private Rect shipHealthRect;
	private Rect resourceRect;
	
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
		
		shipHealthBar = new Texture2D(1,1,TextureFormat.RGB24, false);
		shipHealthBar.SetPixel(0, 0, Color.green);
		shipHealthBar.Apply();
		
		grayBar = new Texture2D(1, 1, TextureFormat.RGB24, false);
		grayBar.SetPixel(0, 0, Color.gray);
		grayBar.Apply();
		
		playerHealthRect = new Rect(20, Screen.height-85, 135, 22);
		shipHealthRect = new Rect(20, Screen.height-55, 135, 22);
		resourceRect = new Rect(20, Screen.height-30, 135, 22);
	}
	
	void Update(){
		if(isPaused){
			Time.timeScale = 0;
		} else {
			if(!selection.changingWeapons){
				Time.timeScale = 1;
			}
		}
	}
	
	public void SetFortification(GameObject fort){
		fortification = fort;
		uiState = UIState.FORTINFO;
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
		case UIState.FORTINFO:
			DrawFortInfo();
			break;
		case UIState.YESORNO:
			DrawYesOrNoScreen();
			break;
		case UIState.GAMEWON:
			DrawGameWonScreen();
			break;
		}
		
		if(MenuManager.Instance.menuState == MenuManager.MenuState.INGAME && !GameController.Instance.GetPlayerHealth().IsDead){
			DrawPlayerHealth();
			DrawShipHealth();
			DrawResources();
		}
	}
	
	void DrawPauseScreen(){
		if(GUI.Button(new Rect(Screen.width/2, Screen.height/2, 100, 50), "Resume")){
			isPaused = !isPaused;
			UIManager.Instance.uiState = UIState.NONE;
		}
		if(GUI.Button(new Rect(Screen.width/2, Screen.height/4, 100, 50), "Restart")){
			Application.LoadLevel(Application.loadedLevel);
		}
		if(GUI.Button(new Rect(Screen.width/2, Screen.height/3, 100, 50), "Quit")){
			Application.Quit();
		}
	}
	
	void DrawWaveWonScreen(){
		GUI.Box(new Rect(Screen.width/3, Screen.height/5, 400, 300), "Wave Won");
	}
	
	void DrawWaveLostScreen(){
		GUI.Box(new Rect(Screen.width/3, Screen.height/5, 400, 300), "Wave Lost");
	}

	void DrawNextWaveScreen(){
		GUI.Box(new Rect(Screen.width/3, Screen.height/5, 400, 300), "Next Wave");
	}
	
	void DrawGameOverScreen(){
		if(fadeComplete){
			if(GUI.Button(new Rect(Screen.width/3, Screen.height/3, 100, 50), "Restart")){
				GameController.Instance.Reset();
				MenuManager.Instance.menuState = MenuManager.MenuState.ENDGAME;
				uiState = UIState.NONE;
			}
		}
	}

	void DrawGameWonScreen(){
		GUI.Box(new Rect(Screen.width/3, Screen.height/5, 400, 300), "YOU WON!");
	}
	
	void DrawCurWaveScreen(){
		
	}
	
	void DrawPlayerHealth(){
		GUI.BeginGroup(playerHealthRect);
		{
			GUI.DrawTexture(new Rect(0, 0, 
				playerHealthRect.width*GameController.Instance.GetPlayerHealth().GetMaxHealth(), playerHealthRect.height), 
				grayBar, ScaleMode.StretchToFill);
			GUI.DrawTexture(new Rect(0, 0, 
				playerHealthRect.width*GameController.Instance.GetPlayerHealth().curHealth/GameController.Instance.GetPlayerHealth().GetMaxHealth(), playerHealthRect.height), 
				playerHealthBar, ScaleMode.StretchToFill);
		}
		GUI.EndGroup();
	}
	
	void DrawShipHealth(){
		GUI.BeginGroup(shipHealthRect);
		{
			GUI.DrawTexture(new Rect(0, 0, 
				shipHealthRect.width*GameController.Instance.GetShipHealth().GetMaxHealth(), shipHealthRect.height), 
				grayBar, ScaleMode.StretchToFill);
			GUI.DrawTexture(new Rect(0, 0, 
				shipHealthRect.width*GameController.Instance.GetShipHealth().curHealth/GameController.Instance.GetShipHealth().GetMaxHealth(), shipHealthRect.height), 
				shipHealthBar, ScaleMode.StretchToFill);
		}
		GUI.EndGroup();
	}
	
	void DrawResources(){
		GUI.BeginGroup(resourceRect);
		{
			GUI.Label(new Rect(0, 0, resourceRect.width, resourceRect.height), "Resources: " + GameController.Instance.GetResources());
		}
		GUI.EndGroup();
	}

	void DrawYesOrNoScreen(){
		GUI.BeginGroup(displayScreen);
		
		GUI.Box(new Rect(0, 0, displayScreen.width, displayScreen.height), "ARE YOU SURE");
		string[] fortInfo = new string[2]{"Yes", "No"};
		for(int i=0; i<fortInfo.Length; i++){
			string curFortInfo = fortInfo[i];
			if(GUI.Button(new Rect(displayScreen.width/4f, displayScreen.height/6f+(i*60), 100, 50), curFortInfo)){
				if(curFortInfo == fortInfo[0]){
					// Destory the object and update graph
					Health fortHealth = fortification.GetComponent<Health>();
					if(fortHealth.curHealth == fortHealth.GetMaxHealth() && fortification.GetComponent<Dragable>() != null){
						GameController.Instance.AddResources(fortification.GetComponent<SellableItem>().cost);
					} else {
						GameController.Instance.AddResources(Mathf.RoundToInt(fortHealth.curHealth / 2));
					}
					GameController.Instance.UpdateGraphOnDestroyedObject(fortification.collider,fortification.gameObject);
					fortification = null;
					uiState = UIState.NONE;
				} else if(curFortInfo == fortInfo[1]){
					uiState = UIState.FORTINFO;
				}
			}
		}
		
		GUI.EndGroup();
	}
	
	void DrawFortInfo(){
		GUI.BeginGroup(displayScreen);
		
		GUI.Box(new Rect(0, 0, displayScreen.width, displayScreen.height), "Fort Info");
		string[] fortInfo = new string[3]{"Upgrade", "Destroy", "Cancel"};
		for(int i=0; i<fortInfo.Length; i++){
			string curFortInfo = fortInfo[i];
			if(GUI.Button(new Rect(displayScreen.width/4f, displayScreen.height/6f+(i*60), 100, 50), curFortInfo)){
				if(curFortInfo == fortInfo[0]){
					uiState = UIState.FORT_UPGRADE_SCREEN;
				} else if(curFortInfo == fortInfo[1]){
					uiState = UIState.YESORNO;
				} else if(curFortInfo == fortInfo[2]){
					uiState = UIState.NONE;
				}
			}
		}
		
		GUI.EndGroup();
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
