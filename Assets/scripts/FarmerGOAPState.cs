using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmerGOAPState
{
    public int step = 0;
    public FarmerWorldValues worldState = new FarmerWorldValues();
    public FarmerGOAPAction genAction;

    public FarmerGOAPState (FarmerGOAPAction gen = null)
    {
        genAction = gen;
    }

    public FarmerGOAPState (FarmerGOAPState input, FarmerGOAPAction gen = null)
    {
        worldState = new FarmerWorldValues();
        worldState.
        SetCorn(input.worldState.cornStored).
        SetEnergy(input.worldState.farmerEnergy).
        SetFarmG(input.worldState.farmGrowth).
        SetItem(input.worldState.currentItem).
        SetMoney(input.worldState.money).
        SetSeeds(input.worldState.seedAmount).
        SetWateredForToday(input.worldState.hasWateredToday);
        genAction = gen;
    }
}
