using UnityEngine;
using System.Collections;

public class StartGameButton : MonoBehaviour {
	
	public bool beginGame = false;
	private Transform cam;
	private Transform camPos;
	private Transform player;
	
	void Execute(){
		MenuManager.Instance.menuState = MenuManager.MenuState.INGAME;
	}
	
	void Start(){
		cam = Camera.main.transform;
		camPos = GameObject.Find("CameraPos").transform;
		player = GameObject.FindWithTag(Globals.PLAYER).transform;
	}
	
	void Update(){
		
		if(UIManager.Instance.uiState == UIManager.UIState.GAMEOVER){
			return;
		}
		
		if(beginGame){
			cam.position = Vector3.Lerp(cam.position, new Vector3(player.position.x, player.position.y+15, player.position.z), 0.5f*Time.deltaTime);
			cam.rotation = Quaternion.Lerp(cam.rotation, Quaternion.Euler(new Vector3(85, 0, 0)), 0.5f*Time.deltaTime);
		}
		
		if(cam.position.z >= player.position.z-0.1f && beginGame){
			cam.position = new Vector3(player.position.x, player.position.y+15, player.position.z);
			cam.rotation = Quaternion.Euler(85, 0, 0);
			player.gameObject.AddComponent<LocalInput>();
			player.gameObject.AddComponent<PlayerMovement>();
			beginGame = false;
		}
	}
}
