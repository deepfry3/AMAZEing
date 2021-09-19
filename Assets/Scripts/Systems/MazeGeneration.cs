using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Cameron, Declan
 * 
 * MazeGeneration is a Singleton used to manage everything related to the generation of
 * the maze, including instantiating prefabs on the board.
 * 
 * It can be accessed with MazeGeneration.Instance.
 */

/// <summary>
/// Used to store information about each position ("node") on a generated maze, including
/// its size, position in the array and in world space, etc.
/// </summary>
public class GridNode
{
	#region Variables/Properties
	// -- Properties --
	public Vector3 Position { get; set; } = Vector3.zero;       // Local position on the inner maze box
	public Vector3 Scale { get; set; } = Vector3.zero;          // Local scale on the inner maze box
	public Vector2Int GridPosition { get; set; }                // Position in the maze array
	public bool IsVisited { get; set; } = false;                // Whether node has been visited during maze generation
	public bool[] IsConnectedNeighbour = new bool[]				// Whether each neighbour can be traversed to (is connected)
	{
		false, false, false, false
	};
	#endregion
}

/// <summary>
/// Manages everything relating to maze generation.
/// (Singleton, accessed with <c>MazeGeneration.Instance</c>.)
/// </summary>
[System.Serializable]
public class MazeGeneration : MonoBehaviour
{
	#region Variables/Properties
	// -- Serialized --
	[Header("Parent Objects")]
	[SerializeField] GameObject m_Board = null;                 // Object to parent all walls to (Inner-most box)
	[SerializeField] GameObject m_Floor = null;                 // Floor object, solely for width/height dimensions
	[Header("Prefabs")]
	[SerializeField] GameObject m_WallPrefab = null;            // Prefab used to instantiate walls
	[SerializeField] GameObject m_BallPrefab = null;            // Prefab used to instantiate ball
	[SerializeField] GameObject m_FlagPrefab = null;            // Prefab used to instantiate flag
	[SerializeField] GameObject[] m_GemPrefabs = null;          // Prefabs used to instantiate gems (randomly selected from array)
	[SerializeField] Material[] m_GemMaterials = null;          // Materials used to instantiate gems (randomly selected from array)
	[Header("Maze Settings")]
	[SerializeField] Vector2Int[] m_GridSizes = null;           // Possible maze sizes (cells in maze horizontally/vertically)
	[SerializeField] float m_WallThickness = 0.5f;              // Thickness of walls instantiated in maze
	[SerializeField] float m_WallHeight = 1.5f;                 // Height of walls instantiated in maze
	[SerializeField] int m_MinGemCount = 1;						// Minimum GemCount allowed in Options menu
	[SerializeField] int m_MaxGemCount = 6;                     // Maximum Gem Count allowed in Options menu

	// -- Private --
	private Stack<GridNode> m_Path = null;                      // Path used for maze generation
	private GridNode m_StartNode = null;                        // Start node to spawn ball at
	private GridNode m_EndNode = null;                          // End node to spawn flag at
	private GridNode[,] m_MazeGrid = null;                      // The final stored maze (grid of nodes)
	private Stack<GameObject> m_Walls = null;                   // Instantiated walls after generation
	private List<GameObject> m_Gems = null;                     // Instantiated gems after generation
	private GameObject m_Ball = null;                           // Instantiated ball after generation
	private GameObject m_Flag = null;                           // Instantiated flag after generation
	private int m_GridSizeIndex;                                // Current grid size (index of Grid Sizes array)
	private Vector2Int m_GridSize;                              // Current grid size (actual size)
	private int m_GemSpawnCount;								// Current gem count (to use upon regeneration)

	// -- Properties --
	/// <summary>
	/// Gets or sets the count of gems to spawn in the maze upon regeneration.
	/// </summary>
	public int GemSpawnCount
	{
		get
		{
			return m_GemSpawnCount;
		}
		set
		{
			if (value < m_MinGemCount || value > m_MaxGemCount)
			{
				Debug.Log("Unable to set Gem Spawn Count: Value " + value + " was invalid.");
			}
			else
			{
				m_GemSpawnCount = value;
				MenuController.Instance.SetGemCountText(value);
			}
		}
	}

	/// <summary>
	/// Gets the minimum count of gems to spawn in the maze upon regeneration that can be set.
	/// </summary>
	public int MinGemSpawnCount
	{
		get	{ return m_MinGemCount;	}
	}

	/// <summary>
	/// Gets the maximum count of gems to spawn in the maze upon regeneration that can be set.
	/// </summary>
	public int MaxGemSpawnCount
	{
		get { return m_MaxGemCount; }
	}

	/// <summary>
	/// Gets or sets the index of the Grid Sizes array to use in the maze upon regeneration.
	/// Error logged to Debug console upon invalid value.
	/// </summary>
	public int GridSizeIndex
	{
		get
		{
			return m_GridSizeIndex;
		}
		set
		{
			try
			{
				m_GridSize = m_GridSizes[value];
				m_GridSizeIndex = value;
				MenuController.Instance.SetMazeSizeText(m_GridSize);
			}
			catch { Debug.Log("Unable to set Grid Size: Index " + value + " was invalid."); }
		}
	}

	/// <summary>
	/// Gets the count of the Grid Sizes array to use in the maze upon regeneration.
	/// </summary>
	public int GridSizesCount
	{
		get { return m_GridSizes.Length; }
	}

	/// <summary>
	/// Gets the current grid size to use in the maze upon regeneration.
	/// </summary>
	public Vector2Int GridSize
	{
		get { return m_GridSize; }
	}

	// -- Singleton --
	public static MazeGeneration Instance { get; private set; }
	#endregion

	#region Unity Functions
	/// <summary>
	/// Called on Awake.
	/// Initializes Singleton.
	/// </summary>
	void Awake()
	{
		Instance = this;
	}
	#endregion

	#region Public Functions
	/// <summary>
	/// Destroys any current maze, then regenerates and instantiates a new maze
	/// </summary>
	public void NewMaze()
	{
		// Destroy existing maze
		DestroyMaze();

		// Initialize variables
		m_Walls = new Stack<GameObject>();
		m_Gems = new List<GameObject>();
		m_Path = new Stack<GridNode>();
		m_MazeGrid = new GridNode[m_GridSize.x, m_GridSize.y];

		// Calculate floor and node size using m_Floor transforms (-1 removed from scale due to walls lining the edge)
		float floorW = m_Floor.transform.localScale.x - 1;
		float floorH = m_Floor.transform.localScale.z - 1;
		float nodeW = floorW / m_GridSize.x;
		float nodeH = floorH / m_GridSize.y;

		// Create nodes at each element in the grid array to populate grid
		for (int x = 0; x < m_GridSize.x; x++)
		{
			for (int y = 0; y < m_GridSize.y; y++)
			{
				// Create GridNode
				m_MazeGrid[x, y] = new GridNode();

				// Calculate and set position
				m_MazeGrid[x, y].Position = new Vector3(
					-(floorW * 0.5f) + (nodeW * 0.5f) + (x * nodeW),
					(m_WallHeight * 0.5f) - 0.25f,
					-(floorH * 0.5f) + (nodeH * 0.5f) + (y * nodeH));

				// Calculate and set scale
				m_MazeGrid[x, y].Scale = new Vector3(nodeW, m_WallHeight, nodeH);

				// Parse grid position
				m_MazeGrid[x, y].GridPosition = new Vector2Int(x, y);
			}
		}

		// Generate and instantiate maze
		GenerateMaze();
		InstantiateMaze();
		GameManager.Instance.ResetTimer();
	}

	/// <summary>
	/// Resets the maze by returning all prefabs to their default positions.
	/// </summary>
	public void ResetMaze()
	{
		// Reset inner boxes to default rotation
		BoxController.Instance.ResetRotation();

		// Reset collectibles' active state
		m_Flag.SetActive(false);
		for (int i = 0; i < m_Gems.Count; i++)
			m_Gems[i].SetActive(false);
		m_Gems[0].SetActive(true);

		// Return ball to default position
		if (m_Ball != null)
		{
			Vector3 spawnOffset = new Vector3(0.0f, 4.0f, 0.0f);
			m_Ball.transform.parent = m_Board.transform;
			m_Ball.transform.localPosition = m_StartNode.Position + spawnOffset;
		}

		GameManager.Instance.ResetTimer();
	}

	/// <summary>
	/// Destroys any current maze by destroying all generated prefabs and restoring all defaults
	/// </summary>
	public void DestroyMaze()
	{
		// Destroy walls
		if (m_Walls != null)
		{
			while (m_Walls.Count > 0)
			{
				GameObject wall = m_Walls.Pop();
				if (wall != null) Destroy(wall);
			}
		}
		
		// Destroy ball and collectibles
		if (m_Ball != null) Destroy(m_Ball);
		if (m_Flag != null) Destroy(m_Flag);
		if (m_Gems != null)
		{
			for (int i = m_Gems.Count - 1; i >= 0; i--)
			{
				if (m_Gems[i] != null) Destroy(m_Gems[i]);
				m_Gems.RemoveAt(i);
			}
		}

		// Reset box rotation and set generated state to false
		BoxController.Instance.ResetRotation();
	}

	/// <summary>
	/// Activates/Deactivates the specified Gem, based on the given true or false value.
	/// Error logged to Debug console upon invalid value.
	/// </summary>
	/// <param name="index">The Gem to set (index of Gems array)</param>
	/// <param name="value">The active state to set</param>
	public void SetGemActive(int index, bool value = true)
	{
		try { m_Gems[index].SetActive(value); }
		catch { Debug.Log("Unable to set Activate/Deactivate Gem: Index " + index + " was invalid."); }
	}

	/// <summary>
	/// Activates/Deactivates the Flag, based on the given true or false value.
	/// </summary>
	/// <param name="value">The active state to set</param>
	public void SetFlagActive(bool value = true)
	{
		m_Flag.SetActive(value);
	}
	#endregion

	#region Private Functions
	/// <summary>
	/// Links all GridNodes in the Maze Grid array together to generate a new maze.
	/// </summary>
	private void GenerateMaze()
	{
		#region Generate start and end positions
		// Generate random start and end positions (not at an edge), ensuring they are not identical
		int xStart, yStart, xEnd, yEnd;
		do
		{
			xStart = Random.Range(0, m_GridSize.x);
			yStart = Random.Range(0, m_GridSize.y);
			xEnd = Random.Range(0, m_GridSize.x);
			yEnd = Random.Range(0, m_GridSize.y);
		} while (xStart == xEnd && yStart == yEnd);

		// Store results
		m_StartNode = m_MazeGrid[xStart, yStart];
		m_EndNode = m_MazeGrid[xEnd, yEnd];
		#endregion

		#region Generate maze by connecting nodes to random neighbours
		// Push start node onto path
		m_Path.Push(m_StartNode);
		GridNode node = m_StartNode;

		while (m_Path.Count > 0)
		{
			// Initialize temp variables, set current node as visited, and read node from top of path stack
			bool[] neighbourInvalid = new bool[] { false, false, false, false };
			bool connectionCreated = false;
			node.IsVisited = true;
			node = m_Path.Peek();

			// Flag all null or visited neighbours as invalid
			for (int i = 0; i < 4; i++)
			{
				if (GetNeighbour(node, i) == null || GetNeighbour(node, i).IsVisited)
					neighbourInvalid[i] = true;
			}

			while (true)
			{
				// If all neighbours are invalid, break the loop
				if (neighbourInvalid[0] && neighbourInvalid[1] && neighbourInvalid[2] && neighbourInvalid[3])
					break;

				// Otherwise, choose a random valid neighbour and connect this node to is
				int n;
				do { n = Random.Range(0, neighbourInvalid.Length); } while (neighbourInvalid[n]);
				node.IsConnectedNeighbour[n] = true;

				// Connect the neighbour to this node as well, then push neighbour onto path
				node = GetNeighbour(node, n);
				node.IsConnectedNeighbour[(n + 2) % 4] = true;
				m_Path.Push(node);
				connectionCreated = true;
				break;
			}

			// If no connection to neighbour occurred, go back one step
			if (!connectionCreated)
				m_Path.Pop();
		}
		#endregion
	}

	/// <summary>
	/// Instantiates all prefabs in the world according to the generated maze.
	/// </summary>
	private void InstantiateMaze()
	{
		#region Instantiate walls/maze
		for (int x = 0; x < m_GridSize.x; x++)
		{
			for (int y = 0; y < m_GridSize.y; y++)
			{
				GridNode node = m_MazeGrid[x, y];

				// Loop through each neighbouring node and create a wall if there is no connection
				for (int n = 0; n < 4; n++)
				{
					GridNode neighbour = GetNeighbour(node, n);
					if (!node.IsConnectedNeighbour[n] && neighbour != null)
					{
						// Set neighbour to be connected so another wall is not created
						neighbour.IsConnectedNeighbour[(n + 2) % 4] = true;

						// Instantiate wall and link transform parent to board
						GameObject wall = Instantiate(m_WallPrefab);
						wall.transform.parent = m_Board.transform;

						// Set position and scale according to if wall is running horizontally or vertically
						Vector3 position = node.Position;
						Vector3 scale = node.Scale;
						if (n == 0 || n == 2)       // Vertical path blocked - create horizontal wall
						{
							position.z = (node.Position.z + neighbour.Position.z) * 0.5f;
							scale.z = m_WallThickness;
							scale.x += 0.5f;
						}
						else if (n == 1 || n == 3)  // Horizontal path blocked - create vertical wall
						{
							position.x = (node.Position.x + neighbour.Position.x) * 0.5f;
							scale.x = m_WallThickness;
							scale.z += 0.5f;
						}
						wall.transform.localPosition = position;
						wall.transform.localScale = scale;
						m_Walls.Push(wall);
					}
				}
			}
		}
		#endregion

		#region Instantiate Ball and Collectibles
		// -- Ball --
		m_Ball = Instantiate(m_BallPrefab);

		// -- Gems --
		// Add start and end nodes as invalid spawn locations for gem
		List<Vector2Int> invalidGemSpawns = new List<Vector2Int>();
		invalidGemSpawns.Add(m_StartNode.GridPosition);
		invalidGemSpawns.Add(m_EndNode.GridPosition);
		// Instantiate Gems
		for (int i = 0; i < GemSpawnCount; i++)
		{
			// Randomly generate gem position on the maze grid until valid position is found
			Vector2Int spawn = new Vector2Int();
			do
			{
				spawn.x = Random.Range(0, m_GridSize.x);
				spawn.y = Random.Range(0, m_GridSize.y);
			} while (invalidGemSpawns.Contains(spawn));

			// Add found position to invalid gem spawns
			invalidGemSpawns.Add(spawn);

			// Instantiate gem with random material and color
			GameObject gem = Instantiate(m_GemPrefabs[Random.Range(0, m_GemPrefabs.Length)]);
			Material gemMaterial = m_GemMaterials[Random.Range(0, m_GemMaterials.Length)];
			gem.GetComponent<Renderer>().material = gemMaterial;
			gem.GetComponent<Gem>().SetLightColor(gemMaterial.GetColor("_Color"));

			//Set local position based on position of randomly-selected grid position
			Vector3 gemSpawnOffset = new Vector3(0.0f, 1.0f, 0.0f);
			gem.transform.parent = m_Board.transform;
			gem.transform.localPosition = m_MazeGrid[spawn.x, spawn.y].Position + gemSpawnOffset;
			m_Gems.Add(gem);
		}

		// -- Flag --
		GameObject flag = Instantiate(m_FlagPrefab);
		Vector3 flagSpawnOffset = new Vector3(0.0f, -0.5f, 0.0f);
		flag.transform.parent = m_Board.transform;
		flag.transform.localPosition = m_EndNode.Position + flagSpawnOffset;
		m_Flag = flag;
		#endregion

		// Reset all positions based on their defaults
		ResetMaze();

		GameManager.Instance.ChangeSkybox();
	}

	/// <summary>
	/// Calculates and returns the resulting GridNode from the specified neighbour index of the specified GridNode.
	/// </summary>
	/// <param name="node">The node to get the neighbour of</param>
	/// <param name="neighbourIndex">The index of the neighbour to get</param>
	/// <returns>The specified neighbouring node</returns>
	private GridNode GetNeighbour(GridNode node, int neighbourIndex)
	{
		// Early-exit if provided a null node or invalid neighbour index
		if (node == null || neighbourIndex < 0 || neighbourIndex > 3)
			return null;

		// Get the grid position of the specified neighbour node
		int x = (int)node.GridPosition.x;
		int y = (int)node.GridPosition.y;
		switch (neighbourIndex)
		{
			case 0: y--; break;     // Top
			case 1: x++; break;     // Right
			case 2: y++; break;     // Bottom
			case 3: x--; break;     // Left
		}

		// Return null if neighbour doesn't exist
		if (x < 0 || x >= m_GridSize.x)
			return null;
		if (y < 0 || y >= m_GridSize.y)
			return null;

		// Otherwise, return neighbour node
		return m_MazeGrid[x, y];
	}
	#endregion
}
