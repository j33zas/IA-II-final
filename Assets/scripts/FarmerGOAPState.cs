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
        SetEnergy(input.worldState.farmerEnergy).
        SetMoney(input.worldState.money).
        SetFarmG(input.worldState.farmGrowth).
        SetSeeds(input.worldState.seedAmount).
        SetCorn(input.worldState.cornStored).
        SetItem(input.worldState.currentItem).
        SetWateredForToday(input.worldState.hasWateredToday);
        genAction = gen;
    }
}
