﻿using System.Collections;
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
	public TextMeshPro m_TimeCounter = null;			// TMP object that displays time to player
	public TextMeshPro m_GemCounter = null;             // TMP object that displays gem count to player

	// Private variables
	private float m_TimeRemaining = 0;                  // Time taken during gameplay

	private int m_Gemscollected = 0;                         
	
	// Static
	public static int GemCount = 0;
	private SoundManager m_Sound;                       // The Sound manager
	#endregion

	#region Functions

	void Start()
	{
		m_Sound = GetComponent<SoundManager>();
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
						Debug.Log("Clicked 'Restart'");
				}
			}
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

	// Runs when the maze has been completed
	public void OnComplete()
	{
		// do stuff
	}

	// Generated the maze
	public void GenerateMaze()
	{
		// generate maze
	}

	// Starts the game
	private void StartGame()
	{
		m_Sound.SetMusicVolume(0.5f);
		m_Sound.SetGemVolume(1.0f);
		m_Sound.PlayBackroundMusic();
	}

	#endregion
}
