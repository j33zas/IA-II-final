using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FarmerGOAP
{
    public static IEnumerable<FarmerGOAPAction> ExecuteGOAP(FarmerGOAPState start, FarmerGOAPState final, IEnumerable<FarmerGOAPAction> posibleActions)
    {
        int watchdog = 300;
        var seq = AStarNormal<FarmerGOAPState>.Run(start, final,
            (curr, goal) => goal.worldState.money - curr.worldState.money,//Heuristica
            curr => curr.worldState.money >= final.worldState.money,//como satisfacer al objetivo final
            curr =>
            {
                if (watchdog == 0)
                    return Enumerable.Empty<AStarNormal<FarmerGOAPState>.Arc>();
                else
                    watchdog--;

                return posibleActions.Where(action => action.preCon(curr))
                .Aggregate(new FList<AStarNormal<FarmerGOAPState>.Arc>(), (actionResult, actionIN) =>
                {
                    //Debug.Log(actionIN.name);
                    var newstate = new FarmerGOAPState(curr);
                    newstate = actionIN.effect(newstate);
                    newstate.genAction = actionIN;
                    newstate.step = curr.step + 1;
                    return actionResult + new AStarNormal<FarmerGOAPState>.Arc(newstate, actionIN.cost);
                });
            });

        if (seq == null)
            Debug.LogError("No Plan posible");

        return seq.Skip(1).Select(x => x.genAction);
    }
}
