using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

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
    public CargoSO currentShopItemSelectedSO;

    public UpgradesSO currentUpgradeItem;
    public LicenceItemDisplay currentLicenceSelected;
    [Header("Cart Managment")]
    public ShopItemDisplay currentManagmentItem;
    public UpgradeItemDisplay currentManagmentUpgrade;
    public SettlementRuntime currentSettlement;


    //Managers
    //public TimeManager timeManager;
    //public LicenseManager licenseManager;   
    void Start()
    {
        GameServices.Instance.timeManager.onDayChanged.AddListener(dailyEvents);
        initializeSettlements();

        calculateDailyPrices();
    }

    public void initializeSettlements()
    {
        foreach(var settl in settlementsList)
        {
            SettlementRuntime currentSettlement = new SettlementRuntime(settl.settlementSo);
            currentSettlement.contractsPanel = settl.contractsPanel;

            foreach (var res in settl.settlementSo.production)
            {
                currentSettlement.settlementStock.Add(new TradeGood(res.resource, res.startingStock, res.resource.baseValue));
            }

            foreach (var res in settl.settlementSo.consumption)
            {
                currentSettlement.settlementStock.Add(new TradeGood(res.resource, res.startingStock, res.resource.baseValue));
            }

            foreach (var item in settl.settlementSo.availableUpgrades)
            {
                currentSettlement.availableUpgrades.Add(item);
            }

            foreach (var item in settl.settlementSo.itemsShopSO)
            {
                currentSettlement.availableCargoItems.Add(item);
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

    public string getSettlementWihtNeed(ResourceSO res)
    {
        List<string> settlmentsWithNeed = new();
        settlmentsWithNeed.Clear();

        foreach (var set in settlements)
        {
            if (set.needResource(res))
            {
                Debug.Log($"Res{res.itemName} is needed in {set.settlementName}");
                settlmentsWithNeed.Add(set.settlementName);
            }
        }

        if (settlmentsWithNeed.Count == 0)
            return "";

        return settlmentsWithNeed[UnityEngine.Random.Range(0, settlmentsWithNeed.Count)];
    }

    public string getSettlementWithProduction(ResourceSO res)
    {
        List<string> settlementsWithProduction = new();

        foreach (var set in settlements)
        {
            if (set.defaultSettlementData.production.Any(p => p.resource.itemName == res.itemName))
                settlementsWithProduction.Add(set.settlementName);
        }

        if (settlementsWithProduction.Count == 0)
            return "";

        return settlementsWithProduction[UnityEngine.Random.Range(0, settlementsWithProduction.Count)];
    }

    public float getDistance(string locationFrom, string locationTo)
    {
        if (locationFrom == locationTo)
            return 0f;

        Dictionary<string, float> distances = new();
        HashSet<string> visited = new();

        distances[locationFrom] = 0f;

        while (true)
        {
            string current = null;
            float minDistance = float.MaxValue;

            foreach (var entry in distances)
            {
                if (!visited.Contains(entry.Key) &&
                    entry.Value < minDistance)
                {
                    current = entry.Key;
                    minDistance = entry.Value;
                }
            }

            if (current == null)
                return -1f;

            if (current == locationTo)
                return minDistance;

            visited.Add(current);

            foreach (var road in GameServices.Instance.npcManager.roads)
            {
                string neighbour = null;

                if (road.startSettlement.settlementName == current)
                    neighbour = road.endSettlement.settlementName;
                else if (road.endSettlement.settlementName == current)
                    neighbour = road.startSettlement.settlementName;

                if (neighbour == null || visited.Contains(neighbour))
                    continue;

                float newDistance = minDistance + road.totalLength;

                if (!distances.ContainsKey(neighbour) ||
                    newDistance < distances[neighbour])
                {
                    distances[neighbour] = newDistance;
                }
            }
        }
    }

    public void refreshSettlementQuests()
    {
        //currentSettlement.contractsPanel
        //foreach (var settl in settlements)
        //{

        //    settl.contractsPanel = contractsPanel
        //}
    }
    public void OnDestroy()
    {
        GameServices.Instance.timeManager.onDayChanged.RemoveListener(dailyEvents);
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
        foreach (var settlement in settlements)
        {
            if (settlement.settlementGo.name == playerAtSettlement.name)
            {
                currentSettlement = settlement;
                SettlementUI.GetComponent<CityUI>().prepareSettlement(settlement);

                break;
            }
        }
        currentItemSelected = null;

        SettlementUI.SetActive(true);

        GameServices.Instance.soundManager.playSettlementSound();
    }

    public void leaveSettlement()
    {
        takeNotes(currentSettlement);
        GameServices.Instance.timeManager.resume();
        GameServices.Instance.soundManager.stopSettlementSound();
    }

    public void takeNotes(SettlementRuntime currentSettlementForNotes)
    {
        foreach(var tradeGood in currentSettlementForNotes.defaultSettlementData.production)
        {
            if (GameServices.Instance.licenseManager.CanTrade(tradeGood.resource, currentSettlement.defaultSettlementData))
            {
                //if can trade to be added here
                float price = currentSettlementForNotes.getPriceFor(tradeGood.resource);
                DateTime currentTimeForNote = GameServices.Instance.timeManager.currentTime;
                GameServices.Instance.player.notes.AddBuyNote(tradeGood.resource, price, currentSettlementForNotes.settlementName, currentTimeForNote);
            }
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
        Debug.Log("BUY OR SELL WITH PARAMS");
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
            
            
            GameServices.Instance.contractManager.buyResources(currentItemSelected.tradeSettlementItem.resource, amount, currentSettlement.defaultSettlementData);
            
            cityUI.DisplayTrade();            
            return;
        }

        //SELL
        if (currentItemSelected.tradeInventoryItem != null)
        {
            player.invetory.tradeOut(currentItemSelected.tradeInventoryItem.resourceSO, transactionTotalPrice/amount, amount);
            currentSettlement.increaseStock(currentItemSelected.tradeInventoryItem.resourceSO, cityUI.ReturnCurentSliderValue());
            updateCurrentSettlementPrices();
            GameServices.Instance.contractManager.sellResources(currentItemSelected.tradeInventoryItem.resourceSO, amount, currentSettlement.defaultSettlementData);

            if(currentSettlement.defaultSettlementData.settlementNeedsResource(currentItemSelected.tradeInventoryItem.resourceSO))
            {
                currentSettlement.reputation += GlobalSettingsManager.REPUTATION_GAIN_PER_SALE * amount;
            }

            cityUI.DisplayTrade();
            return;
        }
    }

    public void BuyOrSell()
    {
        if (currentItemSelected == null)
            return;

        if(currentItemSelected.isLocked == false)
        {
            //BUY
            if (currentItemSelected.tradeSettlementItem != null)
            {
                if (currentItemSelected.tradeSettlementItem.amount == 0)
                    return;
                //Debug.Log($"SETTLEMENT IS SELLING {currentItemSelected.tradeSettlementItem.resource.itemName} with prce {currentItemSelected.tradeSettlementItem.price}");

                //Check player money & space in backpack

                if (player.invetory.CurrentCoins >= currentItemSelected.tradeSettlementItem.price * cityUI.ReturnCurentSliderValue()
                && player.invetory.CanCarry(currentItemSelected.tradeSettlementItem.resource.volume * cityUI.ReturnCurentSliderValue(),
                currentItemSelected.tradeSettlementItem.resource.weight * cityUI.ReturnCurentSliderValue()))

                {
                    player.invetory.tradeIn(currentItemSelected.tradeSettlementItem.resource, currentItemSelected.tradeSettlementItem.price, cityUI.ReturnCurentSliderValue());
                    currentSettlement.decreaseStock(currentItemSelected.tradeSettlementItem.resource, cityUI.ReturnCurentSliderValue());
                    GameServices.Instance.contractManager.buyResources(currentItemSelected.tradeSettlementItem.resource, cityUI.ReturnCurentSliderValue(), currentSettlement.defaultSettlementData);
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
                if (currentItemSelected.tradeInventoryItem.amount == 0)
                    return;

                if (!player.invetory.hasResources(currentItemSelected.tradeInventoryItem.resourceSO))
                    return;

                player.invetory.tradeOut(currentItemSelected.tradeInventoryItem.resourceSO, currentSettlement.getStockPrice(currentItemSelected.tradeInventoryItem.resourceSO), cityUI.ReturnCurentSliderValue());
                currentSettlement.increaseStock(currentItemSelected.tradeInventoryItem.resourceSO, cityUI.ReturnCurentSliderValue());
                GameServices.Instance.contractManager.sellResources(currentItemSelected.tradeInventoryItem.resourceSO, cityUI.ReturnCurentSliderValue(), currentSettlement.defaultSettlementData);
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
        if(currentShopItemSelectedSO == null)
        {
            Debug.Log("No item selected");
            return;
        }

        if (GameServices.Instance.cargoManager.CanBuy(currentShopItemSelectedSO,player.invetory.CurrentCoins))
        {
            GameServices.Instance.cargoManager.AddCargoItem(currentShopItemSelectedSO,ref player.invetory.CurrentCoins);
        }else
        {
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

        if (currentUpgradeItem.price > player.invetory.CurrentCoins)
        {
            return;
            //jucatorul nu are bani - Feedback on that
            
        }
        else
        {
            player.invetory.CurrentCoins -= currentUpgradeItem.price;
            GameServices.Instance.cargoManager.playerOwnedUpgrades.Add(currentUpgradeItem);
        }
       

        //Remove from setlment        
        //CargoManager.instance.RemoveCargoItemFromShop(currentUpgradeItem,ref currentSettlement.defaultSettlementData.upgradeShop);
        //CargoManager.instance.RemoveCargoItemFromShop(currentUpgradeItem, ref currentSettlement.settlmentUpgradeItems);
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

        GameServices.Instance.licenseManager.PuchaseLicenses(currentLicenceSelected.licenseSO, ref player.invetory.CurrentCoins);

        //Reset Panel
        cityUI.DisplayTradeGuildPanel();       
          
    }

    //Equip upgrade on item
    public void EquipUpgrade()
    {
        if(currentManagmentUpgrade == null || currentManagmentItem == null)
        {
            return;
        }
        ShopItemDisplay sameItem = currentManagmentItem;
        
            //Add it to current item selected
            currentManagmentItem.cargo.Upgrades.Add(currentManagmentUpgrade.upgrade.cargoSO);
            currentManagmentItem.cargo.CalculateTotalWeightAndVolume();
            //Remove upgrade from player cargo  
            //CargoManager.instance.RemoveCargoItemFromInvetory(currentManagmentUpgrade.upgrade);
            
            //Add it to player stats
            //player.stats.GetItemValues(player.attachments.currentItemSelected);

        Destroy(currentManagmentUpgrade.gameObject);
        //Visual 
//        cityUI.SetCurrentCartManagmentItem(sameItem);
            //cityUI.LoadItemUpgrades(currentManagmentItem);
            cityUI.DisplayCartManagmentPanel();





    }
    
    public void addReputation(SettlementSO settlement, float amount)
    {
        foreach (var settl in settlements)
        {
            if (settl.defaultSettlementData.settlementName == settlement.settlementName)
            {
                settl.reputation += amount;
                Debug.Log($"Reputation for {settl.settlementName} is now {settl.reputation}");
            }
        }
    }

}