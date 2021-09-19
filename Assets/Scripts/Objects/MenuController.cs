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
 * [4] - Gem Count
 * [5] - Maze Size
 * [6] - Calibrate Gyro
 * [7] - Input
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
	private bool m_MazeUpdated = false;                         // Whether maze needs to be regenerated automatically after changing options
	private bool m_ShowInputOptions = false;					// Whether options menu includes Input buttons

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
		m_ShowInputOptions = true;
		for (int i = 0; i < m_MenuButtons.Length; i++)
			EnableButton(i);

		// Disable Calibrate Gyro button (until Gyro is enabled later), and Input button if gyro not supported
		DisableButton(6);
		if (!SystemInfo.supportsAccelerometer)
		{
			DisableButton(7);
			m_ShowInputOptions = false;
		}

		// Set all dynamic button text
		m_MenuButtonsText[7].text = "Input:\nMouse/Touch";
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
			if (Input.GetKeyDown(KeyCode.Escape) || (mouseHit.transform.gameObject == m_TrayTrigger && Input.GetMouseButtonDown(0)))
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
			// Update menu buttons
			if (state == MenuState.MAIN)
			{
				SetTargetZ(-12.5f);
				m_MenuButtonsText[3].text = "▲ Options ▲";
				EnableButton(0, 1, 2);
			}
			else
			{
				SetTargetZ(m_ShowInputOptions ? -20.5f : -16.5f);
				m_MenuButtonsText[3].text = "▼ Back ▼";
				DisableButton(0, 1, 2);
			}

			// Update other systems
			CameraController.Instance.TransitionToMenu();
			if (GameManager.Instance.State == GameState.GAME)
				GameManager.Instance.State = GameState.PAUSED;
			if (m_MazeUpdated)
				MazeGeneration.Instance.NewMaze();
			m_MazeUpdated = false;
		}
		else
		{
			// Update menu buttons
			SetTargetZ(-4.5f);
			m_MenuButtonsText[3].text = "▲ Options ▲";

			// Update other systems
			switch (GameManager.Instance.State)
			{
				case GameState.GAME:
				case GameState.FINISH:
					CameraController.Instance.TransitionToGame();
					break;
				case GameState.PAUSED:
					CameraController.Instance.TransitionToGame();
					GameManager.Instance.State = GameState.GAME;
					break;
				case GameState.START:
					CameraController.Instance.TransitionToStart();
					break;
			}
		}
	}

	/// <summary>
	/// Toggles the menu tray between opened (MenuState.MAIN) and closed (MenuState.CLOSED) states.
	/// </summary>
	public void ToggleMenu()
	{
		SetState(IsMenuOpen ? MenuState.CLOSED : MenuState.MAIN);
	}

	/// <summary>
	/// Sets the label of the "Maze Size" button in the Options menu
	/// </summary>
	public void SetMazeSizeText(Vector2Int gridSize)
	{
		m_MenuButtonsText[5].text = "Maze Size:\n" + gridSize.x + "x" + gridSize.y;
	}

	/// <summary>
	/// Sets the label of the "Gem Count" buttonin the Options menu
	/// </summary>
	public void SetGemCountText(int gemCount)
	{
		m_MenuButtonsText[4].text = "Gems:\n" + gemCount;
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
				MazeGeneration.Instance.NewMaze();
				GameManager.Instance.State = GameState.GAME;
				ToggleMenu();
				break;
			case 1: // Retry Button
				MazeGeneration.Instance.ResetMaze();
				GameManager.Instance.State = GameState.GAME;
				ToggleMenu();
				break;
			case 2: // Quit Button
				Application.Quit();
				break;
			case 3: // Options Button
				SetState(m_State == MenuState.MAIN ? MenuState.OPTIONS : MenuState.MAIN);
				break;
			case 4: // Gem Count button
				if (MazeGeneration.Instance.GemSpawnCount == MazeGeneration.Instance.MaxGemSpawnCount)
					MazeGeneration.Instance.GemSpawnCount = MazeGeneration.Instance.MinGemSpawnCount;
				else
					MazeGeneration.Instance.GemSpawnCount++;
				m_MazeUpdated = true;
				break;
			case 5: // Maze Size button
				if (MazeGeneration.Instance.GridSizeIndex == MazeGeneration.Instance.GridSizesCount - 1)
					MazeGeneration.Instance.GridSizeIndex = 0;
				else
					MazeGeneration.Instance.GridSizeIndex++;
				m_MazeUpdated = true;
				break;
			case 6: // Calibrate Gyro Button
				InputManager.Instance.CalibrateGyro();
				ToggleMenu();
				break;
			case 7: // Input Button
				InputManager.Instance.ToggleGyro();
				if (InputManager.Instance.IsGyroActive)
				{
					m_MenuButtonsText[7].text = "Input\nGyro";
					EnableButton(6);
				}
				else
				{
					m_MenuButtonsText[7].text = "Input\nMouse/Touch";
					DisableButton(6);
				}
				break;
		}
	}

	/// <summary>
	/// Sets the specified Menu Button to be disabled.
	/// If disabled, Menu Buttons are greyed out and cannot be pressed.
	/// </summary>
	/// <param name="index">The index of the Menu Button to disable</param>
	private void DisableButton(params int[] index)
	{
		foreach (int button in index)
		{
			// Set color to be greyed out, and store button as disabled
			try
			{
				m_MenuButtonsText[button].color = new Color(0.75f, 0.75f, 0.75f, 0.25f);
				m_MenuButtonsEnabled[button] = false;
			}
			// Throw error if index is invalid
			catch
			{
				Debug.Log("MenuController.cs - Unable to disable button: " + button);
			}
		}
	}

	/// <summary>
	/// Sets the specified Menu Button to be enabled.
	/// If disabled, Menu Buttons are greyed out and cannot be pressed.
	/// </summary>
	/// <param name="index">The index of the Menu Button to enable</param>
	private void EnableButton(params int[] index)
	{
		foreach (int button in index)
		{
			// Set color to be greyed out, and store button as disabled
			try
			{
				m_MenuButtonsText[button].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
				m_MenuButtonsEnabled[button] = true;
			}
			// Throw error if index is invalid
			catch
			{
				Debug.Log("MenuController.cs - Unable to enable button: " + button);
			}
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
