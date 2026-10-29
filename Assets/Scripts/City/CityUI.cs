using JetBrains.Annotations;
using Mono.Cecil;
using NUnit.Framework;
using System;
using System.ComponentModel;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum CityUIPage
{
    Lobby=0,
    Shop=1,
    Trade=2,
    UpgradeShop = 3,
    TradeGuild=4,
    CartManagment=5,
    Rest =6
}

[System.Serializable]
public class CityUI : MonoBehaviour
{
    public SettlementsManager settlementsManager;
    public SettlementRuntime currentSettlement;
    public CityUIPage currentpage;
    [Header("Lobby Panel")]
    
    [SerializeField] private VerticalInvetoryItem itemPrefab;
    [SerializeField] private Transform Content;
    [SerializeField] private TMP_Text settlementName;

    [Header("Trade Panel")]
    [SerializeField] private TradeItemDisplay tradePrfab;
    [SerializeField] private Transform buyContent;

    [SerializeField] private TradeItemDisplay tradePlayerPrefab;
    [SerializeField] private Transform SellContent;

    [Header("Player Components")]    
    public PlayerInventory_M playerInventory;
    public Player_M player;
    [SerializeField] private TextMeshProUGUI playerCoinsDisplay;


    [Header("Current TRADE_ITEM_DISPLAY")]    
    [SerializeField] private Image currentTradeItemImage;
    [SerializeField] private TextMeshProUGUI currentTradeItemName;
    [SerializeField] private TextMeshProUGUI currentTradeItemPrice;
    [SerializeField] private TextMeshProUGUI buySellButtonText;
    [SerializeField] private TextMeshProUGUI currentItemAmount;
    public GameObject CurrentLockedPanel;
    private SettlementGood selectedSettlementGood;

    //Price Tier Display
    [Header("Current Item Price Tier")]

    [SerializeField] private TextMeshProUGUI TierVAmoun;
    [SerializeField] private TextMeshProUGUI TierIVAmount;
    [SerializeField] private TextMeshProUGUI TierIIIAmount;
    [SerializeField] private TextMeshProUGUI TierIIAmount;
    [SerializeField] private TextMeshProUGUI TierIAmount;
    [SerializeField] private TextMeshProUGUI TierVPrice;
    [SerializeField] private TextMeshProUGUI TierIVPrice;
    [SerializeField] private TextMeshProUGUI TierIIIPrice;
    [SerializeField] private TextMeshProUGUI TierIIPrice;
    [SerializeField] private TextMeshProUGUI TierIPrice;
    public TradeTierUIIndicator tierIndicator;


    [Header("SLIDER_TRADE")]
    public Slider sliderAmount;
    [SerializeField] private TextMeshProUGUI currentTextAmount;
    //weight display
    public Image weight_Invetory_Slider;
    public Image weight_Trade_Slider;
    //volume Display
    public Image volume_Invetory_Slider;
    public Image volume_Trade_Slider;

    [Header("SHOP PAGE")]

    [SerializeField] private Transform ShopContent;
    [SerializeField] private ShopItemDisplay ItemShopPrefab;

    [Header("Current SHOP_ITEM_DISPLAY")]
    [SerializeField] private Image currentShopItemIcon;
    [SerializeField] private TextMeshProUGUI currentShopItemName;
    [SerializeField] private TextMeshProUGUI currentShopItemPrice;
    [SerializeField] private TextMeshProUGUI currentShopItemDescription;
    [SerializeField] private TextMeshProUGUI buyShopButtonText;
    [SerializeField] private BulletLIstCreator bulletList;
    

    [Header("UPGRADE PAGE")]
    [SerializeField] private Transform UpgradeContent;
    [SerializeField] UpgradeItemDisplay upgradeItemDisplayPrefab;

    [Header("Current UPGRADE_SHOP_ITEM_DISPLAY")]
    [SerializeField] private Image currentUpgradeItemIcon;
    [SerializeField] private TextMeshProUGUI currentUpgradeItemName;
    [SerializeField] private TextMeshProUGUI upgradeItemPrice;
    [SerializeField] private TextMeshProUGUI upgradeItemDescription;

    [Header("TRADE GUILD PAGE")]
    [SerializeField] private Transform TradeGuildContent;
    [SerializeField] private LicenceItemDisplay LicensesDisplayPrefab;

    [Header("Current LICENSE DISPLAY")]
    [SerializeField] private Image LicenseIcon;
    [SerializeField] private TextMeshProUGUI LicenseName;
    [SerializeField] private TextMeshProUGUI LicensePrice;
    [SerializeField] private Transform LicenseUnlockList;
    [SerializeField] private LicenceUnlock unlockItem;
    [SerializeField] private TextMeshProUGUI notEnoughMoneyText;

    
    [Header("Cart Managment Display")]
    [SerializeField] private Transform managmentContent;
    [SerializeField] private UpgradeItemDisplay upgradeDisplayPrefab;

     private int pageIndex=1;

    [Header("Current Item_Cart_Managment")]
    [SerializeField] private Image currentManagmentItemIcon;
    [SerializeField] private TextMeshProUGUI currentManagmentItemName;

    [Header("Current Upgrade_Cart_Managment")]
    [SerializeField] private Image currentManagmentUpgradeIcon;
    [SerializeField] private TextMeshProUGUI currentManagmentUpgradeName;
    [SerializeField] private TextMeshProUGUI currentManagmentUpgradeBonus;

    [Header("PAGES")]
    [SerializeField] private GameObject LobbyPanel;
    [SerializeField] private GameObject ShopPanel;
    [SerializeField] private GameObject TradePanel;
    [SerializeField] private GameObject UpgradePanel;

    [SerializeField] private GameObject GuildPanel;
    [SerializeField] private GameObject CartManagmentPanel;

    
    private void OnEnable()
    {
        
        ShowCurrentPage(currentpage);
    }

    private void Update()
    {
        //DisplayPlayerCoins();
        //ShowTotalPrice();
        currentTextAmount.text = sliderAmount.value.ToString();

        SetPlayerWeightVolumeTrade();
        
    }

    private void ShowCurrentPage(CityUIPage page)
    {
        currentpage = page;

        LobbyPanel.SetActive(page == CityUIPage.Lobby);
        ShopPanel.SetActive(page == CityUIPage.Shop);
        TradePanel.SetActive(page == CityUIPage.Trade);
        UpgradePanel.SetActive(page == CityUIPage.UpgradeShop);
        GuildPanel.SetActive(page == CityUIPage.TradeGuild);
        CartManagmentPanel.SetActive(page == CityUIPage.CartManagment);

        switch (currentpage)
        {
            case CityUIPage.Lobby:
                DisplayInvetory();
               
                break;
            case CityUIPage.Shop:
                DisplayShop();
                break;
            case CityUIPage.Trade:
                DisplayTrade();
                break;
            case CityUIPage.UpgradeShop:
                DisplayUpgradePanel();
                break;
            case CityUIPage.TradeGuild:
                DisplayTradeGuildPanel();
                break;
            case CityUIPage.CartManagment:
                DisplayCartManagmentPanel();
                break;
            case CityUIPage.Rest:

                break;
            default:
                break;
        }
    }


    //############
    //LOBBY AREA
    //############

    private void DisplayInvetory()
    {
        settlementName.text = currentSettlement.settlementName;
        //Delete List
        foreach (Transform child in Content)
        {
            Destroy(child.gameObject);
        }


        //Create List   
        foreach (InventoryItem item in playerInventory.Inventory)
        {
            VerticalInvetoryItem row = Instantiate(itemPrefab, Content);
            row.CreateIcon(item);
        }
    }


    //############
    //TRADE AREA
    //############

    public void DisplayTrade()
    {
        //Populate Player Money
        DisplayPlayerCoins();

        foreach (Transform child in buyContent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in SellContent)
        {
            Destroy(child.gameObject);
        }
        
        //Create list
        foreach (var item in currentSettlement.settlementStock)
        {
            TradeItemDisplay row;

            if (LicenseManager.instance.CanTrade(item.resource,currentSettlement.defaultSettlementData))
            {
                 row = Instantiate(tradePrfab, buyContent);
                row.PopulateBuyIcon(item, this,false);
            }
            else
            {
                row = Instantiate(tradePrfab, buyContent);
                row.PopulateBuyIcon(item, this, true);
            } 
        }

        foreach (InventoryItem item in playerInventory.Inventory)
        {
            TradeItemDisplay row = Instantiate(tradePlayerPrefab, SellContent);

            row.PopulateSellIcon(item, this);
        }

    }

    public void DisplayPlayerCoins()
    {
        playerCoinsDisplay.text= $": {playerInventory.CurrentCoins:F1}";
    }

    public void SetCurrentTradeItem(TradeItemDisplay item)
    {
        selection_totalPrice = 0f;

        settlementsManager.currentItemSelected = item;
        currentTradeItemImage.sprite = item.itemIcon.sprite;
        currentTradeItemName.text = item.itemNameText.text;

        //Tiers
        GetCurrentItemTiers();
        tierIndicator.clearIndicator();

        //Buttons
        SetButtonText(item);
        SliderMaxValue();
    }

    public void GetCurrentItemTiers()
    {
        TradeItemDisplay currentItem = settlementsManager.currentItemSelected;
        SettlementSO currentSettlement = settlementsManager.currentSettlement.defaultSettlementData;
        ResourceSO resToShowTier = currentItem.tradeSettlementItem == null ? currentItem.tradeInventoryItem.resourceSO : currentItem.tradeSettlementItem.resource;
        if (currentItem == null)
        {
            return;
        }

        foreach(var production in currentSettlement.production)
        {
            if(resToShowTier == production.resource)
            {
                selectedSettlementGood = production;
                SetTierList(production);
                return;
            }
        }

        foreach (var consumption in currentSettlement.consumption)
        {
            if (resToShowTier == consumption.resource)
            {
                selectedSettlementGood = consumption;
                SetTierList(consumption);
                return;
            }
        }
    }

    private void SetTierList(SettlementGood settlemeGood)
    {
        TierIAmount.text = $"Stock: below {settlemeGood.tier1_price_stock}";
        TierIPrice.text = $"{(settlemeGood.resource.baseValue * GlobalSettingsManager.TIER_1_PRICE_MULTIPLIER):F1}";

        TierIIAmount.text = $"Stock: {settlemeGood.tier1_price_stock}–{settlemeGood.tier2_price_stock}";
        TierIIPrice.text = $"{(settlemeGood.resource.baseValue * GlobalSettingsManager.TIER_2_PRICE_MULTIPLIER):F1}";

        TierIIIAmount.text = $"Stock: {settlemeGood.tier2_price_stock}–{settlemeGood.tier3_price_stock}";
        TierIIIPrice.text = $"{(settlemeGood.resource.baseValue * GlobalSettingsManager.TIER_3_PRICE_MULTIPLIER):F1}";

        TierIVAmount.text = $"Stock: {settlemeGood.tier3_price_stock}–{settlemeGood.tier4_price_stock}";
        TierIVPrice.text = $"{(settlemeGood.resource.baseValue * GlobalSettingsManager.TIER_4_PRICE_MULTIPLIER):F1}";

        TierVAmoun.text = $"Stock: above {settlemeGood.tier4_price_stock}";
        TierVPrice.text = $"{(settlemeGood.resource.baseValue * GlobalSettingsManager.TIER_5_PRICE_MULTIPLIER):F1}";
    }

    //Slider
    public void SliderMaxValue()
    {
        if (settlementsManager.currentItemSelected.tradeSettlementItem != null)
        {
            float maxSetlementAmount = settlementsManager.currentItemSelected.tradeSettlementItem.amount;
            float maxAmountBasedOnWeight = (GameServices.Instance.player.stats.maxWeight - GameServices.Instance.player.invetory.currentWeight) / settlementsManager.currentItemSelected.tradeSettlementItem.resource.weight;
            float maxAmountBasedOnVolume = (GameServices.Instance.player.stats.maxVolume - GameServices.Instance.player.invetory.currentVolume) / settlementsManager.currentItemSelected.tradeSettlementItem.resource.volume;
            //float maxAmountBasedOnCoins = GameServices.Instance.player.invetory.CurrentCoins / settlementsManager.currentItemSelected.tradeSettlementItem.resource.baseValue;
            
            sliderAmount.maxValue = Mathf.Max(0, Mathf.FloorToInt(Mathf.Min(maxSetlementAmount,maxAmountBasedOnWeight,maxAmountBasedOnVolume)));
        }
        if (settlementsManager.currentItemSelected.tradeInventoryItem != null)
        {
            sliderAmount.maxValue = settlementsManager.currentItemSelected.tradeInventoryItem.amount;
        }
    }

    //Weight & Volume
    private void SetPlayerWeightVolumeTrade() 
    {
        TradeItemDisplay currentItem = settlementsManager.currentItemSelected;
        //already in invetory
        weight_Invetory_Slider.fillAmount = playerInventory.currentWeight / player.stats.maxWeight;
        volume_Invetory_Slider.fillAmount = playerInventory.currentVolume / player.stats.maxVolume;

        if(settlementsManager.currentItemSelected == null)
        {
         
            return;
        }

        //Adding from shop
        //shop production
        if(settlementsManager.currentItemSelected.tradeSettlementItem != null)
        {
            //Trade Slider
            float weightBuy = playerInventory.currentWeight + currentItem.tradeSettlementItem.resource.weight * ReturnCurentSliderValue();
            float volumeBuy = playerInventory.currentVolume + currentItem.tradeSettlementItem.resource.volume * ReturnCurentSliderValue();

            weight_Trade_Slider.fillAmount = weightBuy / player.stats.maxWeight;
            volume_Trade_Slider.fillAmount = volumeBuy / player.stats.maxVolume;

            
        }
        if(settlementsManager.currentItemSelected.tradeInventoryItem != null)
        {
            weight_Trade_Slider.gameObject.SetActive(false);
            volume_Trade_Slider.gameObject.SetActive(false);

            float weightAfterSale = playerInventory.currentWeight - currentItem.tradeInventoryItem.resourceSO.weight* ReturnCurentSliderValue();
            float volumeAfterSale = playerInventory.currentVolume - currentItem.tradeInventoryItem.resourceSO.volume * ReturnCurentSliderValue();

            //Sell Slider
            weight_Invetory_Slider.fillAmount = weightAfterSale / player.stats.maxWeight ;
            volume_Invetory_Slider.fillAmount = volumeAfterSale / player.stats.maxVolume ;
        
        }
    }

    public void BuyOrSell()
    {
        if (selection_totalPrice == 0 || Mathf.RoundToInt(ReturnCurentSliderValue()) == 0)
            return;

        GameServices.Instance.settlementManager.BuyOrSell(selection_totalPrice, Mathf.RoundToInt(ReturnCurentSliderValue()));
    }

    public int ReturnCurentSliderValue()
    {
        return Mathf.RoundToInt(sliderAmount.value);
    }

    //Total Price
    private float selection_totalPrice;
    public void ShowTotalPrice()
    {
        if (settlementsManager.currentItemSelected == null)
        {
            currentTradeItemPrice.text = "0";
            return;
        }

        int amount = Mathf.RoundToInt(ReturnCurentSliderValue());
        float totalPrice = 0f;

        // BUY
        if (settlementsManager.currentItemSelected.tradeSettlementItem != null)
        {
            float currentStock = settlementsManager.currentSettlement.getStock(settlementsManager.currentItemSelected.tradeSettlementItem.resource);

            for (int i = 0; i < amount; i++)
            {
                float newStock = currentStock - i - 1;
                float currentPrice = 0f;

                if (newStock < selectedSettlementGood.tier1_price_stock)
                {
                    currentPrice = selectedSettlementGood.resource.baseValue * GlobalSettingsManager.TIER_1_PRICE_MULTIPLIER;
                    tierIndicator.indicateTier(1);
                }
                else if (newStock < selectedSettlementGood.tier2_price_stock)
                {
                    currentPrice = selectedSettlementGood.resource.baseValue * GlobalSettingsManager.TIER_2_PRICE_MULTIPLIER;
                    tierIndicator.indicateTier(2);
                }
                else if (newStock < selectedSettlementGood.tier3_price_stock)
                {
                    currentPrice = selectedSettlementGood.resource.baseValue * GlobalSettingsManager.TIER_3_PRICE_MULTIPLIER;
                    tierIndicator.indicateTier(3);
                }
                else if (newStock < selectedSettlementGood.tier4_price_stock)
                {
                    currentPrice = selectedSettlementGood.resource.baseValue * GlobalSettingsManager.TIER_4_PRICE_MULTIPLIER;
                    tierIndicator.indicateTier(4);
                }
                else
                {
                    currentPrice = selectedSettlementGood.resource.baseValue * GlobalSettingsManager.TIER_5_PRICE_MULTIPLIER;
                    tierIndicator.indicateTier(5);
                }

                totalPrice += currentPrice;
                currentTradeItemPrice.color = totalPrice > GameServices.Instance.player.invetory.CurrentCoins ? new Color32(220, 80, 80, 255) : Color.white;
            }
        }

        // SELL
        else if (settlementsManager.currentItemSelected.tradeInventoryItem != null)
        {
            float currentStock = settlementsManager.currentSettlement.getStock(settlementsManager.currentItemSelected.tradeInventoryItem.resourceSO);

            for (int i = 0; i < amount; i++)
            {
                float newStock = currentStock + i;
                float currentPrice = 0f;

                if (newStock < selectedSettlementGood.tier1_price_stock)
                {
                    currentPrice = selectedSettlementGood.resource.baseValue * GlobalSettingsManager.TIER_1_PRICE_MULTIPLIER;
                    tierIndicator.indicateTier(1);
                }
                else if (newStock < selectedSettlementGood.tier2_price_stock)
                {
                    currentPrice = selectedSettlementGood.resource.baseValue * GlobalSettingsManager.TIER_2_PRICE_MULTIPLIER;
                    tierIndicator.indicateTier(2);
                }
                else if (newStock < selectedSettlementGood.tier3_price_stock)
                {
                    currentPrice = selectedSettlementGood.resource.baseValue * GlobalSettingsManager.TIER_3_PRICE_MULTIPLIER;
                    tierIndicator.indicateTier(3);
                }
                else if (newStock < selectedSettlementGood.tier4_price_stock)
                {
                    currentPrice = selectedSettlementGood.resource.baseValue * GlobalSettingsManager.TIER_4_PRICE_MULTIPLIER;
                    tierIndicator.indicateTier(4);
                }
                else
                {
                    currentPrice = selectedSettlementGood.resource.baseValue * GlobalSettingsManager.TIER_5_PRICE_MULTIPLIER;
                    tierIndicator.indicateTier(5);
                }
                currentTradeItemPrice.color = Color.white;
                totalPrice += currentPrice;
            }
        }
        
        selection_totalPrice = totalPrice;

        currentTradeItemPrice.text = $"{totalPrice:F1}";
    }

    //############
    //SHOP AREA
    //############

    public void DisplayShop()
    {
        foreach(Transform child in ShopContent)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in currentSettlement.settlementShopItems)
        {
            ShopItemDisplay row = Instantiate(ItemShopPrefab, ShopContent);
            row.PopulateIcons(item, this);
        }

        
    }
    public void SetCurrentShopItem(ShopItemDisplay shopItem)
    {
        settlementsManager.currentShopItemSelected = shopItem;
        //Visual
        currentShopItemIcon.sprite = shopItem.IconImage.sprite;
        currentShopItemPrice.text = shopItem.cargo.cargoSO.price.ToString();
        currentShopItemName.text = shopItem.cargo.cargoSO.itemName.ToString();
        currentShopItemDescription.text = shopItem.cargo.cargoSO.Description;

        //Button
        SetButtonTextShop(shopItem.cargo);
        bulletList.CreateBulletList(settlementsManager.currentShopItemSelected.cargo.cargoSO.features);
    }


    //############
    //UPGRADE AREA
    //############

    public void DisplayUpgradePanel()
    {
        foreach(Transform child in UpgradeContent)
        {
            Destroy (child.gameObject);
        }

        foreach (var item in currentSettlement.settlmentUpgradeItems)
        {
            UpgradeItemDisplay row = Instantiate(upgradeItemDisplayPrefab, UpgradeContent);
            row.PopulateUpgrade(item, this);
            settlementsManager.currentUpgradeItem =row ;
        }
    }

    public void SetCurrentUpgradeItem(UpgradeItemDisplay item)
    {
        settlementsManager.currentUpgradeItem= item;

        currentUpgradeItemIcon = item.iconImage;
        currentUpgradeItemName.text = item.upgrade.cargoSO.itemName.ToString();
        upgradeItemPrice.text = item.upgrade.cargoSO.price.ToString();
        
        
    }


    //############
    //TRADE GUILD
    //############

    public void DisplayTradeGuildPanel()
    {
        foreach (Transform child in TradeGuildContent)
        {
            Destroy(child.gameObject);
        }
        foreach (var license in currentSettlement.settlmentLicenses)
        {

            if (!LicenseManager.instance.AlreadyOwnsLicense(license))
            {
                LicenceItemDisplay row = Instantiate(LicensesDisplayPrefab, TradeGuildContent);
                row.PopulateLicense(license, this);
            }
                
        }
       
    }

    public void SetCurrentLicence(LicenceItemDisplay licenceItemDisplay)
    {
        settlementsManager.currentLicenceSelected= licenceItemDisplay;

        LicenseIcon.sprite = licenceItemDisplay.icon.sprite;
        LicenseName.text = licenceItemDisplay.licencename.text;
        LicensePrice.text = licenceItemDisplay.price.text;
        NotEnoughMoneyDisplay(LicenseManager.instance.CanBuy(settlementsManager.currentLicenceSelected.licenseSO, playerInventory.CurrentCoins));

        foreach(Transform child in LicenseUnlockList)
        {
            Destroy(child.gameObject);
        }

        foreach(var item in licenceItemDisplay.licenseSO.resourse_unlock)
        {
            LicenceUnlock row = Instantiate(unlockItem, LicenseUnlockList);
            row.Create(item);
        }
    }

    public void NotEnoughMoneyDisplay(bool verify)
    {
        if (!verify)
        {
            notEnoughMoneyText.text = "NOT ENOUGH COINS";

        }
        else
        {
            notEnoughMoneyText.text = "";
        }
    }
    

    //############
    //CART MANAGMENT
    //############
    public void DisplayCartManagmentPanel()
    {
        

        //Delete extinting items 
        
        if(pageIndex == 1)
        {
            foreach (Transform child in managmentContent)
            {
                Destroy(child.gameObject);
            }
            //Display invetory items

            foreach (var item in CargoManager.instance.PlayerOwnedCargo)
            {
                ShopItemDisplay row = Instantiate(ItemShopPrefab, managmentContent);
                row.PopulateIcons(item, this);
            }
        }
        else if (pageIndex == 2)  
        {
            foreach (Transform child in managmentContent)
            {
                Destroy(child.gameObject);
            }
            //Display invetory upgrades
            foreach (var item in CargoManager.instance.PlayerOwnedUpgrades)
            {
                UpgradeItemDisplay row = Instantiate(upgradeDisplayPrefab, managmentContent);
                row.PopulateUpgrade(item, this);
            }
        }
    }
    public void SetCurrentCartManagmentItem(ShopItemDisplay shopItem)
    {
        currentManagmentItemIcon.sprite = shopItem.IconImage.sprite;
        currentManagmentItemName.text = shopItem.cargo.cargoSO.itemName;
        int i = shopItem.cargo.upgradesSpaces;
        for (i;)
    } 

    public void SetCurrentCartManagemtnUpgrade(UpgradeItemDisplay upgradeItem)
    {
       
        currentManagmentUpgradeIcon.sprite = upgradeItem.iconImage.sprite;
        currentManagmentUpgradeName.text = upgradeItem.upgrade.cargoSO.itemName;

        currentManagmentUpgradeBonus.text = "";
            if (upgradeItem.upgrade.cargoSO.itemMaxVolume != 0)
            {
                currentManagmentUpgradeBonus.text = "Volume : + " + upgradeItem.upgrade.cargoSO.itemMaxVolume.ToString() + "    ";
                if (upgradeItem.upgrade.cargoSO.itemMaxWeight != 0)
                {
                    currentManagmentUpgradeBonus.text += "Weight : + " + upgradeItem.upgrade.cargoSO.itemMaxWeight.ToString();
                     return;
                }


            }
            else if (upgradeItem.upgrade.cargoSO.itemMaxWeight != 0)
            {
                currentManagmentUpgradeBonus.text += "Weight : + " + upgradeItem.upgrade.cargoSO.itemMaxWeight.ToString();
            }
           
        
        
    }

    public void ChangeCargoContentPage(int x)
    {
        pageIndex = x;
        DisplayCartManagmentPanel();
    }

    //############
    //REST 
    //############

    //############
    //Button Functo
    //############
    public void ChangePage(int pageIndex)
    {
        ShowCurrentPage((CityUIPage)pageIndex);
    }

    public void LeaveSettlement()
    {
        ChangePage(0);
        //setlement.CityObjectUI.SetActive(false);
        this.gameObject.SetActive(false);
        GameServices.Instance.player.UnRoot();
    }

    //BUY || SELL BUTTON  -> TRADE PANEL
    private void SetButtonText(TradeItemDisplay typeItem)
    {
        if(typeItem.tradeInventoryItem != null)
        {
            buySellButtonText.text = "SELL";
        }
        if(typeItem.tradeSettlementItem != null)
        {
            buySellButtonText.text = "BUY";
        }
    }

    //BUY BUTTON -> SHOP PANEL
    public void SetButtonTextShop (Cargo cargo)
    {
        if (cargo == null)
        {
            return;
        }
        foreach(Cargo item in CargoManager.instance.PlayerOwnedCargo)
        {
            if(cargo.cargoSO == item.cargoSO)
            {
                buyShopButtonText.text = "OWNED";
            }
            else
            {
                buyShopButtonText.text = "BUY";
            }
            
        }
    } 
    
    //initial setup
    public void prepareSettlement(SettlementRuntime settlement)
    {
        Debug.Log($"prepareSettlement({settlement.settlementName})");
        currentSettlement = settlement;
        DisplayTrade();
    }

}
