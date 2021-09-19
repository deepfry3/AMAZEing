using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Blake, Cameron, Declan
 * 
 * SkyboxController is a Singleton used to manage the rotation of the Skybox camera
 * and Sunlight within the skybox.
 * 
 * It can be accessed via SkyboxController.Instance.
 */

/// <summary>
/// Manages Skybox and Sunlight.
/// (Singleton, accessed with <c>SkyboxController.Instance</c>.)
/// </summary>
[System.Serializable]
public class SkyboxController : MonoBehaviour
{
	#region Variables/Properties
	// -- Serialized --
	[Header("Skybox Settings")]
	[SerializeField] Vector3 m_SkyboxRotation = Vector3.zero;   // Skybox's rotational speed on each axis
	[Header("Primary Sunlight Settings")]
	[SerializeField] Vector3[] m_Rotations = new Vector3[5];    // Sunlight rotations (euler angles) per Skybox
	[SerializeField] Color[] m_Colors = new Color[5];           // Sunlight colors per Skybox

	// -- Private --
	private Light m_Light = null;								// Light component of the active Sunlight
	private int m_ActiveLight;									// Active light index in array

	// -- Properties --
	/// <summary>
	/// Gets or sets the active light index to be used as Sunlight.
	/// </summary>
	public int LightIndex
	{
		get
		{
			return m_ActiveLight;
		}
		set
		{
			if (value < 0 || value >= m_Rotations.Length)
				Debug.Log("Unable to set Light Index. Invalid index: " + value);
			else
			{
				m_ActiveLight = value;
				m_Light.transform.rotation = Quaternion.Euler(m_Rotations[value]);
				m_Light.color = m_Colors[value];
			}
		}
	}

	// -- Singleton --
	public static SkyboxController Instance { get; private set; }
	#endregion

	#region Unity Functions
	/// <summary>
	/// Called on Awake.
	/// Initializes Singleton and caches components.
	/// </summary>
	void Awake()
	{
		// Initialize Singleton
		Instance = this;

		// Cache components
		m_Light = GetComponentInChildren<Light>();
	}

	/// <summary>
	/// Called on Update.
	/// Rotates skybox.
	/// </summary>
	void Update()
	{
		transform.Rotate(m_SkyboxRotation.x, m_SkyboxRotation.y, m_SkyboxRotation.z);
	}
	#endregion
}
