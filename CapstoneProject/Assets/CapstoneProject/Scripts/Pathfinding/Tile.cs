using UnityEngine;

public class Tile {
	
	public Vector2 id;
	public GameObject cube;
	public GameObject sphere;
	public int gScore = 0; // Cost of tile
	public int hScore = 0; // Estimated cost to get to tile
	public int fScore = 0; // Total of g and h
	public bool walkable;
	
	public static Vector4 red = new Vector4(255,0,0,0);
	public static Vector4 green = new Vector4(0,255,0,0);
	public static Vector4 black = new Vector4(0,0,0,1);
	public static Vector4 pink = new Vector4(60,60,60,0);
	public static Vector4 yellow = new Vector4(255,255,0,0);
	
	public Tile(Vector2 id, Vector3 scale, Vector3 pos, GameObject cube, GameObject sphere, bool walkable){
		this.id = id;
		this.cube = cube;
		this.sphere = sphere;
		this.walkable = walkable;
		this.cube.renderer.enabled = true;
		
		// Postion and scale
		this.cube.transform.localScale = scale;
		this.cube.transform.position = pos + new Vector3(-21,0,-21);
		
		this.sphere.transform.localScale = scale;
		this.sphere.transform.position = new Vector3(pos.x, pos.y+1, pos.z) + new Vector3(-21,0,-21);
		
		// If tile isnt walkable, set it to black
		if(!this.walkable){
			this.cube.renderer.material.color = Tile.black;
		} else {
			this.cube.renderer.material.color = Tile.pink;
		}
		
		/*GameObject c = (GameObject)Object.Instantiate(this.cube, pos+new Vector3(-3,0,-3), Quaternion.identity);
		c.transform.parent = GameObject.Find("WaypointMaster").transform;
		
		GameObject s = (GameObject)Object.Instantiate(this.sphere, pos+new Vector3(-3,1,-3), Quaternion.identity);
		s.transform.parent = GameObject.Find("WaypointMaster").transform;*/
		
		cube.transform.parent = GameObject.Find("WaypointMaster").transform;
		sphere.transform.parent = GameObject.Find("WaypointMaster").transform;
	}
}
