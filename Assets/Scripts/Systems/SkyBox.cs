using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Blake
 * 
 * Manages the rotation of the Skybox camera
 */

public class SkyBox : MonoBehaviour
{
	#region Variables/Properties

	// -- Public -- 
	public float RotateX = 0.01f;
	public float RotateY = 0.01f;
	public float RotateZ = 0.01f;
	#endregion

	#region Unity Functions
	private void Update()
    {
        transform.Rotate(RotateX, RotateY, RotateZ);
    }

	#endregion
}
