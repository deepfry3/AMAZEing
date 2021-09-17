using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  Worked on by:
//  Declan Doller
//

public class SoundManager : MonoBehaviour
{
    public AudioSource m_GemCollected;
    public AudioSource m_BackroundNoise;
    public AudioSource m_FlagCollected;

    private GameObject m_Camera;

    // From 0.0f - 1.0f
    private float m_MusicVolume = 0.0f;
    private float m_GemVolume = 0.0f;

    private bool m_VolumeChange = false;
    private static SoundManager m_Instance;
    public static SoundManager Instance
    {
        get { return m_Instance; }
    }

    // Called when script is being loaded
    void Awake()
    {
        // Initialize Singleton
        if (m_Instance != null && m_Instance != this)
            Destroy(this.gameObject);
        else
            m_Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = GameObject.FindGameObjectWithTag("MainCamera");
        if(m_Camera != null)
		{
           m_BackroundNoise = m_Camera.GetComponent<AudioSource>();
		}

        m_GemCollected = GetComponent<AudioSource>();
    }

	void update()
	{
		if(m_VolumeChange)
		{
            m_BackroundNoise.volume = m_MusicVolume;
            m_GemCollected.volume = m_GemVolume;
        }
	}

	// Sets the  Gem collection volume from 0.0f - 1.0f
	public void SetGemVolume(float volume)
	{
        m_VolumeChange = true;

        if(volume > 1)
		{
            volume = 1.0f;
		}

        else if(volume < 0)
		{
            volume = 0;
		}

        m_GemVolume = volume;
	}

    // Sets the volume from 0.0f - 1.0f
    public void SetMusicVolume(float volume)
    {
        m_VolumeChange = true;

        if (volume > 1)
        {
            volume = 1.0f;
        }

        else if (volume < 0)
        {
            volume = 0;
        }

        m_MusicVolume = volume;
    }


    // Plays the Backround music
    public void PlayBackroundMusic()
    {
        Debug.Log("Playing: Backround Music");
        m_BackroundNoise.Play();
    }

    // Plays the Gem collected sound
    public void PlayGemCollected()
    {
        Debug.Log("Playing: Gem Collected");
        m_GemCollected.Play();
    }

    // Plays when flag is collected
    public void PlayFlagCollected()
	{
        Debug.Log("Playing: Flag Collected");
        m_FlagCollected.Play();
    }
}


