using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* Author: Cameron, Declan
 * 
 * GameController is a Singleton used to manage everything within the 'Game' state, meaning references to
 * it do not need to be stored, and its public properties can be accessed via GameController.Instance.
 * 
 * This class manages everything relating to managing the game state.
 */

/// <summary>
/// Enum for the basic states in the game for what the player is currently doing.
/// </summary>
public enum GameState
{
	/// <summary> An active game is currently paused. </summary>
	PAUSED,
	/// <summary> The game is currently in progress. </summary>
	GAME,
	/// <summary> Game is over (won/lost). </summary>
	FINISH
}

/// <summary>
/// Manages everything relating to the active game.
/// (Properties accessed with <c>GameController.Instance</c>.)
/// </summary>
public class GameController : MonoBehaviour
{
	#region Variables/Properties
	// -- Public --
	public GameObject m_DanceGuyPrefab;                 // Prefab for dancing man objects
	public GameObject m_DanceGirlPrefab;
	public TextMeshPro m_LCDText;                       // TMP that displays info to player
	public static int m_GemCount = 2;					// ???
	public Material[] m_Skyboxes = null;          // Skyboxe materials used for theskybox

	// -- Private --
	private GameState m_State;                          // Current GameController state
	private GameObject m_DanceGirl = null;
	private GameObject m_DanceGuy = null;
	private SoundManager m_Sound;                       // Reference to SoundManager class
	private float m_TimeCounter = 0.0f;                 // Time taken during gameplay
	private int m_GemsCollected = 0;                    // ???
	private bool InAnimation = false;
	private Skybox m_Skybox = null;

	// -- Singleton --
	public static GameController Instance { get; private set; }
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

	/// <summary>
	/// Called on Start.
	/// Caches components and initializes the game.
	/// </summary>
	void Start()
	{
		m_Skybox = Camera.main.GetComponent<Skybox>();


		// Cache components
		m_Sound = GetComponent<SoundManager>();
	//_Animator = ;

		// Initialize variables
		m_TimeCounter = 0.0f;
		SetState(GameState.GAME);

		// Initialize game
		StartGame();
	}

	/// <summary>
	/// Called on Update.
	/// Processes game loop.
	/// </summary>
	void Update()
	{
		// Update time counter
		if (m_State == GameState.GAME)
		{
			m_TimeCounter += Time.deltaTime;
			float minutes = Mathf.FloorToInt(m_TimeCounter / 60.0f);
			float seconds = Mathf.FloorToInt(m_TimeCounter % 60.0f);
			m_LCDText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
			Time.timeScale = 1;

		}
		if (m_State == GameState.PAUSED)
		{
		}

		if (Input.GetKeyDown(KeyCode.Escape) && InAnimation)
		{
			CameraController.Instance.TransitionToMenu();
			MenuController.Instance.ToggleMenu();
			Destroy(m_DanceGirl);
			Destroy(m_DanceGuy);
			SetState(GameState.PAUSED);
			// Open menu

		}

		else if (Input.GetKeyDown(KeyCode.Escape) && m_State == GameState.PAUSED)
		{
			CameraController.Instance.TransitionToGame();
			MenuController.Instance.ToggleMenu();
			SetState(GameState.GAME);
			
		}

		else if (Input.GetKeyDown(KeyCode.Escape))
		{
			CameraController.Instance.TransitionToMenu();
			SetState(GameState.PAUSED);
			MenuController.Instance.ToggleMenu();
			//Open menu
		}
	}
	#endregion

	#region Functions
	// -- Public --
	/// <summary>
	/// Sets the active GameController state, and the timer/LCD text accordingly.
	/// </summary>
	/// <param name="state">The GameState to set the game to.</param>
	public void SetState(GameState state)
	{
		// Set state as specified
		m_State = state;

		// Update LCD text accordingly
		switch (state)
		{
			case GameState.PAUSED:
				m_LCDText.text = "PAUSED";
				break;
			case GameState.GAME:
				m_LCDText.text = "00:00";
				break;
			case GameState.FINISH:
				m_LCDText.text = "YOU WIN";
				m_TimeCounter = 0.0f;
				break;
		}
	}

	/// <summary>
	/// Increments the Gem counter and plays a sound
	/// </summary>
	public void AddGem()
	{
		m_GemsCollected++;
		m_Sound.PlayGemCollected();

		if (m_GemsCollected < m_GemCount)
		{
			MazeGeneration.Instance.SetGemActive(m_GemsCollected);
		}
		else if(m_GemsCollected == m_GemCount)
		{
			MazeGeneration.Instance.SetFlagActive();
		}
	}

	/// <summary>
	/// Initialize the total amount of Gems as specified
	/// </summary>
	/// <param name="count">The count of gems to initialize with.</param>
	public void SetGemCount(int count)
	{
		m_GemCount = count;
	}

	/// <summary>
	/// Called when all Gems are collected.
	/// Regenerates the maze.
	/// </summary>
	public void OnAllGemsCollected()
	{
		MazeGeneration.Instance.NewMaze();
	}

	/// <summary>
	/// Called when Maze is completed.
	/// Does absolutely nothing.
	/// </summary>
	public void OnComplete()
	{
		// do stuff
	}

	/// <summary>
	/// Appears to do exactly the same thing as RestartMaze - Declan what are you doing bro  - This is called when the ball interacts with the end flag
	/// // Spawns animations for celebration
	/// </summary>
	public void OnFinish()
	{
		// Reset variables
		m_GemsCollected = 0;
		m_TimeCounter = 0.0f;
		int rand = Random.Range(0, 8);
		// Pans camera to dancing guy
		if(m_DanceGuy == null)
		{
			GameObject guy = Instantiate(m_DanceGuyPrefab);
			Vector3 position = new Vector3(10.0f, -1.5f, -50.0f);
			guy.transform.position = position;
			m_DanceGuy = guy;
			guy.GetComponent<DanceAnimation>().PlayAnimation(rand);
		}

		if(m_DanceGirl == null)
		{
			GameObject girl = Instantiate(m_DanceGirlPrefab);
			Vector3 position = new Vector3(-10.0f, -1.5f, -50.0f);
			girl.transform.position = position;
			girl.GetComponent<DanceAnimation>().PlayAnimation(rand);
			m_DanceGirl = girl;
		}
		InAnimation = true;
		CameraController.Instance.TransitionToAnimation();
	}

	// -- Private --
	/// <summary>
	/// Generates Maze and initializes dance guy and music.
	/// </summary>
	public void StartGame()
	{
		// Initialize  music
		m_Sound.SetMusicVolume(0.3f);
		m_Sound.SetGemVolume(1.0f);
		m_Sound.PlayBackroundMusic();

		// Generate maze
		MazeGeneration maze = MazeGeneration.Instance;
		maze.GridSizeIndex = (maze.GridSizesCount - 1) / 2;
		maze.GemSpawnCount = maze.MinGemSpawnCount + ((maze.MaxGemSpawnCount - maze.MinGemSpawnCount) / 2);
		maze.NewMaze();
		maze.SetGemActive(m_GemsCollected);

		// Set camera to transition to game (if it isn't already)
		CameraController.Instance.TransitionToGame();
	}

	/// <summary>
	/// Restarts the Maze by resetting Gem Counter, Timer and Player Position.
	/// </summary>
	public void RestartMaze()
	{
		// Reset variables
		m_GemsCollected = 0;
		m_TimeCounter = 0.0f;

		// Restart maze
		MazeGeneration.Instance.ResetMaze();
		MazeGeneration.Instance.SetGemActive(m_GemsCollected);
	}

	/// <summary>
	/// Deletes existing maze and spawns new maze
	/// </summary>
	public void GenerateNewMaze()
	{
		// Reset variables
		m_GemsCollected = 0;
		m_TimeCounter = 0.0f;

		MazeGeneration.Instance.NewMaze();
		MazeGeneration.Instance.SetGemActive(m_GemsCollected);
	}

	public void SetMenu()
	{

		SetState(GameState.PAUSED);
		MenuController.Instance.ToggleMenu();
		
		//Open menu
	}
	
	// Destroys the dancers
	public void DestroyDancers()
	{
		Destroy(m_DanceGirl);
		Destroy(m_DanceGuy);
	}

	public void ChangeSkybox()
	{
		int r = Random.Range(0, 5);
		LightPosManager.Instance.SetLight(r);
		RenderSettings.skybox = m_Skyboxes[r];
		Debug.Log("Set Skybox: " + r);
	}
	#endregion
}
