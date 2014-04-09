using UnityEngine;

public class Crosshair : MonoBehaviour {
	
	public Texture2D crossHair; // Creates a 2D texture
	
	void OnGUI(){
		if(MenuManager.Instance.menuState == MenuManager.MenuState.MAINMENU || MenuManager.Instance.menuState == MenuManager.MenuState.ENDGAME || 
			!UIManager.Instance.displayUI || UIManager.Instance.uiState == UIManager.UIState.PAUSE || 
			GameObject.FindWithTag(Globals.PLAYER).GetComponentInChildren<WeaponSelection>().changingWeapons){
			
			// Sets the cross hair position to the mouse position
			Rect pos = (new Rect(Input.mousePosition.x - (crossHair.width/2)+24, (Screen.height - Input.mousePosition.y) - 
					(crossHair.height/2)+24, crossHair.width/2, crossHair.height/2));
			// Draws the texture to the mouse position
			GUI.DrawTexture(pos, crossHair);
		}
	}
}