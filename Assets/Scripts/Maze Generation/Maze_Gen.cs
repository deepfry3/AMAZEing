using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reference https://www.algosome.com/articles/maze-generation-depth-first.html
public class Maze_Gen : MonoBehaviour
{
	public GameObject m_Board;
	public GameObject m_Floor;
	public GameObject m_Wall;

	int neighbourCount = 4;
	public int gridHeight;
	public int gridWidth;

	int wallSize = 1;
	float floorHeight = 0.5f;

	float pathWidth = 1.5f;
	float pathHeight = 1.5f;
	float pathTallness = 2.0f;

	float nodeWidth = 0.5f;
	float nodeTallness = 2.0f;


	float wallWidth = 1.5f;
	float wallHeight = 2.0f;

	Stack<Grid_Node> m_Path;

	Vector3 m_Start = Vector3.zero;
	public Grid_Node[,] m_Grid;
	public GameObject wall;

	private void Start()
	{
		m_Path = new Stack<Grid_Node>();
		gridWidth = (int)((m_Floor.transform.localScale.x / (pathWidth + wallWidth)));
		gridHeight = (int)((m_Floor.transform.localScale.z / (pathWidth + wallWidth)));
		m_Grid = new Grid_Node[gridWidth, gridHeight];
		CreateGrid();
		Generate(m_Grid[0, 0]);
		InstantiateMaze();
	}

	// Creates the grid
	private void CreateGrid()
	{
		for (int x = 0; x < gridWidth; x++)
		{
			for (int y = 0; y < gridHeight; y++)
			{
				m_Grid[x, y] = new Grid_Node();

				Vector3 pathScale;
				pathScale.x = pathWidth;
				pathScale.y = pathTallness;
				pathScale.z = pathHeight;
				m_Grid[x, y].SetScale(pathScale);

				Vector3 position;
				position.x = (m_Start.x + x - (gridWidth / 2)) * (pathWidth + wallWidth);
				position.y = floorHeight + (floorHeight / 2.0f);
				position.z = (m_Start.z + y - gridHeight / 2) * (pathHeight + wallWidth);
				position = position + m_Start;
				m_Grid[x, y].SetPosition(position);

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

	// The code that Generates the path for the maze
	private void Generate(Grid_Node node)
	{
		Grid_Node[] neighbour = new Grid_Node[4];

		// Selected start node, added to stack and marked as visited
		m_Path.Push(node);

		while (m_Path.Count != 0)
		{
			node.SetVisited();
			node = m_Path.Peek();

			// Gathers all the neighbours
			for (int i = 0; i < neighbourCount; i++)
			{
				neighbour[i] = GetNeighbour(node, i);
			}

			bool neighbourFound = false;

			// Selects a neighbour that hasn't been visited 
			for (int i = 0; i < neighbourCount; i++)
			{
				if (neighbour[i] != null && !neighbour[i].IsVisited())
				{
					node.ConnectToNeighbour(i);
					node = neighbour[i];
					int reverseIndex = ReverseIndex(i);
					node.ConnectToNeighbour(reverseIndex);
					m_Path.Push(node);
					neighbourFound = true;
					break;
				}
			}


			if (!neighbourFound)
			{
				m_Path.Pop();
			}
			//int[] Num = new int[4] {0, 1, 2, 10};
			//bool randFound = false;
			//int rand = 10;

			//while (randFound == false)
			//{
			//	rand = Random.Range(0, 4);

			//	for (int i = 0; i < Num.Length; i++)
			//	{
			//		if (rand == Num[i])
			//		{
			//			Num[i] = 10;
			//			randFound = true;
			//			break;
			//		}
			//	}
			//}

			//while (randFound == false)
			//{

			//do
			//{
			//	rand = Random.Range(0, 4);
			//	//randFound = true;
			//	//Num[i] = 10;
			//	//break;
			//	if (neighbour[rand] != null && !neighbour[rand].IsVisited())
			//	{
			//		lastPos = rand;
			//		Debug.Log("rand found" + rand);
			//		node.ConnectToNeighbour(rand);
			//		node = neighbour[rand];
			//		int reverseIndex = ReverseIndex(rand);
			//		node.ConnectToNeighbour(reverseIndex);
			//		m_Path.Push(node);
			//		neighbourFound = true;

			//	//	break;
			//	}

			//	//}
			//} while (rand == lastPos);

			// while all neighbours have been visited 
			
		}
	}
	//}

	//for (int i = 0, i < neighbour.Length; i++)
	//{




	// Spawns maze
	private void InstantiateMaze()
	{
		// The current node and it's neighbour
		Grid_Node node;
		Grid_Node neighbour;

		for (int x = 0; x < gridWidth; x++)
		{
			for (int y = 0; y < gridHeight; y++)
			{
				node = m_Grid[x, y];

				for (int i = 0; i < neighbourCount; i++)
				{
					neighbour = GetNeighbour(node, i);

					if (!node.ConnectedToNeighbour(i) && neighbour != null)
					{

						Vector3 pos = Vector3.zero;

						switch (i)
						{
							// The upper neighbour
							case 0:
								wall.transform.position = HorizontalPosition(node, neighbour);
								wall.transform.localScale = HorizontalScale();
								break;

							// The left neighbour
							case 1:
								wall.transform.position = VerticalPosition(node, neighbour);
								wall.transform.localScale = VerticalScale();
								break;

							// The right neighbour
							case 2:
								wall.transform.position = VerticalPosition(node, neighbour);
								wall.transform.localScale = VerticalScale();

								break;

							// The bottom neighbour
							case 3:
								wall.transform.position = HorizontalPosition(node, neighbour);
								wall.transform.localScale = HorizontalScale();
								break;
						}
						 
						
						GameObject b = Instantiate(wall);
						b.transform.parent = m_Board.transform;

						// For debug sake
						if (!node.IsVisited())
						{
							wall.transform.position = node.GetPosition();
							wall.transform.localScale = node.GetScale();
							GameObject a = Instantiate(wall);
							a.transform.parent = m_Board.transform;
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
