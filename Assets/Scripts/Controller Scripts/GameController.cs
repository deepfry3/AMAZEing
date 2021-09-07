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
	public GameObject m_DanceGuyPrefab;                 // Prefab for dancing man object
	public TextMeshPro m_LCDText;                       // TMP that displays info to player
	public static int m_GemCount = 0;					// ???

	// -- Private --
	private GameState m_State;							// Current GameController state
	private SoundManager m_Sound;                       // Reference to SoundManager class
	private MazeGeneration m_MazeGen;                   // Reference to MazeGeneration class
	private float m_TimeCounter = 0.0f;                 // Time taken during gameplay
	private int m_GemsCollected = 0;					// ???

	// -- Properties --
	// (currently blank)

	#region Singleton
	private static GameController m_Instance;
	public static GameController Instance
	{ 
		get { return m_Instance; }
	}
	#endregion
	#endregion

	#region Unity Functions
	/// <summary>
	/// Called on Awake.
	/// Initializes Singleton.
	/// </summary>
	void Awake()
	{
		// Initialize Singleton
		if (m_Instance != null && m_Instance != this)
			Destroy(this.gameObject);
		else
			m_Instance = this;
	}

	/// <summary>
	/// Called on Start.
	/// Caches components and initializes the game.
	/// </summary>
	void Start()
	{
		// Cache components
		m_Sound = GetComponent<SoundManager>();
		m_MazeGen = GetComponent<MazeGeneration>();

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
		}

		// If all gems are collected, regenerate maze
		if (m_GemsCollected == m_GemCount)
		{
			m_GemsCollected = 0;
			OnAllGemsCollected();
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
		m_MazeGen.GenerateNewMaze();
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
	/// Appears to do exactly the same thing as RestartMaze - Declan what are you doing bro
	/// </summary>
	public void OnFinish()
	{
		// Reset variables
		m_GemsCollected = 0;
		m_TimeCounter = 0.0f;

		// Restart maze
		m_MazeGen.RestartMaze();
	}

	// -- Private --
	/// <summary>
	/// Generates Maze and initializes dance guy and music.
	/// </summary>
	public void StartGame()
	{
		// Initialize dance guy and music
		Instantiate(m_DanceGuyPrefab);
		m_Sound.SetMusicVolume(0.3f);
		m_Sound.SetGemVolume(1.0f);
		m_Sound.PlayBackroundMusic();

		// Generate maze
		m_MazeGen.GenerateNewMaze();
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
		m_MazeGen.RestartMaze();
	}

	/// <summary>
	/// Deletes existing maze and spawns new maze
	/// </summary>
	public void GenerateNewMaze()
	{
		// Reset variables
		m_GemsCollected = 0;
		m_TimeCounter = 0.0f;

		m_MazeGen.GenerateNewMaze();
	}
	#endregion
}
