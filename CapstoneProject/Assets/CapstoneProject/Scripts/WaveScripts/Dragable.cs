using UnityEngine;

public class Dragable : MonoBehaviour {
	
	public enum FortState { THREE_ONE, THREE_THREE, ONE_ONE };
	public FortState state;
	public float height;
	public bool canDrag = true;
	public bool canUpdate = true;
	private GameObject curObj;

	private Vector3 screenPoint;
	private Vector3 offset;
	private bool checkPlaneHit = false;
	
	private Vector3 lastPosition;
	
	void OnMouseOver(){
		if(canUpdate){
			// If we right click on a gameobject, display upgrade item screen
			if(Input.GetMouseButton(1)){
				ItemVendor itemVendor = GameObject.Find("Vendor").GetComponent<ItemVendor>();
				itemVendor.upgradeItemVendor = gameObject;
				UIManager.Instance.SetFortification(gameObject);
			}
		}
	}

	void OnMouseDown(){
		if(canDrag){
	    	screenPoint = Camera.main.WorldToScreenPoint(transform.position);
	
	    	offset = transform.position - Camera.main.ScreenToWorldPoint(
				new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
			
			checkPlaneHit = true;
		}
	}

	void OnMouseDrag(){
		if(canDrag){
	    	Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
	
	    	Vector3 curPos = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
			Vector3 setY = new Vector3(curPos.x, height, curPos.z); // Object moves with mouse location
			lastPosition = transform.position;
			
			transform.position = setY; // Fixes the objects Y postion
		}
	}
	
	void OnMouseUp(){
		checkPlaneHit = false;
	}
	
	void OnMouseEnter(){
		if(GameController.Instance.current == null && curObj == null && canUpdate){
			curObj = (GameObject)Instantiate(GameController.Instance.haloEffectObject, transform.position, Quaternion.identity);
		}
	}
	
	void OnMouseExit(){
		if(curObj){
			Destroy(curObj);
		}
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
			
			if(Physics.Raycast(transform.position, Vector3.down, out hit)){
				if(hit.collider.tag == Globals.GRID){
					
					float yRot = transform.eulerAngles.y;
					float gridx = 1f;
					float gridz = 1f;
					
					switch(state){
					case FortState.THREE_ONE:
						if(Mathf.FloorToInt(yRot) == Globals.ROTATION_H_LEFT || Mathf.FloorToInt(yRot) == Globals.ROTATION_H_RIGHT){
							gridz = 3f;
						} else {
							gridz = 1f;
						}
					
						if(Mathf.FloorToInt(yRot) == Globals.ROTATION_V_UP || Mathf.FloorToInt(yRot) == Globals.ROTATION_V_DOWN){
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
				} else {
					transform.position = lastPosition;
				}
			}
		}
	}
}
