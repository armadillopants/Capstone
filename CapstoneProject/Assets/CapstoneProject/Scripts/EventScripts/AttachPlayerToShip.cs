using UnityEngine;

public class AttachPlayerToShip : MonoBehaviour {
	
	private float timer = 3f;
	
	void Update(){
		if(GameController.Instance.GetPlayer()){
			Transform player = GameController.Instance.GetPlayer();
			if(MenuManager.Instance.menuState != MenuManager.MenuState.INGAME){
				player.position = transform.position;
			} else {
				timer -= 1*Time.deltaTime;
				Vector3 playerPos = player.position;
				playerPos.y = 0;
				player.position = playerPos;
				player.position += Vector3.left * 3f * Time.deltaTime;
				player.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
			}
			
			if(timer <= 0){
				timer = 0;
				player.position = new Vector3(player.position.x, 0, player.position.z);
				player.rotation = Quaternion.Euler(0, -90, 0);
				StartGameButton survival = GameObject.Find("PlayButton").GetComponent<StartGameButton>();
				survival.beginGame = true;
				Destroy(this);
			}
		}
	}
}
