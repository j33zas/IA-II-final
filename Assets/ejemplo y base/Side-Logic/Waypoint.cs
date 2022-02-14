using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Waypoint : MonoBehaviour
{
	public List<Waypoint> adyacent;
    public float distance;
    Waypoint meWP;
	public HashSet<Item> nearbyItems = new HashSet<Item>();

    private void Awake()
    {
        meWP = GetComponent<Waypoint>();
        var temp = Physics.OverlapSphere(transform.position, distance).Where(x=>x.GetComponent<Waypoint>());
        foreach (var item in temp)
            if(item.GetComponent<Waypoint>() != meWP)
                adyacent.Add(item.GetComponent<Waypoint>());
    }

    void Start ()
    {
		//Make bidirectional
		foreach(var wp in adyacent) {
			if(wp != null && wp.adyacent != null) {
				if(!wp.adyacent.Contains(this))
					wp.adyacent.Add(this);
			}
		}
		adyacent = adyacent.Where(x=>x!=null).Distinct().ToList();
	}
	
	void Update ()
    {
		//For debugging: Pause then inactivate
		nearbyItems.RemoveWhere(it => !it.isActiveAndEnabled);
	}
}
