using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Cameron
 * 
 * CameraController is a Singleton used to manage everything related to the Camera, meaning references to
 * it do not need to be stored, and its public properties can be accessed via CameraController.Instance.
 * 
 * This class manages the main and only camera in the game.
 */

/// <summary>
/// Stores the 
/// </summary>
[System.Serializable]
public class CameraSettings
{
	// Constructor
	public CameraSettings(Vector3 position, Quaternion rotation, float fieldOfView)
	{
		this.position = position;
		this.rotation = rotation;
		this.fieldOfView = fieldOfView;
	}

	// Transform settings
	public Vector3 position;								// Camera transform's position
	public Quaternion rotation;                             // Camera transform's rotation

	// Camera settings
	[Range(0.00001f, 179.0f)]
	public float fieldOfView;								// Camera's FOV
}

/// <summary>
/// Manages everything relating to the main Camera.
/// (Properties accessed with <c>CameraController.Instance</c>.)
/// </summary>
public class CameraController : MonoBehaviour
{
	#region Variables/Properties
	// -- Public --
	public float m_TransitionSpeed = 1.0f;                  // Transition speed between two camera settings
	public CameraSettings m_GameSettings;                   // Target camera settings during gameplay
	public CameraSettings m_MenuSettings;                   // Target camera settings while accessing menu
	public CameraSettings m_AnimationSettings;

	// -- Private --
	private Transform m_Transform;							// Camera's transform component
	private Camera m_Camera;                                // Camera's Camera component
	private CameraSettings m_TargetSettings;                // Camera settings currently targeted

	// -- Properties --
	// (currently blank)

	#region Singleton
	private static CameraController m_Instance;
	public static CameraController Instance
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
	/// Called on Start.
	/// Caches components and initializes target settings to current.
	/// </summary>
	void Start()
	{
		// Cache components
		m_Transform = GetComponent<Transform>();
		m_Camera = GetComponent<Camera>();

		// Initialize target settings to current
		m_TargetSettings = new CameraSettings(m_Transform.position, m_Transform.rotation, m_Camera.fieldOfView);
	}

	/// <summary>
	/// Called on Update.
	/// Processes camera transitions.
	/// </summary>
	void Update()
	{
		// If target settings does not match current, lerp towards target
		if (m_Transform.position != m_TargetSettings.position || m_Transform.rotation != m_TargetSettings.rotation)
		{
			m_Transform.position = Vector3.Lerp(m_Transform.position, m_TargetSettings.position, Time.deltaTime * m_TransitionSpeed);
			m_Transform.rotation = Quaternion.Lerp(m_Transform.rotation, m_TargetSettings.rotation, Time.deltaTime * m_TransitionSpeed);
			m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, m_TargetSettings.fieldOfView, Time.deltaTime * m_TransitionSpeed);
		}
	}
	#endregion

	#region Functions
	// -- Public --
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
	/// Initiates the camera transition towards the 'animation'
	/// </summary>
	public void TransitionToAnimation()
	{
		m_TargetSettings = m_AnimationSettings;
	}
	#endregion
}
