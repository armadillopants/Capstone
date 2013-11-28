using UnityEngine;
using System.Collections;

public class MainPanelGUI : MonoBehaviour {
	
	public Texture2D backGround;
	
	public Texture2D buttonNormal;
	public Texture2D buttonHover;
	public Texture2D buttonActive;
	
	private WeaponSelection selection;
	private WeaponPanelGUI weaponPanel;
	
	private int buttonWidth = 200;
	private int buttonHeight = 50;
	
	private int labelWidth = 250;
	private int labelHeight = 200;
	
	private int offsetWidth = 256;
	private int offsetHeight = 80;
	
	public Texture2D header;
	public Texture2D[] fortNames;
	
	void Start(){
		selection = GameController.Instance.GetPlayer().GetComponentInChildren<WeaponSelection>();
	}
	
	public void Draw(Rect drawRect, Wave buildWave){
		Rect drawArea = drawRect;
		
		GUI.BeginGroup(drawArea);
		
		GUI.DrawTexture(new Rect(0, 0, drawArea.width, drawArea.height), backGround);
		
		GUI.Label(new Rect(offsetWidth-50, offsetHeight-15, labelWidth, labelHeight), header);
		
		for(int i=0; i<fortNames.Length; i++){
			Texture2D curFortName = fortNames[i];
			
			GUI.BeginGroup(new Rect(0, i * offsetHeight+offsetHeight, drawArea.width, drawArea.height));
			
			GUIStyle style = new GUIStyle();
			style.normal.background = buttonNormal;
			style.hover.background = buttonHover;
			style.active.background = buttonActive;
			style.alignment = TextAnchor.MiddleCenter;
			
			GUIContent content = new GUIContent(curFortName);

			if(GUI.Button(new Rect(offsetWidth, offsetHeight, buttonWidth, buttonHeight), content, style)){
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
					//GameObject.Find("CombinedMeshes").GetComponent<CombineChildren>().Combine();
					buildWave.BeginWave();
					Destroy(GameObject.Find("WaveController").GetComponent<Fortification>());
				}
			}
			GUI.EndGroup();
		}
		GUI.EndGroup();
	}
}
