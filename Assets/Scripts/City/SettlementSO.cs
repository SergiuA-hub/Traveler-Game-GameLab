using NUnit.Framework;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

[CreateAssetMenu(fileName = "Settlement", menuName = "SettlementSO")]
public class SettlementSO : ScriptableObject
{
    public string settlementName;
    public Season region;
    public List<SettlementGood> production;
    public List<SettlementGood> consumption;
    public List<Cargo> itemsShop;
    public List<Cargo> upgradeShop;
    public List<LicensesSO> Licenses_Shop;


    
}

[System.Serializable]
public class SettlementGood
{
    public ResourceSO resource;
    public float dailyChange;
    public float startingStock;
    public float minStock;
    public float maxStock;
    public float tier1_price_stock;
    public float tier2_price_stock;
    public float tier3_price_stock;
    public float tier4_price_stock;
    public float tier5_price_stock;

    public float licenseCost;

    public int getPriceTier(float stock)
    {
        if (stock <= tier1_price_stock)
            return 1;
        if (stock > tier1_price_stock && stock <= tier2_price_stock)
            return 2;
        if (stock > tier2_price_stock && stock <= tier3_price_stock)
            return 3;
        if (stock > tier3_price_stock && stock <= tier4_price_stock)
            return 4;
        if (stock > tier4_price_stock)
            return 5;

        return 0;
    }
}
public enum CargoCategory
{
    
    Backpack,
    HandCart,
    OxCart,
    HorseCart
}

public enum ItemType
{
    Item,
    Upgrade
}

[System.Serializable]
public class Cargo
{
    //Total Stats
    public float weight;
    public float volume;

    public CargoSO cargoSO;

    //Upgrading Items
    public int upgradesSpaces;
    public List<CargoSO> Upgrades = new List<CargoSO>();
    

    //Item enums
    public CargoCategory category;
    public ItemType type;
    

    public Cargo( CargoSO cargoSO,int upSpace, List<CargoSO> upgrades ,CargoCategory category,ItemType type)
    {
        this.cargoSO = cargoSO;
        this.upgradesSpaces = upSpace;
        this.Upgrades = upgrades;
        this.category= category;
        this .type = type;
        
    }

    public void CalculateTotalWeightAndVolume()
    {
        weight = cargoSO.itemMaxWeight;
        volume = cargoSO.itemMaxVolume;
        //Adding Upgrades
        foreach (CargoSO item in Upgrades)
        {
            weight += item.itemMaxWeight;
            volume += item.itemMaxVolume;
        }
    }

    public bool CanAddUpgrade(CargoSO cargo)
    {
        if(Upgrades.Count < upgradesSpaces)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    
}

