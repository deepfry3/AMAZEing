using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Declan
 * 
 * Soundmanager is a singleton used to manage some of the sounds within the scene
 */

public class SoundManager : MonoBehaviour
{
    #region Variables/Propeties

    // -- Private -- 
    private GameObject m_Camera = null;             // The Camera object which holds some Audio sources

    // -- Public --
    public AudioSource m_GemCollected = null;       // The Audiosource for the gem collected sound
    public AudioSource m_BackroundNoise = null;     // The Audiosource for the background noise sound
    public AudioSource m_FlagCollected = null;      // The Audiosource for the Flag collection sound

    // -- Singleton Instances --
    private static SoundManager m_Instance;
    public static SoundManager Instance
    {
        get { return m_Instance; }
    }
	#endregion

	#region Unity Functions

    /// <summary>
    /// RUns on awake.
    /// Makes sure there is only one instance of SoundManager
    /// </summary>
	void Awake()
    {
        // Initialize Singleton
        if (m_Instance != null && m_Instance != this)
            Destroy(this.gameObject);
        else
            m_Instance = this;
    }

    /// <summary>
    /// Runs on first frame.
    /// Caches needed components
    /// </summary>
    void Start()
    {
        m_Camera = GameObject.FindGameObjectWithTag("MainCamera");
        if(m_Camera != null)
		{
           m_BackroundNoise = m_Camera.GetComponent<AudioSource>();
		}

        m_GemCollected = GetComponent<AudioSource>();
    }
	#endregion

	#region Public Functions

    /// <summary>
    /// Called when game starts
    /// Plays the background music
    /// </summary>
	public void PlayBackroundMusic()
    {
        Debug.Log("Playing: Backround Music");
        m_BackroundNoise.Play();
    }

    /// <summary>
    /// Called when gem is collected
    /// Plays the gem collection sound
    /// </summary>
    public void PlayGemCollected()
    {
        Debug.Log("Playing: Gem Collected");
        m_GemCollected.Play();
    }

    /// <summary>
    /// Called when flag is collected
    /// Plays the flag collection sound
    /// </summary>
    public void PlayFlagCollected()
	{
        Debug.Log("Playing: Flag Collected");
        m_FlagCollected.Play();
    }
	#endregion
}


