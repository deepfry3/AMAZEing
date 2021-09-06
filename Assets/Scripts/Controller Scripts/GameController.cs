using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* Author: Cameron, Declan
 * 
 * GameController is used to manage everything within the 'Game' state.
 */

public enum GameState
{
	PAUSED,												// No game has been initiated or active game is paused
	GAME,												// Currently playing
	FINISH												// Player won
}

public class GameController : MonoBehaviour
{
	#region Variables
	// Public variables
	public GameObject m_DanceGuyPrefab;                 // Prefab for the dance animation
	public TextMeshPro m_LCDText;						// TMP object that displays info to player

	// Private variables
	private float m_TimeCounter = 0.0f;					// Time taken during gameplay
	private GameState m_State;                          // Current game state
	private static GameController m_Instance;			// Instance of singleton

	private int m_Gemscollected = 0;                         
	
	// Static
	public static int GemCount = 0;
	private SoundManager m_Sound;                       // The Sound manager
	private MazeGeneration m_MazeGen;                   // The maze generator

	// Properties
	public static GameController Instance { get { return m_Instance; } }
	#endregion

	#region Functions
	// Initialize Singleton
	void Awake()
	{
		if (m_Instance != null && m_Instance != this)
			Destroy(this.gameObject);
		else
			m_Instance = this;
	}
	void Start()
	{
		m_Sound = GetComponent<SoundManager>();
		m_MazeGen = GetComponent<MazeGeneration>();
		StartGame();
		SetState(GameState.GAME);
		m_TimeCounter = 0;
	}

	// Called every frame - updates the time remaining counter
	void Update()
	{
		// Process clicks on GUI buttons
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, 100.0f))
			{
				if (hit.transform != null)
				{
					if (hit.transform.gameObject.name == "MainMenuButton")
						Debug.Log("Clicked 'Main Menu'");
					if (hit.transform.gameObject.name == "RestartButton")
					{
						Debug.Log("Clicked 'Restart'");
						RestartMaze();
					}
				}
			}
		}
		
		// All the gems have been collected
		if (m_Gemscollected == GemCount)
		{
			m_Gemscollected = 0;
			OnAllGemsCollected();
		}

		// Update time counter
		if (m_State == GameState.GAME)
		{
			m_TimeCounter += Time.deltaTime;
			float minutes = Mathf.FloorToInt(m_TimeCounter / 60.0f);
			float seconds = Mathf.FloorToInt(m_TimeCounter % 60.0f);
			m_LCDText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
		}
	}

	// Adds gems to the gem counter
	public void AddGem()
	{
		m_Gemscollected++;
		m_Sound.PlayGemCollected();
	}
	
	// Sets the total amount of gems 
	public void SetGemCount(int count)
	{
		GemCount = count;
	}

	// Runs when all gems are collected
	public void OnAllGemsCollected()
	{
		m_MazeGen.Generate();
	}

	// Runs when the maze has been completed
	public void OnComplete()
	{
		// do stuff
	}

	// Starts the game
	private void StartGame()
	{
		Instantiate(m_DanceGuyPrefab);
		m_MazeGen.Generate();
		m_Sound.SetMusicVolume(0.3f);
		m_Sound.SetGemVolume(1.0f);
		m_Sound.PlayBackroundMusic();
	}

	// Restarts the maze
	private void RestartMaze()
	{
		m_Gemscollected = 0;
		m_TimeCounter = 0;
		m_MazeGen.RestartMaze();
	}

	public void OnFinish()
	{
		m_Gemscollected = 0;
		m_TimeCounter = 0;
		m_MazeGen.RestartMaze();
	}

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

	#endregion
}
