using System.Collections;
using UnityEngine;

public class ObjectPool : MonoBehaviour {
	
	public static ObjectPool spawner;
	
	private Hashtable activeCachedObjects;
	
	public PooledObjects[] caches;
	
	void Awake(){
		spawner = this;
		
		// Total number of cached objects
		int amount = 0;
		
		for(int i=0; i<caches.Length; i++){
			caches[i].Initialize();
			
			amount += caches[i].cacheSize;
		}
		
		// Create a hashtable with the capacity set to the amount of cached objects specified
		activeCachedObjects = new Hashtable(amount);
	}
	
	public static GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot){
		PooledObjects cache = null;
		
		// Find the cache for the specified prefab
		if(spawner){
			for(int i=0; i<spawner.caches.Length; i++){
				if(spawner.caches[i].prefab == prefab){
					cache = spawner.caches[i];
				}
			}
		}
		
		// If there's no cache for this prefab type, just instantiate normally
		if(cache == null){
			return Instantiate(prefab, pos, rot) as GameObject;
		}
		
		// Find the next object in the cache
		GameObject obj = cache.GetNextObjectInCache();
		
		// Set the postion and rotation of the object
		obj.transform.position = pos;
		obj.transform.rotation = rot;
		
		// Set the object to be active
		obj.SetActive(true);
		spawner.activeCachedObjects[obj] = true;
		
		return obj;
	}
	
	public static void DestroyCachedObject(GameObject obj){
		if(spawner && spawner.activeCachedObjects.ContainsKey(obj)){
			obj.SetActive(false);
			spawner.activeCachedObjects[obj] = false;
		} else {
			Destroy(obj);
		}
	}
	
	public static GameObject GetCachedObject(GameObject prefab){
		PooledObjects cache = null;
		
		if(spawner){
			for(int i=0; i<spawner.caches.Length; i++){
				if(spawner.caches[i].prefab == prefab){
					cache = spawner.caches[i];
				}
			}
		}
		
		return cache.GetActiveObject();
	}
}

[System.Serializable]
public class PooledObjects {
	public GameObject prefab;
	public int cacheSize = 0;
	
	private GameObject[] objects;
	private int cacheIndex = 0;
	
	public void Initialize(){
		objects = new GameObject[cacheSize];
		
		// Instantiate the objects in the array and set them to be inactive
		for(int i=0; i<cacheSize; i++){
			objects[i] = MonoBehaviour.Instantiate(prefab) as GameObject;
			objects[i].SetActive(false);
			objects[i].name = objects[i].name + i;
			objects[i].transform.parent = GameObject.Find("PooledObjects").transform;
		}
	}
	
	public GameObject GetActiveObject(){
		GameObject obj = null;
		
		for(int i=0; i<cacheSize; i++){
			obj = objects[cacheIndex];
			
			// If we found an active object in the cache, use that
			if(obj.activeSelf){
				Debug.Log(obj.name);
				break;
			}
			
			// If not, increment the index and make it loop around
			// if it exceeds the size of the cache
			cacheIndex = (cacheIndex + 1) % cacheSize;
		}
		
		// Increment index and make it loop around
		// if it exceeds the size of the cache
		cacheIndex = (cacheIndex + 1) % cacheSize;

		return obj;
	}
	
	public GameObject GetNextObjectInCache(){
		GameObject obj = null;
		
		// The cacheIndex starts out at the position of the object created
		// the longest time ago, so that one is usually free,
		// but in case not, loop through the cache until we find a free one.
		for(int i=0; i<cacheSize; i++){
			obj = objects[cacheIndex];
			
			// If we found an inactive object in the cache, use that
			if(!obj.activeSelf){
				break;
			}
			
			// If not, increment the index and make it loop around
			// if it exceeds the size of the cache
			cacheIndex = (cacheIndex + 1) % cacheSize;
		}
		
		// The object should be inactive. If it's not, log a warning and use
		// the object created the longest ago even though it's still active.
		if(obj.activeSelf){
			Debug.LogWarning("Spawn of " + prefab.name + 
				" exceeds cache size of " + cacheSize + 
				"! Reusing already active object.", obj);
			ObjectPool.DestroyCachedObject(obj);
		}
		
		// Increment index and make it loop around
		// if it exceeds the size of the cache
		cacheIndex = (cacheIndex + 1) % cacheSize;

		return obj;
	}
}