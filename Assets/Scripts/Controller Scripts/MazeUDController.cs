using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Cameron
 * 
 * MazeUDController manages the controls of the 'Up-Down' Maze box.
 */

public class MazeUDController : MonoBehaviour
{
	#region Variables
	// Public variables
	public GameObject m_Handle = null;						// Physical handle to visually rotate

	// Private variables
	private Rigidbody m_Rigidbody;							// Rigidbody of the UD Maze
	private Transform m_Transform;                          // Cached Transform component
	private Transform m_HandleTransform;                    // Cached Transform component of the Handle
	private Vector3 m_InputVector = Vector3.zero;			// Up-down input stored as a Vector3
	#endregion

	#region Functions
	// Called on Start - Initialises Rigidbody
	void Start()
	{
		// Get the Rigidbody component and reset its center of mass to be no longer auto-calculated by Unity
		m_Rigidbody = GetComponent<Rigidbody>();
		m_Rigidbody.centerOfMass = Vector3.zero;
		m_Rigidbody.inertiaTensorRotation = Quaternion.identity;

		// Cache Transform components to prevent GetComponent being called each Update
		m_Transform = transform;
		m_HandleTransform = m_Handle.transform;
	}

	// Called every frame - Updates input vector according to key-press
	void Update()
	{
		// Get input from W/S keys or U/D arrows and store in vector
		m_InputVector = Vector3.zero;
		if (InputController.Instance.Up)
			m_InputVector.x += 1.0f;
		if (InputController.Instance.Down)
			m_InputVector.x -= 1.0f;
	}

	// Called every fixed frame - Adds torque to Rigidbody
	void FixedUpdate()
	{
		// Apply torque to rotate Rigidbody based on input
		m_Rigidbody.AddTorque(50.0f * m_InputVector);
	}

	// Called every frame after Update - resets automatically-updated axis to zero
	void LateUpdate()
	{
		// Manually set transform angles, resetting y and z axis to zero
		m_Transform.rotation = Quaternion.Euler(m_Transform.eulerAngles.x, 0.0f, 0.0f);

		// Visually update handle rotation
		m_HandleTransform.rotation = Quaternion.Euler(8.0f * transform.eulerAngles.x, m_Handle.transform.eulerAngles.y, m_Handle.transform.eulerAngles.z);
	}
	#endregion
}
