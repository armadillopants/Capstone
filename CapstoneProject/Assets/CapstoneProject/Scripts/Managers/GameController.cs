using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class GameController : MonoBehaviour {
	
	public GameObject playerPrefab;
	
	public GameObject rescueShip;
	private GameObject shipToSpawn;
	
	private Transform player;
	
	private int amountOfResources = 0;
	public bool canShoot = false;
	public bool canChangeWeapons = false;
	private bool beginFade = false;
	private int curWave;
	private int endWave;
	public int amountOfWavesLeft = 5;
	
	// Fortification data
	public GameObject current;
	private GameObject originalObject;
	public Material validGreen;
	public Material invalidRed;
	private Material originalMat;
	private List<Material> originalMats = new List<Material>();
	public GameObject haloEffectObject;
	
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
		
		SpawnPlayer();
		
		cycle = GameObject.Find("Sun").GetComponent<DayNightCycle>();
	}

	void OnApplicationQuit(){
		DestroyEnemies();
		DestroyFortifications();
		Destroy(GameController.Instance.GetWaveController().GetComponent<Wave>());
		Destroy(GameController.Instance.GetWaveController().GetComponent<Fortification>());
		Destroy(GameObject.FindWithTag(Globals.PLAYER));
		Destroy(GameObject.Find("LaserLight"));
		if(shipToSpawn){
			Destroy(shipToSpawn);
		}
		_instance = null;
	}
	
	#endregion
	
	void SpawnPlayer(){
		Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
		player = GameObject.FindWithTag(Globals.PLAYER).transform;
	}
	
	public WaveController GetWaveController(){
		return GameObject.Find("WaveController").GetComponent<WaveController>();
	}
	
	public Transform GetPlayer(){
		return player;
	}
	
	public Transform GetShip(){
		return GameObject.Find(Globals.SHIP).transform;
	}
	
	public Health GetPlayerHealth(){
		return player.GetComponent<Health>();
	}
	
	public Health GetShipHealth(){
		return GameObject.FindWithTag(Globals.SHIP).GetComponent<Health>();
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
		if(player){
			if(GetPlayerHealth().IsDead){
				UIManager.Instance.uiState = UIManager.UIState.GAMEOVER;
				canShoot = false;
				canChangeWeapons = false;
				if(!beginFade){
					StartCoroutine(UIManager.Instance.Fade());
					StartCoroutine(UIManager.Instance.FadeComplete());
					Destroy(GameController.Instance.GetWaveController().GetComponent<Wave>());
					Destroy(GameController.Instance.GetWaveController().GetComponent<Fortification>());
					GameObject.Find("GridContainer").GetComponent<GridSpawner>().DisableGrid();
					beginFade = true;
				}
			}
			
			if((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) && UIManager.Instance.uiState != UIManager.UIState.GAMEOVER && MenuManager.Instance.menuState == MenuManager.MenuState.INGAME){
				UIManager.Instance.isPaused = true;
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
	}
	
	public IEnumerator RestartGame(){
		DestroyEnemies();
		DestroyFortifications();
		amountOfWavesLeft = 5;
		curWave = 0;
		endWave = 0;
		AbilitiesManager.Instance.ResetAbilities();
		GetShip().parent = null;
		GetShip().position = new Vector3(9.382379f, 1.982083f, 8.945202f);
		GetShip().rotation = Quaternion.Euler(0, 90f, 0);
		if(shipToSpawn){
			Destroy(shipToSpawn);
		}
		Destroy(GameObject.FindWithTag(Globals.PLAYER));
		Destroy(GameObject.Find("LaserLight"));
		yield return new WaitForSeconds(0.1f);
		SpawnPlayer();
		GameObject.Find("XMLReader").GetComponent<XMLReader>().Reset();
		GetPlayer().GetComponentInChildren<WeaponManager>().Reset();
		GameObject.Find("Sun").GetComponent<DayNightCycle>().Initialize();
		Reset();
	}
	
	public void Reset(){
		foreach(ParticleEmitter light in GameObject.FindWithTag(Globals.SHIP).GetComponentsInChildren<ParticleEmitter>()){
			light.emit = false;
		}
		GameObject.Find(Globals.SHIP).AddComponent<AttachPlayerToShip>();
		GetShipHealth().ModifyHealth(GetShipHealth().GetMaxHealth());
		GetShipHealth().IsDead = false;
		GameObject.Find("Tutorial").GetComponent<Tutorial>().ResetTutorial();
		GameObject.Find("Vendor").GetComponent<WeaponPanelGUI>().Reset();
		GameObject.Find("Vendor").GetComponent<BuildPanelGUI>().Reset();
		GameObject.Find("Vendor").GetComponent<MainPanelGUI>().Reset();
		GameObject.Find("Vendor").GetComponent<AbilityPanelGUI>().Reset();
		beginFade = false;
		UIManager.Instance.FadeCompleted();
		GetWaveController().ResetWave();
		amountOfResources = 0;
		UIManager.Instance.Reset();
		UIManager.Instance.uiState = UIManager.UIState.NONE;
	}
	
	void DestroyEnemies(){
		foreach(GameObject enemy in GameObject.FindGameObjectsWithTag(Globals.ENEMY)){
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
	
	public void SetFortificationToSpawn(GameObject fort, float rot){
		originalMats.Clear();
		
		originalObject = fort;
		
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
			
			if(fort.GetComponent<MeshRenderer>()){
				originalMat = fort.renderer.sharedMaterial;
			} else {
				foreach(MeshRenderer rend in fort.GetComponentsInChildren<MeshRenderer>()){
					if(rend.enabled){
						originalMats.Add(rend.sharedMaterial);
					}
				}
			}
			current = fort;
		}
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
		bool canPlace = true;
		
		if(Physics.Raycast(current.transform.position, Vector3.down, out hit)){
			if(hit.collider.tag == Globals.GRID){
				canPlace = true;
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
			} else {
				canPlace = false;
			}
		}
		
		Vector3 snapPos = mouseLoc;
		snapPos = SnapToGrid(snapPos, gridX, gridZ, current);
		current.transform.position = snapPos;
		
		for(int i=0; i<forts.Length; i++){
			if(current.gameObject != forts[i].gameObject && forts.Length > 1){
				if(current.collider.bounds.Intersects(forts[i].collider.bounds)){
					canPlace = false;
				}
			}
		}
		
		if(canPlace){
			if(current.GetComponent<MeshRenderer>()){
				current.renderer.material = validGreen;
			} else {
				foreach(MeshRenderer rend in current.GetComponentsInChildren<MeshRenderer>()){
					if(rend.enabled){
						rend.material = validGreen;
					}
				}
			}
		} else {
			if(current.GetComponent<MeshRenderer>()){
				current.renderer.material = invalidRed;
			} else {
				foreach(MeshRenderer rend in current.GetComponentsInChildren<MeshRenderer>()){
					if(rend.enabled){
						rend.material = invalidRed;
					}
				}
			}
		}
		
		// Place current fortification
		if(Input.GetMouseButtonDown(0) && canPlace && GameObject.Find("Tutorial").GetComponent<Tutorial>().key != "QandE" && GameObject.Find("Tutorial").GetComponent<Tutorial>().key != "ClickRight"){
			current.transform.position = SnapToGrid(current.transform.position, gridX, gridZ, current);
			float rot = Mathf.FloorToInt(current.transform.eulerAngles.y);
			if(current.GetComponent<MeshRenderer>()){
				current.renderer.material = originalMat;
			} else {
				for(int i=0; i<originalMats.Count; i++){
					foreach(MeshRenderer rend in current.GetComponentsInChildren<MeshRenderer>()){
						if(rend.enabled){
							rend.material = originalMats[i];
							originalMats.RemoveAt(i);
						}
					}
				}
			}
			DeleteResources(current.GetComponent<SellableItem>().cost);
			current.GetComponent<Dragable>().enabled = true;
			current.GetComponent<Dragable>().canUpdate = true;
			// Keep respawning fortifications until we've run out of money
			if(amountOfResources < current.GetComponent<SellableItem>().cost){
				current = null;
				UIManager.Instance.uiState = UIManager.UIState.NONE;
			} else {
				current = null;
				SpawnFortification(originalObject, rot);
			}
		}
		
		// Cancel current fortification
		if(Input.GetMouseButtonDown(1)){
			UIManager.Instance.uiState = UIManager.UIState.NONE;
			Destroy(current);
			current = null;
			originalMats.Clear();
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
		}
	}
	
	public void UpdateGraphOnDestroyedObject(Collider col, GameObject g){
		Bounds b = col.bounds;
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
			}
		}
	}
	
	public void SpawnRescueShip(){
		shipToSpawn = (GameObject)Instantiate(rescueShip, new Vector3(-100,60,30), Quaternion.identity);
		shipToSpawn.name = rescueShip.name;
	}
	
	public GameObject FindNearestTarget(string nearestTarget, Transform other){
		GameObject[] targets;
		targets = GameObject.FindGameObjectsWithTag(nearestTarget);
		GameObject closest = null;
		float distance = Mathf.Infinity;
		
		foreach(GameObject targetCheck in targets){
			if(targetCheck != other){
				Vector3 diff = targetCheck.transform.position - other.position;
				float curDist = diff.sqrMagnitude;
				if(curDist < distance){
					closest = targetCheck;
					distance = curDist;
				}
			}
		}
		return closest;
	}
}
