using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FortPlane : MonoBehaviour {
	
	private MeshCollider[] meshColliders;
	private Renderer[] rends;
	
	public List<GameObject> fortPlanes = new List<GameObject>();
	private Vector3[] quadrants = new Vector3[6];
	
	public GameObject plane;
	private float gridX = 8f;
	private float gridY = 8f;
	private float spacing = 3f;
	
	void Awake(){
		for(int x =0; x<gridX; x++){
			for(int y=0; y<gridY; y++){
				Vector3 pos = new Vector3(x, 0.1f, y) * spacing;
				GameObject p = (GameObject)Instantiate(plane, pos, Quaternion.identity);
				p.name = plane.name;
				p.transform.parent = transform;
				fortPlanes.Add(p);
				
				quadrants[0] = new Vector3(p.transform.position.x-1,0.5f,p.transform.position.z);
				quadrants[1] = new Vector3(p.transform.position.x,0.5f,p.transform.position.z);
				quadrants[2] = new Vector3(p.transform.position.x+1,0.5f,p.transform.position.z);
				
				quadrants[3] = new Vector3(p.transform.position.x,0.5f,p.transform.position.z-1);
				quadrants[4] = new Vector3(p.transform.position.x,0.5f,p.transform.position.z);
				quadrants[5] = new Vector3(p.transform.position.x,0.5f,p.transform.position.z+1);
			}
		}
	}
	
	void Start(){
		meshColliders = GetComponentsInChildren<MeshCollider>();
		rends = GetComponentsInChildren<Renderer>();
		
		/*foreach(MeshCollider mesh in meshColliders){
			mesh.enabled = false;
		}
		
		foreach(Renderer rend in rends){
			rend.enabled = false;
		}*/
	}
	
	public void DeterminePlane(Transform hit, GameObject p){
		if(fortPlanes != null){
			if(fortPlanes.Contains(p)){
				hit.transform.position = DetermineQuadrant(p.transform.position);
			}
		}
	}
	
	Vector3 DetermineQuadrant(Vector3 quadrant){
		if(quadrants != null){
			for(int i=0; i<quadrants.Length; i++){
				if(quadrant == quadrants[i]){
					return quadrant;
				}
			}
		}
		return Vector3.zero;
	}
	
	public void EnablePlanes(){
		foreach(MeshCollider mesh in meshColliders){
			mesh.enabled = true;
		}
		
		foreach(Renderer rend in rends){
			rend.enabled = true;
		}
	}
	
	public void DisablePlanes(){
		foreach(MeshCollider mesh in meshColliders){
			mesh.enabled = false;
		}
		
		foreach(Renderer rend in rends){
			rend.enabled = false;
		}
	}
}
