using UnityEngine;
using System.Collections;

public class Fortification : MonoBehaviour {
	
	public enum FortState { MAIN_SCREEN, BUILD_SCREEN, UPGRADE_SCREEN, BUY_SCREEN };
	private FortState state;

	void Start(){
		
	}
	
	void Update(){
	
	}
	
	public void StartFortifying(Wave wave){
		state = FortState.MAIN_SCREEN;
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
	}
	
	void DrawBuildScreen(){
	}
	
	void DrawUpgradeScreen(){
	}
	
	void DrawBuyScreen(){
	}
}
