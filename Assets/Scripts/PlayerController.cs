using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Transform m_TransformComponent;
    private Vector3 m_MovementAngles = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        m_TransformComponent = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate angle difference to apply from input
        m_MovementAngles = Vector3.zero;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            m_MovementAngles.z += 0.5f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            m_MovementAngles.z -= 0.5f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            m_MovementAngles.x += 0.5f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            m_MovementAngles.x -= 0.5f;
    }

	private void FixedUpdate()
	{
        // Apply rotation from input
        m_TransformComponent.Rotate(m_MovementAngles);
	}//if
}
