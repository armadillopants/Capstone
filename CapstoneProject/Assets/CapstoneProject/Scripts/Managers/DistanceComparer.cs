using UnityEngine;
using System.Collections;

public class DistanceComparer : IComparer {
	
	private GameObject target;
	
	public void SetTarget(GameObject t){
		target = t;
	}
	
	int IComparer.Compare(object a, object b){
		float distToA = Vector3.Magnitude(target.transform.position - ((GameObject)a).transform.position);
    	float distToB = Vector3.Magnitude(target.transform.position - ((GameObject)b).transform.position);

		if(distToA < distToB){
		  return -1;
		}
		if(distToA > distToB){
		  return 1;
		}
		return 0;
	}
}