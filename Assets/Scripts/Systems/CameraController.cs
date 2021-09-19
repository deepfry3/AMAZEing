using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Cameron
 * 
 * CameraController is a Singleton used to manage everything related to the main Camera.
 * 
 * It can be accessed via CameraController.Instance.
 */

/// <summary>
/// Stores settings that the CameraController can tween between.
/// </summary>
[System.Serializable]
public class CameraSettings
{
	// -- Constructor --
	public CameraSettings(Vector3 position, Quaternion rotation, float fieldOfView)
	{
		this.position = position;
		this.rotation = rotation;
		this.fieldOfView = fieldOfView;
	}

	// -- Data --
	public Vector3 position;                                    // Camera transform's position
	public Quaternion rotation;                                 // Camera transform's rotation
	[Range(0.00001f, 179.0f)] public float fieldOfView;         // Camera's FOV
}

/// <summary>
/// Manages everything relating to the main Camera.
/// (Singleton, accessed with <c>CameraController.Instance</c>.)
/// </summary>
public class CameraController : MonoBehaviour
{
	#region Variables/Properties
	// -- Serialized --
	[SerializeField] float m_TransitionSpeed = 1.0f;        // Speed to transition/tween between two camera settings
	[SerializeField] CameraSettings m_StartSettings = null;	// Target settings upon game start
	[SerializeField] CameraSettings m_GameSettings = null;  // Target settings during gameplay
	[SerializeField] CameraSettings m_MenuSettings = null;  // Target settings from within menu
	[SerializeField] CameraSettings m_WinSettings = null;   // Target settings during Win fanfare

	// -- Private --
	private Transform m_Transform;                          // Camera's transform component
	private Camera m_Camera;                                // Camera's Camera component
	private CameraSettings m_TargetSettings;                // Camera settings currently targeted

	// -- Singleton --
	public static CameraController Instance { get; private set; }
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
		m_Transform = GetComponent<Transform>();
		m_Camera = GetComponent<Camera>();

		// Initialize variables
		m_TargetSettings = new CameraSettings(m_Transform.position, m_Transform.rotation, m_Camera.fieldOfView);
	}

	/// <summary>
	/// Called on Update.
	/// Processes camera transitions.
	/// </summary>
	void Update()
	{
		// If target settings does not match current, lerp towards target
		if (m_Transform.position != m_TargetSettings.position || m_Transform.rotation != m_TargetSettings.rotation || m_Camera.fieldOfView != m_TargetSettings.fieldOfView)
		{
			m_Transform.position = Vector3.Lerp(m_Transform.position, m_TargetSettings.position, Time.deltaTime * m_TransitionSpeed);
			m_Transform.rotation = Quaternion.Lerp(m_Transform.rotation, m_TargetSettings.rotation, Time.deltaTime * m_TransitionSpeed);
			m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, m_TargetSettings.fieldOfView, Time.deltaTime * m_TransitionSpeed);
		}
	}
	#endregion

	#region Public Functions
	/// <summary>
	/// Initiates the camera transition towards the 'start' settings
	/// </summary>
	public void TransitionToStart()
	{
		m_TargetSettings = m_StartSettings;
	}

	/// <summary>
	/// Initiates the camera transition towards the 'game' settings
	/// </summary>
	public void TransitionToGame()
	{
		m_TargetSettings = m_GameSettings;
	}

	/// <summary>
	/// Initiates the camera transition towards the 'menu' settings
	/// </summary>
	public void TransitionToMenu()
	{
		m_TargetSettings = m_MenuSettings;
	}

	/// <summary>
	/// Initiates the camera transition towards the 'win' settings
	/// </summary>
	public void TransitionToWin()
	{
		m_TargetSettings = m_WinSettings;
	}
	#endregion
}
