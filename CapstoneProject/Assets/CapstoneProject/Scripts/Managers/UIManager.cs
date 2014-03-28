using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour {
	
	public enum UIState { PAUSE, 
		WAVEWON, WAVELOST, NEXTWAVE, 
		GAMEOVER, NONE, CURWAVE, FORT_BUILD_SCREEN,
		FORT_WEAPON_SCREEN, FORT_UPGRADE_SCREEN, 
		FORT_ABILITY_SCREEN, GAMEWON };
	public UIState uiState = UIState.NONE;
	
	public bool isPaused = false;
	private WeaponSelection selection;
	private bool fadeComplete = false;
	
	public GameObject fortification;
	
	private Texture2D grayBar;
	
	private Rect waveRect;
	private Rect shipHealthRect;
	private Rect fortHealthDisplayRect;
	private Rect fortWeaponDisplayRect;
	private Rect enemyHealthDisplayRect;
	private bool displayFortHealthData = false;
	private bool displayFortWeaponData = false;
	private bool displayEnemyHealthData = false;
	private Health fortHealth;
	private BaseWeapon fortWeapon;
	private Health enemyHealth;
	
	public Texture2D resourceBackground;
	public Font resourceFont;
	public Texture2D ammoUI;
	public Texture2D shotgunShellUI;
	public Texture2D[] canisters;
	public Texture2D[] batteries;
	public bool displayUI = true;
	public Texture2D infintyUI;
	
	public Texture2D resumeNormal;
	public Texture2D resumeHover;
	public Texture2D resumeActive;
	
	public Texture2D restartNormal;
	public Texture2D restartHover;
	public Texture2D restartActive;
	
	public Texture2D quitNormal;
	public Texture2D quitHover;
	public Texture2D quitActive;
	
	public float heightDerp = 0;
	
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
	
	public void FadeCompleted(){
		fadeComplete = false;
	}
	
	void Start(){
		grayBar = new Texture2D(1, 1, TextureFormat.RGB24, false);
		grayBar.SetPixel(0, 0, Color.gray);
		grayBar.Apply();
		
		waveRect = new Rect((Screen.width/2) - (750/2), (Screen.height/2) - (600/2) - 100, 750, 600);
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
		
		/*Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit = new RaycastHit();
		if(Physics.Raycast(ray, out hit)){
			if(hit.transform.tag == Globals.FORTIFICATION && GameController.Instance.current == null){
				Vector3 fortPos = Camera.main.WorldToScreenPoint(hit.point);
				fortHealth = hit.transform.GetComponent<Health>();
				if(hit.transform.GetComponentInChildren<BaseWeapon>() != null){
					fortWeapon = hit.transform.GetComponentInChildren<BaseWeapon>();
					displayFortWeaponData = true;
				}
				fortHealthDisplayRect = new Rect(fortPos.x, Screen.height-fortPos.y, 100, 10);
				fortWeaponDisplayRect = new Rect(fortPos.x, (Screen.height-fortPos.y)+10, 100, 50);
				displayFortHealthData = true;
			} else {
				fortWeapon = null;
				displayFortHealthData = false;
				displayFortWeaponData = false;
			}
			
			if(hit.transform.tag == Globals.ENEMY){
				Vector3 enemyPos = Camera.main.WorldToScreenPoint(hit.point);
				enemyHealth = hit.transform.GetComponent<Health>();
				enemyHealthDisplayRect = new Rect(enemyPos.x, Screen.height-enemyPos.y, 100, 10);
				displayEnemyHealthData = true;
			} else {
				displayEnemyHealthData = false;
			}
		}*/
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
		}
		
		if(GameObject.FindWithTag(Globals.PLAYER)){
			if(GameObject.FindWithTag(Globals.PLAYER).GetComponent<PlayerMovement>() != null){
				DrawResources();
				DrawCurWaveScreen();
				DrawAmmoDisplay();
				if(displayEnemyHealthData){
					DrawEnemyHealthDisplay();
				}
				if(displayUI){
					//DrawPlayerHealth();
					DrawShipHealth();
					if(displayFortHealthData){
						DrawFortHealthDisplay();
					}
					if(displayFortWeaponData){
						DrawFortWeaponDisplay();
					}
				}
			}
		}
	}
	
	void DrawPauseScreen(){
		GUIContent content = new GUIContent();
		
		GUIStyle resumeButton = new GUIStyle();
		resumeButton.normal.background = resumeNormal;
		resumeButton.hover.background = resumeHover;
		resumeButton.active.background = resumeActive;
		
		GUIStyle restartButton = new GUIStyle();
		restartButton.normal.background = restartNormal;
		restartButton.hover.background = restartHover;
		restartButton.active.background = restartActive;
		
		GUIStyle quitButton = new GUIStyle();
		quitButton.normal.background = quitNormal;
		quitButton.hover.background = quitHover;
		quitButton.active.background = quitActive;
		
		
		if(GUI.Button(new Rect((Screen.width/2) - (100/2), (Screen.height/2) - (50/2)-50, 100, 50),content,resumeButton)){
			isPaused = !isPaused;
			UIManager.Instance.uiState = UIState.NONE;
		}
		if(GUI.Button(new Rect((Screen.width/2) - (100/2), (Screen.height/2) - (50/2), 100, 50),content,restartButton)){
			Application.LoadLevel(Application.loadedLevel);
		}
		if(GUI.Button(new Rect((Screen.width/2) - (100/2), (Screen.height/2) - (50/2)+50, 100, 50),content,quitButton)){
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
		
		GUI.Label(new Rect(0, 0, waveRect.width, waveRect.height), "WAVE "+GameController.Instance.GetWaveController().GetWaveNumber().ToString(), style);
		
		GUI.EndGroup();
	}
	
	void DrawGameOverScreen(){
		GUIContent content = new GUIContent();
		
		GUIStyle restartButton = new GUIStyle();
		restartButton.normal.background = restartNormal;
		restartButton.hover.background = restartHover;
		restartButton.active.background = restartActive;
		
		GUIStyle quitButton = new GUIStyle();
		quitButton.normal.background = quitNormal;
		quitButton.hover.background = quitHover;
		quitButton.active.background = quitActive;
		
		
		if(fadeComplete){
			if(GUI.Button(new Rect((Screen.width/2f)-(100/2), (Screen.height/2)-(50/2), 100, 50),content,restartButton)){
				//GameController.Instance.Reset();
				MenuManager.Instance.menuState = MenuManager.MenuState.ENDGAME;
				//uiState = UIState.NONE;
				StartCoroutine(GameController.Instance.RestartGame());
			}
			if(GUI.Button(new Rect((Screen.width/2f)-(100/2), (Screen.height/2)-(50/2)+50, 100, 50),content,quitButton)){
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
		
		Rect waveNumberRect = new Rect(50, Screen.height-50, 150, 50);
		
		GUI.BeginGroup(waveNumberRect);
		
		GUI.Label(new Rect(0, 0, waveNumberRect.width, waveNumberRect.height), GameController.Instance.GetWaveController().GetWaveNumber().ToString(), style);
		
		GUI.EndGroup();
	}
	
	void DrawPlayerHealth(){
		Rect playerHealthRect = new Rect(Screen.width-250, Screen.height-50, 200, 30);
		
		GUI.BeginGroup(playerHealthRect);
		
		Texture2D playerHealthBar = new Texture2D(1, 1, TextureFormat.RGB24, false);
		playerHealthBar.SetPixel(0, 0, Color.red);
		playerHealthBar.Apply();
		
		GUI.DrawTexture(new Rect(playerHealthRect.width, 0, 
			-playerHealthRect.width*GameController.Instance.GetPlayerHealth().GetMaxHealth(), playerHealthRect.height), 
			grayBar, ScaleMode.StretchToFill);
		GUI.DrawTexture(new Rect(playerHealthRect.width, 0, 
			-playerHealthRect.width*GameController.Instance.GetPlayerHealth().curHealth/GameController.Instance.GetPlayerHealth().GetMaxHealth(), playerHealthRect.height), 
			playerHealthBar, ScaleMode.StretchToFill);

		GUI.EndGroup();
	}
	
	void DrawShipHealth(){
		GUI.BeginGroup(shipHealthRect);
		
		Texture2D shipHealthBar = new Texture2D(1, 1, TextureFormat.RGB24, false);
		shipHealthBar.SetPixel(0, 0, Color.green);
		shipHealthBar.Apply();
		
		GUI.DrawTexture(new Rect(shipHealthRect.width, 0, 
			-shipHealthRect.width*GameController.Instance.GetShipHealth().GetMaxHealth(), shipHealthRect.height), 
			grayBar, ScaleMode.StretchToFill);
		GUI.DrawTexture(new Rect(shipHealthRect.width, 0, 
			-shipHealthRect.width*GameController.Instance.GetShipHealth().curHealth/GameController.Instance.GetShipHealth().GetMaxHealth(), shipHealthRect.height), 
			shipHealthBar, ScaleMode.StretchToFill);
		
		GUI.EndGroup();
	}
	
	void DrawFortHealthDisplay(){
		GUI.BeginGroup(fortHealthDisplayRect);
		
		Texture2D fortHealthBar = new Texture2D(1, 1, TextureFormat.RGB24, false);
		fortHealthBar.SetPixel(0, 0, Color.red);
		fortHealthBar.Apply();
		
		GUI.DrawTexture(new Rect(fortHealthDisplayRect.width, 0, 
			-fortHealthDisplayRect.width*fortHealth.GetMaxHealth(), fortHealthDisplayRect.height), 
			grayBar, ScaleMode.StretchToFill);
		GUI.DrawTexture(new Rect(fortHealthDisplayRect.width, 0, 
			-fortHealthDisplayRect.width*fortHealth.curHealth/fortHealth.GetMaxHealth(), fortHealthDisplayRect.height), 
			fortHealthBar, ScaleMode.StretchToFill);
		
		GUI.EndGroup();
	}
	
	void DrawFortWeaponDisplay(){
		GUI.BeginGroup(fortWeaponDisplayRect);
		
		GUIStyle style = new GUIStyle();
		style.normal.background = resourceBackground;
		style.normal.textColor = Color.white;
		style.font = resourceFont;
		style.fontSize = 8;
		style.alignment = TextAnchor.MiddleCenter;
		
		GUI.Label(new Rect(0, 0, fortWeaponDisplayRect.width, fortWeaponDisplayRect.height), 
							"Ammo: "+fortWeapon.bulletsLeft+"/"+fortWeapon.clips, style);
		
		GUI.EndGroup();
	}
	
	void DrawEnemyHealthDisplay(){
		GUI.BeginGroup(enemyHealthDisplayRect);
		
		Texture2D enemyHealthBar = new Texture2D(1, 1, TextureFormat.RGB24, false);
		enemyHealthBar.SetPixel(0, 0, Color.red);
		enemyHealthBar.Apply();
		
		GUI.DrawTexture(new Rect(enemyHealthDisplayRect.width, 0, 
			-enemyHealthDisplayRect.width*enemyHealth.GetMaxHealth(), enemyHealthDisplayRect.height), 
			grayBar, ScaleMode.StretchToFill);
		GUI.DrawTexture(new Rect(enemyHealthDisplayRect.width, 0, 
			-enemyHealthDisplayRect.width*enemyHealth.curHealth/enemyHealth.GetMaxHealth(), enemyHealthDisplayRect.height), 
			enemyHealthBar, ScaleMode.StretchToFill);
		
		GUI.EndGroup();
	}
	
	void DrawResources(){
		Rect resourceRect = new Rect((Screen.width/2)-(300/2), (Screen.height-Screen.height)+20, 300, 50);
		GUI.BeginGroup(resourceRect);
		
		GUIStyle style = new GUIStyle();
		style.normal.background = resourceBackground;
		style.font = resourceFont;
		style.alignment = TextAnchor.MiddleCenter;
		GUI.Label(new Rect(0, 0, resourceRect.width, resourceRect.height), "RESOURCES: " + GameController.Instance.GetResources(), style);
		
		GUI.EndGroup();
	}
	
	void DrawAmmoDisplay(){
		BaseWeapon weapon = GameObject.FindWithTag(Globals.PLAYER).GetComponentInChildren<BaseWeapon>();
		WeaponManager manager = GameObject.FindWithTag(Globals.PLAYER).GetComponentInChildren<WeaponManager>();
		
		GUI.BeginGroup(new Rect(Screen.width-500, Screen.height-100, 500, 100));
		
		if(weapon != manager.allWeapons[1]){
			for(int i=0; i<weapon.bulletsLeft; i++){
				
				if(weapon == manager.allWeapons[5]){
					if(i % 8 == 0){
						GUI.DrawTexture(new Rect(440 +(i*-2),50,20,20), shotgunShellUI);
					}
				} else if(weapon == manager.allWeapons[7]){
					
					if(weapon.bulletsLeft >= weapon.bulletsPerClip){
						GUI.DrawTexture(new Rect(440, 45, -200, 30), canisters[0]);
					} else if(weapon.bulletsLeft >= (weapon.bulletsPerClip-(weapon.bulletsPerClip/3)) && weapon.bulletsLeft < weapon.bulletsPerClip){
						GUI.DrawTexture(new Rect(440, 45, -200, 30), canisters[1]);
					} else if(weapon.bulletsLeft >= (weapon.bulletsPerClip-(weapon.bulletsPerClip/2)) && weapon.bulletsLeft < (weapon.bulletsPerClip-(weapon.bulletsPerClip/3))){
						GUI.DrawTexture(new Rect(440, 45, -200, 30), canisters[2]);
					} else if(weapon.bulletsLeft > 0 && weapon.bulletsLeft < (weapon.bulletsPerClip-(weapon.bulletsPerClip/2))){
						GUI.DrawTexture(new Rect(440, 45, -200, 30), canisters[3]);
					} else if(weapon.bulletsLeft <= 0){
						GUI.DrawTexture(new Rect(440, 45, -200, 30), canisters[4]);
					}
					
					/*else if(weapon == manager.allWeapons[3] || weapon == manager.allWeapons[7]){
					Texture2D ammoBar = new Texture2D(1, 1, TextureFormat.RGB24, false);
					ammoBar.SetPixel(0, 0, Color.blue);
					ammoBar.Apply();
					
					float width = Mathf.Clamp(200f*Mathf.Round((weapon.bulletsLeft/150f)*100f)/100f, 0, 200);
					GUI.DrawTexture(new Rect(440, 0, -width, 20), ammoBar, ScaleMode.StretchToFill);*/
				} else if(weapon == manager.allWeapons[3]){
					if(weapon.bulletsLeft >= weapon.bulletsPerClip){
						GUI.DrawTexture(new Rect(440, 10, -200, 100), batteries[0]);
					} else if(weapon.bulletsLeft >= (weapon.bulletsPerClip-(weapon.bulletsPerClip/6)) && weapon.bulletsLeft < weapon.bulletsPerClip){
						GUI.DrawTexture(new Rect(440, 10, -200, 100), batteries[1]);
					} else if(weapon.bulletsLeft >= (weapon.bulletsPerClip-(weapon.bulletsPerClip/5)) && weapon.bulletsLeft < (weapon.bulletsPerClip-(weapon.bulletsPerClip/6))){
						GUI.DrawTexture(new Rect(440, 10, -200, 100), batteries[2]);
					} else if(weapon.bulletsLeft >= (weapon.bulletsPerClip-(weapon.bulletsPerClip/4)) && weapon.bulletsLeft < (weapon.bulletsPerClip-(weapon.bulletsPerClip/5))){
						GUI.DrawTexture(new Rect(440, 10, -200, 100), batteries[3]);
					} else if(weapon.bulletsLeft >= (weapon.bulletsPerClip-(weapon.bulletsPerClip/3)) && weapon.bulletsLeft < (weapon.bulletsPerClip-(weapon.bulletsPerClip/4))){
						GUI.DrawTexture(new Rect(440, 10, -200, 100), batteries[4]);
					} else if(weapon.bulletsLeft >= (weapon.bulletsPerClip-(weapon.bulletsPerClip/2)) && weapon.bulletsLeft < (weapon.bulletsPerClip-(weapon.bulletsPerClip/3))){
						GUI.DrawTexture(new Rect(440, 10, -200, 100), batteries[5]);
					} else if(weapon.bulletsLeft > 0 && weapon.bulletsLeft < (weapon.bulletsPerClip-(weapon.bulletsPerClip/2))){
						GUI.DrawTexture(new Rect(440, 10, -200, 100), batteries[6]);
					} else if(weapon.bulletsLeft <= 0){
						GUI.DrawTexture(new Rect(440, 10, -200, 100), batteries[7]);
					}
				} else {
					GUI.DrawTexture(new Rect(440 +(i*-5),50,20,20), ammoUI);
				}
			}
			GUIStyle style = new GUIStyle();
			style.font = resourceFont;
			style.fontSize = 15;
			style.normal.textColor = Color.white;
			GUI.Label(new Rect(455, 50, 20, 30), weapon.clips.ToString(), style);
		} else {
			GUI.DrawTexture(new Rect(350,50,100,40), infintyUI);
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
