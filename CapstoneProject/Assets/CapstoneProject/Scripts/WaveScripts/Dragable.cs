using UnityEngine;

public class Dragable : MonoBehaviour {
	
	public enum FortState { THREE_ONE, THREE_THREE, ONE_ONE };
	public FortState state;
	public float height;
	public bool canDrag = true;

	private Vector3 screenPoint;
	private Vector3 offset;
	private Vector3 setY; // A fix for the Y position of the gameobject
	private bool checkPlaneHit = false;
	//private FortPlane fp;
	
	private Vector3 lastPosition;
	private float gridx = 1f;
	private float gridz = 1f;
	
	void OnMouseOver(){
		// If we right click on a gameobject, display destroy item screen
		if(Input.GetMouseButton(1)){
			UIManager.Instance.SetFortification(gameObject);
			ItemVendor itemVendor = GameObject.Find("Vendor").GetComponent<ItemVendor>();
			itemVendor.upgradeItemVendor = gameObject;
			itemVendor.CancelUpgrades();
		}
	}

	void OnMouseDown(){
    	screenPoint = Camera.main.WorldToScreenPoint(transform.position);

    	offset = transform.position - Camera.main.ScreenToWorldPoint(
			new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
		
		checkPlaneHit = true;
	}

	void OnMouseDrag(){
		if(canDrag){
	    	Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
	
	    	Vector3 curPos = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
			setY = new Vector3(curPos.x, transform.position.y, curPos.z); // Object moves with mouse location
			lastPosition = transform.position;
			
			transform.position = setY; // Fixes the objects Y postion
		}
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
			
			LayerMask path = 11;
			path = ~(1<<path);
			if(Physics.Raycast(transform.position, Vector3.down, out hit, 5f, path)){
				if(hit.transform.tag == Globals.GRID){
					//fp = hit.transform.parent.gameObject.GetComponent<FortPlane>();
					
					//NullableVector3 newPos = fp.DeterminePlane(transform, hit.transform.gameObject, hit.point);
					
					float yRot = transform.eulerAngles.y;
					
					switch(state){
					case FortState.THREE_ONE:
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
						break;
					case FortState.THREE_THREE:
						gridz = 3f;
						gridx = 3f;
						break;
					case FortState.ONE_ONE:
						gridx = 1f;
						gridz = 1f;
						break;
					}
					
					Vector3 snapPos = transform.position;
					snapPos = GameController.Instance.SnapToGrid(snapPos, gridx, gridz, gameObject);
					transform.position = snapPos;
					
					/*if(newPos != null){
						transform.position = newPos.vector;
					}*/
				} else {
					transform.position = lastPosition;
				}
			}
		}
	}
}
