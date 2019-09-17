using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
	public bool onlyDisplayPathGizmos;
	public Transform player;
	public LayerMask unwalkableMask;

	//define area in worldcoordinates that grid is covering, able to be set by user in editor
	public Vector2 gridWorldSize;

	//define how much space each indiviual node covers  
	public float nodeRadius;

	//creates a two dimensional array w. id grid, represent the nodegrid
	Node[,] grid;

	private float nodeDiameter;
	private int gridSizeX, gridSizeY;

	private void Start()
	{
		//calculates the nodediameter
		nodeDiameter = nodeRadius * 2;
		//how many nodes we can fit into to the worldsize x 
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
		//how many nodes we can fit into to the worldsize y
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

		//calls the creategrid method
		CreateGrid();
	}

	public int MaxSize
	{
		get { return gridSizeX * gridSizeY; }
	}

	/// <summary>
	/// Creates a grid of nodes
	/// </summary>
	private void CreateGrid()
	{
		//creates a grid with gridsizeX and gridsizeY as its size
		grid = new Node[gridSizeX, gridSizeY];
		
		//Gets the bottomleft corner of our world
		//takes the transform.position which is (0,0) and subtracts 30/2 on x-axis and 30/2 on y-axis. Beacuse the gridsizeX is 30 and Y is 30.
		//worldbottomLeft = (0,0) - (-15,0) - (0,-15) = (-15, -15)
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;

		//for-loop that loops through all gridpositions that nodes will be in. If they are walkable or not
		for (int x = 0; x < gridSizeX; x++)
		{
			for (int y = 0; y < gridSizeY; y++)
			{
				//gets the point that a node is going to occupy in our world
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				
				//walkable is going to be true if we don´t collied with anything in the unwalkable mask
				bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

				//filling the grid with nodes
				grid[x, y] = new Node(walkable, worldPoint, x, y);

			}
		}	
	}

	/// <summary>
	/// Gets the neighbouring nodes to a specific node
	/// </summary>
	/// <param name="node"> from which to get neighbouring nodes </param>
	/// <returns> list of neighbouring nodes </returns>
	public List<Node> GetNeighbours(Node node)
	{
		//creates a new list that takes nodes w. id neighbours
		List<Node> neighbours = new List<Node>();

		//for-loop that searches 3 by 3 blocks around a specific node
		for (int x = -1; x <= 1 ; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				//IF we get to position x = 0 and y = 0 we are at the node we want to get neighbouring nodes from, so we don´t want to include that node so we skipp it by continue
				if(x == 0 && y == 0)
				{
					continue;
				}

				//creates two int variable w. id checkX and checkY, there value is set equal to a specific node x- and y-coordinate value plus the x and y value from the for-loop´s  
				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				//IF checkX and checkY is inside the nodegrid
				if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
				{
					//then we add that node to our neighbour list
					neighbours.Add(grid[checkX, checkY]);
				}
			}
		}
		//returns the neighbour list w. neighbouring nodes
		return neighbours;
	}

	/// <summary>
	/// Converts a worldpoint into gridcoordinates
	/// </summary>
	/// <param name="worldPosition"> where we want to get a gridcoordinate </param>
	/// <returns> node at specific gridcoordinate </returns>
	public Node NodeFromWorldPoint(Vector3 worldPosition)
	{
		//converts the worldposition into a percent on how far along it´s on the x- or y-axis, 0 percent is on the far left and 0,5 percent is in the middle and 1 percent on the far right
		//So i.ex on the x-axis, percentX is equal to (worldposition of X plus gridworldsize X divided by 2) divided by gridworldsize X again.
		// So if worldposition is -15 and the gridworldsize is 30 we get: (-15 + 30/2) / 30 = (-15 + 15)/30 = 0/30 = 0 percent which is on the far left 
		float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.x / 2) / gridWorldSize.y;
		
		//if the worldposition is outside of the nodegrid it dosn´t give us a invalied index
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		//get the x and y in the grid array, we subtracts 1 beacuse arrays are 0-based. The array has 30 elements between 0-29. Otherwise we would get an index out of bound
		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

		//returns the node from or grid at coordinates x and y
		return grid[x, y];
	}

	public List<Node> path;

	/// <summary>
	/// Draws the nodegrid, walkable and not, and the node where our player is positioned
	/// </summary>
	private void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

		if (onlyDisplayPathGizmos)
		{
			if (path != null)
			{
				foreach (Node n in path)
				{
					Gizmos.color = Color.black;
					Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
				}
			}
		}
		else
		{
			if (grid != null)
			{
				Node playerNode = NodeFromWorldPoint(player.position);

				foreach (Node n in grid)
				{
					//if n is walkable we set the cube to color white otherwise to red
					Gizmos.color = (n.walkable) ? Color.white : Color.red;

					if (path != null)
					{
						if (path.Contains(n))
						{
							Gizmos.color = Color.black;
						}
					}

					if (playerNode == n)
					{
						Gizmos.color = Color.cyan;
					}

					//for each node in the grid we draw a cube
					Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
				}
			}
		}
	}

}

