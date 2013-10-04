using UnityEngine;

public class Laser : MonoBehaviour {
	
	private LineRenderer laser;
	private float width = 0.05f;
	private float length = 10f;
	
	public GameObject light;
	private GameObject lightObj;

	void Start(){
		laser = GetComponent<LineRenderer>();
		laser.SetVertexCount(2);
		laser.SetWidth(width, width);
		lightObj = (GameObject)Instantiate(light);
		lightObj.light.enabled = false;
	}
	
	void LateUpdate(){
		Vector3 startPos = transform.position;
		Vector3 dir = transform.rotation * new Vector3(0,0,1);
		Vector3 endPos = startPos + dir * length;
		
		Ray ray = new Ray(startPos, dir);
		RaycastHit hit = new RaycastHit();
		
		if(Physics.Raycast(ray, out hit)){
			laser.SetPosition(0, startPos);
			laser.SetPosition(1, hit.point);
			lightObj.transform.position = hit.point + hit.normal * 0.2f;
			lightObj.light.enabled = true;
		} else {
			laser.SetPosition(0, startPos);
			laser.SetPosition(1, endPos);
			lightObj.light.enabled = false;
		}
	}
}
