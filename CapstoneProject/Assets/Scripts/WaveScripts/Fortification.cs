using UnityEngine;
using System.Collections;

public class Fortification : MonoBehaviour {
	
	public enum FortState { MAIN_SCREEN, BUILD_SCREEN, UPGRADE_SCREEN, BUY_SCREEN };
	private FortState state = FortState.MAIN_SCREEN;
	private Wave buildWave;
	private WeaponSelection selection;
	private float infinity = Mathf.Infinity;
	private Rect mainScreen = new Rect(Screen.width-250, Screen.height-(Screen.height-16), 200, 500);
	private Rect displayScreen = new Rect(Screen.width/3, Screen.height/5, 500, 500);

	void Awake(){
		selection = GameObject.FindWithTag("Player").GetComponentInChildren<WeaponSelection>();
		selection.canShoot = false;
	}
	
	void Update(){
		selection.canShoot = false;
	}
	
	public void StartFortifying(Wave wave){
		buildWave = wave;
		StartCoroutine("FortifyHandling");
	}
	
	IEnumerator FortifyHandling(){
		buildWave.StopWave(); // Stops the wave
		// And displays the fortification screen
		yield return new WaitForSeconds(infinity); // Allows unlimited amount of time to select fortifications
	}
	
	void OnGUI(){
		DrawMainScreen();
		switch(state){
		case FortState.BUILD_SCREEN:
			DrawBuildScreen();
			break;
		case FortState.UPGRADE_SCREEN:
			DrawUpgradeScreen();
			break;
		case FortState.BUY_SCREEN:
			DrawBuyScreen();
			break;
		}
	}
	
	void DrawMainScreen(){
		GUI.BeginGroup(mainScreen);
		
		GUI.Box(new Rect(0, 0, mainScreen.width, mainScreen.height), "Fortifications");
		string[] fortNames = new string[4]{"Fortify", "Weapons", "Upgrades", "Begin Wave"};
		for(int i=0; i<fortNames.Length; i++){
			string curFortName = fortNames[i];
			if(GUI.Button(new Rect(mainScreen.width/4, mainScreen.height/10+(i*100), 100, 50), curFortName)){
				if(curFortName == fortNames[0]){
					state = FortState.BUILD_SCREEN;
				} else if(curFortName == fortNames[1]){
					state = FortState.BUY_SCREEN;
				} else if(curFortName == fortNames[2]){
					state = FortState.UPGRADE_SCREEN;
				} else {
					selection.canShoot = true;
					buildWave.BeginWave();
					Destroy(this);
				}
			}
		}
		
		GUI.EndGroup();
	}
	
	void DrawBuildScreen(){
		GUI.BeginGroup(displayScreen);
		
		GUI.Box(new Rect(0, 0, displayScreen.width, displayScreen.height), "Build Screen");
		
		GUI.EndGroup();
	}
	
	void DrawUpgradeScreen(){
		GUI.BeginGroup(displayScreen);
		
		GUI.Box(new Rect(0, 0, displayScreen.width, displayScreen.height), "Upgrade Screen");
		
		GUI.EndGroup();
	}
	
	void DrawBuyScreen(){
		GUI.BeginGroup(displayScreen);
		
		GUI.Box(new Rect(0, 0, displayScreen.width, displayScreen.height), "Weapons Screen");
		
		GUI.EndGroup();
	}
}
