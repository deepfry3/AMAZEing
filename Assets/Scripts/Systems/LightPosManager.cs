using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Declan
 * 
 *  This scrip is a singleton and controls the light position and colour for each sky box
 */

public class LightPosManager : MonoBehaviour
{
	#region Variables/Properties

    // -- Private --
	private Vector3[] m_Positions = new Vector3[5];     // Array of Positions for the light source
    private Vector3[] m_Rotations = new Vector3[5];     // Array of rotations for the light source
    private Light m_Light = null;                       // THe Light component

    // -- Public -- 
    public Color[] m_Colours = new Color[5];            // Array of colours for the light source
    public int m_Index;                                 // 

    // -- Singleton --
    private static LightPosManager m_Instance;          // The current instance
    public static LightPosManager Instance              // The public Instance
    {
        get { return m_Instance; }
    }

	#endregion

	#region Unity Functions

	/// <summary>
    /// Runs on awake.
    /// Caches components and ensures only a single instance is used
    /// </summary>
	void Awake()
    {
        m_Light = GetComponentInChildren<Light>();

        // Initialize Singleton
        if (m_Instance != null && m_Instance != this)
            Destroy(this.gameObject);
        else
            m_Instance = this;
    }

    /// <summary>
    /// Runs on first frame
    /// sets all the hardcoded positions and rotations of the light
    /// </summary>
	private void Start()
	{
        m_Positions[0] = new Vector3(30, 50, 30);
        m_Rotations[0] = new Vector3(51, -84, 73);

        m_Positions[1] = new Vector3(30, 30, 23);
        m_Rotations[1] = new Vector3(60, -73, 77);

        m_Positions[2] = new Vector3(37, 50, -26);
        m_Rotations[2] = new Vector3(31, -50, 118);

        m_Positions[3] = new Vector3(-5, 53, 76);
        m_Rotations[3] = new Vector3(4, -184, -36);

        m_Positions[4] = new Vector3(-9, -12, 1);
        m_Rotations[4] = new Vector3(-29, 18, 101);
    }
	#endregion

	#region Public Functions

    /// <summary>
    /// Called when the skybox changes
    /// Sets the position and rotation
    /// </summary>
    /// <param name="index"></param>
	public void SetLight(int index)
	{
        m_Index = index;
        m_Light.transform.position = m_Positions[index];
        m_Light.transform.rotation = Quaternion.Euler(m_Rotations[index]);
        m_Light.color = m_Colours[index];
	}
	#endregion
}
