using UnityEngine;

public class GridSpawner : MonoBehaviour {
	
	private MeshCollider meshCollider;
	private Renderer rend;
	
	public GameObject grid;

	void Awake(){
		GameObject p = (GameObject)Instantiate(grid, new Vector3(12, 0.01f, 9), Quaternion.identity);
		p.name = grid.name;
		p.transform.parent = transform;
		GetComponent<CombineChildren>().Combine();
	}
	
	void Start(){
		meshCollider = GetComponentInChildren<MeshCollider>();
		rend = GetComponent<Renderer>();
		
		DisableGrid();
	}
	
	public void EnableGrid(){
		meshCollider.enabled = true;
		rend.enabled = true;
	}
	
	public void DisableGrid(){
		meshCollider.enabled = false;
		rend.enabled = false;
	}
}
