using System.Collections;
using UnityEngine;

public class ShipIndicator : MonoBehaviour {
	
	private Vector3 shipPos;
	private Vector3 playerPos;
	
	public Texture2D indicator;
	
	void Update(){
		playerPos = Camera.main.WorldToScreenPoint(GameController.Instance.GetPlayer().position);
		shipPos = Camera.main.WorldToScreenPoint(GameController.Instance.GetShip().position);
		
		if(Vector3.Distance(playerPos, shipPos) < 20f){
			
		}
		
		Ray screenCenter = Camera.main.ScreenPointToRay(GameController.Instance.GetPlayer().position);
		Vector3 distance = GameController.Instance.GetShip().position - screenCenter.origin;
		
		float halfy = Screen.height/2f;
		float halfx = Screen.width/2f;

        float slope = (shipPos.y-halfy)/(shipPos.x-halfx); // slope with the center of the screen
		
        if(shipPos.y > Screen.height){ // to the top
           shipPos.y = Screen.height+100f;
           shipPos.x = (Screen.height-halfy)/slope;                  
        } else if(shipPos.y < 0) { // to the bottom
           shipPos.y = 100f;
           shipPos.x = (-halfy/slope);
        } else if(shipPos.x > Screen.width) { // to the right
           shipPos.x = Screen.width-100f;
           shipPos.y = slope*shipPos.x + halfy;
        } else { // to the left
           shipPos.x = 100f;
     	}
	}
	
	void OnGUI(){
		GUI.DrawTexture(new Rect(shipPos.x, Screen.height-shipPos.y, 100, 100), indicator);
	}
}
