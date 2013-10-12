using UnityEngine;
using System.Collections;

public class AttachPlayerToShip : MonoBehaviour {
	
	private Transform player;
	private float timer = 3f;
	
	void Start(){
		player = GameController.Instance.GetPlayer();
	}
	
	void Update(){
		if(MenuManager.Instance.menuState != MenuManager.MenuState.INGAME){
			player.position = transform.position;
		} else {
			timer -= 1*Time.deltaTime;
			Vector3 playerPos = player.position;
			playerPos.y = 1;
			player.position = playerPos;
			player.position += Vector3.left * 3f * Time.deltaTime;
			player.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
		}
		
		if(timer <= 0){
			timer = 0;
			player.position = new Vector3(player.position.x, 1, player.position.z);
			player.rotation = Quaternion.Euler(0, -90, 0);
			StartGameButton survival = GameObject.Find("SurvivalButton").GetComponent<StartGameButton>();
			survival.beginGame = true;
			Destroy(this);
		}
	}
}
