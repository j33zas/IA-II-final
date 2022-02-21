using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : MonoBehaviour
{
    public Transform plantedPos;
    public Transform grownPos;
    public GameObject cornGO;
    public Transform startCornPos;

    public void Plant()
    {
        cornGO.transform.position = plantedPos.position;
    }

    public void Water(float percent)
    {
        cornGO.transform.localPosition = grownPos.localPosition * percent;
    }

    public void Harvest()
    {
        cornGO.transform.position = startCornPos.position;
    }
}
