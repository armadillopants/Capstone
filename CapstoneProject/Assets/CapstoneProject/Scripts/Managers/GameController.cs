using UnityEngine;
using System.Collections;
using Pathfinding;

public class GameController : MonoBehaviour {
	
	public enum TimeOfDay { DAYTIME, NIGHTTIME };
	public TimeOfDay timeOfDay = TimeOfDay.DAYTIME;
	
	private Health defendHealth;
	private Health playerHealth;
	private GameObject[] enemies;
	
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
	}

	void OnApplicationQuit(){
		_instance = null;
	}
	
	#endregion

	void Start(){
		defendHealth = GameObject.FindWithTag("Defend").GetComponent<Health>();
		playerHealth = GameObject.FindWithTag("Player").GetComponent<Health>();
	}
	
	void Update(){
		if(MenuManager.Instance.menuState != MenuManager.MenuState.INGAME){
			return;
		}
		
		enemies = GameObject.FindGameObjectsWithTag("Enemy");
		if(playerHealth.IsDead()){
			foreach(GameObject enemy in enemies){
				Destroy(enemy);
			}
			SwitchUIState(UIManager.UIState.GAMEOVER);
		}
		
		if(playerHealth.IsDead() && defendHealth.IsDead()){
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
		
		GameObject[] forts = GameObject.FindGameObjectsWithTag("Fortification");
		
		RaycastHit hit = new RaycastHit();	
		Vector3 mouseLoc = new Vector3();
		if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)){
			mouseLoc = hit.point;
			mouseLoc.y = 0.5f;
		}
		current.transform.position = mouseLoc;
		
		canPlace = true;
		current.renderer.material = validGreen;
		
		for(int i=0; i<forts.Length; i++){
			if(current.gameObject != forts[i].gameObject && forts.Length > 1){
				if(current.collider.bounds.Intersects(forts[i].collider.bounds) || 
					!current.collider.bounds.Intersects(GameObject.FindWithTag("FortPlane").collider.bounds)){
					
					canPlace = false;
					current.renderer.material = invalidRed;
					Debug.Log("Cannot place object");
				}
			} else {
				if(!current.collider.bounds.Intersects(GameObject.FindWithTag("FortPlane").collider.bounds)){
					canPlace = false;
					current.renderer.material = invalidRed;
					Debug.Log("Cannot place object");
				}
			}
		}
		
		if(Input.GetMouseButtonDown(0) && canPlace){
			current.transform.position = new Vector3(Mathf.Round(hit.point.x),0.5f,Mathf.Round(hit.point.z));
			current.renderer.material = originalMat;
			StartCoroutine("AddDragable");
			fortSpawned = false;
		}
	}
	
	private IEnumerator AddDragable(){
		yield return new WaitForSeconds(0.1f);
		current.AddComponent<Dragable>();
		fortToSpawn = null;
		current = null;
	}
	
	public void UpdateGraph(){
		GameObject[] fortifications = GameObject.FindGameObjectsWithTag("Fortification");
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
		GameObject[] fortifications = GameObject.FindGameObjectsWithTag("Fortification");
		foreach(GameObject fort in fortifications){
			if(fort.GetComponent<DynamicGridObstacle>() == null){
				fort.AddComponent<DynamicGridObstacle>();
				fort.GetComponent<DynamicGridObstacle>().updateError = 0.001f;
			}
		}
	}
}
