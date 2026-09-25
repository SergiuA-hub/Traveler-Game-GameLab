using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SettlementDeployment
{
    public SettlementSO settlementSo;
    public GameObject settlmentGo;
}

[System.Serializable]
public class TradeGood
{
    public ResourceSO resource;
    public float amount;
    public float price;

    public TradeGood(ResourceSO resource, float amount, float price)
    {
        this.resource = resource;
        this.amount = amount;
        this.price = price;
    }
}

[System.Serializable]
public class SettlementRuntime
{
    public string settlementName;
    public SettlementSO defaultSettlementData;
    public List<TradeGood> settlementStock;
    public GameObject settlementGo;

    public SettlementRuntime(SettlementSO so)
    {
        defaultSettlementData = so;
        this.settlementName = defaultSettlementData.settlementName;
        settlementStock = new List<TradeGood>();

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
            if(trade.resource.itemName == res.resource.itemName)
            {
                int tier = trade.getPriceTier(res.amount);
                //Debug.Log($"Price Tier for {res.resource.itemName} is T{tier}");
                if (tier == 1)
                    return res.resource.baseValue * 1.6f;
                if (tier == 2)
                    return res.resource.baseValue * 1.2f;
                if (tier == 3)
                    return res.resource.baseValue * 1f;
                if (tier == 4)
                    return res.resource.baseValue * 0.8f;
                if (tier == 5)
                    return res.resource.baseValue * 0.6f;
            }
        }

        foreach (var trade in defaultSettlementData.consumption)
        {
            if (trade.resource.itemName == res.resource.itemName)
            {
                int tier = trade.getPriceTier(res.amount);
                //Debug.Log($"Price Tier for {res.resource.itemName} is T{tier}");
                if (tier == 1)
                    return res.resource.baseValue * 1.6f;
                if (tier == 2)
                    return res.resource.baseValue * 1.2f;
                if (tier == 3)
                    return res.resource.baseValue * 1f;
                if (tier == 4)
                    return res.resource.baseValue * 0.8f;
                if (tier == 5)
                    return res.resource.baseValue * 0.6f;
            }
        }

        return currentPrice;
    }

    public float getStockPrice(ResourceSO res)
    {
        foreach(var r in settlementStock)
        {
            if (r.resource.itemName == res.itemName)
                return r.price;
        }

        return res.baseValue * GlobalSettingsManager.UNWANTED_GOOD_PRICE_MULTIPLIER;
    }

    public void increaseStock(ResourceSO res, int amount =1)
    {
        foreach (var r in settlementStock)
        {
            if (r.resource.itemName == res.itemName)
                r.amount += amount;
        }
    }

    public void decreaseStock(ResourceSO res, int amount = 1)
    {
        foreach (var r in settlementStock)
        {
            if (r.resource.itemName == res.itemName)
                r.amount -= amount;
        }
    }
}

public class SettlementsManager : MonoBehaviour
{
    public GameObject SettlementUI;
    public Player_M player;
    public GameObject playerAtSettlement;
    public List<SettlementDeployment> settlementsList;

    public List<SettlementRuntime> settlements;
    public TradeItemDisplay currentItemSelected;
    public SettlementRuntime currentSettlement;
    public TimeManager timeManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(timeManager!=null)
            timeManager.onDayChanged.AddListener(dailyEvents);
        initializeSettlements();

        calculateDailyPrices();
    }

    public void initializeSettlements()
    {
        foreach(var settl in settlementsList)
        {
            SettlementRuntime currentSettlement = new SettlementRuntime(settl.settlementSo);

            foreach (var res in settl.settlementSo.production)
            {
                currentSettlement.settlementStock.Add(new TradeGood(res.resource, res.startingStock, res.resource.baseValue));
            }

            foreach (var res in settl.settlementSo.consumption)
            {
                currentSettlement.settlementStock.Add(new TradeGood(res.resource, res.startingStock, res.resource.baseValue));
            }
            currentSettlement.calculatePriceTiers();

            currentSettlement.settlementGo = settl.settlmentGo;

            settlements.Add(currentSettlement);
        }
    }
    public void OnDestroy()
    {
        if(timeManager != null)
            timeManager.onDayChanged.RemoveListener(dailyEvents);
    }

    public void calculateDailyPrices()
    {
        foreach(var settlement in settlements)
        {
            foreach(var res in settlement.settlementStock)
            {                
                res.price = settlement.getPriceFor(res);
                //Debug.Log($"{settlement.settlementName}: {res.resource.itemName} with stock {res.amount} have price of {res.price} where base price is {res.resource.baseValue}");
            }
        }
    }

    public void updateCurrentSettlementPrices()
    {
        foreach (var res in currentSettlement.settlementStock)
        {            
            res.price = currentSettlement.getPriceFor(res);
            //Debug.Log($"Res {res.resource.itemName} new pices -> {res.price}");
        }
    }

    public void dailyEvents(DateTime t)
    {
        foreach (var settlement in settlements)
        {
            foreach(var good in settlement.defaultSettlementData.consumption)
            {
                settlement.decreaseStock(good.resource, (int)good.dailyChange);
            }
        }
    }

    public void playerAtSettlementGate(GameObject settlementGo)
    {
        playerAtSettlement = settlementGo;
    }

    public void playerLeftSettlement()
    {
        playerAtSettlement = null;
        currentSettlement = null;
    }

    public void EnterCity()
    {
        Debug.Log($"Player enering {playerAtSettlement}");

        foreach (var settlement in settlements)
        {
            if (settlement.settlementGo.name == playerAtSettlement.name)
            {
                Debug.Log($"Player entered in {settlement.settlementName}");
                currentSettlement = settlement;
                SettlementUI.GetComponent<CityUI>().prepareSettlement(settlement);
            }
        }

        SettlementUI.SetActive(true);
    }

    public void BuyOrSell()
    {
        Debug.Log("BUY SELL");
        
        //BUY
        if (currentItemSelected.tradeSettlementItem != null)
        {
            //Debug.Log($"SETTLEMENT IS SELLING {currentItemSelected.tradeSettlementItem.resource.itemName} with proce {currentItemSelected.tradeSettlementItem.price}");
            //Check player money & space in backpack
            if (player.invetory.CurrentCoins >= currentItemSelected.tradeSettlementItem.price
            && player.invetory.CanCarry(currentItemSelected.tradeSettlementItem.resource.volume, currentItemSelected.tradeSettlementItem.resource.weight))
            {
                player.invetory.tradeIn(currentItemSelected.tradeSettlementItem.resource, currentItemSelected.tradeSettlementItem.price);                
                currentSettlement.decreaseStock(currentItemSelected.tradeSettlementItem.resource);
                updateCurrentSettlementPrices();
                
                //Display
                SettlementUI.GetComponent<CityUI>().DisplayTrade();
                SettlementUI.GetComponent<CityUI>().DisplayPlayerCoins();
                return;
            }
        }

        //Sell
        if (currentItemSelected.tradeInventoryItem != null)
        {
            if (!player.invetory.hasResources(currentItemSelected.tradeInventoryItem.resourceSO))
                return;

            player.invetory.tradeOut(currentItemSelected.tradeInventoryItem.resourceSO, currentSettlement.getStockPrice(currentItemSelected.tradeInventoryItem.resourceSO));            
            currentSettlement.increaseStock(currentItemSelected.tradeInventoryItem.resourceSO);
            updateCurrentSettlementPrices();
            
            //Display
            SettlementUI.GetComponent<CityUI>().DisplayTrade();
            SettlementUI.GetComponent<CityUI>().DisplayPlayerCoins();
            return;
        }
    }
}