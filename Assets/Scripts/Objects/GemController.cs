using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Declan
 * 
 * Manages the Gem object.
 */
public class GemController : MonoBehaviour
{
	#region Variables/Properties
	// -- Private --
	private Light m_Light = null;                               // The light Attached to each Gem
	#endregion

	#region Unity Functions
	/// <summary>
	/// Called on Awake.
	/// Caches components.
	/// </summary>
	void Awake()
	{
		m_Light = GetComponentInChildren<Light>();
	}
	#endregion

	#region Public Functions
    /// <summary>
    /// Sets the color of the light output from the gem.
    /// </summary>
    /// <param name="color"></param>
	public void SetLightColor(Color color)
    {
        m_Light.color = color;
    }
	#endregion
}
