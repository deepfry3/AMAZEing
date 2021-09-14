using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Worked on by:
//  Declan Doller
//
public class Gem : MonoBehaviour
{
    private GameObject m_Maze;
    private Light m_Light;
    private GameController m_GameController;

    // Start is called before the first frame update
    void Awake()
    {
        m_Maze = GameObject.FindGameObjectWithTag("Maze");
        if (m_Maze != null)
        {
            m_GameController = m_Maze.GetComponent<GameController>();
        }
        m_Light = GetComponentInChildren<Light>();
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag + " Collided with: " + this.tag);

        if (other.tag == "Ball")
        {
            gameObject.SetActive(false);
        }
    }

    // Sets the gems light colour
    public void SetLightColor(Color color)
    {
        m_Light.color = color;
    }
}
