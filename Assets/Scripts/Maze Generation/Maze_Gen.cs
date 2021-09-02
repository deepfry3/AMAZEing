using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reference https://www.algosome.com/articles/maze-generation-depth-first.html

// Worked on by:
//	Declan Doller
//

public class Maze_Gen : MonoBehaviour
{
	// The Board to be parented to and the floor use for dimentions
	// Bard is the parent object the transforms will use
	public GameObject m_Board;
	public GameObject m_Floor;

	// The wall, ball and gem prefabs
	public GameObject wallPrefab;
	public GameObject ballPrefab;
	public GameObject gemPrefab;

	private GameObject m_Maze;
	private GameController m_GameController;

	// Always will have 4 possible neighbours
	private const int neighbourCount = 4;

	// Size to be determined by the size of the floor
	private float gridHeight;
	private float gridWidth;
	private float floorHeight;

	private int nodeCountX;
	private int nodeCountY;

	// Set Path width
	private float pathWidth = 1.5f;
	private float pathHeight = 1.5f;
	private float pathTallness = 2.0f;
	private float wallWidth = 1.5f;

	// Stack for the depth first Gen path
	Stack<Grid_Node> m_Path;

	Grid_Node m_StartNode;
	Grid_Node m_EndNode;

	// Starting point
	Vector3 m_Start = Vector3.zero;

	// The 2D grid array
	public Grid_Node[,] m_Grid;

	private int m_GemCount;

	// On start
	private void Start()
	{
		m_Maze = GameObject.FindGameObjectWithTag("Maze");
		if (m_Maze != null)
		{
			m_GameController = m_Maze.GetComponent<GameController>();
		}

		m_Path = new Stack<Grid_Node>();

		// Setting the Dimensioons of the array
		gridWidth = (m_Board.transform.localScale.x * m_Floor.transform.localScale.x);
		gridHeight = (m_Board.transform.localScale.z * m_Floor.transform.localScale.z);
		nodeCountX = (int)(gridWidth / (pathWidth + wallWidth));
		nodeCountY = (int)(gridHeight / (pathWidth + wallWidth));

		floorHeight = m_Floor.transform.localScale.y;
		m_Grid = new Grid_Node[nodeCountX, nodeCountY];

		// Generates the maze
		CreateGrid();
		int randX, randY;
		randX = Random.Range(0, nodeCountX);
		randY = Random.Range(0, nodeCountY);

		m_GemCount = Random.Range(1, 4);
		m_GameController.SetGemCount(m_GemCount);
		Generate(m_Grid[randX, randY]);
		InstantiateMaze();
	}

	// Creates the grid
	private void CreateGrid()
	{
		for (int x = 0; x < nodeCountX; x++)
		{
			for (int y = 0; y < nodeCountY; y++)
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
				position.x = (m_Start.x - (gridWidth / 2)) + ((x + 1) * (pathWidth + wallWidth));
				position.y = m_Floor.transform.position.y + floorHeight + (floorHeight / 2.0f);
				position.z = (m_Start.z - (gridHeight / 2)) + ((y + 0.5f) * (pathHeight + wallWidth));

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
			maxX = nodeCountX;

			float minY, maxY;
			minY = 0;
			maxY = nodeCountY;

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
		// sets the start and end nodes
		m_StartNode = node;

		int randX, randY;
		randX = Random.Range(0, nodeCountX);
		randY = Random.Range(0, nodeCountY);
		m_EndNode = m_Grid[randX, randY];

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
		for (int x = 0; x < nodeCountX; x++)
		{
			for (int y = 0; y < nodeCountY; y++)
			{
				node = m_Grid[x, y];
				node.SetVisited();

				// Draws a wall if the neighbour isn't connected
				for (int i = 0; i < neighbourCount; i++)
				{
					neighbour = GetNeighbour(node, i);

					if (!node.ConnectedToNeighbour(i) && neighbour != null)
					{
						neighbour.ConnectToNeighbour(ReverseIndex(i));
						Vector3 pos = Vector3.zero;

						GameObject b = Instantiate(wallPrefab);
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
							GameObject a = Instantiate(wallPrefab);
							a.transform.parent = m_Board.transform;
							a.transform.position = node.GetPosition();
							a.transform.localScale = node.GetScale();
						}
					}
				}
			}
		}

		GameObject ball = Instantiate(ballPrefab);
		Vector3 Offset = new Vector3(0, 5, 0);
		ball.transform.position = (m_StartNode.GetPosition() + Offset);

		for (int i = 0; i < m_GemCount; i++)
		{
			int randX, randY;
			randX = Random.Range(0, nodeCountX);
			randY = Random.Range(0, nodeCountY);
			GameObject gem = Instantiate(gemPrefab);
			gem.transform.position = m_Grid[randX, randY].GetPosition();
			gem.transform.parent = m_Board.transform;
			Debug.Log("Spawned Gem");
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
		scale.x = 3.0f / m_Board.transform.localScale.x;
		scale.y = 2.0f / m_Board.transform.localScale.y;
		scale.z = 0.5f / m_Board.transform.localScale.z;

		return scale;
	}

	// Sets the scale to Vertical
	public Vector3 VerticalScale()
	{
		Vector3 scale;
		scale.x = 0.5f / m_Board.transform.localScale.x;
		scale.y = 2.0f / m_Board.transform.localScale.y;
		scale.z = 3.0f / m_Board.transform.localScale.z;

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

	// Returns the start node
	public Grid_Node GetStartNode()
	{
		return m_StartNode;
	}

	// Returns the Final node
	public Grid_Node GetEndNode()
	{
		return m_EndNode;
	}


}
