using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Cameron
 * 
 * BoxController is a Singleton used to manage everything inner-box rotation based on user input.

 * It can be accessed with BoxController.Instance.
 */

/// <summary>
/// Manages the rotation of the inner boxes based on input.
/// (Singleton, accessed with <c>BoxController.Instance</c>.)
/// </summary>
public class BoxController : MonoBehaviour
{
	#region Variables/Properties
	// -- Serialized --
	[SerializeField] float m_MaxHRotation =  4.0f;              // Maximum degrees box can rotate horizontally
	[SerializeField] float m_MaxVRotation = 4.0f;               // Maximum degrees box can rotate vertically
	[SerializeField] GameObject m_LRBox = null;                 // Left-Right Inner Box to rotate
	[SerializeField] GameObject m_UDBox = null;                 // Up-Down Inner Box to rotate
	[SerializeField] GameObject m_LRHandle = null;              // Left-Right handle to rotate
	[SerializeField] GameObject m_UDHandle = null;              // Up-Down handle to rotate

	// -- Private --
	private Transform m_LRBoxTransform;                         // Left-Right Inner Box's Transform component
	private Transform m_UDBoxTransform;                         // Up-Down Inner Box's Transform component
	private Transform m_LRHandleTransform;                      // Left-Right handle's Transform component
	private Transform m_UDHandleTransform;                      // Up-Down handle's Transform component

	// -- Singleton --
	public static BoxController Instance { get; private set; }
	#endregion

	#region Unity Functions
	/// <summary>
	/// Called on Awake.
	/// Initializes Singleton, and caches components.
	/// </summary>
	void Awake()
	{
		// Initialize Singleton
		Instance = this;

		// Cache components
		m_LRBoxTransform = m_LRBox.GetComponent<Transform>();
		m_UDBoxTransform = m_UDBox.GetComponent<Transform>();
		m_LRHandleTransform = m_LRHandle.GetComponent<Transform>();
		m_UDHandleTransform = m_UDHandle.GetComponent<Transform>();
	}

	/// <summary>
	/// Called on Update.
	/// Rotates objects according to player input.
	/// </summary>
	void Update()
	{
		// Calculate target angle based on input
		float xEuler = m_MaxVRotation * InputManager.Instance.VAxis;
		float zEuler = m_MaxHRotation * -InputManager.Instance.HAxis;

		// Apply rotation to Inner Boxes' transforms
		m_LRBoxTransform.rotation = Quaternion.Euler(xEuler, 0.0f, zEuler);
		m_UDBoxTransform.rotation = Quaternion.Euler(xEuler, 0.0f, 0.0f);

		// Apply rotation to handles accordingly
		m_LRHandleTransform.rotation = Quaternion.Euler(7.0f * zEuler, m_LRHandleTransform.eulerAngles.y, m_LRHandleTransform.eulerAngles.z);
		m_UDHandleTransform.rotation = Quaternion.Euler(7.0f * xEuler, m_UDHandleTransform.eulerAngles.y, m_UDHandleTransform.eulerAngles.z);
	}
	#endregion

	#region Public Functions
	/// <summary>
	/// Resets the rotation transforms of the inner boxes to their default flat position.
	/// </summary>
	public void ResetRotation()
	{
		m_LRBoxTransform.rotation = Quaternion.identity;
		m_UDBoxTransform.rotation = Quaternion.identity;
	}
	#endregion
}
