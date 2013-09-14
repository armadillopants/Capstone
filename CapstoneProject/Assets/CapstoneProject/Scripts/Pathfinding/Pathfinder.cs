using UnityEngine;
using System.Collections.Generic;

public class Pathfinder {
	
	public Tile[,] grid = new Tile[FortPlane.gridX, FortPlane.gridY];
	Vector2 currentTile;
	Vector2 startTile;
	Vector2 endTile;
	
	// Create the lists that store already checked tiles
	List<Vector2> closedList = new List<Vector2>();
	List<Vector2> openList = new List<Vector2>();
	
	public Pathfinder(Tile[,] grid){
		this.grid = grid;
	}
	
	public void FindPath(Vector2 startTile, Vector2 endTile){
		this.startTile = startTile;
		this.endTile = endTile;
		
	    //Reset all the values
        for(int i=0; i<FortPlane.gridX; i++){
            for(int j=0; j<FortPlane.gridY; j++){
                grid[i,j].gScore = 0;
                grid[i,j].hScore = 0;
            }
        }
		
		#region Path Validation
		
		bool canSearch = true;
		
		if(grid[(int)startTile.x, (int)startTile.y].walkable == false){
			Debug.Log("Start tile isnt walkable");
			canSearch = false;
		}
		
		if(grid[(int)endTile.x, (int)endTile.y].walkable == false){
			Debug.Log("End tile isnt walkable");
			canSearch = false;
		}
		
		#endregion
		
		// Begin A* algorithhm
		if(canSearch){
			// Add the starting tile to open list
			openList.Add(startTile);
			currentTile = new Vector2(-1,-1);
			
			// While open list is not empty
			while(openList.Count != 0){
				currentTile = GetTileWithLowestTotal(openList);
				
				// If current tile is the end tile, stop searching
				if(currentTile == endTile){
					Debug.Log("At the end tile");
					break;
				} else {
					// Move current tile to closed list and remove from open list
					openList.Remove(currentTile);
					closedList.Add(currentTile);
					
					// Get all the adjacent tiles
					List<Vector2> adjacentTiles = GetAdjacentTiles(currentTile);
					
					if(adjacentTiles.Count > 0){
						foreach(Vector2 adjacentTile in adjacentTiles){
							// Adjacent tile can not be in open list
							if(!openList.Contains(adjacentTile)){
								// Adjacent tile can not be in closed list
								if(!closedList.Contains(adjacentTile)){
									// Move to open list and calculate cost
									openList.Add(adjacentTile);
									
									Tile tile = grid[(int)adjacentTile.x, (int)adjacentTile.y];
									
									// Calculate the cost
									tile.gScore = grid[(int)currentTile.x, (int)currentTile.y].gScore + 1;
									
									// Calculate the manhattan distance
									tile.hScore = ManhattanDistance(adjacentTile);
									
									// Calculate total amount
									tile.fScore = tile.gScore + tile.hScore;
									
									// Make tile green
									tile.cube.renderer.material.color = Tile.green;
								}
							}
						}
					} else {
						Debug.Log("UMMMMMMM, NO NEIGHBORS ARE WALKABLE FROM START NODE, GET OUTTA THERE");
						return;
					}
				}
			}
		}
		
		// Make start and end tile red
		grid[(int)startTile.x, (int)startTile.y].cube.renderer.material.color = Tile.red;
		grid[(int)endTile.x, (int)endTile.y].cube.renderer.material.color = Tile.red;
		
		// Show path
		ShowPath();
	}
	
	public void ShowPath(){
		bool startFound = false;
		
		Vector2 currentTile = endTile;
		List<Vector2> pathTiles = new List<Vector2>();
		
		while(!startFound){
			List<Vector2> adjacentTiles = GetAdjacentTiles(currentTile);
			
			// Check to see what the newest current tile is
			foreach(Vector2 adjacentTile in adjacentTiles){
				// Check if its the start tile
				if(adjacentTile == startTile){
					startFound = true;
				}
				
				// It has to be inside the closed as well as inside the open list
				if(closedList.Contains(adjacentTile) || openList.Contains(adjacentTile)){
					if(grid[(int)adjacentTile.x, (int)adjacentTile.y].gScore <= grid[(int)currentTile.x, (int)currentTile.y].gScore &&
						grid[(int)adjacentTile.x, (int)adjacentTile.y].gScore > 0){
						// Change the current tile
						currentTile = adjacentTile;
						
						// Add adjacent tile to path list
						pathTiles.Add(adjacentTile);
						
						// Display sphere
						grid[(int)adjacentTile.x, (int)adjacentTile.y].sphere.renderer.material.color = Tile.yellow;
						
						break;
					}
				}
			}
		}
	}
	
	// Check if tiles are in the boundary of grid and if walkable
	public List<Vector2> GetAdjacentTiles(Vector2 currentTile){
		List<Vector2> adjacentTiles = new List<Vector2>();
		Vector2 adjacentTile;
		
		// Tile above
		adjacentTile = new Vector2(currentTile.x, currentTile.y + 1);
		if(adjacentTile.y < FortPlane.gridY && grid[(int)adjacentTile.x, (int)adjacentTile.y].walkable){
			adjacentTiles.Add(adjacentTile);
		}
		
		// Tile below
		adjacentTile = new Vector2(currentTile.x, currentTile.y - 1);
		if(adjacentTile.y >= 0 && grid[(int)adjacentTile.x, (int)adjacentTile.y].walkable){
			adjacentTiles.Add(adjacentTile);
		}
		
		// Right Tile
		adjacentTile = new Vector2(currentTile.x + 1, currentTile.y);
		if(adjacentTile.x < FortPlane.gridX && grid[(int)adjacentTile.x, (int)adjacentTile.y].walkable){
			adjacentTiles.Add(adjacentTile);
		}
		
		// Left Tile
		adjacentTile = new Vector2(currentTile.x - 1, currentTile.y);
		if(adjacentTile.x >= 0 && grid[(int)adjacentTile.x, (int)adjacentTile.y].walkable){
			adjacentTiles.Add(adjacentTile);
		}
		
		// Top Right
		
		// Top Left
		
		// Bottom Right
		
		// Bottom Left
		
		return adjacentTiles;
	}
	
	// Get tile with the lowest total value
	public Vector2 GetTileWithLowestTotal(List<Vector2> openList){
		// Temp variables
		Vector2 tileWithLowestTotal = new Vector2(-1,-1);
		int lowestTotal = int.MaxValue;
		
		// Search all open tiles and get tile with lowest total cost
		foreach(Vector2 openTile in openList){
			if(grid[(int)openTile.x, (int)openTile.y].fScore <= lowestTotal){
				lowestTotal = grid[(int)openTile.x, (int)openTile.y].fScore;
				tileWithLowestTotal = new Vector2((int)openTile.x, (int)openTile.y);
			}
		}
		
		return tileWithLowestTotal;
	}
	
	// Calculate the manhattan distance
	public int ManhattanDistance(Vector2 adjacentTile){
		int x = Mathf.Abs((int)(endTile.x-adjacentTile.x)*((int)(endTile.x-adjacentTile.x)));
		int y = Mathf.Abs((int)(endTile.y-adjacentTile.y)*((int)(endTile.y-adjacentTile.y)));
		
		return x+y;
	}
}
