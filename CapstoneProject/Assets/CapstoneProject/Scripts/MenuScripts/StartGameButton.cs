using UnityEngine;
using System.Collections;

public class StartGameButton : MonoBehaviour {
	
	public bool beginGame = false;
	private Transform cam;
	public Transform startCam;
	public Transform endCam;
	public bool lerpToStart = false;
	
	public void Execute(){
		MenuManager.Instance.menuState = MenuManager.MenuState.INGAME;
	}
	
	void Start(){
		cam = Camera.main.transform;
	}
	
	void Update(){
		if(GameController.Instance.GetPlayer()){
			Transform player = GameController.Instance.GetPlayer();
			if(beginGame && MenuManager.Instance.menuState == MenuManager.MenuState.INGAME){
				cam.position = Vector3.Lerp(cam.position, endCam.position, 0.5f*Time.deltaTime);
				cam.rotation = Quaternion.Lerp(cam.rotation, Quaternion.Euler(new Vector3(endCam.eulerAngles.x, 0, 0)), 0.5f*Time.deltaTime);
			}
			
			if(cam.position.z >= player.position.z-0.5f && beginGame){
				cam.position = new Vector3(player.position.x, player.position.y+17, player.position.z);
				cam.rotation = Quaternion.Euler(endCam.eulerAngles.x, 0, 0);
				player.gameObject.AddComponent<PlayerMovement>();
				player.gameObject.AddComponent<LocalInput>();
				player.gameObject.AddComponent<AnimationController>();
				StartCoroutine(GameController.Instance.GetWaveController().BeginFirstWave());
				GameObject.Find("Hull").AddComponent<DynamicGridObstacle>();
				beginGame = false;
			}
			
			if(MenuManager.Instance.menuState == MenuManager.MenuState.ENDGAME){
				if(GameController.Instance.GetWaveController().GetComponent<Wave>()){
					Destroy(GameController.Instance.GetWaveController().GetComponent<Wave>());
				}
				if(GameController.Instance.GetWaveController().GetComponent<Fortification>()){
					Destroy(GameController.Instance.GetWaveController().GetComponent<Fortification>());
				}
				beginGame = false;
				lerpToStart = true;
				cam.position = Vector3.Lerp(cam.position, startCam.position, 0.5f*Time.deltaTime);
				cam.rotation = Quaternion.Lerp(cam.rotation, Quaternion.Euler(new Vector3(startCam.eulerAngles.x, 0, 0)), 0.5f*Time.deltaTime);
			}
			
			if(cam.position.z <= startCam.position.z+0.5f && lerpToStart){
				cam.position = startCam.position;
				cam.rotation = Quaternion.Euler(startCam.eulerAngles.x, 0, 0);
				StartCoroutine(RenderMenu());
			}
		}
	}
	
	private IEnumerator RenderMenu(){
		yield return new WaitForSeconds(3f);
		GameObject.Find("MainMenu").GetComponent<MenuContainer>().UnRenderMenu();
		GameObject.Find("MainMenu").GetComponent<MeshRenderer>().enabled = true;
		MenuManager.Instance.menuState = MenuManager.MenuState.MAINMENU;
		lerpToStart = false;
	}
}
