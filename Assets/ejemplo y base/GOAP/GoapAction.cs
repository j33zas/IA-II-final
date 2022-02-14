using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoapAction
{
    public Dictionary<string, bool> preconditions { get; private set; }
    public Dictionary<string, bool> effects { get; private set; }
    public string Name { get; private set; }
    public float Cost { get; private set; }

    public GoapAction(string name)
    {
        this.Name = name;
        Cost = 1f;
        preconditions = new Dictionary<string, bool>();
        effects = new Dictionary<string, bool>();
    }

    public GoapAction SetCost(float cost)
    {
        if (cost < 1f)
            Debug.Log(string.Format("Warning: Using cost < 1f for '{0}' could yield sub-optimal results", Name));
        this.Cost = cost;
        return this;
    }
    public GoapAction Pre(string s, bool value)//cambiar
    {
        preconditions[s] = value;
        return this;
    }
    public GoapAction Effect(string s, bool value)//cambiar
    {
        effects[s] = value;
        return this;
    }
}
