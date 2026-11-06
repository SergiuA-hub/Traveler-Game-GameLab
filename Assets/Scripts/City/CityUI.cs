using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private TMP_Text reputation;

    [Header("Trade Panel")]
    [SerializeField] private TradeItemDisplay tradePrfab;
    [SerializeField] private Transform buyContent;

    [SerializeField] private TradeItemDisplay tradePlayerPrefab;
    [SerializeField] private Transform SellContent;

    [Header("Player Components")]    
    public PlayerInventory_M playerInventory;
    public Player_M player;
    [Header("Player Coins display")]
    [SerializeField] private TextMeshProUGUI playerCoinsDisplay;
    [SerializeField] private TextMeshProUGUI playerCoinsShopPanel;
    [SerializeField] private TextMeshProUGUI playerCoinsLicensePanel;
    [SerializeField] private TextMeshProUGUI playerCoinsUpgradePanel;



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
    public GameObject TierDisplayPanel;
    public TMP_Text additionalInfo;
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
    [SerializeField] private GameObject ItemShopPrefab;

    public GameObject selectedShopItemPanel;
    public GameObject noShopItemSelected;

    [Header("Current SHOP_ITEM_DISPLAY")]
    [SerializeField] private Image currentShopItemIcon;
    [SerializeField] private TextMeshProUGUI currentShopItemName;
    [SerializeField] private TextMeshProUGUI currentShopItemPrice;
    [SerializeField] private TextMeshProUGUI currentShopItemDescription;
    [SerializeField] private TextMeshProUGUI buyShopButtonText;
    [SerializeField] public TextMeshProUGUI restItemShop;
    [SerializeField] private TextMeshProUGUI shopItemDetails;


    [Header("UPGRADE PAGE")]
    [SerializeField] private Transform UpgradeContent;
    [SerializeField] private GameObject upgradeItemDisplayPrefab;

    public GameObject upgradePageSelectedItemPanel;
    public GameObject upgradePageNoItemSelectedPanel;
    public Button buyUpgradesButton;

    [Header("Current UPGRADE_SHOP_ITEM_DISPLAY")]
    [SerializeField] private Image currentUpgradeItemIcon;
    [SerializeField] private TextMeshProUGUI currentUpgradeItemName;
    [SerializeField] private TextMeshProUGUI upgradeItemPrice;
    [SerializeField] private TextMeshProUGUI restUpgradeShop;
    [SerializeField] private TextMeshProUGUI buttonUpgradeShop;
    [SerializeField] private TextMeshProUGUI upgradeDescriptionText;

    [Header("TRADE GUILD PAGE")]
    [SerializeField] private Transform TradeGuildContent;
    [SerializeField] private LicenceItemDisplay LicensesDisplayPrefab;
    public GameObject selectedTradeLicensePanel;
    public GameObject noTradeLicenseSelectedPanel;

    [Header("Current LICENSE DISPLAY")]
    [SerializeField] private Image LicenseIcon;
    [SerializeField] private TextMeshProUGUI LicenseName;
    [SerializeField] private TextMeshProUGUI LicensePrice;
    [SerializeField] private Transform LicenseUnlockList;
    [SerializeField] private LicenceUnlock unlockItem;
    [SerializeField] private TextMeshProUGUI notEnoughMoneyText;

    
    [Header("Cart Managment Display")]
    [SerializeField] private Transform itemManagmentContent;
    [SerializeField] private Transform upgradeManagmentContent;
    [SerializeField] private GameObject managementCargoDisplayPrefab;
    [SerializeField] private GameObject managementUpgradeDisplayPrefab;
    private int pageIndex=1;

    [Header("Current Item_Cart_Managment")]
    [SerializeField] private Image currentManagmentItemIcon;
    [SerializeField] private TextMeshProUGUI currentManagmentItemName;
    [SerializeField] private Transform componentsArea;
    [SerializeField] private UpgradeSlot upgradeSlotPrefab;
    [SerializeField] private GameObject upgradeFramePrefab;

    [Header("Current item stats")]
    [SerializeField] private TextMeshProUGUI itemVolume;
    [SerializeField] private TextMeshProUGUI itemweight;
    [SerializeField] private TextMeshProUGUI upgradeVolume;
    [SerializeField] private TextMeshProUGUI upgradeWeight;


    [Header("Current Upgrade_Cart_Managment")]
    [SerializeField] private TextMeshProUGUI upgradeSlotsText;

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
        DisplayPlayerCoins();
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
        reputation.text = $"Reputation\n{currentSettlement.reputation:F1}";

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

        bool selectionReseted = false;
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

            if (settlementsManager.currentItemSelected != null && settlementsManager.currentItemSelected.tradeSettlementItem != null && settlementsManager.currentItemSelected.tradeSettlementItem.resource.itemName == item.resource.itemName)
            {
                selectionReseted = true;
                SetCurrentTradeItem(row);
            }
        }

        foreach (InventoryItem item in playerInventory.Inventory)
        {
            TradeItemDisplay row = Instantiate(tradePlayerPrefab, SellContent);

            row.PopulateSellIcon(item, this);

            if (settlementsManager.currentItemSelected != null && settlementsManager.currentItemSelected.tradeInventoryItem != null && settlementsManager.currentItemSelected.tradeInventoryItem.resourceSO.itemName == item.resourceSO.itemName)
            {
                selectionReseted = true;
                SetCurrentTradeItem(row);
            }
        }

        if (!selectionReseted)
        {
            settlementsManager.currentItemSelected = null;
            SliderMaxValue();
        }

        if(settlementsManager.currentItemSelected == null)
        {
            TierDisplayPanel.SetActive(false);
            additionalInfo.text = "Please select one resources to trade";
            additionalInfo.gameObject.SetActive(true);
            currentTradeItemName.text = "No selection";
            currentTradeItemImage.gameObject.SetActive(false);
        }
    }

    public void resetTransaction()
    {
        sliderAmount.value = 0;
    }

    public void DisplayPlayerCoins()
    {
        playerCoinsDisplay.text= $": {playerInventory.CurrentCoins:F1}";
        playerCoinsLicensePanel.text = $": {playerInventory.CurrentCoins:F1}";
        playerCoinsShopPanel.text = $": {playerInventory.CurrentCoins:F1}";
        playerCoinsUpgradePanel.text = $": {playerInventory.CurrentCoins:F1}";
    }

    public void SetCurrentTradeItem(TradeItemDisplay item)
    {
        selection_totalPrice = 0f;

        settlementsManager.currentItemSelected = item;
        currentTradeItemImage.sprite = item.itemIcon.sprite;
        currentTradeItemName.text = item.itemNameText.text;

        TierDisplayPanel.SetActive(true);
        additionalInfo.gameObject.SetActive(false);
        currentTradeItemImage.gameObject.SetActive(true);
        //Tiers
        GetCurrentItemTiers();
        tierIndicator.clearIndicator();

        //Buttons
        SetButtonText(item);
        SliderMaxValue();
        
        sliderAmount.value = 0;

        if(settlementsManager.currentItemSelected != null)
        {
            if(settlementsManager.currentItemSelected.tradeInventoryItem != null)
            {
                //inventory seleceted -> flip slider (right to left)
                sliderAmount.gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);

            }

            if (settlementsManager.currentItemSelected.tradeSettlementItem != null)
            {
                //settlement selected -> normal orientation to slider (left to right)
                sliderAmount.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
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

        TierDisplayPanel.SetActive(false);
        additionalInfo.gameObject.SetActive(true);
        additionalInfo.text = $"Current resource is not needed by the settlement. It will be bought at a price of {GlobalSettingsManager.UNWANTED_GOOD_PRICE_MULTIPLIER * resToShowTier.baseValue}";        
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
        if(settlementsManager.currentItemSelected == null)
        {
            Debug.LogWarning("No item selected for trade. Cannot set slider max value.");
            sliderAmount.maxValue = 0;
            sliderAmount.value = 0;
            return;
        }

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
            weight_Trade_Slider.gameObject.SetActive(true);
            volume_Trade_Slider.gameObject.SetActive(true);

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

            if(settlementsManager.currentSettlement.isUnwantedGood(settlementsManager.currentItemSelected.tradeInventoryItem.resourceSO))
            {
                currentTradeItemPrice.color = Color.yellow;
                float currentPrice = settlementsManager.currentItemSelected.tradeInventoryItem.resourceSO.baseValue * GlobalSettingsManager.UNWANTED_GOOD_PRICE_MULTIPLIER;
                totalPrice = currentPrice * amount;
                Debug.LogWarning("Selected Settlement Good is null. Using UNWANTED_GOOD_PRICE_MULTIPLIER for price calculation.");
            }
            else
            {
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

        foreach (var item in currentSettlement.availableCargoItems)
        {
            if(GameServices.Instance.cargoManager.playerOwnsCargo(item))
                continue;

            GameObject row = Instantiate(ItemShopPrefab, ShopContent);
            row.GetComponent<CargoItemDisplay>().setup(item);
        }

        setNoShopSelection();
    }

    public void setNoShopSelection()
    {
        selectedShopItemPanel.SetActive(false);
        noShopItemSelected.SetActive(true);
    }

public void SetCurrentShopItem(CargoSO cargoItem)
    {
        settlementsManager.currentShopItemSelectedSO = cargoItem;

        //Visual
        currentShopItemIcon.sprite = cargoItem.icon;
        currentShopItemPrice.text = cargoItem.price.ToString();
        currentShopItemName.text = cargoItem.itemName.ToString();
        currentShopItemDescription.text = cargoItem.Description;
        //Set not enoughMoney
        
        if(cargoItem.price > player.invetory.CurrentCoins)//And not owned
        {
            restItemShop.text = "Need " + (cargoItem.price - player.invetory.CurrentCoins);
            buyShopButtonText.color = Color.red;
            buyShopButtonText.text = "Not enough coins";
        }
        else
        {

            SetButtonTextShop(cargoItem);
            buyShopButtonText.color = Color.grey;
            restItemShop.text = "";
        }

        shopItemDetails.text = $"Weight: {cargoItem.itemMaxWeight}\nVolume: {cargoItem.itemMaxVolume}\nUpgrade slots: {cargoItem.upgradesSpaces}";

        selectedShopItemPanel.SetActive(true);
        noShopItemSelected.SetActive(false);
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

        foreach (var item in currentSettlement.availableUpgrades)
        {
            if (GameServices.Instance.cargoManager.playerOwnsUpgrade(item))
                continue;

            GameObject row = Instantiate(upgradeItemDisplayPrefab, UpgradeContent);
            row.GetComponent<UpgradeItemPrefab>().setup(item);            
            //SetCurrentUpgradeItem(item);
        }

        noUpgradeSelected();
    }

    public void noUpgradeSelected()
    {
        upgradePageSelectedItemPanel.SetActive(false);
        upgradePageNoItemSelectedPanel.SetActive(true);
        buyUpgradesButton.interactable = false;
    }

    public void SetCurrentUpgradeItem(UpgradesSO item)
    {
        settlementsManager.currentUpgradeItem= item;
        //item.SetActivSelectedUpgrade();
        currentUpgradeItemIcon.sprite = item.icon;
        currentUpgradeItemName.text = item.itemName.ToString();
        upgradeItemPrice.text = item.price.ToString();

        upgradeDescriptionText.text = $"Volume: {item.additionalVolume:F0}\nWeight: {item.additionalWeight:F0}";

        if (player.invetory.CurrentCoins < item.price)
        {
            //buuton text
            buttonUpgradeShop.text = "Not enough coins";
            buttonUpgradeShop.color = Color.red;
            //Need rest money text
            restUpgradeShop.text = "Need " + (item.price - player.invetory.CurrentCoins);

        }
        else
        {

            //buuton text
            buttonUpgradeShop.text = "BUY";
            buttonUpgradeShop.color = Color.grey;
            //Need rest money text
            restUpgradeShop.text = "";
        }

        upgradePageSelectedItemPanel.SetActive(true);
        upgradePageNoItemSelectedPanel.SetActive(false);
        buyUpgradesButton.interactable = true;
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

        noTradeLinceseSelected();
    }

    public void noTradeLinceseSelected()
    {
        selectedTradeLicensePanel.SetActive(false);
        noTradeLicenseSelectedPanel.SetActive(true);
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

        selectedTradeLicensePanel.SetActive(true);
        noTradeLicenseSelectedPanel.SetActive(false);
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
        //SetCurrentCartManagmentItem(CreatePlayerShopItem(ref settlementsManager.currentManagmentItem));

        //Display Invetory Items

        foreach (Transform child in itemManagmentContent)
        {
            Destroy(child.gameObject);

        }
        foreach (var item in GameServices.Instance.cargoManager.playerOwnedCargo)
        {
            if (GameServices.Instance.player.attachments.HasCargo(item))
                continue;

            GameObject row = Instantiate(managementCargoDisplayPrefab, itemManagmentContent);
            row.GetComponent<CargoEquipItemPrefab>().setup(item);            
        }

        foreach (Transform child in upgradeManagmentContent)
        {
            Destroy(child.gameObject);

        }

        //Display Invetory Upgrades
        foreach (var upgrade in GameServices.Instance.cargoManager.playerOwnedUpgrades)
        {
            if (GameServices.Instance.player.attachments.HasUpgrade(upgrade))
                continue;

            GameObject row = Instantiate(managementUpgradeDisplayPrefab, upgradeManagmentContent);
            row.GetComponent<UpgradesEquipItemPrefab>().setup(upgrade);
        }
        
        showEquippment();        
    }

    public void showEquippment()
    {

        if (player.attachments.carryCargo != null)
        {
            currentManagmentItemIcon.sprite = player.attachments.carryCargo.icon;
            currentManagmentItemName.text = player.attachments.carryCargo.itemName;

            foreach (Transform child in componentsArea)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in componentsArea)
            {
                Destroy(child.gameObject);
            }

            //Set components Slots 
            for (int i = 1; i <= player.attachments.carryCargo.upgradesSpaces; i++)
            {
                //Frame
                GameObject go = Instantiate(upgradeFramePrefab, componentsArea);
                if (GameServices.Instance.player.attachments.carryUpgrades.Count >= i)
                    go.GetComponent<UpgradeFrame>().showUpgrade(GameServices.Instance.player.attachments.carryUpgrades[i-1]);
                else go.GetComponent<UpgradeFrame>().showUpgrade(null);
            }

            itemVolume.text = player.attachments.carryCargo.itemMaxVolume.ToString();
            itemweight.text = player.attachments.carryCargo.itemMaxWeight.ToString();

            upgradeVolume.text = $"{GameServices.Instance.player.attachments.GetTotalUpgradesVolume():F0}";
            upgradeWeight.text = $"{GameServices.Instance.player.attachments.GetTotalUpgradesWeight():F0}";
        }
    }

    public void showUpgradeMouseOver(UpgradesSO upgradeToShow)
    {
        upgradeVolume.text = $"+{upgradeToShow.additionalVolume}";
        upgradeWeight.text = $"+{upgradeToShow.additionalWeight}";
    }

    public void showCargoMouseOver(CargoSO cargoToShow)
    {
        string volumeSign = (cargoToShow.itemMaxVolume - player.attachments.carryCargo.itemMaxVolume) >= 0 ? "+" : "";
        string weightSing = (cargoToShow.itemMaxWeight - player.attachments.carryCargo.itemMaxWeight) >= 0 ? "+" : "";
        string upgradeSlotsSign = (cargoToShow.upgradesSpaces - player.attachments.carryCargo.upgradesSpaces) >= 0 ? "+" : "";

        itemVolume.text = $"{player.attachments.carryCargo.itemMaxVolume}({volumeSign}{cargoToShow.itemMaxVolume - player.attachments.carryCargo.itemMaxVolume})";
        itemweight.text = $"{player.attachments.carryCargo.itemMaxWeight}({weightSing}{cargoToShow.itemMaxWeight - player.attachments.carryCargo.itemMaxWeight})";
        upgradeSlotsText.text = $"Upgrades Slots ({upgradeSlotsSign}{cargoToShow.upgradesSpaces - player.attachments.carryCargo.upgradesSpaces})";
    }

    public void clearMouseOver()
    {
        upgradeVolume.text = $"{GameServices.Instance.player.attachments.GetTotalUpgradesVolume():F0}";
        upgradeWeight.text = $"{GameServices.Instance.player.attachments.GetTotalUpgradesWeight():F0}";
        upgradeSlotsText.text = $"Upgrades Slots";

        itemVolume.text = player.attachments.carryCargo.itemMaxVolume.ToString();
        itemweight.text = player.attachments.carryCargo.itemMaxWeight.ToString();
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

        GameServices.Instance.settlementManager.leaveSettlement();
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
    public void SetButtonTextShop (CargoSO cargo)
    {
        if (cargo == null)
        {
            return;
        }
        foreach(CargoSO item in GameServices.Instance.cargoManager.playerOwnedCargo)
        {
            if(cargo == item)
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
        currentSettlement = settlement;
        DisplayTrade();
    }

}
