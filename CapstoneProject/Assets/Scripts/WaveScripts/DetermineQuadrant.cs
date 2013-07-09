using UnityEngine;
using System.Collections;

public class DetermineQuadrant : MonoBehaviour {
	
	private Vector3[] q1 = new Vector3[3];
	private Vector3[] q2 = new Vector3[3];

	void Start(){
		q1[0] = new Vector3(transform.position.x-1,0.5f,transform.position.z);
		q1[1] = new Vector3(transform.position.x,0.5f,transform.position.z);
		q1[2] = new Vector3(transform.position.x+1,0.5f,transform.position.z);
		
		q2[0] = new Vector3(transform.position.x,0.5f,transform.position.z-1);
		q2[1] = new Vector3(transform.position.x,0.5f,transform.position.z);
		q2[2] = new Vector3(transform.position.x,0.5f,transform.position.z+1);
	}
	
	public void QuadrantOne(Transform hit){
		if(Vector3.Distance(q1[0], hit.position) < 1f){
			hit.position = q1[0];
		}
		if(Vector3.Distance(q1[1], hit.position) < 1f){
			hit.position = q1[1];
		}
		if(Vector3.Distance(q1[2], hit.position) < 1f){
			hit.position = q1[2];
		}
	}
	
	public void QuadrantTwo(Transform hit){
		if(Vector3.Distance(q2[0], hit.position) < 1f){
			hit.position = q2[0];
		}
		if(Vector3.Distance(q2[1], hit.position) < 1f){
			hit.position = q2[1];
		}
		if(Vector3.Distance(q2[2], hit.position) < 1f){
			hit.position = q2[2];
		}
	}
}
