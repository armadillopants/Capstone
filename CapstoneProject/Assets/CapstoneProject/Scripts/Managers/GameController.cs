using UnityEngine;
using System.Collections;
using Pathfinding;

public class GameController : MonoBehaviour {
	
	private Health shipHealth;
	private Health playerHealth;
	private GameObject[] enemies;
	
	private Transform player;
	private Transform ship;
	
	public GameObject rescueShip;
	private GameObject shipToSpawn;
	
	private int amountOfResources = 1000000;
	public bool canShoot = false;
	public bool canChangeWeapons = false;
	private bool beginFade = false;
	
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
		
		playerHealth = player.GetComponent<Health>();
		shipHealth = ship.GetComponent<Health>();
	}

	void OnApplicationQuit(){
		_instance = null;
	}
	
	#endregion
	
	void Start(){
		
	}
	
	public Transform GetPlayer(){
		return player;
	}
	
	public Transform GetShip(){
		return ship;
	}
	
	public Health GetPlayerHealth(){
		return playerHealth;
	}
	
	public Health GetShipHealth(){
		return shipHealth;
	}
	
	void Update(){
		if((playerHealth.IsDead && shipHealth.IsDead) || playerHealth.IsDead){
			SwitchUIState(UIManager.UIState.GAMEOVER);
			if(!beginFade){
				StartCoroutine(UIManager.Instance.Fade());
				StartCoroutine(UIManager.Instance.FadeComplete());
				Destroy(GameObject.Find("WaveController").GetComponent<Wave>());
				beginFade = true;
			}
			return;
		}
		
		if(Input.GetKeyDown(KeyCode.Escape) && UIManager.Instance.uiState != UIManager.UIState.GAMEOVER && MenuManager.Instance.menuState == MenuManager.MenuState.INGAME){
			UIManager.Instance.isPaused = true;
			SwitchUIState(UIManager.UIState.PAUSE);
		}
		
		if(fortSpawned){
			UpdateFortPosWithMouseLoc();
		}
	}
	
	public void Reset(){
		DestroyEnemies();
		GameObject.Find("Ship").AddComponent<AttachPlayerToShip>();
		DestroyFortifications();
		if(shipToSpawn){
			Destroy(shipToSpawn);
		}
		canShoot = false;
		canChangeWeapons = false;
		GetPlayerHealth().ModifyHealth(100f);
		GetShipHealth().ModifyHealth(300f);
		GetPlayerHealth().IsDead = false;
		GetShipHealth().IsDead = false;
		beginFade = false;
		UIManager.Instance.FadeCompleted = false;
		GameObject.Find("WaveController").GetComponent<WaveController>().ResetWave(1);
		amountOfResources = 0;
		foreach(ParticleEmitter light in GameObject.FindWithTag(Globals.SHIP).GetComponentsInChildren<ParticleEmitter>()){
			light.emit = false;
		}
	}
	
	void DestroyEnemies(){
		enemies = GameObject.FindGameObjectsWithTag(Globals.ENEMY);
		foreach(GameObject enemy in enemies){
			Destroy(enemy);
		}
	}
	
	void DestroyFortifications(){
		foreach(GameObject fort in GameObject.FindGameObjectsWithTag(Globals.FORTIFICATION)){
			UpdateGraphOnDestroyedObject(fort.collider.bounds, fort.collider, fort.gameObject);
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
		
		switch(current.GetComponent<Dragable>().state){
		case Dragable.FortState.THREE_ONE:
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
			break;
		case Dragable.FortState.THREE_THREE:
			gridZ = 3f;
			gridX = 3f;
			break;
		case Dragable.FortState.ONE_ONE:
			gridZ = 1f;
			gridX = 1f;
			break;
		}
		
		Vector3 snapPos = mouseLoc;
		snapPos = SnapToGrid(snapPos, gridX, gridZ, current);
		current.transform.position = snapPos;
		
		canPlace = true;
		current.renderer.material = validGreen;
		
		for(int i=0; i<forts.Length; i++){
			Vector3 gridPos = GameObject.FindWithTag(Globals.GRID).transform.position;
			if(hit.point.x <= gridPos.x-Globals.GRID_SIZE || hit.point.x >= gridPos.x+Globals.GRID_SIZE || 
				hit.point.z <= gridPos.z-Globals.GRID_SIZE || hit.point.z >= gridPos.z+Globals.GRID_SIZE){
				
				canPlace = false;
				current.renderer.material = invalidRed;
				Debug.Log("Cannot place object");
			}
			
			/*GeomRect rectA = new GeomRect();
			rectA.x1 = current.collider.transform.position.x;
			rectA.x2 = current.collider.transform.position.x + current.collider.bounds.size.x;
			rectA.y1 = current.collider.transform.position.z;
			rectA.y2 = current.collider.transform.position.z + current.collider.bounds.size.z;
			
			GeomRect rectB = new GeomRect();
			rectB.x1 = forts[i].collider.transform.position.x;
			rectB.x2 = forts[i].collider.transform.position.x + current.collider.bounds.size.x;
			rectB.y1 = forts[i].collider.transform.position.z;
			rectB.y2 = forts[i].collider.transform.position.z + current.collider.bounds.size.z;
			
			if(current.gameObject != forts[i].gameObject && forts.Length > 1){
				if(rectA.RectInside(rectB)){
					canPlace = false;
					current.renderer.material = invalidRed;
					Debug.Log("Cannot place object");
				}
			}*/
			
			if(current.gameObject != forts[i].gameObject && forts.Length > 1){
				if(current.transform.eulerAngles.y == Globals.ROTATION_H_LEFT || current.transform.eulerAngles.y == Globals.ROTATION_H_RIGHT){
					if(current.collider.bounds.Contains(forts[i].collider.bounds.center)){
						canPlace = false;
						current.renderer.material = invalidRed;
						Debug.Log("Cannot place object");
					}
				}
				
				if(current.transform.eulerAngles.y == Globals.ROTATION_V_UP || current.transform.eulerAngles.y == Globals.ROTATION_V_DOWN){
					if(current.collider.bounds.Contains(forts[i].collider.bounds.center) || 
						current.collider.bounds.Contains(forts[i].collider.bounds.center-new Vector3(0,0,1)) || 
						current.collider.bounds.Contains(forts[i].collider.bounds.center+new Vector3(0,0,1))){
						
						canPlace = false;
						current.renderer.material = invalidRed;
						Debug.Log("Cannot place object");
					}
				}
			}
		}
		
		if(Input.GetMouseButtonDown(0) && canPlace){
			current.transform.position = SnapToGrid(current.transform.position, gridX, gridZ, current);
			current.renderer.material = originalMat;
			StartCoroutine("AddDragable");
			fortSpawned = false;
		}
	}
	
	public Vector3 SnapToGrid(Vector3 pos, float rot1, float rot2, GameObject g){
		return new Vector3(Mathf.RoundToInt(pos.x/rot1)*rot1, g.GetComponent<Dragable>().height, Mathf.RoundToInt(pos.z/rot2)*rot2);
	}
	
	private IEnumerator AddDragable(){
		yield return new WaitForSeconds(0.1f);
		current.GetComponent<Dragable>().enabled = true;
		fortToSpawn = null;
		current = null;
	}
	
	public void TurnDragableOn(){
		GameObject[] fortifications = GameObject.FindGameObjectsWithTag(Globals.FORTIFICATION);
		foreach(GameObject fort in fortifications){
			fort.GetComponent<Dragable>().enabled = true;
		}
	}
	
	public void UpdateGraph(){
		GameObject[] fortifications = GameObject.FindGameObjectsWithTag(Globals.FORTIFICATION);
		foreach(GameObject fort in fortifications){
			fort.GetComponent<Dragable>().enabled = false;
			fort.GetComponent<Dragable>().canDrag = false;
			//Destroy(fort.GetComponent<Dragable>());
			Bounds b = fort.collider.bounds;
			GraphUpdateObject guo = new GraphUpdateObject(b);
			AstarPath.active.UpdateGraphs(guo);
			AstarPath.active.FlushGraphUpdates();
		}
	}
	
	public void UpdateGraphOnDestroyedObject(Bounds b, Collider col, GameObject g){
		Destroy(col);
		Destroy(g);
		GraphUpdateObject guo = new GraphUpdateObject(b);
		AstarPath.active.UpdateGraphs(guo);
		AstarPath.active.FlushGraphUpdates();
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
	
	public void SpawnRescueShip(){
		shipToSpawn = (GameObject)Instantiate(rescueShip, new Vector3(-100,60,30), Quaternion.identity);
	}
	
	public GameObject FindNearestTarget(string nearestTarget, Transform other){
		GameObject[] targets;
		targets = GameObject.FindGameObjectsWithTag(nearestTarget);
		GameObject closest = null;
		float distance = Mathf.Infinity;
		
		foreach(GameObject targetCheck in targets){
			Vector3 diff = targetCheck.transform.position - other.position;
			float curDist = diff.sqrMagnitude;
			if(curDist < distance){
				closest = targetCheck;
				distance = curDist;
			}
		}
		return closest;
	}
}
