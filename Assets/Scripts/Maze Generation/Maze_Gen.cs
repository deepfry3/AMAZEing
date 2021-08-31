using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reference https://www.algosome.com/articles/maze-generation-depth-first.html

// Worked on by:
//	Declan Doller
//

public class Maze_Gen : MonoBehaviour
{
	// The Bard to be parented to and the floor use for dimentions
	public GameObject m_Board;
	public GameObject m_Floor;

	// The wall prefab
	public GameObject wall;

	// Always will have 4 possible neighbours
	int neighbourCount = 4;

	// Size to be determined by the size of the floor
	public int gridHeight;
	public int gridWidth;
	float floorHeight;

	// Set Path width
	float pathWidth = 1.5f;
	float pathHeight = 1.5f;
	float pathTallness = 2.0f;
	float wallWidth = 1.5f;

	// Stack for the depth first Gen path
	Stack<Grid_Node> m_Path;

	// Starting point
	Vector3 m_Start = Vector3.zero;

	// The 2D grid array
	public Grid_Node[,] m_Grid;

	// On start
	private void Start()
	{
		m_Path = new Stack<Grid_Node>();

		// Setting the Dimensioons of the array
		gridWidth = (int)((m_Floor.transform.localScale.x / (pathWidth + wallWidth)));
		gridHeight = (int)((m_Floor.transform.localScale.z / (pathWidth + wallWidth)));
		floorHeight = m_Floor.transform.localScale.y;
		m_Grid = new Grid_Node[gridWidth, gridHeight];

		// Generates the maze
		CreateGrid();
		int randX, randY;
		randX = Random.Range(0, gridWidth);
		randY = Random.Range(0, gridHeight);
		Generate(m_Grid[randX, randY]);
		InstantiateMaze();
	}

	// Creates the grid
	private void CreateGrid()
	{
		for (int x = 0; x < gridWidth; x++)
		{
			for (int y = 0; y < gridHeight; y++)
			{
				// Creates Gridnodes for each element of the 2d array
				m_Grid[x, y] = new Grid_Node();

				// Sets the scale
				Vector3 pathScale;
				pathScale.x = pathWidth;
				pathScale.y = pathTallness;
				pathScale.z = pathHeight;
				m_Grid[x, y].SetScale(pathScale);

				// Sets the position
				Vector3 position;
				position.x = (m_Start.x + x - (gridWidth / 2)) * (pathWidth + wallWidth);
				position.y = floorHeight + (floorHeight / 2.0f);
				position.z = (m_Start.z + y - gridHeight / 2) * (pathHeight + wallWidth);
				position = position + m_Start;
				m_Grid[x, y].SetPosition(position);

				// Gives it it's owen grid position
				Vector2 gridPos = new Vector2(x, y);
				m_Grid[x, y].SetGridPos(gridPos);
			}
		}
	}

	// Returns the Neighbour at an index
	private Grid_Node GetNeighbour(Grid_Node node, int neighbourIndex)
	{
		if (node != null)
		{
			Vector2 pos = node.GetGridPos();
			int x = (int)pos.x;
			int y = (int)pos.y;

			switch (neighbourIndex)
			{
				// Above neighbour
				case 0: --y; break;

				// Left neighbour
				case 1: ++x; break;

				// Right neighbour
				case 2: --x; break;

				// Bottom neighbour
				case 3: ++y; break;
			}

			// Returns null if the neighbour doesn't exist
			float minX, maxX;
			minX = 0;
			maxX = gridWidth;

			float minY, maxY;
			minY = 0;
			maxY = gridHeight;

			if (x < minX || x >= maxX)
				return null;

			if (y < minY || y >= maxY)
				return null;

			return m_Grid[x, y];
		}

		else
		{
			return null;
		}
	}

	// Generates the maze
	private void Generate(Grid_Node node)
	{
		// The possible  neighbours
		Grid_Node[] neighbour = new Grid_Node[neighbourCount];

		// Selected start node, added to stack and marked as visited
		m_Path.Push(node);

		// continues while the nodes are 
		while (m_Path.Count != 0)
		{
			node.SetVisited();
			node = m_Path.Peek();

			bool[] chosenNeighbour = new bool[4] { false, false, false, false };
			bool neighbourFound = false;
			bool randFound = false;
			int rand;
			int chosenIndex;

			// Gathers all the neighbours
			for (int i = 0; i < neighbourCount; i++)
			{
				neighbour[i] = GetNeighbour(node, i);
				
				// Doesnt bother searching through these neighbours
				if (neighbour[i] == null)
				{
					chosenNeighbour[i] = true;
				}
			}

			// Random Neighbour isn't found
			while (!randFound)
			{
				// Random number between 0 and 3
				rand = Random.Range(0, 4);

				// All neighbours have been searched
				if (chosenNeighbour[0] == true && chosenNeighbour[1] == true && chosenNeighbour[2] == true && chosenNeighbour[3])
				{
					neighbourFound = false;
					randFound = true;
				}

				// The neighbour hasnt been chosen
				else if (chosenNeighbour[rand] == false)
				{
					chosenNeighbour[rand] = true;
					chosenIndex = rand;

					// The neighbour hasn't been visiten before
					if (!neighbour[chosenIndex].IsVisited())
					{
						node.ConnectToNeighbour(chosenIndex);
						node = neighbour[chosenIndex];
						int reverseIndex = ReverseIndex(chosenIndex);
						node.ConnectToNeighbour(reverseIndex);
						m_Path.Push(node);
						neighbourFound = true;
						randFound = true;
					}
				}
			}

			// No neighbour has been found that hasn't been visited
			if (!neighbourFound)
			{
				m_Path.Pop();
			}
		}
	}

	// Spawns maze
	private void InstantiateMaze()
	{
		// The current node and it's neighbour
		Grid_Node node;
		Grid_Node neighbour;

		// Goes through all nodes
		for (int x = 0; x < gridWidth; x++)
		{
			for (int y = 0; y < gridHeight; y++)
			{
				node = m_Grid[x, y];
				
				// Draws a wall if the neighbour isn't connected
				for (int i = 0; i < neighbourCount; i++)
				{
					neighbour = GetNeighbour(node, i);

					if (!node.ConnectedToNeighbour(i) && neighbour != null)
					{
						neighbour.ConnectedToNeighbour(ReverseIndex(i));
						Vector3 pos = Vector3.zero;
						
						GameObject b = Instantiate(wall);
						b.transform.parent = m_Board.transform;

						// Determins if the wall is verticle or horizontal
						switch (i)
						{
							// The upper neighbour
							case 0:
								b.transform.position = HorizontalPosition(node, neighbour);
								b.transform.localScale = HorizontalScale();
								break;

							// The left neighbour
							case 1:
								b.transform.position = VerticalPosition(node, neighbour);
								b.transform.localScale = VerticalScale();
								break;

							// The right neighbour
							case 2:
								b.transform.position = VerticalPosition(node, neighbour);
								b.transform.localScale = VerticalScale();

								break;

							// The bottom neighbour
							case 3:
								b.transform.position = HorizontalPosition(node, neighbour);
								b.transform.localScale = HorizontalScale();
								break;
						}
						 
						// For debug sake Generates a the actual node if it's not visited
						if (!node.IsVisited())
						{
							GameObject a = Instantiate(wall);
							a.transform.parent = m_Board.transform;
							a.transform.position = node.GetPosition();
							a.transform.localScale = node.GetScale();
						}
					}
				}
			}
		}
	}

	// Sets the horizontal position
	public Vector3 HorizontalPosition(Grid_Node node, Grid_Node neighbour)
	{
		Vector3 pos;
		pos.x = node.GetPosition().x;
		pos.y = node.GetPosition().y;
		pos.z = ((node.GetPosition().z + neighbour.GetPosition().z) / 2);
		return pos;
	}

	// Sets the Vertical position
	public Vector3 VerticalPosition(Grid_Node node, Grid_Node neighbour)
	{
		Vector3 pos;
		pos.x = ((node.GetPosition().x + neighbour.GetPosition().x) / 2);
		pos.y = node.GetPosition().y;
		pos.z = node.GetPosition().z;

		return pos;
	}

	// Sets the scale to horizontal
	public Vector3 HorizontalScale()
	{
		Vector3 scale;
		scale.x = 3.0f;
		scale.y = 2.0f;
		scale.z = 0.5f;

		return scale;
	}

	// Sets the scale to Vertical
	public Vector3 VerticalScale()
	{
		Vector3 scale;
		scale.x = 0.5f;
		scale.y = 2.0f;
		scale.z = 3.0f;

		return scale;
	}

	// Returns the reverse of the neighbour index 
	public int ReverseIndex(int reverseIndex)
	{
		switch (reverseIndex)
		{
			case 0:
				reverseIndex = 3;
				break;

			case 1:
				reverseIndex = 2;
				break;

			case 2:
				reverseIndex = 1;
				break;

			case 3:
				reverseIndex = 0;
				break;
		}
		return reverseIndex;
	}
}
