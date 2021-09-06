using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	#region Variables
	// Public
	public float m_TransitionSpeed = 1.0f;					// How quickly to transition between two camera transforms
	public Vector3 m_GamePosition;							// Target position during gameplay
	public Quaternion m_GameRotation;						// Target rotation during gameplay
	public float m_GameFOV;									// Target FOV during gameplay
	public Vector3 m_MenuPosition;							// Target position while accessing menu
	public Quaternion m_MenuRotation;						// Target rotation while accessing menu
	public float m_MenuFOV;									// Target FOV while accessing menu

	// Private
	private Transform m_Transform;							// The camera's transform component
	private Camera m_Camera;								// The camera's Camera component
	private Vector3 m_TargetPosition;						// The current position the Camera is moving towards
	private Quaternion m_TargetRotation;					// The current rotation the Camera is moving towards
	private float m_TargetFOV;								// The current FOV the Camera is moving towards
	#endregion

	#region Functions
	// Called on Start - Caches components and initializes target properties to current
	void Start()
	{
		// Cache transform and camera components
		m_Transform = transform;
		m_Camera = GetComponent<Camera>();

		// Initialize target position to current
		m_TargetPosition = m_Transform.position;
		m_TargetRotation = m_Transform.rotation;
	}

	// Called on Update - Transitions the camera if required
	private void Update()
	{
		if (m_Transform.position != m_TargetPosition)
		{
			m_Transform.position = Vector3.Lerp(m_Transform.position, m_TargetPosition, Time.deltaTime * m_TransitionSpeed);
			m_Transform.rotation = Quaternion.Lerp(m_Transform.rotation, m_TargetRotation, Time.deltaTime * m_TransitionSpeed);
			m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, m_TargetFOV, Time.deltaTime * m_TransitionSpeed);
		}
	}

	// Sets the camera to transition towards the 'game' properties
	public void TransitionToGame()
	{
		m_TargetPosition = m_GamePosition;
		m_TargetRotation = m_GameRotation;
		m_TargetFOV = m_GameFOV;
	}

	// Sets the camera to transition towards the 'game' properties
	public void TransitionToMenu()
	{
		m_TargetPosition = m_MenuPosition;
		m_TargetRotation = m_MenuRotation;
		m_TargetFOV = m_MenuFOV;
	}
	#endregion
}
