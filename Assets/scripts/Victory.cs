using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Victory : MonoBehaviour
{
    public ParticleSystem[] confettis;

    public void Confetti()
    {
        foreach (var item in confettis)
        {
            item.Play();
        }
    }
}
