using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Declan and Cameron
 * 
 */

public class GridNode
{
	#region Variables
	// Public

	// Private

	// Properties
	public Vector3 Position { get; set; } = Vector3.zero;   // Position on the maze floor
	public Vector3 Scale { get; set; } = Vector3.zero;		// Scale on the maze floor
	public Vector2 GridPosition { get; set; }				// Position in the grid array
	public bool IsVisited { get; set; } = false;            // Whether or not node has been visited by maze generation
	public bool[] IsConnectedNeighbour = new bool[]			// Whether or not each neighbour can be traversed to (is connected)
	{
		false, false, false, false
	};
	#endregion

	#region Functions
	#endregion
}

public class MazeGeneration : MonoBehaviour
{
	#region Variables and Constants
	// Public
	[Header ("Board Stuffs")]
	public GameObject m_MazeUD;
	public GameObject m_MazeLR;
	public GameObject m_Board;                              // Parent object that contains all walls/floor, used for transforms
	public GameObject m_Floor;                              // Floor object, used solely for width and height dimensions

	[Header("Prefab Stuffs")]
	public GameObject m_WallPrefab;                         // Prefab used to instantiate walls
	public GameObject m_BallPrefab;                         // Prefab used to instantiate ball
	public GameObject m_FlagPrefab;                         // Prefab used to instantiate the flag
	public GameObject[] m_GemPrefabs = new GameObject[3];	// Prefab used to instantiate gems
	public Material[] m_Colours = new Material[5];          // Colour materials used for the gems
	public Material[] m_Skyboxs = new Material[5];			// Skyboxe materials used for theskybox

	[Header("Maze Creation Stuffs")]
	public uint m_GridWidth;								// Amount of cells in the maze horizontally
	public uint m_GridHeight;                               // Amount of cells in the maze vertically
	public uint m_MinGemCount;                              // Minimum amount of gems to spawn in the maze
	public uint m_MaxGemCount;                              // Maximum amount of gems to spawn in the maze

	// Private
	private Stack<GameObject> m_Walls;
	private List<GameObject> m_Gems;
	private GameObject m_Ball;
	private GameObject m_Flag;
	private GameObject m_Camera;
	private Skybox m_Skybox;
	private const float m_WallWidth = 0.5f;                 // Width of the walls to be placed in the maze
	private const float m_WallHeight = 1.5f;                // Height of the walls to be placed in the maze
	private const float m_WallYPos = (2.0f * 0.5f) - 0.25f; // Y position the walls will be placed at in the maze
	private Stack<GridNode> m_Path;                         // Path used for maze generation
	private GridNode[,] m_MazeGrid;		                    // Grid of nodes used to store the generated maze
	private GridNode m_StartNode;                           // Node the ball will start at
	private GridNode m_EndNode;                             // Destination node
	private bool m_IsGenerated = false;                     // If the maze has been generated
	private int m_GemAmount;                                // Index for the current gem/checkpoint
	private int m_PreviousSkybox;							// Index for the previous skybox
	#endregion

	#region Functions
	// Called on Start
	private void Start()
	{
		m_Camera = GameObject.FindGameObjectWithTag("MainCamera");
		m_Skybox = m_Camera.GetComponent<Skybox>();
	}

	// Creates a new maze
	public void GenerateNewMaze()
	{

		if(m_IsGenerated)
		{
			DeleteCurrentMaze();
		}

		// Resets both inner boxes to have no rotation
		m_MazeUD.transform.rotation = Quaternion.identity;
		m_MazeLR.transform.rotation = Quaternion.identity;

		m_Walls = new Stack<GameObject>();
		m_Gems = new List<GameObject>();

		// Create path and grid
		m_Path = new Stack<GridNode>();
		m_MazeGrid = new GridNode[m_GridWidth, m_GridHeight];

		// Calculate floor and node size
		float floorW = m_Floor.transform.localScale.x - 1;
		float floorH = m_Floor.transform.localScale.z - 1;
		float nodeW = floorW / m_GridWidth;
		float nodeH = floorH / m_GridHeight;
		
		// Create nodes at each element in the grid array to populate grid
		for (int x = 0; x < m_GridWidth; x++)
		{
			for (int y = 0; y < m_GridHeight; y++)
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
				m_MazeGrid[x, y].GridPosition = new Vector2((float)x, (float)y);
			}
		}

		m_IsGenerated = true;

		// Set random amount of gems to create, then create maze
		GameController.m_GemCount = Random.Range((int)m_MinGemCount, (int)m_MaxGemCount);
		GenerateMaze();
		InstantiateMaze();
	}
	
	// Deletes the current maze
	public void DeleteCurrentMaze()
	{
		while(m_Walls.Count > 0)
		{
			GameObject wall = m_Walls.Pop();
			Destroy(wall);
		}

		int gemcount = m_Gems.Count;
		for (int i = 0; i < gemcount; i ++)
		{
			GameObject gem = m_Gems[i];
			Destroy(gem);
		}
		Destroy(m_Flag);

		Destroy(m_Ball);
	}

	// Links all nodes together to create a unique maze
	private void GenerateMaze()
	{
		#region Generate start and end positions
		// Generate random start and end positions (not at an edge), ensuring they are not identical
		int xStart, yStart, xEnd, yEnd;
		do
		{
			xStart = Random.Range(0, (int)m_GridWidth);
			yStart = Random.Range(0, (int)m_GridHeight);
			xEnd = Random.Range(0, (int)m_GridWidth);
			yEnd = Random.Range(0, (int)m_GridHeight);
		} while (xStart == xEnd && yStart == yEnd);

		// Store results
		m_StartNode = m_MazeGrid[xStart, yStart];
		m_EndNode = m_MazeGrid[xEnd, yEnd];
		#endregion
	
		// Push start node onto path
		m_Path.Push(m_StartNode);
		GridNode node = m_StartNode;

		// Generates maze by connecting nodes to random neighbours
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
				do { n = Random.Range(0, 4); } while (neighbourInvalid[n]);
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

	}

	// Spawns walls, gems and marble in the world to represent the maze
	private void InstantiateMaze()
	{
		// Instantiate Maze
		for (int x = 0; x < m_GridWidth; x++)
		{
			for (int y = 0; y < m_GridHeight; y++)
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
						if (n == 0 || n == 2)		// Vertical path blocked - create horizontal wall
						{
							position.z = (node.Position.z + neighbour.Position.z) * 0.5f;
							scale.z = m_WallWidth;
						}
						else if (n == 1 || n == 3)  // Horizontal path blocked - create vertical wall
						{
							position.x = (node.Position.x + neighbour.Position.x) * 0.5f;
							scale.x = m_WallWidth;
						}
						wall.transform.localPosition = position;
						wall.transform.localScale = scale;
						m_Walls.Push(wall);
					}
				}

				// For debugging purposes - create a block over the node if node was left unvisited
				//if (!node.IsVisited)
				//{
				//	GameObject block = Instantiate(m_WallPrefab);
				//	block.transform.parent = m_Board.transform;
				//	block.transform.position = node.Position;
				//	block.transform.localScale = node.Scale;
				//}
			}
		}

		// Instantiate Ball
		GameObject ball = Instantiate(m_BallPrefab);
		Vector3 spawnOffset = new Vector3(0.0f, 4.0f, 0.0f);
		ball.transform.parent = m_Board.transform;
		ball.transform.localPosition = m_StartNode.Position + spawnOffset;
		m_Ball = ball;

		// Instantiate Gems
		for (int i = 0; i < GameController.m_GemCount; i++)
		{
			// Randomly generate gem position on the maze grid
			int x = Random.Range(0, (int)m_GridWidth);
			int y = Random.Range(0, (int)m_GridHeight);


			int rand = Random.Range(0, 2);
			GameObject gem = Instantiate(m_GemPrefabs[rand]);
			rand = Random.Range(0, 4);
			Renderer mat = gem.GetComponent<Renderer>();
			mat.material = m_Colours[rand];

			// Create gem and set position based on the node's position
			// GameObject gem = Instantiate(m_GemPrefab);
			Vector3 gemSpawnOffset = new Vector3(0.0f, 1.0f, 0.0f);
			gem.transform.parent = m_Board.transform;
			gem.transform.localPosition = m_MazeGrid[x, y].Position + gemSpawnOffset;
			gem.SetActive(false);
			m_Gems.Add(gem);
		}

		// Instantiate flag
		GameObject flag = Instantiate(m_FlagPrefab);
		Vector3 flagSpawnOffset = new Vector3(0.0f, -0.5f, 0.0f);
		flag.transform.parent = m_Board.transform;
		flag.transform.localPosition = m_EndNode.Position + flagSpawnOffset;
		m_Flag = flag;
		m_Flag.SetActive(false);

		int r = Random.Range(0, 4);
		RenderSettings.skybox = m_Skyboxs[r];
		Debug.Log("Set Skybox: " + r);
	}

	// Returns the node at the specified index:
	// [#][0][#]
	// [3][N][1]
	// [#][2][#]
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
			case 0:		y--;	break;		// Top
			case 1:		x++;	break;		// Right
			case 2:		y++;	break;		// Bottom
			case 3:		x--;	break;		// Left
		}

		// Return null if neighbour doesn't exist
		if (x < 0 || x >= m_GridWidth)
			return null;
		if (y < 0 || y >= m_GridHeight)
			return null;

		// Otherwise, return neighbour node
		return m_MazeGrid[x, y];
	}

	// Respawns The items on the maze
	public void RestartMaze()
	{
		// Resets both inner boxes to have 0 rotation
		m_MazeUD.transform.rotation = Quaternion.identity;
		m_MazeLR.transform.rotation = Quaternion.identity;
		m_MazeUD.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		m_MazeLR.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

		for (int i = 0; i < m_Gems.Count; i ++)
		{
			m_Gems[i].SetActive(false);
		}

		if(m_Ball != null)
		{
			Vector3 spawnOffset = new Vector3(0.0f, 10.0f, 0.0f);
			m_Ball.transform.position = m_StartNode.Position + spawnOffset;	
		}
		m_Flag.SetActive(false);
	}

	// Activates Gem
	public void SpawnGem(int index)
	{
		m_Gems[index].SetActive(true);
	}

	// Activates Flag
	public void SpawnFlag()
	{
		m_Flag.SetActive(true);
	} 
	#endregion
}
