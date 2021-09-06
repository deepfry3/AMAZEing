using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
	#region Variables
	// Public
	public float m_TransitionSpeed = 1.0f;                  // How quickly to transition between two positions
	public GameObject m_DrawerButton;                       // Button object that triggers drawer opening/closing
	public GameObject[] m_MenuButtons;                      // Array of button objects that trigger options changing

	// Private
	private Transform m_Transform;                          // The menu drawer's transform component
	private CameraController m_CameraController;            // Reference to CameraController class used by main camera
	private bool m_MenuOpen;                                // Whether or not menu is currently open
	private Vector3 m_TargetPosition;                       // The current position the drawer is moving towards
	#endregion

	#region Functions
	// Called on Start - Caches components, deactivates buttons and initializes target properties to current
	void Start()
	{
		// Cache transform and CameraController components
		m_Transform = transform;
		m_CameraController = Camera.main.GetComponent<CameraController>();

		// Initialize target position to current
		m_TargetPosition = m_Transform.position;
	}

	// Called on Update - Checks for input and transitions the menu if required
	void Update()
	{
		// Raycast mouse position
		RaycastHit mouseHit;
		Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(mouseRay, out mouseHit, 100.0f);

		// Toggle menu on click/touch
		if (Input.GetMouseButtonDown(0))
		{
			if (mouseHit.transform != null && mouseHit.transform.gameObject == m_DrawerButton)
				ToggleMenu();
		}

		// Transition menu if required
		if (m_Transform.position != m_TargetPosition)
			m_Transform.position = Vector3.Lerp(m_Transform.position, m_TargetPosition, Time.deltaTime * m_TransitionSpeed);

		// Process hover/input for menu buttons if drawer is open
		if (m_MenuOpen)
		{
			for (int i = 0; i < m_MenuButtons.Length; i++)
			{
				if (mouseHit.transform != null && mouseHit.transform.gameObject == m_MenuButtons[i])
					m_MenuButtons[i].transform.localPosition = new Vector3(m_MenuButtons[i].transform.localPosition.x, 0.5f, m_MenuButtons[i].transform.localPosition.z);

				else
					m_MenuButtons[i].transform.localPosition = new Vector3(m_MenuButtons[i].transform.localPosition.x, 0.25f, m_MenuButtons[i].transform.localPosition.z);
			}
		}
	}

	void ToggleMenu()
	{
		if (m_MenuOpen)
		{
			// Set target drawer and camera position
			m_TargetPosition = new Vector3(-7.5f, 3.75f, -7.5f);
			m_CameraController.TransitionToGame();
			GameController.Instance.SetState(GameState.GAME);
		}
		else
		{
			// Set target drawer and camera position
			m_TargetPosition = new Vector3(-7.5f, 3.75f, -17.5f);
			m_CameraController.TransitionToMenu();
			GameController.Instance.SetState(GameState.PAUSED);
		}

		m_MenuOpen = !m_MenuOpen;
	}
	#endregion
}
