using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FarmerStatsUI : MonoBehaviour
{
    public Text Money;
    public Text corn;
    public Text seeds;
    Dictionary<string, Image> items = new Dictionary<string, Image>();
    public Image energyBar;
    public Image Hoe;
    public Image WateringCan;
    public Image cornIMG;
    public Image noItem;

    private void Awake()
    {
        items.Add("hoe", Hoe);
        items.Add("wateringcan", WateringCan);
        items.Add("corn", cornIMG);
        items.Add("", noItem);
    }

    public FarmerStatsUI SetMoneyUI(int M)
    {
        Money.text = " " + M;
        return this;
    }
    public FarmerStatsUI SetSeedUI(int S)
    {
        seeds.text = " " + S;
        return this;
    }
    public FarmerStatsUI SetCornUI(int C)
    {
        corn.text = " " + C;
        return this;
    }
    public FarmerStatsUI SetenergyUI(float Current, float Max)
    {
        energyBar.fillAmount = Current / Max;
        return this;
    }
    public FarmerStatsUI SetItemUI(string item)
    {
        foreach (var current in items)
            current.Value.gameObject.SetActive(false);
        items[item].gameObject.SetActive(true);
        return this;
    }
}
