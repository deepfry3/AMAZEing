using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Declan
 * 
 * Flag script is used to handle flag events
 */

public class Flag : MonoBehaviour
{
	#region Unity Functions
    
    /// <summary>
    /// Called once collided with
    /// calles GameController's OnFinish function and deactivates object
    /// </summary>
    /// <param name="other"></param>
	private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag + " Collided with: " + this.tag);

        if (other.tag == "Ball")
        {
            GameController.Instance.OnFinish();
            gameObject.SetActive(false);
        }
    }
	#endregion
}

