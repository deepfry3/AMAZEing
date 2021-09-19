using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* Author: Cameron
 * 
 * InputManager is a Singleton used to detect and manage player inputs from multiple sources.
 * 
 * It can be accessed via InputManager.Instance.
 */

/// <summary>
/// Manages inputs, providing an interface to access multiple input sources.
/// (Singleton, accessed with <c>InputManager.Instance</c>.)
/// </summary>
[System.Serializable]
public class InputManager : MonoBehaviour
{
	#region Variables/Properties
	// -- Serialized --
	[Header("Prefabs/GameObjects")]
	[SerializeField] GameObject[] m_OnScreenButtons = null;     // Array of on-screen buttons (U, D, L, R);
	[Header("Input Settings")]
	[SerializeField] float m_Sensitivity = 10.0f;               // Sensitivity/speed to lerp to target input values
	[SerializeField] [Range(5.0f, 90.0f)]
	float m_GyroRangeH = 30.0f;                                 // Degrees of horizontal rotation accepted in either direction (e.g., -30° to 30°)
	[SerializeField] [Range(5.0f, 90.0f)]
	float m_GyroRangeV = 30.0f;									// Degrees of vertical rotation accepted in either direction (e.g., -30° to 30°)

	// -- Private --
	private Transform[] m_ButtonsTransforms = new Transform[4];	// Transform components of on-screen button objects (U, D, L, R)
	private Vector3 m_GyroOffset = Vector3.zero;                // Offset for calibrating device rotation
	private float m_GyroMultiplierH = 1.0f;                     // Horizontal multiplier for gyro input (calculated from Gyro Range variable)
	private float m_GyroMultiplierV = 1.0f;						// Vertical multiplier for gyro input (calculated from Gyro Range variable)
	private float m_HAxis = 0.0f;                               // Value of the virtual horizontal input axis	(-1 to 1: Left to Right)
	private float m_VAxis = 0.0f;                               // Value of the virtual vertical input axis		(-1 to 1: Down to Up)

	// -- Properties --
	public bool IsGyroActive { get; set; } = false;             // Whether or not device rotation control is active (ignores other inputs)
	public float HAxis                                          // Value of the virtual horizontal input axis	(Public; rounded to 0.00f)
	{
		get { return Mathf.Round(m_HAxis * 100.0f) / 100.0f; }
	}
	public float VAxis                                          // Value of the virtual vertical input axis		(Public; rounded to 0.00f)
	{
		get { return Mathf.Round(m_VAxis * 100.0f) / 100.0f; }
	}

	// -- Singleton --
	public static InputManager Instance { get; private set; }
	#endregion

	#region Unity Functions
	/// <summary>
	/// Called on Awake.
	/// Initializes Singleton, caches components and initializes variables.
	/// </summary>
	void Awake()
	{
		// Initialize Singleton
		Instance = this;

		// Cache components
		for (int i = 0; i < m_OnScreenButtons.Length; i++)
			m_ButtonsTransforms[i] = m_OnScreenButtons[i].GetComponent<Transform>();

		// Initialize variables
		m_GyroMultiplierH = 1 / (m_GyroRangeH / 90.0f);
		m_GyroMultiplierV = 1 / (m_GyroRangeV / 90.0f);
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
				if (buttonHit == m_OnScreenButtons[0]) touchVAxis += 1;
				if (buttonHit == m_OnScreenButtons[1]) touchVAxis -= 1;
				if (buttonHit == m_OnScreenButtons[2]) touchHAxis -= 1;
				if (buttonHit == m_OnScreenButtons[3]) touchHAxis += 1;
			}
		}

		// Calculate target values based on device input (only accepts device rotation if active)
		float HAxisTarget, VAxisTarget;
		if (IsGyroActive)
		{
			HAxisTarget = (Input.acceleration.x - m_GyroOffset.x) * m_GyroMultiplierH;
			VAxisTarget = (Input.acceleration.y - m_GyroOffset.y) * m_GyroMultiplierV;
		}
		else
		{
			HAxisTarget = Input.GetAxis("Horizontal") + (float)touchHAxis;
			VAxisTarget = Input.GetAxis("Vertical") + (float)touchVAxis;
		}

		// Lerp towards target to smooth inputs consistently across devices
		m_HAxis = Mathf.Lerp(m_HAxis, HAxisTarget, m_Sensitivity * Time.deltaTime);
		m_VAxis = Mathf.Lerp(m_VAxis, VAxisTarget, m_Sensitivity * Time.deltaTime);

		// Round and clamp axes
		m_HAxis = Mathf.Clamp(m_HAxis, -1.0f, 1.0f);
		m_VAxis = Mathf.Clamp(m_VAxis, -1.0f, 1.0f);
		#endregion

		// Set position of on-screen buttons based on input
		SetButtonZ(0, Mathf.Clamp(VAxis * 0.15f, 0.0f, 0.15f));
		SetButtonZ(1, Mathf.Clamp(-VAxis * 0.15f, 0.0f, 0.15f));
		SetButtonZ(2, Mathf.Clamp(-HAxis * 0.15f, 0.0f, 0.15f));
		SetButtonZ(3, Mathf.Clamp(HAxis * 0.15f, 0.0f, 0.15f));
	}
	#endregion

	#region Public Functions
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
	#endregion

	#region Private Functions
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
