using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WateringCan : Tool
{
    public ParticleSystem water;

    float percent = 0;
    public override void DropTool()
    {
        base.DropTool();
    }

    public override void PickUpTool()
    {
        base.PickUpTool();
    }

    public override void UseTool()
    {
        base.UseTool();
        water.Play();
        var farm = Physics.OverlapSphere(transform.position, 5, 1 << LayerMask.NameToLayer("node"), QueryTriggerInteraction.Collide)
        .Where(a => a.GetComponent<Farm>()).Select(a => a.GetComponent<Farm>()).First();
        if (farm != null)
            farm.Water(percent);
    }

    public WateringCan SetProggress(float inputPercent)
    {
        percent = inputPercent;
        return this;
    }
}