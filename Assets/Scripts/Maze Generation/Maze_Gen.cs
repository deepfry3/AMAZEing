using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze_Gen : MonoBehaviour
{
	public GameObject m_Board;
	public GameObject m_Floor;
	public GameObject m_Wall;

	public int gridHeight;
	public int gridWidth;

	int wallSize = 1;
	float floorHeight = 0.5f;
	float nodeHeight = 0.5f;
	float nodeWidth = 0.5f;
	float nodeTallness = 2;
	int cellsVisited = 0;

	Stack<Grid_Node> m_Path;
	
	Vector3 m_Start = Vector3.zero;
	Grid_Node[,] m_Grid;
	public GameObject wall;

	private void Start()
	{
		m_Path = new Stack<Grid_Node>();
		gridWidth = (int)((m_Floor.transform.localScale.x - wallSize) / nodeWidth);
		gridHeight = (int)((m_Floor.transform.localScale.z - wallSize) / nodeHeight);
		m_Grid = new Grid_Node[gridWidth, gridHeight];
		m_Start.x = 0;
		m_Start.y = 0;
		m_Start.z = 0;
		CreateGrid();
		InstantiateMaze();
		Generate();
	}

	private void CreateGrid()
	{
		for (int x = 0; x < gridWidth; x++)
		{
			for (int y = 0; y < gridHeight; y++)
			{
				m_Grid[x, y] = new Grid_Node();

				Vector3 scale;
				scale.x = nodeWidth;
				scale.y = nodeTallness;
				scale.z = nodeHeight;
				m_Grid[x, y].SetScale(scale);

				Vector3 position;
				position.x = (m_Start.x + x - gridWidth / 2) * nodeWidth;
				position.y = floorHeight + (floorHeight / 2.0f);
				position.z = (m_Start.z + y - gridHeight / 2) * nodeHeight;
				position = position + m_Start;
				m_Grid[x, y].SetPosition(position);
			}
		}
	}

	private Grid_Node GetNeighbour(Grid_Node node, int neighbourIndex)
	{
		int x = (int)node.GetPosition().x; 
		int y = (int)node.GetPosition().z;

		switch (neighbourIndex)
		{
			case 0:		 ++y;	break;
			case 1: --x;		break;
			case 2: ++x;		break;
			case 3:		 --y;	break;
			
		}

		float minX, maxX;
		minX = 0;
		maxX = gridWidth;

		float minY, maxY;
		minY = 0;
		maxY = gridHeight;



		if (x < minX || x > maxX)
			return null;

		if (y < minY || y > maxY)
			return null;

		return m_Grid[x, y];
	}

	private void Generate()
	{
		m_Path.Push(m_Grid[0,0]);
		m_Grid[0, 0].SetVisited();
		cellsVisited++;
		Debug.Log("Cells visited: " + cellsVisited);
		CreatePath(m_Grid[0,0]);
	}

	private void CreatePath(Grid_Node node)
	{
		if (cellsVisited != gridWidth * gridHeight)
		{
			Grid_Node nextNode;
			Grid_Node[] neighbours = new Grid_Node[4];
			for (int i = 0; i < neighbours.Length; i ++)
			{
				neighbours[i] = GetNeighbour(node, i);
			}

			while (neighbours[0] != null &&  neighbours[0].IsVisited() == false || neighbours[1] != null && neighbours[1].IsVisited() == false || neighbours[2] != null && neighbours[2].IsVisited() == false || neighbours[3] != null && neighbours[3].IsVisited() == false)
			{ 
				Debug.Log("Cells visited: " + cellsVisited);
				int rand = Random.Range(0, 3);
				if (GetNeighbour(node, rand).IsVisited() == false)
				{
					m_Path.Push(node);
					node.ConnectedToNeighbour(rand);
					nextNode = GetNeighbour(node, rand);
					nextNode.IsVisited();
					cellsVisited++;
					CreatePath(nextNode);
				}

				else
				{
					m_Path.Pop();
					CreatePath(m_Path.Peek());
				}
			}
		}
	}

	private void InstantiateMaze()
	{
		for (int x = 0; x < gridWidth; x++)
		{
			for (int y = 0; y < gridHeight; y++)
			{
					wall.transform.position = m_Grid[x,y].GetPosition();
					GameObject a = Instantiate(wall);
					a.transform.parent = m_Board.transform;
					a.transform.localScale = m_Grid[x,y].GetScale();
				if (m_Grid[x,y].IsVisited())
				{
					Destroy(a);
				}
			}
		}
	}
}
