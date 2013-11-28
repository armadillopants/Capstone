using UnityEngine;
using System.Collections;

public class Fortification : MonoBehaviour {
	
	private Rect mainScreen = new Rect(Screen.width-512, Screen.height-(Screen.height-16), 512, 512);
	private Rect buildDisplayScreen = new Rect(Screen.width-700, Screen.height-(Screen.height-200), 350, 500);
	private Rect weaponDisplayScreen = new Rect(Screen.width-700, Screen.height-(Screen.height-200), 350, 500);
	private Rect abilityDisplayScreen = new Rect(Screen.width-700, Screen.height-(Screen.height-200), 350, 500);
	
	private Wave buildWave;
	private float infinity = Mathf.Infinity;
	
	private MainPanelGUI mainPanel;
	private BuildPanelGUI buildPanel;
	private WeaponPanelGUI weaponPanel;
	private AbilityPanelGUI abilityPanel;

	void Awake(){
		GameController.Instance.canShoot = false;
		GameController.Instance.canChangeWeapons = false;
		
		GameObject vendor = GameObject.Find("Vendor");
		
		weaponPanel = vendor.GetComponent<WeaponPanelGUI>();
		mainPanel = vendor.GetComponent<MainPanelGUI>();
		buildPanel = vendor.GetComponent<BuildPanelGUI>();
		abilityPanel = vendor.GetComponent<AbilityPanelGUI>();
		
		/*GameObject[] combinedMeshes = GameObject.FindGameObjectsWithTag("CombinedMesh");
		foreach(GameObject combine in combinedMeshes){
			Destroy(combine);
		}
		
		foreach(Dragable fort in GameObject.Find("CombinedMeshes").GetComponentsInChildren<Dragable>()){
			fort.renderer.enabled = true;
		}*/
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
		if(GameController.Instance.current == null){
			mainPanel.Draw(mainScreen, buildWave);
		}
		
		switch(UIManager.Instance.uiState){
		case UIManager.UIState.FORT_BUILD_SCREEN:
			DrawFortBuyScreen();
			break;
		case UIManager.UIState.FORT_WEAPON_SCREEN:
			DrawWeaponBuyScreen();
			break;
		case UIManager.UIState.FORT_ABILITY_SCREEN:
			DrawAbilityBuyScreen();
			break;
		case UIManager.UIState.FORT_UPGRADE_SCREEN:
			DrawFortUpgradeScreen();
			break;
		}
	}
	
	void DrawFortBuyScreen(){
		buildPanel.Draw(buildDisplayScreen);
	}
	
	void DrawWeaponBuyScreen(){
		weaponPanel.Draw(weaponDisplayScreen);
	}
	
	void DrawAbilityBuyScreen(){
		abilityPanel.Draw(abilityDisplayScreen);
	}

	void DrawFortUpgradeScreen(){
		
	}
}
