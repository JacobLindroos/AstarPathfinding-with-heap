using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class Pathfinding : MonoBehaviour
{
	//creates two public Transform w. id seeker and target
	public Transform seeker, target;

	//creates a reference to our grid class
	Grid grid;

	private void Awake()
	{
		//lets us get methods in our grid class
		grid = GetComponent<Grid>();
	}

	private void Update()
	{
		if (Input.GetButtonDown("Jump"))
		{
			//Finds path between seekerNode and targetNode
			FindPath(seeker.position, target.position);
		}
	}

	/// <summary>
	/// Finds the path to targetnode
	/// </summary>
	/// <param name="startPos"> to search from </param>
	/// <param name="targetPos"> to search to </param>
	private void FindPath(Vector3 startPos, Vector3 targetPos)
	{
		Stopwatch sw = new Stopwatch();
		sw.Start();

		//startnode and targetnode positions is set using the nodefromworldpoint method from grid class, which converts startPos from the world into gridcoordniates and gets that specific node
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);

		//creates a new list that takes nodes w. id openSet. Stores all nodes to be evaluated
		Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
		//creates a new hashset-list that take nodes w. id closedSet. Stores all the nodes already evaluated which creates the path w. lowest cost
		HashSet<Node> closedSet = new HashSet<Node>();
		
		//adds the startnode to openSet list, where we will start searching from
		openSet.Add(startNode);

		//while-loop that will loop until openSet list is greater then 0
		while (openSet.Count > 0)
		{
			//creates a new node w. id currentnode and sets it equal to the node at the first element, which is index 0
			Node currentNode = openSet.RemoveFirstItemOfHeap();
			closedSet.Add(currentNode);

			//IF currentnode is equal to the targetnode we have found are path, exit the loop
			if(currentNode == targetNode)
			{
				sw.Stop();
				print("Path found: " + sw.ElapsedMilliseconds + " ms");
				RetracePath(startNode, targetNode);
				return;
			}

			//otherwise we need to loop through and search the neighbouring nodes of the currentnode, using a foreach-loop
			foreach (Node neighbour in grid.GetNeighbours(currentNode))
			{
				//IF neighbouring node is not walkable OR already exsist in the closedset list we skipp that specific node and goes to the next node instead
				if(!neighbour.walkable || closedSet.Contains(neighbour))
				{
					continue;
				}

				//int variable w. id newmovementcosttoneighbour, which is equal to the currentnode Gcost plus the cost of getting to the currentnode to the neighbournode
				int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

				//IF newMovementToNeighbour path is shorter/cost´s less in Gcost OR IF NOT neighbour is not in the openSet list
				if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
				{
					//in that case we change the Fcost by changing H- and Gcost
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					//set the parent of neighbourNode to the currentnode
					neighbour.parent = currentNode;

					//IF the neighbourNode is NOT in the openSet list
					if (!openSet.Contains(neighbour))
					{
						//Add the neighbourNode to the openSet list
						openSet.Add(neighbour);
					}
				}
			}
		}
	}

	/// <summary>
	/// Retraces the path from targetnode to startnode
	/// </summary>
	/// <param name="startNode"> where to trace from </param>
	/// <param name="endNode"> where to trace to </param>
	private void RetracePath(Node startNode, Node endNode)
	{
		//creates a new List that take Node w. id path, holds the nodes which creates the path
		List<Node> path = new List<Node>();

		//new Node w. id currentnode which is set to endnode
		Node currentNode = endNode;

		//retraces our steps until we have reach the startingnode 
		while (currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		//gets the path right way around
		path.Reverse();

		grid.path = path;
	}

	/// <summary>
	/// Gets the distance between any two given nodes
	/// </summary>
	/// <param name="nodeA"> to get distance from </param>
	/// <param name="nodeB"> to get distance to </param>
	/// <returns> distance in an int value </returns>
	private int GetDistance(Node nodeA, Node nodeB)
	{
		//int variable w. id dstX, refere to the distance on the x-axis. Which is equal to nodeA´s x position subtracted w. nodeB´s y position
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		//int variable w. id dstY, refere to the distance on the y-axis. Which is equal to nodeA´s y position subtracted w. nodeB´s x position
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		//IF the int value for dstX is greater then dstY value
		if(dstX > dstY)
		{
			//then we return the result of this algoritm int value
			return 14*dstY + 10* (dstX - dstY);
		}
		//oterwise the result of this algoritm int value
		return 14*dstX + 10 * (dstY - dstX);
	}
}

