
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEditor.Progress;

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
    public List<Cargo> settlementShopItems;
    public List<Cargo> settlmentUpgradeItems;
    public List<LicensesSO> settlmentLicenses;
    public GameObject settlementGo;

    public SettlementRuntime(SettlementSO so)
    {
        defaultSettlementData = so;
        this.settlementName = defaultSettlementData.settlementName;
        settlementShopItems = new List<Cargo>();
        settlmentUpgradeItems = new List<Cargo>();
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
            if(trade.resource.itemName == res.resource.itemName)
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

        foreach(var trade in settlementStock)
        {
            if (trade.resource.itemName == res.itemName)
                return trade.price;
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

    public void decreaseStock(ResourceSO res, int amount )
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
    public CityUI cityUI;
    public Player_M player;
    public GameObject playerAtSettlement;
    public List<SettlementDeployment> settlementsList;

    public List<SettlementRuntime> settlements;
    
    //Current Cargo selected
    public TradeItemDisplay currentItemSelected;
    public ShopItemDisplay currentShopItemSelected;
    public UpgradeItemDisplay currentUpgradeItem;
    public LicenceItemDisplay currentLicenceSelected;
   
    public SettlementRuntime currentSettlement;

    //Managers
    public TimeManager timeManager;
    public LicenseManager licenseManager;   
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
            foreach(var item in settl.settlementSo.itemsShop)
            {
                currentSettlement.settlementShopItems.Add(new Cargo(item.cargoSO,item.upgradesSpaces,item.Upgrades,item.category,item.type));
            }
            foreach(var upgradeItem in settl.settlementSo.upgradeShop)
            {
                currentSettlement.settlmentUpgradeItems.Add(new Cargo(upgradeItem.cargoSO, upgradeItem.upgradesSpaces, upgradeItem.Upgrades,upgradeItem.category,upgradeItem.type));
            }
            foreach(var l in settl.settlementSo.Licenses_Shop)
            {
                currentSettlement.settlmentLicenses.Add(l);
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
            foreach (var good in settlement.defaultSettlementData.production)
            {
                settlement.increaseStock(good.resource, (int)good.dailyChange);
            }

            foreach (var good in settlement.defaultSettlementData.consumption)
            {
                settlement.decreaseStock(good.resource, (int)good.dailyChange);
            }
        }

        calculateDailyPrices();
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

                takeNotes(settlement);
                break;
            }
        }

        SettlementUI.SetActive(true);
    }

    public void takeNotes(SettlementRuntime currentSettlementForNotes)
    {
        foreach(var tradeGood in currentSettlementForNotes.defaultSettlementData.production)
        {
            //if can trade to be added here
            float price = currentSettlementForNotes.getPriceFor(tradeGood.resource);
            DateTime currentTimeForNote = GameServices.Instance.timeManager.currentTime;
            GameServices.Instance.player.notes.AddBuyNote(tradeGood.resource, price, currentSettlementForNotes.settlementName, currentTimeForNote);
        }

        foreach (var tradeGood in currentSettlementForNotes.defaultSettlementData.consumption)
        {
            //if can trade to be added here
            float price = currentSettlementForNotes.getPriceFor(tradeGood.resource);
            DateTime currentTimeForNote = GameServices.Instance.timeManager.currentTime;
            GameServices.Instance.player.notes.AddSellNote(tradeGood.resource, price, currentSettlementForNotes.settlementName, currentTimeForNote);
        }
    }

    public void BuyOrSell(float transactionTotalPrice, int amount)
    {
        if (currentItemSelected.isLocked)
            return;

        //BUY
        if (currentItemSelected.tradeSettlementItem != null)
        {
            if (transactionTotalPrice > GameServices.Instance.player.invetory.CurrentCoins)
                return;

            GameServices.Instance.player.invetory.tradeIn(currentItemSelected.tradeSettlementItem.resource, transactionTotalPrice/amount,amount);
            currentSettlement.decreaseStock(currentItemSelected.tradeSettlementItem.resource, amount);
            updateCurrentSettlementPrices();

            cityUI.DisplayTrade();
            return;
        }

        //SELL
        if (currentItemSelected.tradeInventoryItem != null)
        {
            player.invetory.tradeOut(currentItemSelected.tradeInventoryItem.resourceSO, transactionTotalPrice/amount, amount);
            currentSettlement.increaseStock(currentItemSelected.tradeInventoryItem.resourceSO, cityUI.ReturnCurentSliderValue());
            updateCurrentSettlementPrices();

            cityUI.DisplayTrade();
        }
    }
    public void BuyOrSell()
    {   
        if(currentItemSelected.isLocked == false)
        {
            //BUY
            if (currentItemSelected.tradeSettlementItem != null)
            {

                //Debug.Log($"SETTLEMENT IS SELLING {currentItemSelected.tradeSettlementItem.resource.itemName} with prce {currentItemSelected.tradeSettlementItem.price}");

                //Check player money & space in backpack

                if (player.invetory.CurrentCoins >= currentItemSelected.tradeSettlementItem.price * cityUI.ReturnCurentSliderValue()
                && player.invetory.CanCarry(currentItemSelected.tradeSettlementItem.resource.volume * cityUI.ReturnCurentSliderValue(),
                currentItemSelected.tradeSettlementItem.resource.weight * cityUI.ReturnCurentSliderValue()))

                {
                    player.invetory.tradeIn(currentItemSelected.tradeSettlementItem.resource, currentItemSelected.tradeSettlementItem.price, cityUI.ReturnCurentSliderValue());
                    currentSettlement.decreaseStock(currentItemSelected.tradeSettlementItem.resource, cityUI.ReturnCurentSliderValue());
                    updateCurrentSettlementPrices();

                    //Slider
                    cityUI.SliderMaxValue();

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

                player.invetory.tradeOut(currentItemSelected.tradeInventoryItem.resourceSO, currentSettlement.getStockPrice(currentItemSelected.tradeInventoryItem.resourceSO), cityUI.ReturnCurentSliderValue());
                currentSettlement.increaseStock(currentItemSelected.tradeInventoryItem.resourceSO, cityUI.ReturnCurentSliderValue());
                updateCurrentSettlementPrices();

                //Slider
                cityUI.SliderMaxValue();
                //Display
                SettlementUI.GetComponent<CityUI>().DisplayTrade();
                SettlementUI.GetComponent<CityUI>().DisplayPlayerCoins();
                return;
            }
        }
        
    }

  

    //Shop buy item
    public void BuyShopItem()
    {
        if(currentShopItemSelected == null)
        {
            return;
        }

        if (CargoManager.instance.CanBuy(currentShopItemSelected.cargo,player.invetory.CurrentCoins))
        {
            CargoManager.instance.AddCargoItem(currentShopItemSelected.cargo,ref player.invetory.CurrentCoins);
        }else
        {
            //Already owned
            return;
        }

        //Refresh Page After buy
        cityUI.DisplayShop();
       
        
    }

    //Buy Upgrade item

    public void BuyUpgradeItem()
    {
        if(currentUpgradeItem == null )
        {
            return;
        }

        CargoManager.instance.AddUpgradeItem(currentUpgradeItem.upgrade,ref player.invetory.CurrentCoins);

        //Remove from setlment
        

        CargoManager.instance.RemoveCargoItemFromShop(currentUpgradeItem.upgrade,ref currentSettlement.defaultSettlementData.upgradeShop);
        CargoManager.instance.RemoveCargoItemFromShop(currentUpgradeItem.upgrade, ref currentSettlement.settlmentUpgradeItems);
        currentUpgradeItem = null;

        //RefreshPage
        cityUI.DisplayUpgradePanel();
        
        
    }
    //Buy License
    public void BuyLicence()
    {
        if(currentLicenceSelected == null)
        {
            return;
        }

        licenseManager.PuchaseLicenses(currentLicenceSelected.licenseSO, ref player.invetory.CurrentCoins);

        //Reset Panel
        cityUI.DisplayTradeGuildPanel();       
          
    }

    
}