using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {
	
	public float scrollSpeed = 0.5f;
	public float pulseSpeed = 1.5f;

	public float noiseSize = 1.0f;

	public float maxWidth = 0.5f;
	public float minWidth = 0.2f;

	public GameObject pointer = null;

	private LineRenderer laser;
	private float aniDir = 1.0f;
	
	public GameObject laserLight;
	private GameObject lightObj;

	void Start(){
		laser = GetComponent<LineRenderer>();
		lightObj = (GameObject)Instantiate(laserLight);
		lightObj.name = laserLight.name;
		lightObj.light.enabled = false;
		
		// Change some animation values here and there
		ChoseNewAnimationTargetCoroutine();
	}

	private IEnumerator ChoseNewAnimationTargetCoroutine(){
		while(true){
			aniDir = aniDir * 0.9f + Random.Range(0.5f, 1.5f) * 0.1f;
			yield return null;
			minWidth = minWidth * 0.8f + Random.Range (0.1f, 1.0f) * 0.2f;
			yield return new WaitForSeconds(1.0f + Random.value * 2.0f - 1.0f);	
		}	
	}
	
	void Update(){
		laser.transform.position = GameController.Instance.GetPlayer().GetComponentInChildren<BaseWeapon>().muzzlePos.position;
		if(!GameController.Instance.canShoot){
			laser.enabled = false;
			lightObj.light.enabled = false;
		} else {
			laser.enabled = true;
		}
	}

	void LateUpdate(){
		if(MenuManager.Instance.menuState == MenuManager.MenuState.INGAME){
			float textOffsetX = renderer.material.mainTextureOffset.x;
			textOffsetX += Time.deltaTime * aniDir * scrollSpeed;
			renderer.material.SetTextureOffset("_NoiseTex", new Vector2(-Time.time * aniDir * scrollSpeed, 0.0f));
	
			float aniFactor = Mathf.PingPong(Time.time * pulseSpeed, 1.0f);
			aniFactor = Mathf.Max(minWidth, aniFactor) * maxWidth;
			laser.SetWidth(aniFactor, aniFactor);
		
			// Cast a ray to find out the end point of the laser
			RaycastHit hit = new RaycastHit();
			Vector3 startPos = transform.position;
			Vector3 dir = transform.rotation * new Vector3(0,0,1);
			Ray ray = new Ray(startPos, dir);
			
			if(Physics.Raycast(ray, out hit) && GameController.Instance.canShoot){
				if(!hit.collider.isTrigger){
					laser.SetPosition(1, (hit.distance * Vector3.forward));
					textOffsetX = 0.1f * (hit.distance);
					renderer.material.SetTextureScale("_NoiseTex", new Vector2(0.1f * hit.distance * noiseSize, noiseSize));		
					
					// Use point and normal to align a nice & rough hit plane
					if(pointer){
						pointer.renderer.enabled = true;
						lightObj.light.enabled = true;
						lightObj.transform.position = hit.point + (transform.position - hit.point) * 0.01f;
						pointer.transform.position = hit.point + (transform.position - hit.point) * 0.01f;
						pointer.transform.rotation = Quaternion.LookRotation(hit.normal, transform.up);
						pointer.transform.eulerAngles = new Vector3(90.0f, 0, 0);
					}
				} else {
					if(pointer){
						pointer.renderer.enabled = false;
						lightObj.light.enabled = false;
					}
					float maxDist = 200.0f;
					laser.SetPosition(1, (maxDist * Vector3.forward));
					textOffsetX = 0.1f * (maxDist);		
					renderer.material.SetTextureScale("_NoiseTex", new Vector2(0.1f * (maxDist) * noiseSize, noiseSize));
				}
			} else {
				if(pointer){
					pointer.renderer.enabled = false;
					lightObj.light.enabled = false;
				}
				float maxDist = 200.0f;
				laser.SetPosition(1, (maxDist * Vector3.forward));
				textOffsetX = 0.1f * (maxDist);		
				renderer.material.SetTextureScale("_NoiseTex", new Vector2(0.1f * (maxDist) * noiseSize, noiseSize));		
			}
			renderer.material.mainTextureOffset = new Vector2(textOffsetX, 0);
		}
	}
}
	
	
	
	
	/*private LineRenderer laser;
	private float width = 0.01f;
	private float length = 30f;
	
	public GameObject laserLight;
	private GameObject lightObj;

	void Start(){
		laser = GetComponent<LineRenderer>();
		laser.SetVertexCount(2);
		laser.SetWidth(width, width);
		lightObj = (GameObject)Instantiate(laserLight);
		lightObj.name = laserLight.name;
		lightObj.light.enabled = false;
	}
	
	void Update(){
		laser.transform.position = GameObject.FindWithTag(Globals.PLAYER).GetComponentInChildren<BaseWeapon>().muzzlePos.position;
		if(!GameController.Instance.canShoot){
			laser.enabled = false;
			lightObj.light.enabled = false;
		} else {
			laser.enabled = true;
		}
	}
	
	void LateUpdate(){
		if(MenuManager.Instance.menuState == MenuManager.MenuState.INGAME){
			Vector3 startPos = transform.position;
			Vector3 dir = transform.rotation * new Vector3(0,0,1);
			Vector3 endPos = startPos + dir * length;
			
			Ray ray = new Ray(startPos, dir);
			RaycastHit hit = new RaycastHit();
			
			if(Physics.Raycast(ray, out hit) && GameController.Instance.canShoot){
				if(!hit.collider.isTrigger){
					laser.SetPosition(0, startPos);
					laser.SetPosition(1, hit.point);
					lightObj.transform.position = hit.point + hit.normal * 0.2f;
					lightObj.light.enabled = true;
				} else {
					laser.SetPosition(0, startPos);
					laser.SetPosition(1, endPos);
					lightObj.light.enabled = false;
				}
			} else {
				laser.SetPosition(0, startPos);
				laser.SetPosition(1, endPos);
				lightObj.light.enabled = false;
			}
		}
	}
}*/