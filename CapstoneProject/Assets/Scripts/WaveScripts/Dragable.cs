using UnityEngine;

public class Dragable : MonoBehaviour {

	private Vector3 screenPoint;
	private Vector3 offset;
	private Vector3 setY; // A fix for the Y position of the gameobject
	private bool destroyItem = false;
	private bool checkPlaneHit = false;
	private Vector3 worldSpaceLocation;
	private FortPlane fp;
	
	private Vector3 lastPosition;
	float gridx = 1f;
	float gridz = 1f;
	
	void OnMouseOver(){
		worldSpaceLocation = transform.position + new Vector3(3, 0, 0);
		// If we right click on a gameobject, display destroy item screen
		if(Input.GetMouseButton(1)){
			destroyItem = true;
		}
	}

	void OnMouseDown(){
    	screenPoint = Camera.main.WorldToScreenPoint(transform.position);

    	offset = transform.position - Camera.main.ScreenToWorldPoint(
			new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
		
		checkPlaneHit = true;
	}

	void OnMouseDrag(){
    	Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

    	Vector3 curPos = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
		setY = new Vector3(curPos.x, transform.position.y, curPos.z); // Object moves with mouse location
		lastPosition = transform.position;
		
		transform.position = setY; // Fixes the objects Y postion
	}
	
	void OnMouseUp(){
		checkPlaneHit = false;
	}
	
	void Update(){
		if(checkPlaneHit){
			if(Input.GetKeyDown(KeyCode.E)){
				transform.Rotate(0, 90, 0);
			}
			if(Input.GetKeyDown(KeyCode.Q)){
				transform.Rotate(0, -90, 0);
			}
			
			RaycastHit hit;
			
			if(Physics.Raycast(transform.position, Vector3.down, out hit, 1f)){
				if(hit.transform.tag == "FortPlane"){
					fp = hit.transform.parent.gameObject.GetComponent<FortPlane>();
					
					NullableVector3 newPos = fp.DeterminePlane(transform, hit.transform.gameObject, hit.point);
					
					float yRot = transform.eulerAngles.y;
					
					if(yRot == Globals.ROTATION_H_LEFT || yRot == Globals.ROTATION_H_RIGHT){
						gridz = 3f;
					} else {
						gridz = 1f;
					}
					
					if(yRot == Globals.ROTATION_V_UP || yRot == Globals.ROTATION_V_DOWN){
						gridx = 3f;
					} else {
						gridx = 1f;
					}
					
					Vector3 snapPos = transform.position;
					snapPos.x = Mathf.Round(snapPos.x / gridx) * gridx;
					snapPos.y = transform.position.y;
					snapPos.z = Mathf.Round(snapPos.z / gridz) * gridz;
					transform.position = snapPos;
					
					if(newPos != null){
						//transform.position = newPos.vector;
					} 
				} else{
					transform.position = lastPosition;
				}
			}
		}
	}
	
	void OnGUI(){
		Vector2 guiLoc = new Vector2();
		Vector3 screenSpace = Camera.main.WorldToScreenPoint(worldSpaceLocation);
		guiLoc.x = screenSpace.x;
		guiLoc.y = Screen.height - screenSpace.y;
		
		if(destroyItem){
			if(GUI.Button(new Rect(guiLoc.x, guiLoc.y, 70, 20), "Destroy?")){
				Destroy(gameObject); // Destroy the game object
			}
			if(GUI.Button(new Rect(guiLoc.x, guiLoc.y+30, 70, 20), "No")){
				destroyItem = false;
			}
		}
	}
}
