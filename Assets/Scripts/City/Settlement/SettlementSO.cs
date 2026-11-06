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
    public List<CargoSO> itemsShopSO;
    public List<UpgradesSO> availableUpgrades;
    public List<Cargo> upgradeShop;
    public List<LicensesSO> Licenses_Shop;

    public bool settlementNeedsResource(ResourceSO resource)
    {
        foreach (var good in consumption)
        {
            if (good.resource == resource)
                return true;
        }
        return false;
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