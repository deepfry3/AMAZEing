using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* Author: Cameron
 * 
 * InputController is a Singleton used to detect and manage inputs from multiple sources.
 * It can be accessed via InputController.Instance.
 */

/// <summary>
/// Manages inputs, providing an interface to access multiple input sources.
/// (Properties accessed with <c>InputController.Instance</c>.)
/// </summary>
public class InputController : MonoBehaviour
{
	#region Variables/Properties
	// -- Public --
	public GameObject[] m_Buttons = null;						// Array of on-screen button objects (U, D, L, R)

	// -- Private --
	private Transform[] m_ButtonsTransforms = new Transform[4];	// Transform components of on-screen button objects (U, D, L, R)
	private Vector3 m_GyroOffset = Vector3.zero;				// Offset for calibrating device rotation
	private float m_GyroMaxTilt = 0.5f;							// Device rotation (between 0 and 90 degrees) required to reach maximum input

	// -- Properties --
	public float HAxis { get; private set; }					// Value of the virtual horizontal input axis	(-1 to 1: Left to Right)
	public float VAxis { get; private set; }					// Value of the virtual vertical input axis		(-1 to 1: Down to Up)
	public bool IsGyroActive { get; set; }                      // Whether or not device rotation control is active (ignores other inputs)

	// -- Singleton --
	public static InputController Instance { get; private set; }
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
		for (int i = 0; i < m_Buttons.Length; i++)
			m_ButtonsTransforms[i] = m_Buttons[i].GetComponent<Transform>();

		// Initialize variables and properties
		HAxis = 0.0f;
		VAxis = 0.0f;
		IsGyroActive = false;
	}

	/// <summary>
	/// Called on Update.
	/// Checks for and updates input interfaces.
	/// </summary>
	void Update()
	{
		#region Process Inputs
		// Process on-screen button presses
		int touchHAxis = 0, touchVAxis = 0;
		if (Input.GetMouseButton(0))
		{
			// Raycast the mouse position
			RaycastHit mouseHit;
			Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			Physics.Raycast(mouseRay, out mouseHit, 100.0f);

			// Record hits
			if (mouseHit.transform != null)
			{
				GameObject buttonHit = mouseHit.transform.gameObject;
				if (buttonHit == m_Buttons[0]) touchVAxis += 1;
				if (buttonHit == m_Buttons[1]) touchVAxis -= 1;
				if (buttonHit == m_Buttons[2]) touchHAxis -= 1;
				if (buttonHit == m_Buttons[3]) touchHAxis += 1;
			}
		}

		// Process inputs (only accepts device rotation if active)
		if (IsGyroActive)
		{
			HAxis = (Input.acceleration.x - m_GyroOffset.x) * (1 / m_GyroMaxTilt);
			VAxis = (Input.acceleration.y - m_GyroOffset.y) * (1 / m_GyroMaxTilt);
		}
		else
		{
			HAxis = Input.GetAxis("Horizontal") + (float)touchHAxis;
			VAxis = Input.GetAxis("Vertical") + (float)touchVAxis;
		}

		// Round and clamp axes
		HAxis = Mathf.Clamp(Mathf.Floor(HAxis * 100.0f) / 100.0f, -1.0f, 1.0f);
		VAxis = Mathf.Clamp(Mathf.Floor(VAxis * 100.0f) / 100.0f, -1.0f, 1.0f);
		#endregion

		// Set position of on-screen buttons based on input
		SetButtonZ(0, Mathf.Clamp(VAxis * 0.15f, 0.0f, 0.15f));
		SetButtonZ(1, Mathf.Clamp(-VAxis * 0.15f, 0.0f, 0.15f));
		SetButtonZ(2, Mathf.Clamp(-HAxis * 0.15f, 0.0f, 0.15f));
		SetButtonZ(3, Mathf.Clamp(HAxis * 0.15f, 0.0f, 0.15f));
	}
	#endregion

	#region Functions
	// -- Public --
	/// <summary>
	/// Toggles device rotation controls on or off if supported.
	/// Enabling device rotation disables other input methods.
	/// </summary>
	public void ToggleGyro()
	{
		if (SystemInfo.supportsAccelerometer)
			IsGyroActive = !IsGyroActive;
	}

	/// <summary>
	/// Calibrates current device rotation to zero by applying an offset to the current rotation.
	/// </summary>
	public void CalibrateGyro()
	{
		if (SystemInfo.supportsGyroscope)
			m_GyroOffset = Input.acceleration;
	}

	// -- Private --
	/// <summary>
	/// Sets the Z position of the specified on-screen Button.
	/// </summary>
	/// <param name="index">The index of the Button to change</param>
	/// <param name="Z">The Z position to set</param>
	private void SetButtonZ(int index, float z)
	{
		Vector3 currentPos = m_ButtonsTransforms[index].localPosition;
		m_ButtonsTransforms[index].localPosition = new Vector3(currentPos.x, currentPos.y, z);
	}
	#endregion
}
