using UnityEngine;
using System.Collections;

public class MapGen : MonoBehaviour
{
	public class Map
	{
		public int length;
		public int width;
		
		// Default Constructor, defaults a 2x2 block map
		public Map()
		{
			length = 2;
			width = 2;
		}
		
		// Constructor for single-input, creates a square map from input
		public Map(int d)
		{
			length = d;
			width = d;
		}
		
		// Constructor for both inputs, creates map of custom length+width
		public Map(int l, int w)
		{
			length = l;
			width = w;
		}
	}
	
	public Transform block;
	public Map gameMap = new Map();
	
	void Awake()
	{
		// Create a border of 1 block around the actual desired area
		for(int i=0; i<gameMap.length+2; i++)
		{
			for(int j=0; j<gameMap.width+2; j++)
			{
				
				Transform b = (Transform)Instantiate(block);
				Debug.Log(b.collider.bounds.size);
				Vector3 pos = new Vector3(i*b.collider.bounds.size.x, 1, j*b.collider.bounds.size.z);
				b.transform.position = pos;
			}
		}
		
	}
}