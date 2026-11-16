using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettlementRuntime
{
    public string settlementName;
    public SettlementSO defaultSettlementData;
    public List<TradeGood> settlementStock;    
    public List<Cargo> settlementShopItems;
    public List<CargoSO> availableCargoItems;
    public List<Cargo> settlmentUpgradeItems;
    public List<UpgradesSO> availableUpgrades;
    public List<LicensesSO> settlmentLicenses;
    public GameObject settlementGo;
    public SettlementContractsPanel contractsPanel;
    public float reputation;
    public SettlementRuntime(SettlementSO so)
    {
        defaultSettlementData = so;
        reputation = 0;
        this.settlementName = defaultSettlementData.settlementName;
        settlementShopItems = new List<Cargo>();
        availableCargoItems = new List<CargoSO>();

        settlmentUpgradeItems = new List<Cargo>();
        availableUpgrades = new List<UpgradesSO>();

        settlementStock = new List<TradeGood>();
        settlmentLicenses = new List<LicensesSO>();

    }
    public void calculatePriceTiers()
    {
        foreach (var trade in defaultSettlementData.production)
        {
            float tier1_stock = trade.dailyChange * 3;
            float tier2_stock = trade.dailyChange * 5;
            float tier3_stock = trade.dailyChange * 7;
            float tier4_stock = trade.dailyChange * 14;
            float tier5_stock = trade.maxStock;

            trade.tier1_price_stock = tier1_stock;
            trade.tier2_price_stock = tier2_stock;
            trade.tier3_price_stock = tier3_stock;
            trade.tier4_price_stock = tier4_stock;
            trade.tier5_price_stock = tier5_stock;
        }

        foreach (var trade in defaultSettlementData.consumption)
        {
            float tier1_stock = trade.dailyChange * 3;
            float tier2_stock = trade.dailyChange * 5;
            float tier3_stock = trade.dailyChange * 7;
            float tier4_stock = trade.dailyChange * 14;
            float tier5_stock = trade.maxStock;

            trade.tier1_price_stock = tier1_stock;
            trade.tier2_price_stock = tier2_stock;
            trade.tier3_price_stock = tier3_stock;
            trade.tier4_price_stock = tier4_stock;
            trade.tier5_price_stock = tier5_stock;
        }
    }

    public float getPriceFor(TradeGood res)
    {
        float currentPrice = res.resource.baseValue;

        foreach (var trade in defaultSettlementData.production)
        {
            if (trade.resource.itemName == res.resource.itemName)
            {
                int tier = trade.getPriceTier(res.amount);
                //Debug.Log($"Price Tier for {res.resource.itemName} is T{tier}");
                if (tier == 1)
                    return res.resource.baseValue * GlobalSettingsManager.TIER_1_PRICE_MULTIPLIER;
                if (tier == 2)
                    return res.resource.baseValue * GlobalSettingsManager.TIER_2_PRICE_MULTIPLIER;
                if (tier == 3)
                    return res.resource.baseValue * GlobalSettingsManager.TIER_3_PRICE_MULTIPLIER;
                if (tier == 4)
                    return res.resource.baseValue * GlobalSettingsManager.TIER_4_PRICE_MULTIPLIER;
                if (tier == 5)
                    return res.resource.baseValue * GlobalSettingsManager.TIER_5_PRICE_MULTIPLIER;
            }
        }

        foreach (var trade in defaultSettlementData.consumption)
        {
            if (trade.resource.itemName == res.resource.itemName)
            {
                int tier = trade.getPriceTier(res.amount);
                //Debug.Log($"Price Tier for {res.resource.itemName} is T{tier}");
                if (tier == 1)
                    return res.resource.baseValue * GlobalSettingsManager.TIER_1_PRICE_MULTIPLIER;
                if (tier == 2)
                    return res.resource.baseValue * GlobalSettingsManager.TIER_2_PRICE_MULTIPLIER;
                if (tier == 3)
                    return res.resource.baseValue * GlobalSettingsManager.TIER_3_PRICE_MULTIPLIER;
                if (tier == 4)
                    return res.resource.baseValue * GlobalSettingsManager.TIER_4_PRICE_MULTIPLIER;
                if (tier == 5)
                    return res.resource.baseValue * GlobalSettingsManager.TIER_5_PRICE_MULTIPLIER;
            }
        }

        return currentPrice;
    }

    public float getPriceFor(ResourceSO res)
    {
        float currentPrice = res.baseValue;

        foreach (var trade in settlementStock)
        {
            if (trade.resource.itemName == res.itemName)
                return trade.price;
        }

        return currentPrice;
    }

    public float getStockPrice(ResourceSO res)
    {
        foreach (var r in settlementStock)
        {
            if (r.resource.itemName == res.itemName)
                return r.price;
        }

        return res.baseValue * GlobalSettingsManager.UNWANTED_GOOD_PRICE_MULTIPLIER;
    }

    public float getStock(ResourceSO res)
    {
        foreach (var r in settlementStock)
        {
            if (r.resource.itemName == res.itemName)
                return r.amount;
        }
        return 0;
    }

    public void increaseStock(ResourceSO res, int amount)
    {
        foreach (var r in settlementStock)
        {
            if (r.resource.itemName == res.itemName)
                r.amount += amount;
        }
    }

    public void decreaseStock(ResourceSO res, int amount)
    {
        foreach (var r in settlementStock)
        {
            if (r.resource.itemName == res.itemName)
                r.amount -= amount;
        }
    }

    public bool isUnwantedGood(ResourceSO res)
    {
        foreach (var r in defaultSettlementData.production)
        {
            if (r.resource.itemName == res.itemName)
                return false;
        }
        foreach (var r in defaultSettlementData.consumption)
        {
            if (r.resource.itemName == res.itemName)
                return false;
        }
        return true;
    }
}
