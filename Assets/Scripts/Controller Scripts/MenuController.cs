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
	public TextMeshPro m_InputButtonText;						// Text displaying the current input method (for mobile devices)

	// -- Private --
	private Transform m_Transform;								// Tray's transform component
	private Vector3 m_TargetPosition;							// Tray position currently targeted
	private bool m_MenuOpen;									// Whether or not Tray is currently open
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
		m_InputButtonText.text = "Input: Mouse/Touch";
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
	/// Toggles the Menu Tray between opened and closed states.
	/// </summary>
	public void ToggleMenu()
	{
		if (m_MenuOpen)
		{
			// Set target drawer and camera position
			m_TargetPosition = new Vector3(m_Transform.position.x, m_Transform.position.y, -7.5f);
			CameraController.Instance.TransitionToGame();
			GameController.Instance.SetState(GameState.GAME);
		}
		else
		{
			// Set target drawer and camera position
			m_TargetPosition = new Vector3(m_Transform.position.x, m_Transform.position.y, (SystemInfo.supportsGyroscope ? -17.5f : -13.5f));
			CameraController.Instance.TransitionToMenu();
			GameController.Instance.SetState(GameState.PAUSED);
		}

		m_MenuOpen = !m_MenuOpen;
		m_InputDisabledTimer = 0.5f;
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
	/// Called on button click.
	/// Performs the action of the specified button.
	/// </summary>
	/// <param name="index">The index of the Menu Button to change</param>
	private void OnButtonPress(int index)
	{
		// Perform action based on specified button index
		switch (index)
		{
			case 0:		GameController.Instance.GenerateNewMaze();			break;		// New Maze Button
			case 1:		GameController.Instance.RestartMaze();				break;		// Retry Button
			case 2:		Application.Quit();									break;		// Quit Button
			case 3:		InputController.Instance.ToggleGyro();				break;		// Input Toggle Button
			case 4:		InputController.Instance.CalibrateGyro();			break;		// Calibrate Gyro Button
		}

		// Update Input Toggle button text to the selected input, otherwise leave menu
		if (index == 3)
			m_InputButtonText.text = (InputController.Instance.IsGyroActive ? "Input: Gyro" : "Input: Mouse/Touch");
		else
			ToggleMenu();
	}
	#endregion
}
