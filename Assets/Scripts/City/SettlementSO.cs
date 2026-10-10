using NUnit.Framework;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

[CreateAssetMenu(fileName = "Settlement", menuName = "SettlementSO")]
public class SettlementSO : ScriptableObject
{
    public string settlementName;
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

public enum Status
{
    Owned,
    Avalabile,
    Locked
}

[System.Serializable]
public class Cargo
{
    public string ItemName;
    public CargoSO cargoSO;
    public int level;

    //enums
    public CargoCategory itemType;
    public Status itemStatus;
    

    public Cargo(string name, CargoSO cargoSO,CargoCategory type)
    {
        this.ItemName = name;
        this.cargoSO = cargoSO;
        this.itemType = type;
    }

    
    
}