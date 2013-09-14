using UnityEngine;

public class NullableVector3 {
	public Vector3 vector;
	
	public NullableVector3(Vector3 vec){
		vector = vec;
	}
	
	public NullableVector3(float x, float y, float z){
		vector = new Vector3(x, y, z);
	}
}