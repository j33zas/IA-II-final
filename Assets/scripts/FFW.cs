using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FFW : MonoBehaviour
{


    public void FastX2()
    {
        Time.timeScale = 2;
    }
    public void FastX4()
    {
        Time.timeScale = 4;
    }
    public void NormalSpeed()
    {
        Time.timeScale = 1;
    }
    public void Pause()
    {
        Time.timeScale = 0;
    }
}
