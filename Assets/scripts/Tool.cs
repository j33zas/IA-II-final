using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour, ITool
{
    public string toolName;
    Animator _anim;
    AudioSource _AU;
    public AudioClip usesound;
    public AudioClip dropsound;
    public AudioClip grabsound;

    public virtual void LeaveTool()
    {
        throw new System.NotImplementedException();
    }

    public virtual void PickUpTool()
    {
        throw new System.NotImplementedException();
    }

    public virtual void UseTool()
    {
        throw new System.NotImplementedException();
    }
}
