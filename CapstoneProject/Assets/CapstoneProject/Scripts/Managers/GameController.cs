using Pathfinding;
using UnityEngine;

public class GameController : MonoBehaviour {
	
	private Health shipHealth;
	private Health playerHealth;
	private GameObject[] enemies;
	
	private Transform player;
	private Transform ship;
	
	public GameObject rescueShip;
	private GameObject shipToSpawn;
	
	private int amountOfResources = 0;
	public bool canShoot = false;
	public bool canChangeWeapons = false;
	private bool beginFade = false;
	private int curWave;
	private int endWave;
	
	// Fortification data
	public GameObject current;
	public Material validGreen;
	public Material invalidRed;
	private Material originalMat;
	
	private DayNightCycle cycle;
	
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
		
		cycle = GameObject.Find("Sun").GetComponent<DayNightCycle>();
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
	
	public int EndWave(){
		return endWave;
	}
	
	public int CurWave(){
		return curWave;
	}
	
	public void SetEndWave(int amount){
		endWave = amount;
	}
	
	public void SetCurWave(int amount){
		curWave = amount;
	}
	
	void Update(){
		if(playerHealth.IsDead){
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
		
		if(current){
			UpdateFortPosWithMouseLoc();
		}
		
		if(cycle.currentPhase == DayNightCycle.DayPhase.DAY || cycle.currentPhase == DayNightCycle.DayPhase.DAWN){
			player.GetComponentInChildren<Flashlight>().TurnOff();
		}
		
		if(cycle.currentPhase == DayNightCycle.DayPhase.NIGHT || cycle.currentPhase == DayNightCycle.DayPhase.DUSK){
			player.GetComponentInChildren<Flashlight>().TurnOn();
		}
	}
	
	public void Reset(){
		DestroyEnemies();
		DestroyFortifications();
		GameObject.Find("Ship").AddComponent<AttachPlayerToShip>();
		if(shipToSpawn){
			Destroy(shipToSpawn);
		}
		canShoot = false;
		canChangeWeapons = false;
		GetPlayerHealth().ModifyHealth(GetPlayerHealth().GetMaxHealth());
		GetShipHealth().ModifyHealth(GetShipHealth().GetMaxHealth());
		GetPlayerHealth().IsDead = false;
		GetShipHealth().IsDead = false;
		beginFade = false;
		UIManager.Instance.FadeCompleted = false;
		GameObject.Find("WaveController").GetComponent<WaveController>().ResetWave(1);
		amountOfResources = 0;
		foreach(ParticleEmitter light in GameObject.FindWithTag(Globals.SHIP).GetComponentsInChildren<ParticleEmitter>()){
			light.emit = false;
		}
		GameObject.Find("XMLReader").GetComponent<XMLReader>().Reset();
	}
	
	void DestroyEnemies(){
		enemies = GameObject.FindGameObjectsWithTag(Globals.ENEMY);
		foreach(GameObject enemy in enemies){
			Destroy(enemy);
		}
	}
	
	void DestroyFortifications(){
		foreach(GameObject fort in GameObject.FindGameObjectsWithTag(Globals.FORTIFICATION)){
			UpdateGraphOnDestroyedObject(fort.collider, fort.gameObject);
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
	
	public void SetFortificationToSpawn(GameObject fort, float rot){
		originalMat = fort.renderer.sharedMaterial;
		
		if(fort){
			SpawnFortification(fort, rot);
		}
	}
	
	void SpawnFortification(GameObject fortToSpawn, float rot){
		RaycastHit hit = new RaycastHit();
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		if(Physics.Raycast(ray, out hit)){
			GameObject fort = (GameObject)Instantiate(fortToSpawn, hit.point, Quaternion.identity);
			fort.transform.eulerAngles = new Vector3(0,rot,0);
			fort.name = fortToSpawn.name;
			//fort.transform.parent = GameObject.Find("CombinedMeshes").transform;
			current = fort;
		}
	}
	
	void UpdateFortPosWithMouseLoc(){
		UIManager.Instance.uiState = UIManager.UIState.CURRENT_FORT_INFO;
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
			if(Mathf.FloorToInt(current.transform.eulerAngles.y) == Globals.ROTATION_H_LEFT || Mathf.FloorToInt(current.transform.eulerAngles.y) == Globals.ROTATION_H_RIGHT){
				gridZ = 3f;
			} else {
				gridZ = 1f;
			}
		
			if(Mathf.FloorToInt(current.transform.eulerAngles.y) == Globals.ROTATION_V_UP || Mathf.FloorToInt(current.transform.eulerAngles.y) == Globals.ROTATION_V_DOWN){
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
		
		bool canPlace = true;
		current.renderer.material = validGreen;
		
		for(int i=0; i<forts.Length; i++){
			Vector3 gridPos = GameObject.FindWithTag(Globals.GRID).transform.position;
			if(hit.point.x <= gridPos.x-Globals.GRID_SIZE || hit.point.x >= gridPos.x+Globals.GRID_SIZE || 
				hit.point.z <= gridPos.z-Globals.GRID_SIZE || hit.point.z >= gridPos.z+Globals.GRID_SIZE){
				
				canPlace = false;
				current.renderer.material = invalidRed;
			}
			
			if(current.gameObject != forts[i].gameObject && forts.Length > 1){
				
				if(current.collider.bounds.Intersects(forts[i].collider.bounds)){
					canPlace = false;
					current.renderer.material = invalidRed;
				}
			}
		}
		
		// Place current fortification
		if(Input.GetMouseButtonDown(0) && canPlace){
			current.transform.position = SnapToGrid(current.transform.position, gridX, gridZ, current);
			float rot = Mathf.FloorToInt(current.transform.eulerAngles.y);
			current.renderer.material = originalMat;
			DeleteResources(current.GetComponent<SellableItem>().cost);
			current.GetComponent<Dragable>().enabled = true;
			current.GetComponent<Dragable>().canUpdate = true;
			// Keep respawning fortifications until we've run out of money
			if(amountOfResources < current.GetComponent<SellableItem>().cost){
				current = null;
				UIManager.Instance.uiState = UIManager.UIState.NONE;
			} else {
				GameObject temp = current;
				current = null;
				SpawnFortification(temp, rot);
			}
		}
		
		// Cancel current fortification
		if(Input.GetMouseButtonDown(1)){
			UIManager.Instance.uiState = UIManager.UIState.NONE;
			Destroy(current);
			current = null;
		}
	}
	
	public Vector3 SnapToGrid(Vector3 pos, float rot1, float rot2, GameObject g){
		return new Vector3(Mathf.RoundToInt(pos.x/rot1)*rot1, g.GetComponent<Dragable>().height, Mathf.RoundToInt(pos.z/rot2)*rot2);
	}
	
	public void TurnDragableOn(){
		GameObject[] fortifications = GameObject.FindGameObjectsWithTag(Globals.FORTIFICATION);
		foreach(GameObject fort in fortifications){
			fort.GetComponent<Dragable>().canUpdate = true;
		}
	}
	
	public void UpdateGraph(){
		GameObject[] fortifications = GameObject.FindGameObjectsWithTag(Globals.FORTIFICATION);
		foreach(GameObject fort in fortifications){
			fort.GetComponent<Dragable>().canDrag = false;
			fort.GetComponent<Dragable>().canUpdate = false;
			Bounds b = fort.collider.bounds;
			GraphUpdateObject guo = new GraphUpdateObject(b);
			AstarPath.active.UpdateGraphs(guo);
			//AstarPath.active.FlushGraphUpdates();
		}
	}
	
	public void UpdateGraphOnDestroyedObject(Collider col, GameObject g){
		Bounds b = col.bounds;
		Destroy(col);
		Destroy(g);
		GraphUpdateObject guo = new GraphUpdateObject(b);
		AstarPath.active.UpdateGraphs(guo, 0.0f);
		//AstarPath.active.FlushGraphUpdates();
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
