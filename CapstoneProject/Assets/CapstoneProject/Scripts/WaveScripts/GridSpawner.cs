using UnityEngine;

public class GridSpawner : MonoBehaviour {
	
	private MeshCollider meshCollider;
	private Renderer[] rend;
	
	public GameObject grid;

	void Awake(){
		GameObject p = ObjectPool.Spawn(grid, new Vector3(12, 0.01f, 9), Quaternion.identity);
		p.name = grid.name;
		p.transform.parent = transform;
		//GetComponent<CombineChildren>().Combine();
	}
	
	void Start(){
		meshCollider = GetComponentInChildren<MeshCollider>();
		rend = GetComponentsInChildren<Renderer>();
		
		DisableGrid();
	}
	
	public void EnableGrid(){
		meshCollider.enabled = true;
		foreach(Renderer r in rend){
			r.enabled = true;
		}
	}
	
	public void DisableGrid(){
		meshCollider.enabled = false;
		foreach(Renderer r in rend){
			r.enabled = false;
		}
	}
}
