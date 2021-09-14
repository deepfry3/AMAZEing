using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBox : MonoBehaviour
{

	public float RotateX = 0.01f;
	public float RotateY = 0f;
	public float RotateZ = 0.01f;

	// Update is called once per frame
	void Update()
    {
        transform.Rotate(RotateX, RotateY, RotateZ);
    }
}
