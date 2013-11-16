using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {
	
	public GameObject WASD;
	public GameObject mouseLeft;
	public GameObject mouseRight;
	public GameObject leftShift;
	public Texture2D arrow;
	
	private WaveController waveController;
	private Vector3 playerPos;
	private Transform playerTrans;
	private bool beginTutorial = false;
	private bool spawnRightMouse = false;
	private GameObject link = null;
	private float waitTime = 5f;
	public string key = "";
	
	void Start(){
		waveController = GameObject.Find("WaveController").GetComponent<WaveController>();
		link = gameObject;
	}
	
	void Update(){
		playerTrans = GameController.Instance.GetPlayer();
		playerPos = playerTrans.position+new Vector3(0,1,0);
		
		if(GameController.Instance.GetPlayer().GetComponent<LocalInput>() != null && waveController.GetWaveNumber() == 1 && !beginTutorial){
			StartCoroutine(BeginWASDLink());
			key = "Player";
			beginTutorial = true;
		}
		
		if(link != gameObject){
			if(key == "Player"){
				link.transform.position = new Vector3(playerPos.x, 1, playerPos.z);
			} else if(key == "LeftClick"){
				link.transform.position = new Vector3(playerPos.x, 1, playerPos.z) - new Vector3(3,0,0);// + playerTrans.TransformDirection(new Vector3(0,0,8));
			} else if(key == "LeftShift"){
				link.transform.position = new Vector3(playerPos.x, 1, playerPos.z) - new Vector3(3,0,0);
			} else if(key == "RightClick"){
				link.transform.position = new Vector3(link.transform.position.x, 1, link.transform.position.z);
			}
		}
	}
	
	GameObject Spawn(GameObject g, Vector3 pos, bool spawned){
		if(!spawned){
			return (GameObject)Instantiate(g, pos, Quaternion.Euler(90,0,0));
			spawned = true;
		}
		
		return null;
	}
			
	IEnumerator BeginWASDLink(){
		link = Spawn(WASD, playerPos, false);
		yield return new WaitForSeconds(waitTime);
		Destroy(GameObject.Find(link.name));
		link = gameObject;
		yield return new WaitForSeconds(waitTime);
		StartCoroutine(BeginMouseLeftLink());
	}
	
	IEnumerator BeginMouseLeftLink(){
		key = "LeftClick";
		link = Spawn(mouseLeft, playerPos, false);
		yield return new WaitForSeconds(waitTime);
		Destroy(GameObject.Find(link.name));
		link = gameObject;
		StartCoroutine(BeginLeftShiftLink());
	}
	
	IEnumerator BeginLeftShiftLink(){
		key = "LeftShift";
		link = Spawn(leftShift, playerPos, false);
		yield return new WaitForSeconds(waitTime);
		Destroy(GameObject.Find(link.name));
		link = gameObject;
	}
	
	IEnumerator CountDown(){
		if(UIManager.Instance.uiState == UIManager.UIState.BUILD_SCREEN){
			yield return new WaitForSeconds(1f);
			key = "Arrow2";
		} else {
			yield return new WaitForSeconds(Time.deltaTime);
		}
	}
	
	IEnumerator CountDownTwo(){
		if(UIManager.Instance.uiState == UIManager.UIState.NONE){
			yield return new WaitForSeconds(waitTime);
			if(!spawnRightMouse){
				StartCoroutine(BeginMouseRightLink());
				spawnRightMouse = true;
			}
		} else {
			yield return new WaitForSeconds(Time.deltaTime);
		}
	}
	
	IEnumerator BeginMouseRightLink(){
		if(GameObject.FindWithTag(Globals.FORTIFICATION)){
			key = "RightClick";
			link = Spawn(mouseRight, GameObject.FindWithTag(Globals.FORTIFICATION).transform.position-new Vector3(2,0,0), false);
			yield return new WaitForSeconds(waitTime);
			Destroy(GameObject.Find(link.name));
			link = gameObject;
		} else {
			yield return new WaitForSeconds(Time.deltaTime);
		}
	}
	
	void OnGUI(){
		if(key == "Arrow"){
			GUI.DrawTexture(new Rect(Screen.width-300, Screen.height/10, 50, 50), arrow);
			StartCoroutine(CountDown());
		}
		
		if(key == "Arrow2"){
			GUI.DrawTexture(new Rect(Screen.width/3.0f, Screen.height/5, 50, 50), arrow);
			StartCoroutine(CountDownTwo());
		}
	}
}
