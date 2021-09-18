using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPosManager : MonoBehaviour
{
    private Vector3[] m_Positions = new Vector3[5];
    private Vector3[] m_Rotations = new Vector3[5];
    public Color[] m_Colours = new Color[5];
    public int m_Index;
    private Light m_Light = null;
    private static LightPosManager m_Instance;
    public static LightPosManager Instance
    {
        get { return m_Instance; }
    }

    // Called when script is being loaded
    void Awake()
    {
        m_Light = GetComponentInChildren<Light>();

        // Initialize Singleton
        if (m_Instance != null && m_Instance != this)
            Destroy(this.gameObject);
        else
            m_Instance = this;
    }

    // On start
	private void Start()
	{

        m_Positions[0] = new Vector3(30, 50, 30);
        m_Rotations[0] = new Vector3(51, -84, 73);
      //  m_Colours[0] = new Color(255, 233, 143, 255);

        m_Positions[1] = new Vector3(30, 30, 23);
        m_Rotations[1] = new Vector3(60, -73, 77);
       // m_Colours[1] = new Color(97, 41, 221, 255);

        m_Positions[2] = new Vector3(37, 50, -26);
        m_Rotations[2] = new Vector3(31, -50, 118);
       // m_Colours[2] = new Color(236, 213, 151, 255);

        m_Positions[3] = new Vector3(-5, 53, 76);
        m_Rotations[3] = new Vector3(4, -184, -36);
      //  m_Colours[3] = new Color(48, 157, 60, 255);

        m_Positions[4] = new Vector3(-9, -12, 1);
        m_Rotations[4] = new Vector3(-29, 18, 101);
       // m_Colours[4] = new Color(255, 255, 255, 255);
    }

    public void SetLight(int index)
	{
        //Color colour = new Color();
        //colour.r = 255 / m_Colours[index].x;
        //colour.g = 255 / m_Colours[index].y;
        //colour.b = 255 / m_Colours[index].z;
        //colour.a = 255 / m_Colours[index].w;
        m_Index = index;
        m_Light.transform.position = m_Positions[index];
        m_Light.transform.rotation = Quaternion.Euler(m_Rotations[index]);
        m_Light.color = m_Colours[index];
	}
}
