using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* Author: Cameron
 * 
 * MenuController is a Singleton used to manage everything related to the menu tray and its contents,
 * including sliding in and out, and performing actions based on button presses.
 * 
 * It can be accessed with MenuController.Instance.
 * 
 * Buttons:
 * [0] - New Maze
 * [1] - Retry
 * [2] - Quit
 * [3] - Options
 * [4] - Calibrate Gyro
 * [5] - Input
 * [6] - Gem Count
 * [7] - Maze Size
 */

/// <summary>
/// Enum for the basic states in the menu.
/// Represents the current position target (e.g., tray currently animating towards closed would be set to CLOSED).
/// </summary>
public enum MenuState
{
	/// <summary> Menu target is completely closed </summary>
	CLOSED,
	/// <summary> Menu target is only exposing game buttons </summary>
	MAIN,
	/// <summary> Menu target is exposing options buttons </summary>
	OPTIONS
}

/// <summary>
/// Manages everything relating to the menu tray.
/// (Singleton, accessed with <c>MenuController.Instance</c>.)
/// </summary>
[System.Serializable]
public class MenuController : MonoBehaviour
{
	#region Variables/Properties
	// -- Serialized --
	[SerializeField] float m_TransitionSpeed = 1.0f;            // Speed to lerp between positions with
	[SerializeField] GameObject m_TrayTrigger = null;           // Triggers tray opening/closing when clicked
	[SerializeField] GameObject[] m_MenuButtons = null;         // Buttons inside the tray

	// -- Private --
	private Transform m_Transform = null;                       // Cached Transform component
	private TextMeshPro[] m_MenuButtonsText = null;             // Cached TextMeshPro components of Menu Buttons
	private bool[] m_MenuButtonsEnabled;                        // Whether each menu button is enabled (can be pressed)
	private Vector3 m_TargetPosition;                           // Current target position
	private MenuState m_State;                                  // Current MenuState
	private float m_InputDisabledTimer;                         // Timer between inputs (preventing accidental double-inputs)
	private bool MazeChanged = false;

	// -- Properties --
	/// <summary>
	/// Returns whether the menu's state is currently open (true) or closed (false).
	/// </summary>
	public bool IsMenuOpen
	{
		get { return m_State != MenuState.CLOSED; }
	}

	// -- Singleton --
	public static MenuController Instance { get; private set; }
	#endregion

	#region Unity Functions
	/// <summary>
	/// Called on Awake.
	/// Initializes Singleton, caches components, initializes variables and enables buttons based on System Info.
	/// </summary>
	void Awake()
	{
		// Initialize Singleton
		Instance = this;

		// Cache components
		m_Transform = GetComponent<Transform>();
		m_MenuButtonsText = new TextMeshPro[m_MenuButtons.Length];
		for (int i = 0; i < m_MenuButtons.Length; i++)
			m_MenuButtonsText[i] = m_MenuButtons[i].GetComponentInChildren<TextMeshPro>();

		// Initialize variables
		m_TargetPosition = m_Transform.position;
		m_InputDisabledTimer = 0.0f;
		m_State = MenuState.CLOSED;

		// Set all buttons to be enabled by default
		m_MenuButtonsEnabled = new bool[m_MenuButtons.Length];
		for (int i = 0; i < m_MenuButtons.Length; i++)
			EnableButton(i);

		// Disable Calibrate Gyro button, and Input button if gyro not supported
		DisableButton(4);
		if (!SystemInfo.supportsAccelerometer)
			DisableButton(5);

		// Set all dynamic button text
		// UNFINISHED - Edit this section MazeGeneration is a Singleton
		MazeGeneration mazeGen = GameController.Instance.GetComponent<MazeGeneration>();
		int ind = mazeGen.GridSizeIndex;
		m_MenuButtonsText[5].text = "Input:\nMouse/Touch";
		m_MenuButtonsText[6].text = "Gems:\n" + mazeGen.m_GemCount;
		m_MenuButtonsText[7].text = "Maze Size:\n" + mazeGen.m_GridSizes[ind].x + "x" + mazeGen.m_GridSizes[ind].y;
	}

	/// <summary>
	/// Called on Update.
	/// Processes transitions and button interactions.
	/// </summary>
	void Update()
	{
		#region Process input
		// Raycast the mouse position
		RaycastHit mouseHit;
		Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(mouseRay, out mouseHit, 100.0f);

		// Process input
		if (mouseHit.transform != null)
		{
			// Tray input
			if (mouseHit.transform.gameObject == m_TrayTrigger && Input.GetMouseButtonDown(0))
				ToggleMenu();

			// Menu Button input
			if (IsMenuOpen)
			{
				for (int i = 0; i < m_MenuButtons.Length; i++)
				{
					// For each button, set position based on mouse click/hover, if button is enabled
					if (mouseHit.transform.gameObject == m_MenuButtons[i] && m_MenuButtonsEnabled[i])
					{
						SetButtonY(i, (Input.GetMouseButton(0) ? 0.15f : 0.35f));
						if (Input.GetMouseButtonUp(0) && m_InputDisabledTimer <= 0.0f)
							OnButtonPress(i);
					}
					else
					{
						SetButtonY(i, 0.25f);
					}
				}
			}
		}

		// Process input timer (prevents accidental double-inputs)
		if (m_InputDisabledTimer > 0.0f)
		{
			m_InputDisabledTimer -= Time.deltaTime;
			if (m_InputDisabledTimer < 0.0f) m_InputDisabledTimer = 0.0f;
		}
		#endregion

		// Transition menu if required
		if (m_Transform.position != m_TargetPosition)
			m_Transform.position = Vector3.Lerp(m_Transform.position, m_TargetPosition, Time.deltaTime * m_TransitionSpeed);
	}
	#endregion

	#region Public Functions
	/// <summary>
	/// Sets the MenuState as specified.
	/// </summary>
	/// <param name="state">The MenuState to set</param>
	public void SetState(MenuState state)
	{
		// Set state as specified, and temporarily disable input
		m_State = state;
		m_InputDisabledTimer = 0.5f;

		// Update target position and game state accordingly
		if (IsMenuOpen)
		{
			SetTargetZ(state == MenuState.MAIN ? -12.5f : -20.5f);
			CameraController.Instance.TransitionToMenu();
			GameController.Instance.SetState(GameState.PAUSED);
			m_MenuButtonsText[3].text = state == MenuState.MAIN ? "▲ Options ▲" : "▼ Back ▼";
		}
		else
		{
			SetTargetZ(-4.5f);
			CameraController.Instance.TransitionToGame();
			GameController.Instance.SetState(GameState.GAME);
			m_MenuButtonsText[3].text = "▲ Options ▲";
		}
	}

	/// <summary>
	/// Toggles the menu tray between opened (MenuState.MAIN) and closed (MenuState.CLOSED) states.
	/// </summary>
	public void ToggleMenu()
	{
		if(IsMenuOpen && MazeChanged)
		{
			GameController.Instance.GenerateNewMaze();
			MazeChanged = false;
		}

		SetState(IsMenuOpen ? MenuState.CLOSED : MenuState.MAIN);
	}
	#endregion

	#region Private Functions
	/// <summary>
	/// Called on button press.
	/// Performs the action of the specified button.
	/// </summary>
	/// <param name="index">The index of the Menu Button that was pressed</param>
	private void OnButtonPress(int index)
	{
		switch (index)
		{
			case 0: // New Maze Button
				GameController.Instance.GenerateNewMaze();
				ToggleMenu();
				break;
			case 1: // Retry Button
				GameController.Instance.RestartMaze();
				ToggleMenu();
				break;
			case 2: // Quit Button
				Application.Quit();
				break;
			case 3: // Options Button
				SetState(m_State == MenuState.MAIN ? MenuState.OPTIONS : MenuState.MAIN);
				break;
			case 4: // Calibrate Gyro Button
				InputController.Instance.CalibrateGyro();
				ToggleMenu();
				break;
			case 5: // Input Button
				InputController.Instance.ToggleGyro();
				if (InputController.Instance.IsGyroActive)
				{
					m_MenuButtonsText[5].text = "Input\nGyro";
					EnableButton(4);
				}
				else
				{
					m_MenuButtonsText[5].text = "Input\nMouse/Touch";
					DisableButton(4);
				}
				break;
			case 6: // Gem Count button
					// UNFINISHED - Replace this section when MazeGeneration is a Singleton
				MazeGeneration mazeGen = GameController.Instance.GetComponent<MazeGeneration>();
				if (mazeGen.m_GemCount == 6)
					mazeGen.m_GemCount = 1;
				else
					mazeGen.m_GemCount++;
				m_MenuButtonsText[6].text = "Gems:\n" + mazeGen.m_GemCount;
				MazeChanged = true;	
				break;
			case 7: // Maze Size button
					// UNFINISHED - Replace this section when MazeGeneration is a Singleton
				MazeGeneration mazeGen2 = GameController.Instance.GetComponent<MazeGeneration>();
				if (mazeGen2.GridSizeIndex == mazeGen2.m_GridSizes.Length - 1)
					mazeGen2.GridSizeIndex = 0;
				else
					mazeGen2.GridSizeIndex++;
				int ind = mazeGen2.GridSizeIndex;
				m_MenuButtonsText[7].text = "Maze Size:\n" + mazeGen2.m_GridSizes[ind].x + "x" + mazeGen2.m_GridSizes[ind].y;
				MazeChanged = true;
				break;
		}
	}

	/// <summary>
	/// Sets the specified Menu Button to be disabled.
	/// If disabled, Menu Buttons are greyed out and cannot be pressed.
	/// </summary>
	/// <param name="index">The index of the Menu Button to disable</param>
	private void DisableButton(int index)
	{
		// Set color to be greyed out, and store button as disabled
		try
		{
			m_MenuButtonsText[index].color = new Color(0.75f, 0.75f, 0.75f, 0.25f);
			m_MenuButtonsEnabled[index] = false;
		}
		// Throw error if index is invalid
		catch
		{
			Debug.Log("MenuController.cs - Unable to disable button: " + index);
		}
	}

	/// <summary>
	/// Sets the specified Menu Button to be enabled.
	/// If disabled, Menu Buttons are greyed out and cannot be pressed.
	/// </summary>
	/// <param name="index">The index of the Menu Button to enable</param>
	private void EnableButton(int index)
	{
		// Set color to be greyed out, and store button as disabled
		try
		{
			m_MenuButtonsText[index].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			m_MenuButtonsEnabled[index] = true;
		}
		// Throw error if index is invalid
		catch
		{
			Debug.Log("MenuController.cs - Unable to enable button: " + index);
		}
	}

	/// <summary>
	/// Sets the Y position of the specified Menu Button (for hover/click effect).
	/// </summary>
	/// <param name="index">The index of the Menu Button to change</param>
	/// <param name="y">The Y position to set</param>
	private void SetButtonY(int index, float y)
	{
		Vector3 currentPos = m_MenuButtons[index].transform.localPosition;
		m_MenuButtons[index].transform.localPosition = new Vector3(currentPos.x, y, currentPos.z);
	}

	/// <summary>
	/// Sets the target Z position of the menu tray.
	/// </summary>
	/// <param name="y">The Z position to set</param>
	private void SetTargetZ(float z)
	{
		m_TargetPosition = new Vector3(m_Transform.position.x, m_Transform.position.y, z);
	}
	#endregion
}
