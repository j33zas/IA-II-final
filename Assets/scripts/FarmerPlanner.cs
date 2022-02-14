using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class FarmerPlanner : MonoBehaviour
{
    public FarmerGOAPState startState = new FarmerGOAPState();
    FarmerGOAPState goal = new FarmerGOAPState();

    Dictionary<string, FarmerAction> actDic;

    IEnumerable<FarmerGOAPAction> actions;

    private void Awake()
    {
        goal.worldState.SetMoney(1000).SetEnergy(100).SetSeeds(0).SetWateredForToday(false).SetItem("").SetFarmG(0).SetCorn(0);

        actDic = new Dictionary<string, FarmerAction>()
        {
            {"sleep", FarmerAction.Sleep},
            {"grabhoe", FarmerAction.GrabHoe},
            {"grabwc", FarmerAction.GrabWateringCan},
            {"grabcorn", FarmerAction.GrabCorn},
            {"harvest", FarmerAction.Harvest},
            {"water", FarmerAction.Water},
            {"plant", FarmerAction.Plant},
            {"sell", FarmerAction.SellCorn},
            {"leavehoe", FarmerAction.LeaveHoe},
            {"leavewc", FarmerAction.LeaveWateringCan},
            {"buyseed", FarmerAction.BuySeed}
        };

        actions = GenerateActions();
    }

    public IEnumerable<FarmerAction> Plan(FarmerWorldValues currFWV)
    {
        startState.worldState.
            SetEnergy(currFWV.farmerEnergy).
            SetMoney(currFWV.money).
            SetSeeds(currFWV.seedAmount).
            SetWateredForToday(currFWV.hasWateredToday).
            SetItem(currFWV.currentItem).
            SetFarmG(currFWV.farmGrowth).
            SetCorn(currFWV.cornStored);
        var plan = FarmerGOAP.ExecuteGOAP(startState, goal, actions);

        if (plan == default)
        {
            Debug.LogError("Can't Plan!");
            return null;
        }
        else
        {
            //Debug.Log(plan.Count());
            return plan.Select( act => act.name).
            Select(a=>
            {
                if (actDic.Any(act => a == act.Key))
                    return actDic[a];
                else
                    return default;
            }).Where(a=> a!= default);
        }
    }


    private List<FarmerGOAPAction> GenerateActions()
    {
        return new List<FarmerGOAPAction>()
        {
            new FarmerGOAPAction("sleep")
            .SetCost(1f)
            .PreCon((a)=>
                    a.worldState.farmerEnergy <= 9)
            .Effect((a)=>
            {
                a.worldState.farmerEnergy = 100;
                a.worldState.hasWateredToday = false;
                return a;
            }),
            new FarmerGOAPAction("grabhoe")
            .SetCost(2f)
            .PreCon((a)=>
                    a.worldState.farmerEnergy > 10 &&
                    a.worldState.currentItem == "")
            .Effect((a)=>
            {
                a.worldState.currentItem = "hoe";
                a.worldState.farmerEnergy -=10;
                return a;
            }),
            new FarmerGOAPAction("grabwc")
            .SetCost(2f)
            .PreCon((a) =>
                    a.worldState.farmerEnergy > 10 &&
                    a.worldState.currentItem == "")
            .Effect((a)=>
            {
                a.worldState.currentItem = "wateringcan";
                a.worldState.farmerEnergy -= 10;
                return a;
            }),
            new FarmerGOAPAction("grabcorn")
            .SetCost(2f)
            .PreCon((a)=>
                    a.worldState.farmerEnergy > 10 &&
                    a.worldState.currentItem == "" &&
                    a.worldState.cornStored > 1)
            .Effect((a)=>
            {
                a.worldState.currentItem = "corn";
                a.worldState.cornStored --;
                a.worldState.farmerEnergy -= 10;
                return a;
            }),
            new FarmerGOAPAction("harvest")
            .SetCost(2f)
            .PreCon((a)=>
                    a.worldState.farmerEnergy >= 20 &&
                    a.worldState.farmGrowth >= 1 )
            .Effect((a)=>
            {
                a.worldState.farmerEnergy -= 20;
                a.worldState.cornStored += 5;
                a.worldState.farmGrowth = 0;
                return a;
            }),
            new FarmerGOAPAction("water")
            .SetCost(2f)
            .PreCon((a)=>
                    a.worldState.farmerEnergy >= 20 &&
                    a.worldState.currentItem == "wateringcan" &&
                    a.worldState.farmGrowth < 1 &&
                    a.worldState.farmGrowth > 0)
            .Effect((a)=>
            {
                a.worldState.farmerEnergy -= 20;
                a.worldState.farmGrowth += .5f;
                a.worldState.hasWateredToday = true;
                return a;
            }),
            new FarmerGOAPAction("plant")
            .SetCost(2f)
            .PreCon((a)=>
                    a.worldState.farmerEnergy >= 20 &&
                    a.worldState.currentItem == "hoe" &&
                    a.worldState.farmGrowth == 0f &&
                    a.worldState.seedAmount >= 5)
            .Effect((a)=>
            {
                a.worldState.farmerEnergy -= 20;
                a.worldState.seedAmount -= 5;
                a.worldState.farmGrowth = 0.0001f;
                return a;
            }),
            new FarmerGOAPAction("sell")
            .SetCost(1f)
            .PreCon((a)=>
                    a.worldState.farmerEnergy >= 20 &&
                    a.worldState.money < 1000 &&
                    a.worldState.currentItem == "corn")
            .Effect((a)=>
            {
                a.worldState.money += 150;
                a.worldState.farmerEnergy -= 15;
                a.worldState.currentItem = "";
                return a;
            }),
            new FarmerGOAPAction("leavehoe")
            .SetCost(2f)
            .PreCon((a)=>
                    a.worldState.currentItem == "hoe" &&
                    a.worldState.farmGrowth > 0f)
            .Effect((a)=>
            {
                a.worldState.currentItem = "";
                return a;
            }),
            new FarmerGOAPAction("leavewc")
            .SetCost(2f)
            .PreCon((a)=>
                    a.worldState.currentItem == "wateringcan" &&
                    a.worldState.hasWateredToday)
            .Effect((a)=>
            {
                a.worldState.currentItem = "";
                return a;
            })
            ,
            new FarmerGOAPAction("buyseed")
            .SetCost(2f)
            .PreCon((a)=>
                    a.worldState.farmerEnergy >= 20 &&
                    a.worldState.seedAmount < 5 &&
                    a.worldState.money >= 50)
            .Effect((a)=>
            {
                a.worldState.farmerEnergy -= 10;
                a.worldState.seedAmount += 5;
                a.worldState.money -= 50;
                return a;
            })
        };
    }
}