using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Blake
 * 
 * Manages the rotation of the Skybox camera
 */

public class SkyBox : MonoBehaviour
{

	public float RotateX = 0.01f;
	public float RotateY = 0.01f;
	public float RotateZ = 0.01f;

	// Update is called once per frame
	void Update()
    {
        transform.Rotate(RotateX, RotateY, RotateZ);
    }
}
