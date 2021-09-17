using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceAnimation : MonoBehaviour
{
    private Animator m_Animator = null;

    // Start is called before the first frame update
    void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void PlayAnimation(int index)
	{
        Debug.Log("Playing animation: " + index);
        m_Animator.SetInteger("Play Animation", index);
        m_Animator.Play("Base Layer.Dance Switch");
	}
}
