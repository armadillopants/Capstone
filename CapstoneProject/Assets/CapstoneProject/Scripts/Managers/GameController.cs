using UnityEngine;
using System.Collections;
using Pathfinding;

public class GameController : MonoBehaviour {
	
	public enum TimeOfDay { DAYTIME, NIGHTTIME };
	public TimeOfDay timeOfDay = TimeOfDay.DAYTIME;
	
	private Health shipHealth;
	private Health playerHealth;
	private GameObject[] enemies;
	
	private Transform player;
	private Transform ship;
	
	protected int amountOfResources = 100000;
	public bool canDisplay = true;
	public bool canShoot = false;
	
	// Fortification data
	private GameObject fortToSpawn;
	private GameObject current;
	private bool fortSpawned = false;
	private bool canPlace = false;
	public Material validGreen;
	public Material invalidRed;
	private Material originalMat;
	
	#region Singleton
	
	private static GameController _instance;

	public static GameController Instance {
		get { return _instance; }
	}

	void Awake(){
		if(GameController.Instance != null){
			DestroyImmediate(gameObject);
			return;
		}
		_instance = this;
		
		player = GameObject.FindWithTag(Globals.PLAYER).transform;
		ship = GameObject.FindWithTag(Globals.SHIP).transform;
	}

	void OnApplicationQuit(){
		_instance = null;
	}
	
	#endregion

	void Start(){
		playerHealth = player.GetComponent<Health>();
		shipHealth = ship.GetComponent<Health>();
	}
	
	public Transform GetPlayer(){
		return player;
	}
	
	public Transform GetShip(){
		return ship;
	}
	
	void Update(){
		if(MenuManager.Instance.menuState != MenuManager.MenuState.INGAME){
			return;
		}
		
		enemies = GameObject.FindGameObjectsWithTag(Globals.ENEMY);
		if(playerHealth.IsDead()){
			foreach(GameObject enemy in enemies){
				Destroy(enemy);
			}
			SwitchUIState(UIManager.UIState.GAMEOVER);
		}
		
		if(playerHealth.IsDead() && shipHealth.IsDead()){
			foreach(GameObject enemy in enemies){
				Destroy(enemy);
			}
			SwitchUIState(UIManager.UIState.GAMEOVER);
		}
		
		if(Input.GetKeyDown(KeyCode.Escape) && UIManager.Instance.uiState != UIManager.UIState.GAMEOVER){
			UIManager.Instance.isPaused = true;
			SwitchUIState(UIManager.UIState.PAUSE);
		}
		
		if(fortSpawned){
			UpdateFortPosWithMouseLoc();
		}
	}
	
	public int GetResources(){
		return amountOfResources;
	}
	
	public void AddResources(int amount){
		amountOfResources += amount;
	}
	
	public void DeleteResources(int amount){
		amountOfResources -= amount;
	}
	
	void SwitchUIState(UIManager.UIState state){
		UIManager.Instance.uiState = state;
	}
	
	public void SetFortificationToSpawn(GameObject fort){
		fortToSpawn = fort;
		originalMat = fortToSpawn.renderer.sharedMaterial;
		
		if(fortToSpawn){
			SpawnFortification();
		}
	}
	
	void SpawnFortification(){
		RaycastHit hit = new RaycastHit();
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		if(Physics.Raycast(ray, out hit)){
			GameObject fort = (GameObject)Instantiate(fortToSpawn, hit.point, Quaternion.identity);
			current = fort;
		}
		fortSpawned = true;
	}
	
	void UpdateFortPosWithMouseLoc(){
		if(Input.GetKeyDown(KeyCode.E)){
			current.transform.Rotate(0, 90, 0);
		}
		if(Input.GetKeyDown(KeyCode.Q)){
			current.transform.Rotate(0, -90, 0);
		}
		
		GameObject[] forts = GameObject.FindGameObjectsWithTag(Globals.FORTIFICATION);
		
		RaycastHit hit = new RaycastHit();	
		Vector3 mouseLoc = new Vector3();
		if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)){
			mouseLoc = hit.point;
			mouseLoc.y = 0.5f;
		}
		
		float gridX = 1f;
		float gridZ = 1f;
		
		if(current.transform.eulerAngles.y == Globals.ROTATION_H_LEFT || current.transform.eulerAngles.y == Globals.ROTATION_H_RIGHT){
			gridZ = 3f;
		} else {
			gridZ = 1f;
		}
		
		if(current.transform.eulerAngles.y == Globals.ROTATION_V_UP || current.transform.eulerAngles.y == Globals.ROTATION_V_DOWN){
			gridX = 3f;
		} else {
			gridX = 1f;
		}
		
		Vector3 snapPos = mouseLoc;
		snapPos = SnapToGrid(snapPos, gridX, gridZ);
		current.transform.position = snapPos;
		
		canPlace = true;
		current.renderer.material = validGreen;
		
		for(int i=0; i<forts.Length; i++){
			if(current.gameObject != forts[i].gameObject && forts.Length > 1){
				Vector3 gridPos = GameObject.FindWithTag(Globals.GRID).transform.position;
				if(current.collider.bounds.Intersects(forts[i].collider.bounds) || 
					hit.point.x < gridPos.x-12f || hit.point.x > gridPos.x+12f || hit.point.z < gridPos.z-12f || hit.point.z > gridPos.z+12f){
					if(Vector3.Distance(current.transform.position, forts[i].transform.position) < 1f){
						canPlace = false;
						current.renderer.material = invalidRed;
						Debug.Log("Cannot place object");
					}
				}
			} else {
				Vector3 gridPos = GameObject.FindWithTag(Globals.GRID).transform.position;
				if(hit.point.x < gridPos.x-12f || hit.point.x > gridPos.x+12f || hit.point.z < gridPos.z-12f || hit.point.z > gridPos.z+12f){
					canPlace = false;
					current.renderer.material = invalidRed;
					Debug.Log("Cannot place object");
				}
			}
		}
		
		if(Input.GetMouseButtonDown(0) && canPlace){
			current.transform.position = SnapToGrid(current.transform.position, gridX, gridZ);
			current.renderer.material = originalMat;
			StartCoroutine("AddDragable");
			fortSpawned = false;
		}
	}
	
	public Vector3 SnapToGrid(Vector3 pos, float rot1, float rot2){
		return new Vector3(Mathf.RoundToInt(pos.x/rot1)*rot1, 0.5f, Mathf.RoundToInt(pos.z/rot2)*rot2);
	}
	
	private IEnumerator AddDragable(){
		yield return new WaitForSeconds(0.1f);
		current.AddComponent<Dragable>();
		fortToSpawn = null;
		current = null;
	}
	
	public void UpdateGraph(){
		GameObject[] fortifications = GameObject.FindGameObjectsWithTag(Globals.FORTIFICATION);
		foreach(GameObject fort in fortifications){
			Destroy(fort.GetComponent<Dragable>());
			Bounds b = fort.collider.bounds;
			GraphUpdateObject guo = new GraphUpdateObject(b);
			AstarPath.active.UpdateGraphs(guo);
		}
	}
	
	public void UpdateGraphOnDestroyedObject(Bounds b, Collider col, GameObject g){
		Destroy(col);
		Destroy(g);
		GraphUpdateObject guo = new GraphUpdateObject(b);
		AstarPath.active.UpdateGraphs(guo, 0.0f);
	}
	
	public void AddDynamicObstacleToFortifications(){
		GameObject[] fortifications = GameObject.FindGameObjectsWithTag(Globals.FORTIFICATION);
		foreach(GameObject fort in fortifications){
			if(fort.GetComponent<DynamicGridObstacle>() == null){
				fort.AddComponent<DynamicGridObstacle>();
				fort.GetComponent<DynamicGridObstacle>().updateError = 0.001f;
			}
		}
	}
}
