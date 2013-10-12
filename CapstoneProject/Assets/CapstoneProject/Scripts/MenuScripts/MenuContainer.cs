using UnityEngine;
using System.Collections;

public class MenuContainer : MonoBehaviour {
	
	private BoxCollider[] boxCols;
	private Renderer[] rends;
	private MoveToTarget target;
	private float waitTime = 5f;
	public bool renderMenu = false;
	
	void Start(){
		target = GameObject.Find("Ship").GetComponent<MoveToTarget>();
		boxCols = GetComponentsInChildren<BoxCollider>();
		rends = GetComponentsInChildren<Renderer>();
		
		UnRenderMenu();
	}
	
	void Update(){
		if(target.curWaypoint == target.totalWayPoints){
			if(MenuManager.Instance.menuState != MenuManager.MenuState.INGAME && !renderMenu){
				StartCoroutine("FadeMenu");
			} else if(MenuManager.Instance.menuState == MenuManager.MenuState.INGAME && renderMenu){
				UnRenderMenu();
			}
		}
		
		if(target.curWaypoint == target.totalWayPoints/3){
			GameObject cargo = GameObject.Find("Cargo");
			cargo.rigidbody.useGravity = true;
			cargo.rigidbody.AddForce(new Vector3(6, 0, 4) * 3f);
		}
		
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out hit)){
			hit.collider.gameObject.SendMessage("OnMouseOver", SendMessageOptions.DontRequireReceiver);
		}
	}
	
	private IEnumerator FadeMenu(){
		renderMenu = true;
		yield return new WaitForSeconds(waitTime);
		RenderMenu();
	}
	
	void RenderMenu(){
		foreach(BoxCollider col in boxCols){
			col.enabled = true;
		}
		
		foreach(Renderer rend in rends){
			rend.enabled = true;
		}
	}
	
	void UnRenderMenu(){
		foreach(BoxCollider col in boxCols){
			col.enabled = false;
		}
		
		foreach(Renderer rend in rends){
			rend.enabled = false;
		}
	}
}
