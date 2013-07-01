using UnityEngine;
using System.Collections;

public class FortPlane : MonoBehaviour {
	
	private MeshCollider[] meshColliders;
	private Renderer[] rends;

	void Awake(){
		meshColliders = GetComponentsInChildren<MeshCollider>();
		rends = GetComponentsInChildren<Renderer>();
		
		/*foreach(MeshCollider mesh in meshColliders){
			mesh.enabled = false;
		}
		
		foreach(Renderer rend in rends){
			rend.enabled = false;
		}*/
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
