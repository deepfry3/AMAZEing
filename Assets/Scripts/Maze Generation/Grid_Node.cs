using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Grid_Node 
{
	Vector3 m_Position = Vector3.zero;
	Vector3 m_Scale = Vector3.zero;
	bool m_Visited = false;
	bool[] m_IsConnectedNeighbour = new bool[4] {false, false, false, false};
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
	private void ConnectToNeighbour(int index)
	{
		m_IsConnectedNeighbour[index] = true;
	}

	// Checks if the node is connected to a neighbour
	public bool ConnectedToNeighbour(int index)
	{
		return m_IsConnectedNeighbour[index];
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
