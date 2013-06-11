using UnityEngine;
using System.Collections;

public class MenuContainer : MonoBehaviour {
	
	private BoxCollider[] boxCols;
	private Renderer[] rends;
	private MoveToTarget target;
	private float waitTime = 5f;
	
	void Start(){
		target = GameObject.Find("Defend").GetComponent<MoveToTarget>();
		boxCols = GetComponentsInChildren<BoxCollider>();
		rends = GetComponentsInChildren<Renderer>();
		
		foreach(BoxCollider col in boxCols){
			col.enabled = false;
		}
		
		foreach(Renderer rend in rends){
			rend.enabled = false;
		}
	}
	
	void Update(){
		if(target.curWaypoint == target.totalWayPoints){
			StartCoroutine("FadeMenu");
		}
		
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out hit)){
			hit.collider.gameObject.SendMessage("OnMouseOver", SendMessageOptions.DontRequireReceiver);
		}
	}
	
	private IEnumerator FadeMenu(){
		yield return new WaitForSeconds(waitTime);
		RenderMenu();
	}
	
	void RenderMenu(){
		StopCoroutine("FadeMenu");
		foreach(BoxCollider col in boxCols){
			col.enabled = true;
		}
		
		foreach(Renderer rend in rends){
			rend.enabled = true;
		}
	}
}
