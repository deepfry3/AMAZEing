using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Cameron
 * 
 * Manages the rotation of the inner boxes based on input.
 */

/// <summary>
/// Manages the rotation of the inner boxes based on input.
/// </summary>
public class BoxController : MonoBehaviour
{
	#region Variables/Properties
	// -- Public --
	public float m_MaxHRotation;                                // Maximum degrees box can rotate horizontally
	public float m_MaxVRotation;								// Maximum degrees box can rotate vertically
	public GameObject m_LRBox = null;                           // Left-Right Inner Box to rotate
	public GameObject m_UDBox = null;                           // Up-Down Inner Box to rotate
	public GameObject m_LRHandle = null;                        // Left-Right handle to rotate
	public GameObject m_UDHandle = null;                        // Up-Down handle to rotate

	// -- Private --
	private Transform m_LRBoxTransform;                         // Left-Right Inner Box's Transform component
	private Transform m_UDBoxTransform;                         // Up-Down Inner Box's Transform component
	private Transform m_LRHandleTransform;				        // Left-Right handle's Transform component
	private Transform m_UDHandleTransform;                      // Up-Down handle's Transform component
	#endregion

	#region Unity Functions
	/// <summary>
	/// Called on Start.
	/// Caches components and initializes and values.
	/// </summary>
	void Start()
	{
		// Cache components
		m_LRBoxTransform = m_LRBox.GetComponent<Transform>();
		m_UDBoxTransform = m_UDBox.GetComponent<Transform>();
		m_LRHandleTransform = m_LRHandle.GetComponent<Transform>();
		m_UDHandleTransform = m_UDHandle.GetComponent<Transform>();

		// Reset Inner Boxes' center of mass to be zero, rather than auto-calculated by Unity
		// I'm honestly not sure if there's still needs to be there now that I'm not using AddTorque, but
		// I figure I'll keep it here anyway for now
		Rigidbody LRRigidbody = m_LRBox.GetComponent<Rigidbody>();
		Rigidbody UDRigidbody = m_UDBox.GetComponent<Rigidbody>();
		LRRigidbody.centerOfMass = Vector3.zero;
		UDRigidbody.centerOfMass = Vector3.zero;
		LRRigidbody.inertiaTensorRotation = Quaternion.identity;
		UDRigidbody.inertiaTensorRotation = Quaternion.identity;
	}

	/// <summary>
	/// Called on Update.
	/// Rotates objects according to player input.
	/// </summary>
	void Update()
	{
		// Calculate target angle based on input
		float xEuler = m_MaxVRotation * InputController.Instance.VAxis;
		float zEuler = m_MaxHRotation * -InputController.Instance.HAxis;

		// Apply rotation to Inner Boxes' transforms
		m_LRBoxTransform.rotation = Quaternion.Euler(xEuler, 0.0f, zEuler);
		m_UDBoxTransform.rotation = Quaternion.Euler(xEuler, 0.0f, 0.0f);

		// Apply rotation to handles accordingly
		m_LRHandleTransform.rotation = Quaternion.Euler(7.0f * zEuler, m_LRHandleTransform.eulerAngles.y, m_LRHandleTransform.eulerAngles.z);
		m_UDHandleTransform.rotation = Quaternion.Euler(7.0f * xEuler, m_UDHandleTransform.eulerAngles.y, m_UDHandleTransform.eulerAngles.z);
	}
	#endregion
}
