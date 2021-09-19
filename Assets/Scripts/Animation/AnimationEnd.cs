using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Cameron, Declan
 * 
 * Returns camera to previous position once dancers have finished.
 */

public class AnimationEnd : StateMachineBehaviour
{
	/// <summary>
	/// Called on StateExit, after animation is finished.
	/// Returns to the previous camera position.
	/// </summary>
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		CameraController.Instance.TransitionToGame();
		GameManager.Instance.DestroyDancers();
	}
}
