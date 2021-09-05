using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* Author: Cameron, Declan
 * 
 * GameController is used to manage everything within the 'Game' state.
 */

public class GameController : MonoBehaviour
{
	#region Variables
	// Public variables
	public GameObject m_DanceGuyPrefab;					// Prefab for the dance animation
	public TextMeshPro m_TimeCounter = null;			// TMP object that displays time to player
	public TextMeshPro m_GemCounter = null;             // TMP object that displays gem count to player

	// Private variables
	private float m_TimeRemaining = 0;                  // Time taken during gameplay

	private int m_Gemscollected = 0;                         
	
	// Static
	public static int GemCount = 0;
	private SoundManager m_Sound;                       // The Sound manager
	private MazeGeneration m_MazeGen;					// The maze generator
	#endregion

	#region Functions

	void Start()
	{
		m_Sound = GetComponent<SoundManager>();
		m_MazeGen = GetComponent<MazeGeneration>();
		StartGame();
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
		m_TimeRemaining += Time.deltaTime;
		float minutes = Mathf.FloorToInt(m_TimeRemaining / 60.0f);
		float seconds = Mathf.FloorToInt(m_TimeRemaining % 60.0f);
		m_TimeCounter.text = string.Format("{0:00}:{1:00}", minutes, seconds);

		// Update Gem counter
		m_GemCounter.text = ("" + m_Gemscollected + " / " + GemCount);
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
		m_TimeRemaining = 0;
		m_MazeGen.RestartMaze();
	}

	public void OnFinish()
	{
		m_Gemscollected = 0;
		m_TimeRemaining = 0;
		m_MazeGen.RestartMaze();
	}

	#endregion
}
