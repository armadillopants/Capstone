using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {
	
	private LineRenderer laser;

	void Start(){
		laser = GetComponent<LineRenderer>();
	}
	
	void Update(){
		
	}
}
