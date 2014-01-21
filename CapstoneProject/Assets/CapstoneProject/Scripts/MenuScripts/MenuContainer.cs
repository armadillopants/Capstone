using UnityEngine;
using System.Collections;

public class MenuContainer : MonoBehaviour {
	
	private BoxCollider[] boxCols;
	private Renderer[] rends;
	private MoveToTarget target;
	private float waitTime = 3f;
	public bool renderMenu = false;
	
	private GameObject barL;
	private GameObject barR;
	
	public GameObject barLeft;
	public GameObject barRight;
	public GameObject call;
	public GameObject sign;
	public GameObject mayday;
	public GameObject CSM;
	public Renderer[] other;
	public GameObject play;
	public GameObject settings;
	public GameObject quit;
	
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
			Transform cargo = GameObject.Find("Cargo").transform;
			foreach(Transform child in cargo){
				child.parent = null;
				child.GetComponent<MoveToTarget>().enabled = true;
				child.gameObject.AddComponent<DynamicGridObstacle>();
			}
		}
		
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out hit)){
			hit.collider.gameObject.SendMessage("OnMouseOver", SendMessageOptions.DontRequireReceiver);
		}
		
		if(barL && barR){
			if(barL.renderer.enabled && barR.renderer.enabled){
				barL.transform.position = Vector3.Lerp(barL.transform.position, new Vector3(-2.6f, 8, -51), 5f*Time.deltaTime);
				barR.transform.position = Vector3.Lerp(barR.transform.position, new Vector3(2.6f, 8, -51), 5f*Time.deltaTime);
			} else {
				barL.transform.position = Vector3.Lerp(barL.transform.position, new Vector3(0, 8, -51), 5f*Time.deltaTime);
				barR.transform.position = Vector3.Lerp(barR.transform.position, new Vector3(0, 8, -51), 5f*Time.deltaTime);
			}
		}
	}
	
	private IEnumerator FadeMenu(){
		renderMenu = true;
		yield return new WaitForSeconds(waitTime);
		StartCoroutine(RenderMenu());
	}
	
	IEnumerator RenderMenu(){
		if(barL == null && barR == null){
			barL = (GameObject)Instantiate(barLeft, new Vector3(0, 8, -51), Quaternion.identity);
			barR = (GameObject)Instantiate(barRight, new Vector3(0, 8, -51), Quaternion.identity);
		} else {
			barL.renderer.enabled = true;
			barR.renderer.enabled = true;
		}
		barL.transform.parent = transform;
		barR.transform.parent = transform;
		yield return new WaitForSeconds(1f);
		foreach(Renderer rend in other){
			rend.enabled = true;
		}
		yield return new WaitForSeconds(1f);
		CSM.renderer.enabled = true;
		yield return new WaitForSeconds(0.5f);
		call.renderer.enabled = true;
		yield return new WaitForSeconds(0.5f);
		sign.renderer.enabled = true;
		yield return new WaitForSeconds(0.5f);
		mayday.renderer.enabled = true;
		yield return new WaitForSeconds(0.5f);
		play.renderer.enabled = true;
		yield return new WaitForSeconds(0.5f);
		settings.renderer.enabled = true;
		yield return new WaitForSeconds(0.5f);
		quit.renderer.enabled = true;
		
		foreach(BoxCollider col in boxCols){
			col.enabled = true;
		}
		
		/*foreach(Renderer rend in rends){
			rend.enabled = true;
		}*/
	}
	
	void UnRenderMenu(){
		foreach(BoxCollider col in boxCols){
			col.enabled = false;
		}
		
		foreach(Renderer rend in rends){
			rend.enabled = false;
		}
		if(barL && barR){
			barL.renderer.enabled = false;
			barR.renderer.enabled = false;
		}
	}
}
