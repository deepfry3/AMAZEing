using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Declan
 * 
 * SoundManager is a Singleton used to play non-diegetic sounds in the scene.
 * 
 *  It can be accessed with SoundManager.Instance.
 * 
 * This class manages everything relating to managing the game state.
 */

/// <summary>
/// Manages non-diegetic sounds in the scene.
/// (Singleton, accessed with <c>SoundManager.Instance</c>.)
/// </summary>
[System.Serializable]
public class SoundManager : MonoBehaviour
{
	#region Variables/Properties
	// -- Serialized --
	[Header("Audio Sources/Clips")]
	[SerializeField] AudioSource m_BackgroundMusic = null;      // Background Music AudioSource
	[SerializeField] AudioSource m_WinSound = null;             // Win Sound AudioSource
	[SerializeField] AudioClip[] m_WinSoundClips = null;        // Possible Win Sound AudioClips

	// -- Singleton --
	public static SoundManager Instance { get; private set; }
	#endregion

	#region Unity Functions
	/// <summary>
	/// Called on Awake.
	/// Initializes Singleton.
	/// </summary>
	void Awake()
	{
		Instance = this;
	}
	#endregion

	#region Public Functions
	/// <summary>
	/// Plays the Background Music audio.
	/// </summary>
	public void PlayBackgroundMusic()
	{
		Debug.Log("Playing Audio: Background Music");
		m_BackgroundMusic.Play();
	}

	/// <summary>
	/// Plays the Win Sound audio using a randomly-chosen clip.
	/// </summary>
	public void PlayWinSound()
	{
		// Randomly choose win sound
		int index = Random.Range(0, m_WinSoundClips.Length);
		m_WinSound.clip = m_WinSoundClips[index];

		// Play sound
		Debug.Log("Playing Audio: Win Sound " + index);
		m_WinSound.Play();
	}
	#endregion
}


