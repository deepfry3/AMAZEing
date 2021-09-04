using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Worked on by:
//  Declan Doller
//
public class Gem : MonoBehaviour
{ 

    private GameObject m_Maze;
    private GameController m_GameController;

    // Start is called before the first frame update
    void Start()
    {
        m_Maze = GameObject.FindGameObjectWithTag("Maze");
        if(m_Maze != null)
		{
            m_GameController = m_Maze.GetComponent<GameController>();
		}
    }

	private void OnTriggerEnter(Collider other)
	{
		 Debug.Log( other.tag + " Collided with: " + this.tag);

        if(other.tag == "Ball")
		{
            m_GameController.AddGem();
            Destroy(gameObject);
		}
	}
}
