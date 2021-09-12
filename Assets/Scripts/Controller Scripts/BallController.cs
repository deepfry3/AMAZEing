using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Cameron, Declan
 * 
 * BallController manages everything relating to the maze ball.
 * Currently, this is limited to playing audio based on velocity.
 */

/// <summary>
/// Manages the Ball object, playing audio based on its velocity.
/// </summary>
public class BallController : MonoBehaviour
{
	#region Variables/Properties
	// -- Public --
	public AudioSource m_HitSound = null;						// Sound to play when ball collides with wall
	public AudioSource m_RollSound = null;						// Sound to play as ball increases in speed
	public AudioClip[] m_HitSoundClips = null;                  // Array of audio clips to choose from when ball collides with wall

	// -- Public --
	private Rigidbody m_Rigidbody = null;						// Rigidbody of the ball
	private float m_VelocityPrev = 0.0f;                        // Ball's velocity from the previous update frame
	#endregion

	#region Unity Functions
	/// <summary>
	/// Called on Start.
	/// Caches components.
	/// </summary>
	void Start()
	{
		// Get and cache the Rigidbody component
		m_Rigidbody = GetComponent<Rigidbody>();
	}

	/// <summary>
	/// Called on FixedUpdate.
	/// Plays appropriate sounds based on Ball velocity.
	/// </summary>
	void FixedUpdate()
	{
		// Get ball's velocity and difference since previous update
		float velocityCurrent = m_Rigidbody.velocity.magnitude;
		float velocityDiff = m_VelocityPrev - velocityCurrent;

		// Adjust roll sound volume based on velocity
		m_RollSound.volume = Mathf.Clamp(velocityCurrent / 8.0f, 0.0f, 1.0f);

		// If ball has suddenly stopped, get correct hit sound and play
		if (velocityDiff >= 0.5f)
		{
			// Set clip based on velocity difference
			m_HitSound.clip =
				(velocityDiff >= 4.689f) ? m_HitSoundClips[4] :
				(velocityDiff >= 2.679f) ? m_HitSoundClips[3] :
				(velocityDiff >= 1.531f) ? m_HitSoundClips[2] :
				(velocityDiff >= 0.875f) ? m_HitSoundClips[1] :
				m_HitSoundClips[0];

			// Play sound with slight random pitch offset
			m_HitSound.pitch = Random.Range(0.85f, 1.15f);
			m_HitSound.Play();
		}

		// Store current velocity for use next frame
		m_VelocityPrev = velocityCurrent;
	}

	/// <summary>
	/// Called on TriggerEnter.
	/// Increments Gem counter and plays sound.
	/// </summary>
	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("Ball collided with: " + other.tag);

		if (other.tag == "Gem")
		{
			GameController.Instance.AddGem();
			other.gameObject.SetActive(false);
		}
	}
	#endregion
}
