using UnityEngine;

public class Crosshair : MonoBehaviour {
	
	public Texture2D crossHair; // Creates a 2D texture
	public Rect pos; // Position for cross hair texture
	
	void OnGUI(){
		// Sets the cross hair position to the mouse position
		pos = (new Rect(Input.mousePosition.x - (crossHair.width/2), (Screen.height - Input.mousePosition.y) - 
				(crossHair.height/2), crossHair.width, crossHair.height));
		// Draws the texture to the mouse position
		GUI.DrawTexture(pos, crossHair);
	}
}