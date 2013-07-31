using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FortPlane : MonoBehaviour {
	/*public class PlaneData {
		public GameObject plane;
		public Vector3[] quadrants = new Vector3[6];
		
		public bool Equals(GameObject other){
			return plane == other;
		}
	}*/
	
	private MeshCollider[] meshColliders;
	private Renderer[] rends;
	
	//public List<PlaneData> planeData = new List<PlaneData>(); 
	
	public GameObject plane;
	//private float gridX = 8f;
	//private float gridY = 8f;
	//private float spacing = 3f;
	
	void Awake(){
		GameObject p = (GameObject)Instantiate(plane, new Vector3(transform.position.x, 0.1f, transform.position.z), Quaternion.identity);
		p.name = plane.name;
		p.transform.parent = transform;
		/*for(int x=0; x<gridX; x++){
			for(int y=0; y<gridY; y++){
				Vector3 pos = new Vector3(x, 0.1f, y) * spacing;
				GameObject p = (GameObject)Instantiate(plane, pos, Quaternion.identity);
				p.name = plane.name;
				p.transform.parent = transform;
				
				PlaneData temp = new PlaneData();
				temp.plane = p;
				
				// For horizontal snapping
				temp.quadrants[0] = new Vector3(p.transform.position.x-1,0.5f,p.transform.position.z);
				temp.quadrants[1] = new Vector3(p.transform.position.x,0.5f,p.transform.position.z);
				temp.quadrants[2] = new Vector3(p.transform.position.x+1,0.5f,p.transform.position.z);
				
				// For vertical snapping
				temp.quadrants[3] = new Vector3(p.transform.position.x,0.5f,p.transform.position.z-1);
				temp.quadrants[4] = new Vector3(p.transform.position.x,0.5f,p.transform.position.z);
				temp.quadrants[5] = new Vector3(p.transform.position.x,0.5f,p.transform.position.z+1);
				
				planeData.Add(temp);
			}
		}*/
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
	
	/*public NullableVector3 DeterminePlane(Transform hit, GameObject plane, Vector3 point){
		if(planeData != null){
			PlaneData data = planeData.Find(x => x.Equals(plane));
			
			if(data != null){
				point = DetermineQuadrant(data, hit);
				return new NullableVector3(point);
			} else {
				return null;
			}
		}
		return null;
	}
	
	Vector3 DetermineQuadrant(PlaneData data, Transform hit){
		Vector3 closestPoint = Vector3.zero;
		if(hit.eulerAngles.y == Globals.ROTATION_H_LEFT || hit.eulerAngles.y == Globals.ROTATION_H_RIGHT){
			for(int i=0; i<3; i++){
				if(i == 0){
					closestPoint = data.quadrants[i];
				} else if((closestPoint-hit.position).sqrMagnitude > (data.quadrants[i]-hit.position).sqrMagnitude){
					closestPoint = data.quadrants[i];
				}
			}
		}
		
		if(hit.eulerAngles.y == Globals.ROTATION_V_UP || hit.eulerAngles.y == Globals.ROTATION_V_DOWN){
			for(int i=3; i<6; i++){
				if(i == 3){
					closestPoint = data.quadrants[i];
				} else if((closestPoint-hit.position).sqrMagnitude > (data.quadrants[i]-hit.position).sqrMagnitude){
					closestPoint = data.quadrants[i];
				}
			}
		}
		return closestPoint;
	}*/
	
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
