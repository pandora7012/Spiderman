using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;


    public void SetBool(string name, bool value)
    {
        animator.SetBool(name.ToString(), value);
    }

    public void SetFloat(string name, float value)
    {
        animator.SetFloat(name.ToString(), value);
    }

    public void SetTrigger(string name)
    {
        animator.SetTrigger(name.ToString());
    }

    public void SetInt(string name, int value)
    {
        animator.SetInteger(name.ToString(), value);
    }
}
