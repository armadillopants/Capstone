using UnityEngine;
using System.Collections;

public class Fortification : MonoBehaviour {
	
	public enum FortState { MAIN_SCREEN, BUILD_SCREEN, UPGRADE_SCREEN, BUY_SCREEN };
	private FortState state = FortState.MAIN_SCREEN;
	private Wave buildWave;
	private WeaponSelection selection;
	private float infinity = Mathf.Infinity;

	void Awake(){
		selection = GameObject.Find("Player").GetComponentInChildren<WeaponSelection>();
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
		switch(state){
		case FortState.MAIN_SCREEN:
			DrawMainScreen();
			break;
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
		if(GUI.Button(new Rect(Screen.width/2, Screen.height/2, 100, 50), "Begin Wave")){
			selection.canShoot = true;
			buildWave.BeginWave();
			Destroy(this);
		}
	}
	
	void DrawBuildScreen(){
	}
	
	void DrawUpgradeScreen(){
	}
	
	void DrawBuyScreen(){
	}
}
