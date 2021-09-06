using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* Author: Cameron
 * 
 * InputController is a Singleton used to detect and manage inputs.
 * It can be accessed via InputController.Instance.
 */

/// <summary>
/// Manages everything relating to the active game.
/// (Properties accessed with <c>GameController.Instance</c>.)
/// </summary>
public class InputController : MonoBehaviour
{
	#region Variables/Properties
	// Pubilc
	public GameObject[] m_Buttons = null;					// Array of on-screen button objects (U, D, L, R)

	// -- Properties --
	public bool Up { get; private set; }					// Whether or not Up key (on-screen or keyboard) is being pressed
	public bool Down { get; private set; }                  // Whether or not Down key (on-screen or keyboard) is being pressed
	public bool Left { get; private set; }                  // Whether or not Left key (on-screen or keyboard) is being pressed
	public bool Right { get; private set; }                 // Whether or not Right key (on-screen or keyboard) is being pressed

	#region Singleton
	private static InputController m_Instance;
	public static InputController Instance
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
	/// Called on Update.
	/// Processes input managing.
	/// </summary>
	void Update()
	{
		// Process on-screen button presses
		bool[] touchInput = new bool[4];
		if (Input.GetMouseButton(0))
		{
			// Raycast the mouse position
			RaycastHit mouseHit;
			Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			Physics.Raycast(mouseRay, out mouseHit, 100.0f);

			// Check for hits
			if (mouseHit.transform != null)
			{
				for (int i = 0; i < 4; i++)
					touchInput[i] = mouseHit.transform.gameObject == m_Buttons[i];
			}
		}

		// Store result of keypress
		Up =	Input.GetKey(KeyCode.UpArrow)		|| Input.GetKey(KeyCode.W)	|| touchInput[0];
		Down =	Input.GetKey(KeyCode.DownArrow)		|| Input.GetKey(KeyCode.S)	|| touchInput[1];
		Left =	Input.GetKey(KeyCode.LeftArrow)		|| Input.GetKey(KeyCode.A)	|| touchInput[2];
		Right =	Input.GetKey(KeyCode.RightArrow)	|| Input.GetKey(KeyCode.D)	|| touchInput[3];

		// Set position of on-screen keys based on press
		SetButtonZ(0, (Up ? 0.1f : 0.0f));
		SetButtonZ(1, (Down ? 0.1f : 0.0f));
		SetButtonZ(2, (Left ? 0.1f : 0.0f));
		SetButtonZ(3, (Right ? 0.1f : 0.0f));
	}
	#endregion

	#region Functions
	/// <summary>
	/// Sets the Z position of the specified Button.
	/// </summary>
	/// <param name="index">The index of the Button to change</param>
	/// <param name="Z">The Z position to set</param>
	private void SetButtonZ(int index, float z)
	{
		Vector3 currentPos = m_Buttons[index].transform.localPosition;
		m_Buttons[index].transform.localPosition = new Vector3(currentPos.x, currentPos.y, z);
	}
	#endregion
}
