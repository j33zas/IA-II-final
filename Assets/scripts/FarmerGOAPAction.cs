using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FarmerGOAPAction
{

    public Func<FarmerGOAPState, bool> preCon;
    public Func<FarmerGOAPState, FarmerGOAPState> effect;
    public float cost;
    public string name;

    public FarmerGOAPAction(string N)
    {
        name = N;
        cost = 1;
    }
    #region Builders
    public FarmerGOAPAction SetCost(float c)
    {
        if (c < 1f)
            Debug.Log(string.Format("combiene usar valor >1"));
        cost = c;
        return this;
    }
    public FarmerGOAPAction PreCon(Func<FarmerGOAPState, bool> f)
    {
        preCon = f;
        return this;
    }
    public FarmerGOAPAction Effect(Func<FarmerGOAPState, FarmerGOAPState> f)
    {
        effect = f;
        return this;
    }
    #endregion
}
