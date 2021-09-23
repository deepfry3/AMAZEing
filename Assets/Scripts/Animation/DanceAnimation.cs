using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Declan
 * 
 * Used to play Dancer animations.
 */

public class DanceAnimation : MonoBehaviour
{
	#region Variables/Properties
	// -- Private --
	private Animator m_Animator = null;							// The animator of the dancing model
	#endregion

	#region Unity Functions
	/// <summary>
	/// Called on Awake.
	/// Caches components.
	/// </summary>
	void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }
	#endregion

	#region Public Functions
	/// <summary>
	/// Plays the specified animation.
	/// </summary>
	/// <param name="index"> The animation index to play </param>
	public void PlayAnimation(int index)
	{
        m_Animator.SetInteger("Play Animation", index);
        m_Animator.Play("Base Layer.Dance Switch");
        Debug.Log("Playing Dancer animation: " + index);
	}
	#endregion
}
