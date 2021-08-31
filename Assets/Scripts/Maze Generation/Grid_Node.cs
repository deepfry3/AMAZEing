using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Worked on by:
//	Declan Doller
//
public class Grid_Node
{
	// Its position, scale and grid position
	Vector3 m_Position = Vector3.zero;
	Vector3 m_Scale = Vector3.zero;
	Vector2 m_GridPos;

	// If the node is visited and what connections are there
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
