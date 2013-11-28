using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridSpawner : MonoBehaviour {
	/*public class PlaneData {
		public GameObject plane;
		public Vector3[] quadrants = new Vector3[6];
		
		public bool Equals(GameObject other){
			return plane == other;
		}
	}*/
	
	private MeshCollider meshCollider;
	private Renderer rend;
	
	//public List<PlaneData> planeData = new List<PlaneData>(); 
	
	public GameObject grid;
	//public GameObject wayPoint;
	//public GameObject wayPointMaster;
	//public GameObject cube;
	//public GameObject sphere;
	
	//public GameObject cell;
	//public GameObject node;
	
	//private float X = 10f;
	//private float Y = 10f;
	//public static int gridX = 20;
	//public static int gridY = 20;
	//private float spacing = 3f;
	
	//public Tile[,] grids;
	
	void Awake(){
		GameObject p = (GameObject)Instantiate(grid, new Vector3(transform.position.x, 0.01f, transform.position.z), Quaternion.identity);
		p.name = grid.name;
		p.transform.parent = transform;
		
		//CalculateGrid();
		
		/*for(int x=0; x<X; x++){
			for(int y=0; y<Y; y++){
				Vector3 pos = new Vector3(x, 0.1f, y) * spacing;
				GameObject c = (GameObject)Instantiate(cell, pos+new Vector3(-3,0,-3), Quaternion.identity);
				c.name = cell.name;
				c.transform.parent = transform;
			}
		}
		
		for(int x=0; x<X-1; x++){
			for(int y=0; y<Y; y++){
				Vector3 pos = new Vector3(x, 0.35f, y) * spacing;
				GameObject n = (GameObject)Instantiate(node, pos+new Vector3(-1.5f,0,-3), Quaternion.identity);
				n.name = node.name;
				n.transform.parent = transform;
			}
		}
		
		for(int x=0; x<X; x++){
			for(int y=0; y<Y-1; y++){
				Vector3 pos = new Vector3(x, 0.35f, y) * spacing;
				GameObject n = (GameObject)Instantiate(node, pos+new Vector3(-3,0,-1.5f), Quaternion.identity);
				n.name = node.name;
				n.transform.parent = transform;
			}
		}*/
		
		/*for(int x=0; x<X-1; x++){
			for(int y=0; y<Y-1; y++){
				Vector3 pos = new Vector3(x, 0.35f, y) * spacing;
				GameObject n = (GameObject)Instantiate(node, pos+new Vector3(-1.5f,0,-1.5f), Quaternion.identity);
				n.name = node.name;
				n.transform.parent = transform;
			}
		}*/
		
		/*for(int x=0; x<gridX; x++){
			for(int y=0; y<gridY; y++){
				Vector3 pos = new Vector3(x, 0.3f, y) * spacing;
				GameObject w = (GameObject)Instantiate(wayPoint, pos+new Vector3(-3,0,-3), Quaternion.identity);
				w.name = wayPoint.name;
				w.transform.parent = wayPointMaster.transform;
			}
		}*/
		
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
		meshCollider = GetComponentInChildren<MeshCollider>();
		rend = GetComponentInChildren<Renderer>();
		
		DisableGrid();
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
	
	public void EnableGrid(){
		meshCollider.enabled = true;
		rend.enabled = true;
	}
	
	public void DisableGrid(){
		meshCollider.enabled = false;
		rend.enabled = false;
	}
}
