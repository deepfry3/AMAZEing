using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
// Made by Declan
public class Simple_Player_Movement : MonoBehaviour
{
	private Transform m_TransformConponent;
	private Vector3 m_AnglesMovement = Vector3.zero;
	float m_RotattionSpeed = 100;

	// Called on first frame update
	private void Start()
	{
		m_TransformConponent = GetComponent<Transform>();
	}

	// Called per frame update
	private void Update()
	{
		// Caluculate angle difference to apply from input
		m_AnglesMovement = Vector3.zero;

		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			m_AnglesMovement.y -= 0.5f;
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			m_AnglesMovement.y += 0.5f;
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
			m_AnglesMovement.x += 0.5f;
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
			m_AnglesMovement.x -= 0.5f;

	}
	private void OnMouseDrag()
	{
		float rotX = Input.GetAxis("Mouse X") * m_RotattionSpeed * Mathf.Deg2Rad;
		float rotY = Input.GetAxis("Mouse Y") * m_RotattionSpeed * Mathf.Deg2Rad;
		m_TransformConponent.Rotate(Vector3.up, rotX);
		m_TransformConponent.Rotate(Vector3.right, rotY);
	}

	// Updated at a fixedd rate
	private void FixedUpdate()
	{
		m_TransformConponent.Rotate(m_AnglesMovement);
	}
}
