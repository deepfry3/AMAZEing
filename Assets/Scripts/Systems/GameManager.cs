using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* Author: Cameron, Declan
 * 
 * GameManager is a Singleton used to manage everything within the 'Game' state, including
 * counting timer, keeping track of game states, etc.
 * 
 *  It can be accessed with GameManager.Instance.
 */

/// <summary>
/// Enum for the basic states in the game for what the player is currently doing.
/// </summary>
public enum GameState
{
	/// <summary> A game has not yet been started </summary>
	START,
	/// <summary> An active game is currently paused </summary>
	PAUSED,
	/// <summary> The game is currently in progress </summary>
	GAME,
	/// <summary> Game is over (won/lost) </summary>
	FINISH
}

/// <summary>
/// Manages everything related to the Game state.
/// (Singleton, accessed with <c>GameManager.Instance</c>.)
/// </summary>
[System.Serializable]
public class GameManager : MonoBehaviour
{
	#region Variables/Properties
	// -- Serialized --
	[Header("Prefabs/GameObjects")]
	[SerializeField] GameObject m_DancerMalePrefab = null;      // Prefab for Dancer Male
	[SerializeField] GameObject m_DancerFemalePrefab = null;    // Prefab for Dancer Female
	[SerializeField] TextMeshPro m_LCDText = null;              // Text that displays info to player
	[SerializeField] Material[] m_Skyboxes = null;              // Materials used to select and display Skybox

	// -- Private --
	private GameState m_State;                                  // Active Game State
	private GameState m_PreviousState;                          // Previous Game State
	private GameObject m_DancerMale = null;                     // Instantiated Dancer Male
	private GameObject m_DancerFemale = null;                   // Instantiated Dancer Female
	private float m_TimeCounter = 0.0f;                         // Counter for time taken during gameplay

	// -- Properties --
	/// <summary>
	/// Gets or Sets the active Game State and updates the text display accordingly.
	/// </summary>
	public GameState State
	{
		get
		{
			return m_State;
		}
		set
		{
			try
			{
				m_State = value;
				switch (value)
				{
					case GameState.START:
						m_LCDText.text = "WELCOME";
						break;
					case GameState.PAUSED:
						m_LCDText.text = "PAUSED";
						BallController.Instance.IsPaused = true;
						DestroyDancers();
						break;
					case GameState.GAME:
						m_LCDText.text = "00:00";
						BallController.Instance.IsPaused = false;
						break;
					case GameState.FINISH:
						m_LCDText.text = m_LCDText.text + " - FIN";
						ResetTimer();
						break;
				}
			}
			catch
			{
				Debug.Log("Attempted to set GameManager State to an invalid state.");
			}
		}
	}

	// -- Singleton --
	public static GameManager Instance { get; private set; }
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
	/// Initializes game.
	/// </summary>
	void Start()
	{
		// Initialize variables
		ResetTimer();
		State = GameState.START;
		m_PreviousState = GameState.START;

		// Initialize game
		SoundManager.Instance.PlayBackgroundMusic();

		// Generate maze
		MazeGeneration maze = MazeGeneration.Instance;
		maze.GridSizeIndex = (maze.GridSizesCount - 1) / 2;
		maze.GemSpawnCount = maze.MinGemSpawnCount + ((maze.MaxGemSpawnCount - maze.MinGemSpawnCount) / 2);

		// Set camera to transition to game (if it isn't already)
		CameraController.Instance.TransitionToStart();
	}

	/// <summary>
	/// Called on Update.
	/// Processes game loop.
	/// </summary>
	void Update()
	{
		// Update time counter
		if (State == GameState.GAME)
		{
			m_TimeCounter += Time.deltaTime;
			float minutes = Mathf.FloorToInt(m_TimeCounter / 60.0f);
			float seconds = Mathf.FloorToInt(m_TimeCounter % 60.0f);
			m_LCDText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
			Time.timeScale = 1;
		}
	}
	#endregion

	#region Public Functions
	/// <summary>
	/// Performs fanfare celebration and sets state to GameState.FINISH.
	/// </summary>
	public void OnFinish()
	{
		// Set state accordingly
		State = GameState.FINISH;

		// Initiate animation
		DestroyDancers();
		int index = Random.Range(0, 8);
		m_DancerMale = Instantiate(m_DancerMalePrefab);
		m_DancerFemale = Instantiate(m_DancerFemalePrefab);
		m_DancerMale.transform.position = new Vector3(10.0f, -1.5f, -50.0f);
		m_DancerFemale.transform.position = new Vector3(-10.0f, -1.5f, -50.0f);
		m_DancerMale.GetComponent<DanceAnimation>().PlayAnimation(index);
		m_DancerFemale.GetComponent<DanceAnimation>().PlayAnimation(index);

		CameraController.Instance.TransitionToWin();
	}

	/// <summary>
	/// Resets the timer to 0.
	/// </summary>
	public void ResetTimer()
	{
		m_TimeCounter = 0.0f;
	}

	/// <summary>
	/// Destroys the Dancer GameObjects, if they exist.
	/// </summary>
	public void DestroyDancers()
	{
		if (m_DancerMale != null) Destroy(m_DancerMale);
		if (m_DancerFemale != null) Destroy(m_DancerFemale);
	}

	/// <summary>
	/// Sets the active skybox randomly.
	/// </summary>
	public void ChangeSkybox()
	{
		// Change skybox
		int index = Random.Range(0, m_Skyboxes.Length);
		RenderSettings.skybox = m_Skyboxes[index];
		LightPosManager.Instance.SetLight(index);

		// Log new skybox to console
		Debug.Log("Skybox set to index: " + index);
	}

	/// <summary>
	/// Does absolutely nothing.
	/// </summary>
	public void OnComplete()
	{
		// do stuff
	}
	#endregion
}