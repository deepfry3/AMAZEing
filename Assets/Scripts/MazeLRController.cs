using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Cameron
 * 
 * MazeLRController manages the controls of the 'Left-Right' Maze box.
 */

public class MazeLRController : MonoBehaviour
{
	#region Variables
	// Public variables
	public GameObject m_UDMaze = null;						// UD Maze to get up-down movements from
	public GameObject m_Handle = null;						// Physical handle to visually rotate

	// Private variables
	private Rigidbody m_Rigidbody;							// Rigidbody of the LR Maze
	private Transform m_Transform;							// Cached Transform component
	private Transform m_UDMazeTransform;					// Cached Transform component of the UD Maze
	private Transform m_HandleTransform;					// Cached Transform component of the Handle
	private Vector3 m_InputVector = Vector3.zero;			// Left-right input stored as a Vector3
	#endregion

	#region Functions
	// Called on Start - Initialises Rigidbody and gets UD Transform component
	void Start()
	{
		// Get the Rigidbody component and reset its center of mass to be no longer auto-calculated by Unity
		m_Rigidbody = GetComponent<Rigidbody>();
		m_Rigidbody.centerOfMass = Vector3.zero;
		m_Rigidbody.inertiaTensorRotation = Quaternion.identity;

		// Cache Transform components to prevent GetComponent being called each Update
		m_Transform = transform;
		m_UDMazeTransform = m_UDMaze.transform;
		m_HandleTransform = m_Handle.transform;
	}

	// Called every frame - Updates input vector according to key-press
	void Update()
	{
		// Get input from W/S keys or U/D arrows and store in vector
		m_InputVector = Vector3.zero;
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			m_InputVector.z += 1.0f;
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			m_InputVector.z -= 1.0f;
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
		// Manually set transform angles, resetting y axis to zero
		m_Transform.rotation = Quaternion.Euler(m_UDMazeTransform.eulerAngles.x, 0.0f, m_Transform.eulerAngles.z);

		// Visually update handle rotation
		m_HandleTransform.rotation = Quaternion.Euler(8.0f * m_Transform.eulerAngles.z, m_HandleTransform.eulerAngles.y, m_HandleTransform.eulerAngles.z);
	}
	#endregion
}
