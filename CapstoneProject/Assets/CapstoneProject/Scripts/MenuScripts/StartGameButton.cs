using UnityEngine;
using System.Collections;

public class StartGameButton : MonoBehaviour {
	
	public bool beginGame = false;
	private Transform cam;
	private Transform player;
	public Transform startCam;
	public Transform endCam;
	public bool lerpToStart = false;
	
	void Execute(){
		MenuManager.Instance.menuState = MenuManager.MenuState.INGAME;
	}
	
	void Start(){
		cam = Camera.main.transform;
		player = GameObject.FindWithTag(Globals.PLAYER).transform;
	}
	
	void Update(){
		
		if(player != null){
			if(beginGame && MenuManager.Instance.menuState == MenuManager.MenuState.INGAME){
				cam.position = Vector3.Lerp(cam.position, endCam.position, 0.5f*Time.deltaTime);
				cam.rotation = Quaternion.Lerp(cam.rotation, Quaternion.Euler(new Vector3(endCam.eulerAngles.x, 0, 0)), 0.5f*Time.deltaTime);
			}
			
			if(cam.position.z >= player.position.z-0.1f && beginGame){
				cam.position = new Vector3(player.position.x, player.position.y+15, player.position.z);
				cam.rotation = Quaternion.Euler(85, 0, 0);
				player.gameObject.AddComponent<PlayerMovement>();
				player.gameObject.AddComponent<LocalInput>();
				player.gameObject.AddComponent<AnimationController>();
				GameObject.Find("Cargo").AddComponent<DynamicGridObstacle>();
				StartCoroutine(GameObject.Find("WaveController").GetComponent<WaveController>().BeginFirstWave());
				//GameObject.Find("Hull").AddComponent<DynamicGridObstacle>();
				//GameObject.Find("Hull").AddComponent<Rigidbody>();
				beginGame = false;
			}
			
			if(MenuManager.Instance.menuState == MenuManager.MenuState.ENDGAME){
				lerpToStart = true;
				cam.position = Vector3.Lerp(cam.position, startCam.position, 0.5f*Time.deltaTime);
				cam.rotation = Quaternion.Lerp(cam.rotation, Quaternion.Euler(new Vector3(startCam.eulerAngles.x, 0, 0)), 0.5f*Time.deltaTime);
			}
			
			if(cam.position.z <= startCam.position.z+0.1f && lerpToStart){
				cam.position = startCam.position;
				cam.rotation = Quaternion.Euler(startCam.eulerAngles.x, 0, 0);
				MenuManager.Instance.menuState = MenuManager.MenuState.MAINMENU;
				StartCoroutine(RenderMenu());
			}
		}
	}
	
	private IEnumerator RenderMenu(){
		yield return new WaitForSeconds(3f);
		GameObject.Find("MainMenu").GetComponent<MenuContainer>().renderMenu = false;
		lerpToStart = false;
	}
}
