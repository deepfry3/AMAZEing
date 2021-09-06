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
	public GameObject m_UpButton = null;
	public GameObject m_DownButton = null;
	public GameObject m_LeftButton = null;
	public GameObject m_RightButton = null;

	// Private
	private bool m_Up = false;
	private bool m_Down = false;
	private bool m_Left = false;
	private bool m_Right = false;

	// -- Properties --
	public bool Up { get { return m_Up; } }
	public bool Down { get { return m_Down; } }
	public bool Left { get { return m_Left; } }
	public bool Right { get { return m_Right; } }

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
		bool upTouch = false, downTouch = false, leftTouch = false, rightTouch = false;
		if (Input.GetMouseButton(0))
		{
			// Raycast the mouse position
			RaycastHit mouseHit;
			Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			Physics.Raycast(mouseRay, out mouseHit, 100.0f);

			// Check for hits
			if (mouseHit.transform != null)
			{
				upTouch = mouseHit.transform.gameObject == m_UpButton;
				downTouch = mouseHit.transform.gameObject == m_DownButton;
				leftTouch = mouseHit.transform.gameObject == m_LeftButton;
				rightTouch = mouseHit.transform.gameObject == m_RightButton;
			}
		}

		// Store result of keypress
		m_Up =		Input.GetKey(KeyCode.UpArrow)		|| Input.GetKey(KeyCode.W)	|| upTouch;
		m_Down =	Input.GetKey(KeyCode.DownArrow)		|| Input.GetKey(KeyCode.S)	|| downTouch;
		m_Left =	Input.GetKey(KeyCode.LeftArrow)		|| Input.GetKey(KeyCode.A)	|| leftTouch;
		m_Right =	Input.GetKey(KeyCode.RightArrow)	|| Input.GetKey(KeyCode.D)	|| rightTouch;
	}
	#endregion
}
