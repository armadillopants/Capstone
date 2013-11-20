using UnityEngine;
using System.Collections;

public class MainPanelGUI : MonoBehaviour {
	
	public Texture2D backGround;
	
	public Texture2D buttonNormal;
	public Texture2D buttonHover;
	public Texture2D buttonActive;
	
	public Font fortFont;
	
	private WeaponSelection selection;
	private WeaponPanelGUI weaponPanel;
	
	void Start(){
		selection = GameController.Instance.GetPlayer().GetComponentInChildren<WeaponSelection>();
	}
	
	public void Draw(Rect drawRect, Wave buildWave){
		Rect drawArea = drawRect;
		
		GUILayout.BeginArea(drawArea);
		
		GUI.DrawTexture(new Rect(0, 0, drawArea.width, drawArea.height), backGround);
		
		string[] fortNames = new string[4]{"BUILD", "WEAPONS", "ABILITIES", "BEGIN"};
		
		for(int i=0; i<fortNames.Length; i++){
			string curFortName = fortNames[i];
			
			GUIStyle style = new GUIStyle();
			style.normal.background = buttonNormal;
			style.hover.background = buttonHover;
			style.active.background = buttonActive;
			style.normal.textColor = Color.white;
			style.hover.textColor = Color.cyan;
			style.active.textColor = Color.yellow;
			style.font = fortFont;
			style.fontSize = 15;
			style.alignment = TextAnchor.MiddleCenter;
			
			GUIContent content = new GUIContent(curFortName);

			if(GUI.Button(new Rect(drawArea.width/4, drawArea.height/10+(i*50), 100, 50), content, style)){
				if(curFortName == fortNames[0] && UIManager.Instance.uiState != UIManager.UIState.FORT_BUILD_SCREEN){
					UIManager.Instance.uiState = UIManager.UIState.FORT_BUILD_SCREEN;
				} else if(curFortName == fortNames[1] && UIManager.Instance.uiState != UIManager.UIState.FORT_WEAPON_SCREEN){
					UIManager.Instance.uiState = UIManager.UIState.FORT_WEAPON_SCREEN;
				} else if(curFortName == fortNames[2] && UIManager.Instance.uiState != UIManager.UIState.FORT_ABILITY_SCREEN){
					UIManager.Instance.uiState = UIManager.UIState.FORT_ABILITY_SCREEN;
				} else if(curFortName == fortNames[3]){
					UIManager.Instance.uiState = UIManager.UIState.NONE;
					GameObject.Find("GridContainer").GetComponent<GridSpawner>().DisableGrid();
					selection.UpdateWeaponsSlots();
					selection.SelectWeapon(selection.weaponSlots[0].GetComponent<BaseWeapon>().id);
					GameController.Instance.canShoot = true;
					GameController.Instance.canChangeWeapons = true;
					GameController.Instance.UpdateGraph();
					buildWave.BeginWave();
					Destroy(GameObject.Find("WaveController").GetComponent<Fortification>());
				}
			}
		}
		GUILayout.EndArea();
	}
}
