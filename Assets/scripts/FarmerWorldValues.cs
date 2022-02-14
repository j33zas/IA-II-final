using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmerWorldValues
{
    public int farmerEnergy;
    public int money;
    public float farmGrowth;
    public int seedAmount;
    public int cornStored;
    public string currentItem;
    public bool hasWateredToday;

    #region Builders
    public FarmerWorldValues SetEnergy(int energy)
    {
        farmerEnergy = energy;
        return this;
    }
    public FarmerWorldValues SetMoney(int cash)
    {
        money = cash;
        return this;
    }
    public FarmerWorldValues SetFarmG(float growth)
    {
        farmGrowth = growth;
        return this;
    }
    public FarmerWorldValues SetSeeds(int seeds)
    {
        seedAmount = seeds;
        return this;
    }
    public FarmerWorldValues SetCorn(int corn)
    {
        cornStored= corn;
        return this;
    }
    public FarmerWorldValues SetItem(string itemName)
    {
        currentItem = itemName;
        return this;
    }
    public FarmerWorldValues SetWateredForToday(bool yn)
    {
        hasWateredToday = yn;
        return this;
    }
    #endregion

    public void Debugvalues()
    {
        Debug.Log("[" + farmerEnergy + " Energy] - [" 
                + money + " money] - [" 
                + farmGrowth + " growth] - [" 
                + seedAmount + " seeds] - ["
                + cornStored + " Corn] - ["
                + currentItem + " item] - ["
                + hasWateredToday + " watered]");
    }
}
