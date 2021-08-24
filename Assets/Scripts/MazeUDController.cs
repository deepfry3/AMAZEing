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
	// Private variables
	private Rigidbody m_Rigidbody;						// Rigidbody of the UD Maze
	private Vector3 m_InputVector = Vector3.zero;       // Up-down input stored as a Vector3
	#endregion

	#region Functions
	// Called on Start - Initialises Rigidbody
	void Start()
	{
		// Get the Rigidbody component and reset its center of mass to be no longer auto-calculated by Unity
		m_Rigidbody = GetComponent<Rigidbody>();
		m_Rigidbody.centerOfMass = Vector3.zero;
		m_Rigidbody.inertiaTensorRotation = Quaternion.identity;
	}

	// Called every frame - Updates input vector according to key-press
	void Update()
	{
		// Get input from W/S keys or U/D arrows and store in vector
		m_InputVector = Vector3.zero;
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
			m_InputVector.x += 1.0f;
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.DownArrow))
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
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0.0f, 0.0f);
	}
	#endregion
}
