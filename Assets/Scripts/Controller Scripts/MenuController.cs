using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* Author: Cameron
 * 
 * MenuController is a Singleton used to manage everything related to the Menu Tray and its contents,
 * meaning references to it do not need to be stored, and its public properties can be accessed via
 * MenuController.Instance.
 */

/// <summary>
/// Enum for the basic states in the menu.
/// Represents the current target, e.g., Tray currently animating towards closed would be set to CLOSED.
/// </summary>
public enum MenuState
{
	/// <summary> Menu target is completely closed </summary>
	CLOSED,
	/// <summary> Menu target is only exposing game buttons. </summary>
	MAIN,
	/// <summary> Menu target is exposing options buttons. </summary>
	OPTIONS
}

/// <summary>
/// Manages everything relating to the Menu Tray.
/// (Properties accessed with <c>MenuController.Instance</c>.)
/// </summary>
public class MenuController : MonoBehaviour
{
	#region Variables/Properties
	// -- Public --
	public float m_TransitionSpeed = 1.0f;						// Transition speed between two positions
	public GameObject m_TrayTrigger;							// Object that triggers Tray opening/closing when clicked
	public GameObject[] m_MenuButtons;                          // Array of option button objects
	public TextMeshPro m_OptionsButtonText;                     // Text displaying 'Options' (to be toggled between 'Options'/'Back')
	public TextMeshPro m_CalibrateGyroButtonText;               // Text displaying 'Calibrate Gyro' (for mobile devices)
	public TextMeshPro m_InputButtonText;                       // Text displaying the current input method (for mobile devices)
	public TextMeshPro m_GemCountButtonText;                    // Text displaying Gem Count
	public TextMeshPro m_MazeSizeButtonText;					// Text displaying Maze Size

	// -- Private --
	private Transform m_Transform;								// Tray's transform component
	private Vector3 m_TargetPosition;                           // Tray position currently targeted
	private MenuState m_State;									// Current Tray open state
	private bool m_MenuOpen;                                    // Whether or not Tray is currently open
	private bool m_MobileButtonsEnabled;						// Whether or not mobile buttons are enabled (Gyro/Input)
	private float m_InputDisabledTimer;                         // Timer between inputs (preventing accidental double-inputs)

	// -- Singleton --
	public static MenuController Instance { get; private set; }
	#endregion

	#region Unity Functions
	/// <summary>
	/// Called on Awake.
	/// Initializes Singleton.
	/// </summary>
	void Awake()
	{
		// Initialize Singleton
		Instance = this;
	}

	/// <summary>
	/// Called on Start.
	/// Caches components and initializes variables.
	/// </summary>
	void Start()
	{
		// Cache components
		m_Transform = GetComponent<Transform>();

		// Initialize variables
		m_TargetPosition = m_Transform.position;
		m_MenuOpen = false;
		m_InputDisabledTimer = 0.0f;
		m_State = MenuState.CLOSED;
		m_MobileButtonsEnabled = true;

		// Set text properties
		if (!SystemInfo.supportsAccelerometer)
		{
			m_InputButtonText.color = new Color(0.8f, 0.8f, 0.8f, 0.3f);
			m_CalibrateGyroButtonText.color = new Color(0.8f, 0.8f, 0.8f, 0.3f);
			m_MobileButtonsEnabled = false;
		}
		m_InputButtonText.text = "Input:\nMouse/Touch";
		MazeGeneration mazeGen = GameController.Instance.GetComponent<MazeGeneration>();
		int ind = mazeGen.GridSizeIndex;
		m_MazeSizeButtonText.text = "Maze Size:\n" + mazeGen.m_GridSizes[ind].x + "x" + mazeGen.m_GridSizes[ind].y;
		m_GemCountButtonText.text = "Gem Count:\n" + mazeGen.m_GemCount;
	}

	/// <summary>
	/// Called on Update.
	/// Processes transitions and interactions.
	/// </summary>
	void Update()
	{
		// Raycast the mouse position
		RaycastHit mouseHit;
		Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(mouseRay, out mouseHit, 100.0f);

		// Transition menu if required
		if (m_Transform.position != m_TargetPosition)
			m_Transform.position = Vector3.Lerp(m_Transform.position, m_TargetPosition, Time.deltaTime * m_TransitionSpeed);

		// Process input
		if (mouseHit.transform != null)
		{
			// Toggle Tray if clicked
			if (mouseHit.transform.gameObject == m_TrayTrigger && Input.GetMouseButtonDown(0))
				ToggleMenu();

			if (m_MenuOpen)
			{
				for (int i = 0; i < m_MenuButtons.Length; i++)
				{
					// Don't respond if mobile buttons are disabled
					if (!m_MobileButtonsEnabled && (i == 4 || i == 5))
						continue;

					// Hover/Click each button based on input
					if (mouseHit.transform.gameObject == m_MenuButtons[i])
					{
						// Set button position based on if mouse is down or just hovered
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
	}
	#endregion

	#region Functions
	// -- Private --
	/// <summary>
	/// Sets the MenuState as specified (CLOSED, MAIN or OPTIONS).
	/// </summary>
	/// <param name="state">The MenuState to set the Tray to</param>
	public void SetState(MenuState state)
	{
		// Set state as specified, and temporarily disable second input
		m_State = state;
		m_InputDisabledTimer = 0.5f;

		// Update target position and game state accordingly
		switch (state)
		{
			case MenuState.CLOSED:
				SetMenuZ(-4.5f);
				CameraController.Instance.TransitionToGame();
				GameController.Instance.SetState(GameState.GAME);
				m_OptionsButtonText.text = "▲ Options ▲";
				break;
			default:
				SetMenuZ(state == MenuState.MAIN ? -12.5f : -20.5f);
				CameraController.Instance.TransitionToMenu();
				GameController.Instance.SetState(GameState.PAUSED);
				m_OptionsButtonText.text = state == MenuState.MAIN ? "▲ Options ▲" : "▼ Back ▼";
				break;
		}
	}

	/// <summary>
	/// Toggles the Menu Tray between opened and closed states.
	/// </summary>
	public void ToggleMenu()
	{
		// Set state and menuOpen flag accordingly
		SetState(m_MenuOpen ? MenuState.CLOSED : MenuState.MAIN);
		m_MenuOpen = !m_MenuOpen;
	}

	/// <summary>
	/// Sets the Y position of the specified Menu Button.
	/// </summary>
	/// <param name="index">The index of the Menu Button to change</param>
	/// <param name="y">The Y position to set</param>
	private void SetButtonY(int index, float y)
	{
		Vector3 currentPos = m_MenuButtons[index].transform.localPosition;
		m_MenuButtons[index].transform.localPosition = new Vector3(currentPos.x, y, currentPos.z);
	}

	/// <summary>
	/// Sets the target Z position of the Menu Tray.
	/// </summary>
	/// <param name="y">The Z position to set</param>
	private void SetMenuZ(float z)
	{
		m_TargetPosition = new Vector3(m_Transform.position.x, m_Transform.position.y, z);
	}

	/// <summary>
	/// Called on button click.
	/// Performs the action of the specified button.
	/// </summary>
	/// <param name="index">The index of the Menu Button to change</param>
	private void OnButtonPress(int index)
	{
		// Perform action based on specified button index
		switch (index)
		{
			case 0:		// New Maze Button
				GameController.Instance.GenerateNewMaze();
				ToggleMenu();
				break;
			case 1:		// Retry Button
				GameController.Instance.RestartMaze();
				ToggleMenu();
				break;
			case 2:		// Quit Button
				Application.Quit();
				break;
			case 3:		// Options Button
				SetState(m_State == MenuState.MAIN ? MenuState.OPTIONS : MenuState.MAIN);
				break;
			case 4:		// Calibrate Gyro Button
				InputController.Instance.CalibrateGyro();
				break;
			case 5:		// Input Button
				InputController.Instance.ToggleGyro();
				m_InputButtonText.text = (InputController.Instance.IsGyroActive ? "Input\nGyro" : "Input\nMouse/Touch");
				break;
			case 6:     // Gem Count button
				MazeGeneration mazeGen = GameController.Instance.GetComponent<MazeGeneration>();
				if (mazeGen.m_GemCount == 6)
					mazeGen.m_GemCount = 1;
				else
					mazeGen.m_GemCount++;
				m_GemCountButtonText.text = "Gem Count:\n" + mazeGen.m_GemCount;
				break;
			case 7:     // Maze Size button
				MazeGeneration mazeGen2 = GameController.Instance.GetComponent<MazeGeneration>();
				if (mazeGen2.GridSizeIndex == mazeGen2.m_GridSizes.Length - 1)
					mazeGen2.GridSizeIndex = 0;
				else
					mazeGen2.GridSizeIndex++;
				int ind = mazeGen2.GridSizeIndex;
				m_MazeSizeButtonText.text = "Maze Size:\n" + mazeGen2.m_GridSizes[ind].x + "x" + mazeGen2.m_GridSizes[ind].y;
				break;
		}
	}
	#endregion
}
