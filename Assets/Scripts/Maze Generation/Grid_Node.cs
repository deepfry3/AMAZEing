using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Grid_Node
{
	Vector3 m_Position = Vector3.zero;
	Vector3 m_Scale = Vector3.zero;
	Vector2 m_GridPos;
	bool m_Visited = false;
	public bool[] m_IsConnectedNeighbour = new bool[4] { false, false, false, false };

	// # | 0 | #
	// 1 | N | 2
	// # | 3 | #

	// Sets the node's position
	public void SetPosition(Vector3 pos)
	{
		m_Position = pos;
	}

	// Returns the node's position
	public Vector3 GetPosition()
	{
		return m_Position;
	}

	// Sets the node's scale
	public void SetScale(Vector3 scale)
	{
		m_Scale = scale;
	}
	
	// Returns the node's scale
	public Vector3 GetScale()
	{
		return m_Scale;
	}

	// Connects the node to a neighbouring node
	public void ConnectToNeighbour(int index)
	{
		m_IsConnectedNeighbour[index] = true;
	}

	public void UnconnectSelfFromNeighbour(Grid_Node neighbour, int neighbourIndex)
	{
		int outIndex;
		switch(neighbourIndex)
		{
			case 0:
				outIndex = 3;
				break;

			case 1:
				outIndex = 2;
				break;

			case 2:
				outIndex = 1;
				break;

			case 3:
				outIndex = 0;
				break;

			default:
				outIndex = 10;
				break;
		}

		neighbour.UnconnectNeighbour(outIndex);
	}

	public void unvisit()
	{
		m_Visited = false;
	}
	public void UnconnectNeighbour(int index)
	{
		if (index == 10)
		{

		}

		else
		{
			m_IsConnectedNeighbour[index] = false;
		}
	}

	public void ConnectNeighbourToSelf(Grid_Node neighbour, int neighbourIndex)
	{
		switch (neighbourIndex)
		{
			case 0:
				neighbourIndex = 3;
				break;

			case 1:
				neighbourIndex = 2;
				break;

			case 2:
				neighbourIndex = 1;
				break;

			case 3:
				neighbourIndex = 0;
				break;
		}

		neighbour.ConnectedToNeighbour(neighbourIndex);
	}

	// Checks if the node is connected to a neighbour
	public bool ConnectedToNeighbour(int index)
	{
		return m_IsConnectedNeighbour[index];
	}

	// Sets the nodes grid position
	public void SetGridPos(Vector2 gridpos)
	{
		m_GridPos = gridpos;
	}

	// Returns the grid position
	public Vector2 GetGridPos()
	{
		return m_GridPos;
	}
	// Sets the node to be visited
	public void SetVisited()
	{
		m_Visited = true;
	}

	// Returns true if the node has been visited
	public bool IsVisited()
	{
		return m_Visited;
	}
}
