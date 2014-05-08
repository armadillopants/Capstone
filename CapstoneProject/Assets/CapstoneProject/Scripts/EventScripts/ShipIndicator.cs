using UnityEngine;

public class ShipIndicator : MonoBehaviour {
	
	private Vector3 shipPos;
	
	public Texture2D indicatorTex;
	public Texture2D arrow;
	private bool drawTexture = false;
	
	private float angle;
	
	private Rect indicatorRect;
	
	private Texture2D shipHealthBar;
	private Texture2D grayBar;
	
	void Start(){
		shipHealthBar = new Texture2D(1, 1, TextureFormat.RGB24, false);
		shipHealthBar.SetPixel(0, 0, Color.green);
		shipHealthBar.Apply();
		
		grayBar = new Texture2D(1, 1, TextureFormat.RGB24, false);
		grayBar.SetPixel(0, 0, Color.gray);
		grayBar.Apply();
	}
	
	void LateUpdate(){
		if(MenuManager.Instance.menuState == MenuManager.MenuState.INGAME 
			&& !GameObject.FindWithTag(Globals.PLAYER).GetComponent<Health>().IsDead 
			&& UIManager.Instance.uiState != UIManager.UIState.GAMEWON){
			
			shipPos = Camera.main.WorldToScreenPoint(GameController.Instance.GetShip().position);
			
			Renderer data = transform.GetComponent<Renderer>().renderer;
			if(!data.IsVisibleFrom(Camera.main)){
				drawTexture = true;
				if(shipPos.z < 0){
					shipPos *= -1;
				}
				
				Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0)/2;
				
				// Make (0,0) the center of the screen instead of bottom left
				shipPos -= screenCenter;
				
				// Find angle from center of screen to ship position
				angle = Mathf.Atan2(shipPos.y, shipPos.x);
				angle -= 90 * Mathf.Deg2Rad;
				
				float cos = Mathf.Cos(angle);
				float sin = -Mathf.Sin(angle);
				
				shipPos = screenCenter + new Vector3(sin*150, cos*150, 0);
				
				// y = mx + b
				float m = cos / sin;
				
				Vector3 screenBounds = screenCenter * 0.9f;
				
				if(cos > 0){
					// Top
					shipPos = new Vector3(screenBounds.y/m, screenBounds.y, 0);
				} else {
					// Bottom
					shipPos = new Vector3(-screenBounds.y/m, -screenBounds.y+100, 0);
				}
				
				// If out of bounds get point on appropriate side
				if(shipPos.x > screenBounds.x){
					// Out of bounds on right
					shipPos = new Vector3(screenBounds.x-100, screenBounds.x*m, 0);
				} else if(shipPos.x < -screenBounds.x){
					// Out of bounds on left
					shipPos = new Vector3(-screenBounds.x, -screenBounds.x*m, 0);
				} else {
					// In bounds
				}
				
				shipPos += screenCenter;
				
				indicatorRect = new Rect(shipPos.x, Screen.height-shipPos.y, 100, 100);
				
			}
			if(data.IsVisibleFrom(Camera.main)){
				drawTexture = false;
			}
		} else {
			drawTexture = false;
		}
	}
	
	void OnGUI(){
		if(drawTexture){
			GUI.BeginGroup(indicatorRect);
			{
				GUI.DrawTexture(new Rect(0, 0, indicatorRect.width, indicatorRect.height), indicatorTex);
				GUI.DrawTexture(new Rect(0, 80, 
					indicatorRect.width*GameController.Instance.GetShipHealth().GetMaxHealth(), 10), 
					grayBar, ScaleMode.StretchToFill);
				GUI.DrawTexture(new Rect(0, 80, 
					indicatorRect.width*GameController.Instance.GetShipHealth().curHealth/GameController.Instance.GetShipHealth().GetMaxHealth(), 10), 
					shipHealthBar, ScaleMode.StretchToFill);
			}
			
			GUI.EndGroup();
		}
	}
}