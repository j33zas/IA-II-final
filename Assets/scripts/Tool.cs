using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour, ITool
{
    public string toolName;
    AudioSource _AU;
    public AudioClip usesound;
    public AudioClip dropsound;
    public AudioClip grabsound;

    private void Awake()
    {
        _AU = GetComponent<AudioSource>();
    }

    public virtual void DropTool()
    {
        if (dropsound != null)
            _AU.PlayOneShot(dropsound);
        else
            Debug.LogError("Assign a drop sound");
    }

    public virtual void PickUpTool()
    {
        if (grabsound != null)
            _AU.PlayOneShot(grabsound);
        else
            Debug.LogError("Assign a pick up sound");
    }

    public virtual void UseTool()
    {
        if (usesound != null)
            _AU.PlayOneShot(usesound);
        else
            Debug.LogError("Assign a use sound");
    }
}
