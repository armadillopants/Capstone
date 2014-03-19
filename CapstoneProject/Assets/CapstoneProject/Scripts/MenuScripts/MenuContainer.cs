using UnityEngine;
using System.Collections;

public class MenuContainer : MonoBehaviour {
	
	private MoveToTarget target;
	private float waitTime = 3f;
	public bool renderMenu = false;
	
	public Texture2D playNormal;
	public Texture2D playHover;
	public Texture2D playActive;
	
	public Texture2D quitNormal;
	public Texture2D quitHover;
	public Texture2D quitActive;
	
	void Start(){
		target = GameObject.Find("Ship").GetComponent<MoveToTarget>();
		
		UnRenderMenu();
	}
	
	void Update(){
		if(target.curWaypoint == target.totalWayPoints){
			if(MenuManager.Instance.menuState != MenuManager.MenuState.INGAME && !renderMenu){
				StartCoroutine("FadeMenu");
			} else if(MenuManager.Instance.menuState == MenuManager.MenuState.INGAME && renderMenu){
				//UnRenderMenu();
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
	}
	
	private IEnumerator FadeMenu(){
		yield return new WaitForSeconds(waitTime);
		RenderMenu();
	}
	
	void RenderMenu(){
		renderMenu = true;
	}
	
	public void UnRenderMenu(){
		renderMenu = false;
	}
	
	void OnGUI(){
		if(renderMenu && MenuManager.Instance.menuState == MenuManager.MenuState.MAINMENU){
			GUIContent content = new GUIContent();
			
			GUIStyle playButton = new GUIStyle();
			playButton.normal.background = playNormal;
			playButton.hover.background = playHover;
			playButton.active.background = playActive;
			
			if(GUI.Button(new Rect((Screen.width/2)+150,(Screen.height/2)+200,200,100),content,playButton)){
				GameObject.Find("PlayButton").GetComponent<StartGameButton>().Execute();
			}
			
			GUIStyle quitButton = new GUIStyle();
			quitButton.normal.background = quitNormal;
			quitButton.hover.background = quitHover;
			quitButton.active.background = quitActive;
			
			if(GUI.Button(new Rect((Screen.width/2)+200,(Screen.height/2)+300,200,100),content,quitButton)){
				Application.Quit();
			}
		}
	}
}
