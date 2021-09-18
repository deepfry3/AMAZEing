using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Declan
 * 
 * Gem scrip is used on each gem object, handling it's events
 */
public class Gem : MonoBehaviour
{
	#region Variables/Properties

	private Light m_Light = null;      // The light Attached to each Gem
	#endregion

	#region Unity Functions

    /// <summary>
    /// Runs on awake.
    /// Caches Light component from child
    /// </summary>
	void Awake()
    {
        m_Light = GetComponentInChildren<Light>();
    }

    /// <summary>
    /// Calls once collided with
    /// Deactivates gem object
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag + " Collided with: " + this.tag);

        if (other.tag == "Ball")
        {
            gameObject.SetActive(false);
        }
    }
	#endregion

	#region Public Functions

    /// <summary>
    /// Called after Gem is instantiated
    ///  Changes the colour output of the gems light
    /// </summary>
    /// <param name="color"></param>
	public void SetLightColor(Color color)
    {
        m_Light.color = color;
    }

	#endregion
}
