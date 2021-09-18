using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Declan
 * 
 * DanceAnimation is a script used to play an animation on A dancing model
 */

public class DanceAnimation : MonoBehaviour
{
	#region Variables/Properties

	// -- Private --
	private Animator m_Animator = null;         // The animator of the dancing model
	#endregion

	#region Unity Functions
	/// <summary>
	/// Called on Awake.
	/// Caches components
	/// </summary>
	void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }
	#endregion

	#region Public Functions
	/// <summary>
	/// Using the animator plays a animation from an integer
	/// Logs the index of the animation
	/// </summary>
	/// <param name="index"></param>
	public void PlayAnimation(int index)
	{
        m_Animator.SetInteger("Play Animation", index);
        m_Animator.Play("Base Layer.Dance Switch");
        Debug.Log("Playing animation: " + index);
	}
	#endregion
}
