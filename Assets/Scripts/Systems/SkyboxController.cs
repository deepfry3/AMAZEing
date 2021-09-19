using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Blake
 * 
 * Manages the rotation of the Skybox camera
 */

public class SkyboxController : MonoBehaviour
{
	#region Variables/Properties
	// -- Public -- 
	public float RotateX = 0.01f;								// Rotational speed on X axis
	public float RotateY = 0.01f;                               // Rotational speed on Y axis
	public float RotateZ = 0.01f;                               // Rotational speed on Z axis
	#endregion

	#region Unity Functions
	/// <summary>
	/// Called on Update.
	/// Rotates skybox.
	/// </summary>
	void Update()
    {
        transform.Rotate(RotateX, RotateY, RotateZ);
    }
	#endregion
}
