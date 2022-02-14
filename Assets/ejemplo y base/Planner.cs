using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Planner : MonoBehaviour 
{
	private readonly List<Tuple<Vector3, Vector3>> _debugRayList = new List<Tuple<Vector3, Vector3>>();

	private void Start ()
    {
		StartCoroutine(Plan());
	}
	
    private void Check(Dictionary<string, bool> state, string name, ItemType type) {

		var items = Navigation.instance.AllItems();
		var inventories = Navigation.instance.AllInventories();
		var floorItems = items.Except(inventories);
		var item = floorItems.FirstOrDefault(x => x.type == type);
		var here = transform.position;
		state["accessible" + name] = item != null && Navigation.instance.Reachable(here, item.transform.position, _debugRayList);

		var inv = inventories.Any(x => x.type == type);
		state["otherHas" + name] = inv;

		state["dead" + name] = false;
	}

    private IEnumerator Plan() {
		yield return new WaitForSeconds(0.2f);

		var observedState = new Dictionary<string, bool>();
		
		var nav = Navigation.instance;//Consigo los items
		var floorItems = nav.AllItems();
		var inventory = nav.AllInventories();
		var everything = nav.AllItems().Union(nav.AllInventories());

        //Chequeo los booleanos para cada Item, generando mi modelo de mundo (mi diccionario de bools) en ObservedState
		Check(observedState, "Key"			, ItemType.Key);
		Check(observedState, "Other"		, ItemType.Entity);
		Check(observedState, "Mace"		    , ItemType.Mace);
		Check(observedState, "PastaFrola"	, ItemType.PastaFrola);
		Check(observedState, "Door"	        , ItemType.Door);
		
        var actions = CreatePossibleActionsList();

        GoapState initial = new GoapState();
		initial.values = observedState; //le asigno los valores actuales, conseguidos antes
		initial.values["doorOpen"] = false; //agrego el bool "doorOpen"

		GoapState goal = new GoapState();
        goal.values["hasKey"] = true;
        goal.values["hasPastaFrola"] = true;
        //goal.values["hasMace"] = true;

		var typeDict = new Dictionary<string, ItemType>() {
			  { "o", ItemType.Entity }
			, { "k", ItemType.Key }
			, { "d", ItemType.Door }
			, { "m", ItemType.Mace }
			, { "pf", ItemType.PastaFrola }
		};
		var actDict = new Dictionary<string, ActionEntity>() {
			  { "Kill"	, ActionEntity.Kill }
			, { "Pickup", ActionEntity.PickUp }
			, { "Open"	, ActionEntity.Open }
		};

		var plan = Goap.Execute(initial, goal, actions);

		if(plan == null)
			Debug.Log("Couldn't plan");
		else {
			GetComponent<Guy>().ExecutePlan(
				plan
                .Select(pa => pa.Name)
				.Select(a => 
                {
                    var i2 = everything.FirstOrDefault(i => typeDict.Any(kv => a.EndsWith(kv.Key)) && i.type == typeDict.First(kv => a.EndsWith(kv.Key)).Value);
                    if (actDict.Any(kv => a.StartsWith(kv.Key)) && i2 != null)
                    {
                        return Tuple.Create(actDict.First(kv => a.StartsWith(kv.Key)).Value, i2);
                    }
                    else
                    {
                        return null;
                    }
				}).Where(a => a != null)
				.ToList()
			);
		}
	}

    private List<GoapAction> CreatePossibleActionsList()
    {
        return new List<GoapAction>()
        {
              new GoapAction("Kill o")
                .SetCost(100f)
                .Pre("deadOther", false)
                .Pre("accessibleOther", true)
                .Pre("hasMace", true)
                //.Pre((curr)=>curr.energy<10 && !daytime)

                .Effect("deadOther", true)
                //.Effect((curr)=>{ curr.energy += time.delta * curr.sleepPower;
                //                  curr.money -=10;
                //                  return curr;})

            , new GoapAction("Loot k")
                .SetCost(1f)
                .Pre("otherHasKey", true)
                .Pre("deadOther", true)

                .Effect("accessibleKey", true)
                .Effect("otherHasKey", false)

            , new GoapAction("Pickup m")
                .SetCost(2f)
                .Pre("deadMace", false)
                .Pre("otherHasMace", false)
                .Pre("accessibleMace", true)

                .Effect("accessibleMace", false)
                .Effect("hasMace", true)

            , new GoapAction("Pickup k")
                .SetCost(1f)
                //.Pre("deadKey", false)
                //.Pre("otherHasKey", false)
                .Pre("accessibleKey", true)

                .Effect("accessibleKey", false)
                .Effect("hasKey", true)

            , new GoapAction("Pickup pf")
                .SetCost(5f)					//La frola es prioritaria!
				.Pre("deadPastaFrola", false)
                .Pre("otherHasPastaFrola", false)
                .Pre("accessiblePastaFrola", true)
                .Pre("hasKey",true)

                .Effect("accessiblePastaFrola", false)
                .Effect("hasPastaFrola", true)

            , new GoapAction("Open d")
                .SetCost(3f)
                .Pre("deadDoor", false)
                .Pre("hasKey", true)

                .Effect("hasKey", false)
                .Effect("doorOpen", true)
                .Effect("deadKey", true)
                .Effect("accessiblePastaFrola", true)

            , new GoapAction("Kill d")
                .SetCost(50f)
                .Pre("deadDoor", false)
                .Pre("hasMace", true)

                .Effect("doorOpen", true)
                .Effect("hasMace", false)
                .Effect("deadMace", true)
                .Effect("deadDoor", true)
                .Effect("accessibleKey", true)
                .Effect("accessiblePastaFrola", true)
        };
    }

    void OnDrawGizmos()
    {
		Gizmos.color = Color.cyan;
		foreach(var t in _debugRayList)
        {
			Gizmos.DrawRay(t.Item1, (t.Item2-t.Item1).normalized);
			Gizmos.DrawCube(t.Item2+Vector3.up, Vector3.one*0.2f);
		}
	}
}
