using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour {
	
	public enum UIState { WAVEWON, NEXTWAVE, 
		GAMEOVER, NONE, CURWAVE, FORT_BUILD_SCREEN,
		FORT_WEAPON_SCREEN, FORT_UPGRADE_SCREEN, 
		FORT_ABILITY_SCREEN, GAMEWON };
	public UIState uiState = UIState.NONE;
	
	public bool isPaused = false;
	private WeaponSelection selection;
	private bool fadeComplete = false;
	
	public GameObject fortification;
	private GameObject holder;
	
	private Texture2D grayBar;
	
	private Rect waveRect;
	private Rect shipHealthRect;
	
	public Texture2D resourceFrame;
	public Font resourceFont;
	public Texture2D ammoUI;
	public Texture2D shotgunShellUI;
	public Texture2D[] canisters;
	public Texture2D[] batteries;
	public bool displayUI = false;
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
	
	private GUIStyle style;
	private GUIStyle resumeButton;
	private GUIStyle restartButton;
	private GUIStyle quitButton;
	private GUIContent content;
	
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
	}

	void OnApplicationQuit(){
		_instance = null;
	}
	
	#endregion
	
	public void FadeCompleted(){
		fadeComplete = false;
	}
	
	void Start(){
		Reset();
		grayBar = new Texture2D(1, 1, TextureFormat.RGB24, false);
		grayBar.SetPixel(0, 0, Color.gray);
		grayBar.Apply();
		waveRect = new Rect((Screen.width/2) - (750/2), (Screen.height/2) - (600/2) - 100, 750, 600);
		Screen.showCursor = false;
		
		resumeButton = new GUIStyle();
		resumeButton.normal.background = resumeNormal;
		resumeButton.hover.background = resumeHover;
		resumeButton.active.background = resumeActive;
		
		restartButton = new GUIStyle();
		restartButton.normal.background = restartNormal;
		restartButton.hover.background = restartHover;
		restartButton.active.background = restartActive;
		
		quitButton = new GUIStyle();
		quitButton.normal.background = quitNormal;
		quitButton.hover.background = quitHover;
		quitButton.active.background = quitActive;
		
		style = new GUIStyle();
		style.alignment = TextAnchor.MiddleCenter;
		style.normal.textColor = Color.white;
		style.font = resourceFont;
		style.fontSize = 50;
		
		content = new GUIContent();
	}
	
	public void Reset(){
		selection = GameObject.Find("WeaponSelection").GetComponent<WeaponSelection>();
		holder = GameObject.Find("AbilityHolder");
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
		
	}
	
	public void SetFortification(GameObject fort){
		fortification = fort;
		uiState = UIState.FORT_UPGRADE_SCREEN;
		BuildUpgradeGUI build = GameObject.Find("Vendor").GetComponent<BuildUpgradeGUI>();
		build.Reset();
	}
	
	void OnGUI(){
		switch(uiState){
		case UIState.WAVEWON:
			DrawWaveWonScreen();
			break;
		case UIState.NEXTWAVE:
			DrawNextWaveScreen();
			break;
		case UIState.CURWAVE:
			DrawCurWaveScreen();
			break;
		case UIState.GAMEOVER:
			DrawGameOverScreen();
			break;
		case UIState.GAMEWON:
			DrawGameWonScreen();
			break;
		}
		
		if(isPaused){
			DrawPauseScreen();
		}
		
		if(displayUI){
			DrawAmmoDisplay();
			DrawAbilityDisplay();
			DrawShipHealth();
		}
		
		if(GameController.Instance.GetWaveController().canBeginWave){
			DrawResources();
			DrawCurWaveScreen();
		}
		
		if(GameController.Instance.EndWave() > 0){
			if(GameController.Instance.amountOfWavesLeft > 0){
				DrawFinalWaveCountdown();
			}
		}
		
		if(GameObject.Find("RescueShip") && uiState != UIState.GAMEWON){
			DrawFinalWaveTimer();
		}
	}
	
	void DrawFinalWaveCountdown(){
		Rect finalWaveRect = new Rect((Screen.width/2)-(500/2), (Screen.height-Screen.height)+50, 500, 50);
		GUI.BeginGroup(finalWaveRect);
		
		GUI.Label(new Rect(0, 0, finalWaveRect.width, finalWaveRect.height), "Waves until Rescue Ship's arrival: "
			+GameController.Instance.amountOfWavesLeft, style);
		
		GUI.EndGroup();
	
	}
	
	void DrawFinalWaveTimer(){
		Rect finalWaveRect = new Rect((Screen.width/2)-(500/2), (Screen.height-Screen.height)+50, 500, 50);
		GUI.BeginGroup(finalWaveRect);
		
		GUI.Label(new Rect(0, 0, finalWaveRect.width, finalWaveRect.height), "Timer: "
			+GuiTime(GameObject.Find("RescueShip").GetComponent<FlyToShip>().GetTimer()), style);
		
		GUI.EndGroup();
	}
	
	void DrawPauseScreen(){
		GUI.BeginGroup(waveRect);
		GUI.Label(new Rect(0, 0, waveRect.width, waveRect.height), "GAME PAUSED", style);
		GUI.EndGroup();
		if(GUI.Button(new Rect((Screen.width/2) - (100/2), (Screen.height/2) - (50/2)-50, 100, 50),content,resumeButton)){
			isPaused = !isPaused;
		}
		if(GUI.Button(new Rect((Screen.width/2) - (100/2), (Screen.height/2) - (50/2), 100, 50),content,restartButton)){
			isPaused = false;
			uiState = UIState.NONE;
			GameController.Instance.canShoot = false;
			GameController.Instance.canChangeWeapons = false;
			Destroy(GameController.Instance.GetWaveController().GetComponent<Wave>());
			Destroy(GameController.Instance.GetWaveController().GetComponent<Fortification>());
			GameObject.Find("GridContainer").GetComponent<GridSpawner>().DisableGrid();
			MenuManager.Instance.menuState = MenuManager.MenuState.ENDGAME;
			StartCoroutine(GameController.Instance.RestartGame());
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
		displayUI = false;
		
		if(fadeComplete){
			GUI.BeginGroup(waveRect);
			GUI.Label(new Rect(0, 0, waveRect.width, waveRect.height), "YOU ARE DEAD", style);
			GUI.EndGroup();
			
			if(GUI.Button(new Rect((Screen.width/2f)-(100/2), (Screen.height/2)-(50/2), 100, 50),content,restartButton)){
				MenuManager.Instance.menuState = MenuManager.MenuState.ENDGAME;
				StartCoroutine(GameController.Instance.RestartGame());
			}
			if(GUI.Button(new Rect((Screen.width/2f)-(100/2), (Screen.height/2)-(50/2)+50, 100, 50),content,quitButton)){
				Application.Quit();
			}
		}
	}

	void DrawGameWonScreen(){
		
		GUI.BeginGroup(waveRect);
		
		GUI.Label(new Rect(0, 0, waveRect.width, waveRect.height), "YOU HAVE ESCAPED!", style);
		
		GUI.EndGroup();
		
		displayUI = false;
		
		if(GUI.Button(new Rect((Screen.width/2) - (100/2), (Screen.height/2) - (50/2), 100, 50),content,restartButton)){
			isPaused = false;
			uiState = UIState.NONE;
			GameController.Instance.canShoot = false;
			GameController.Instance.canChangeWeapons = false;
			Destroy(GameController.Instance.GetWaveController().GetComponent<Wave>());
			Destroy(GameController.Instance.GetWaveController().GetComponent<Fortification>());
			GameObject.Find("GridContainer").GetComponent<GridSpawner>().DisableGrid();
			MenuManager.Instance.menuState = MenuManager.MenuState.ENDGAME;
			StartCoroutine(GameController.Instance.RestartGame());
		}
		if(GUI.Button(new Rect((Screen.width/2) - (100/2), (Screen.height/2) - (50/2)+50, 100, 50),content,quitButton)){
			Application.Quit();
		}
	}
	
	void DrawCurWaveScreen(){
		
		Rect waveNumberRect = new Rect(50, Screen.height-50, 150, 50);
		
		GUI.BeginGroup(waveNumberRect);
		
		GUI.Label(new Rect(0, 0, waveNumberRect.width, waveNumberRect.height), GameController.Instance.GetWaveController().GetWaveNumber().ToString(), style);
		
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
	
	void DrawResources(){
		GUI.DrawTexture(new Rect(Screen.width-365, Screen.height-(Screen.height-1), 256, 128), resourceFrame);
		
		GUIStyle style = new GUIStyle();
		style.font = resourceFont;
		style.alignment = TextAnchor.MiddleCenter;
		style.normal.textColor = Color.cyan;
		style.fontSize = 25;
		GUI.Label(new Rect(Screen.width-260, Screen.height-(Screen.height-28), 0, 0), "RESOURCES", style);
		GUI.Label(new Rect(Screen.width-245, Screen.height-(Screen.height-75), 0, 0), GameController.Instance.GetResources().ToString(), style);
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
	
	void DrawAbilityDisplay(){
		Rect abilityRect = new Rect(Screen.width-250, Screen.height-100, 250, 30);
		
		GUI.BeginGroup(abilityRect);
		
		Texture2D abilityBar = new Texture2D(1, 1, TextureFormat.RGB24, false);
		abilityBar.SetPixel(0, 0, Color.blue);
		abilityBar.Apply();
		
		GUIStyle style = new GUIStyle();
		style.font = resourceFont;
		style.fontSize = 15;
		style.normal.textColor = Color.white;
		
		if(holder.GetComponent<OrbitAbility>() != null){
			GUI.DrawTexture(new Rect(200, 0, 
				-abilityRect.width*AbilitiesManager.Instance.orbitAbility.maxCoolDown, abilityRect.height), 
				grayBar, ScaleMode.StretchToFill);
			GUI.DrawTexture(new Rect(200, 0, 
				-abilityRect.width*AbilitiesManager.Instance.orbitAbility.coolDown/AbilitiesManager.Instance.orbitAbility.maxCoolDown, abilityRect.height), 
				abilityBar, ScaleMode.StretchToFill);
			if(AbilitiesManager.Instance.orbitAbility.coolDown <= 0){
				GUI.Label(new Rect(25, 0, 200, 25), "Ability Available", style);
				GUI.Label(new Rect(25, 15, 200, 20), "Press 'E' to use", style);
			} else {
				GUI.Label(new Rect(25, 0, 200, 25), "Ability Unavailable", style);
			}
			GUI.Label(new Rect(230, 0, 20, 30), AbilitiesManager.Instance.orbitAbility.amount.ToString(), style);
		} else if(holder.GetComponent<RockRainAbility>() != null){
			GUI.DrawTexture(new Rect(200, 0, 
				-abilityRect.width*AbilitiesManager.Instance.rockRainAbility.maxCoolDown, abilityRect.height), 
				grayBar, ScaleMode.StretchToFill);
			GUI.DrawTexture(new Rect(200, 0, 
				-abilityRect.width*AbilitiesManager.Instance.rockRainAbility.coolDown/AbilitiesManager.Instance.rockRainAbility.maxCoolDown, abilityRect.height), 
				abilityBar, ScaleMode.StretchToFill);
			if(AbilitiesManager.Instance.rockRainAbility.coolDown <= 0){
				GUI.Label(new Rect(25, 0, 200, 25), "Ability Available", style);
				GUI.Label(new Rect(25, 15, 200, 20), "Press 'E' to use", style);
			} else {
				GUI.Label(new Rect(25, 0, 200, 25), "Ability Unavailable", style);
			}
			GUI.Label(new Rect(230, 0, 20, 30), AbilitiesManager.Instance.rockRainAbility.amount.ToString(), style);
		} else if(holder.GetComponent<StrikerAbility>() != null){
			GUI.DrawTexture(new Rect(200, 0, 
				-abilityRect.width*AbilitiesManager.Instance.strikerAbility.maxCoolDown, abilityRect.height), 
				grayBar, ScaleMode.StretchToFill);
			GUI.DrawTexture(new Rect(200, 0, 
				-abilityRect.width*AbilitiesManager.Instance.strikerAbility.coolDown/AbilitiesManager.Instance.strikerAbility.maxCoolDown, abilityRect.height), 
				abilityBar, ScaleMode.StretchToFill);
			if(AbilitiesManager.Instance.strikerAbility.coolDown <= 0){
				GUI.Label(new Rect(25, 0, 200, 25), "Ability Available", style);
				GUI.Label(new Rect(25, 15, 200, 20), "Press 'E' to use", style);
			} else {
				GUI.Label(new Rect(25, 0, 200, 25), "Ability Unavailable", style);
			}
			GUI.Label(new Rect(230, 0, 20, 30), AbilitiesManager.Instance.strikerAbility.amount.ToString(), style);
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
	
	public string GuiTime(float time){
		float guiTime = time;
		int minutes = (int)(guiTime / 60); // Creates 00 for minutes
		int seconds = (int)(guiTime % 60); // Creates 00 for seconds
		int fraction = (int)(time * 100); // Creates 000 for fractions
		fraction = fraction % 100;
		string text = ""; // For displaying the the timer in min, sec, frac
	    text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, fraction);
	    return text;
	}
}
