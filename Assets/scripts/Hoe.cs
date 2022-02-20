using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hoe : Tool
{
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
        var farm = Physics.OverlapSphere(transform.position, 5, 1 << LayerMask.NameToLayer("node"), QueryTriggerInteraction.Collide)
        .Where(a => a.GetComponent<Farm>()).Select(a => a.GetComponent<Farm>()).First();
        if(farm != null)
            farm.Plant();
    }
}