using JetBrains.Annotations;
using Mono.Cecil;
using System;
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
    [SerializeField] private TextMeshProUGUI upgradeItemName;
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

    //Cart Managment

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
        ShowTotalPrice();
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
        playerCoinsDisplay.text= ": "+playerInventory.CurrentCoins.ToString();
    }

    public void SetCurrentTradeItem(TradeItemDisplay item)
    {
        settlementsManager.currentItemSelected = item;
        currentTradeItemImage.sprite = item.itemIcon.sprite;
        currentTradeItemName.text = item.itemNameText.text;

        //Tiers
        GetCurrentItemTiers();

        //Buttons
        SetButtonText(item);
        SliderMaxValue();

    }

    public void GetCurrentItemTiers()
    {
        TradeItemDisplay currentItem = settlementsManager.currentItemSelected;
        SettlementSO currentSettlement = settlementsManager.currentSettlement.defaultSettlementData;
        if(currentItem == null)
        {
            return;
        }
        float tierIStock;
        float tierIIStock;
        float tierIIIStock;
        float tierIVStock;
        float tierVStock;

        foreach(var production in currentSettlement.production)
        {
            if(currentItem.tradeSettlementItem.resource == production.resource)
            {
                tierIStock = production.tier1_price_stock;
                tierIIStock = production.tier2_price_stock;
                tierIIIStock = production.tier3_price_stock;
                tierIVStock = production.tier4_price_stock;
                tierVStock = production.tier5_price_stock;
                SetTierList(tierIStock, 0, 0, 0, tierVStock);
            }
            else
            {
                foreach(var consumption in currentSettlement.consumption)
                {
                    if (currentItem.tradeSettlementItem.resource == consumption.resource)
                    {
                        tierIStock = consumption.tier1_price_stock;
                        tierIIStock = consumption.tier2_price_stock;
                        tierIIIStock = consumption.tier3_price_stock;
                        tierIVStock = consumption.tier4_price_stock;
                        tierVStock = consumption.tier5_price_stock;
                        SetTierList(tierIStock, 0, 0, 0, tierVStock);
                    }
                }
            }
        }
        


    }

    private void SetTierList(float tierI,float tierII,float tierIII,float tierIV,float tierV)
    {
        //Amount
        TierIAmount.text =tierI.ToString();
        TierVAmoun.text = "+" + tierV.ToString();

        //Price
        TierVPrice.text = (tierI * GlobalSettingsManager.TIER_1_PRICE_MULTIPLIER +
                    tierII * GlobalSettingsManager.TIER_2_PRICE_MULTIPLIER +
                    tierIII * GlobalSettingsManager.TIER_3_PRICE_MULTIPLIER +
                    tierIV * GlobalSettingsManager.TIER_4_PRICE_MULTIPLIER +
                    tierV * GlobalSettingsManager.TIER_5_PRICE_MULTIPLIER).ToString();
    }

    private float CalculatePriceTiers(float tierI, float tierII, float tierIII, float tierIV, float tierV)
    {
        float sum = tierI * GlobalSettingsManager.TIER_1_PRICE_MULTIPLIER +
                    tierII * GlobalSettingsManager.TIER_2_PRICE_MULTIPLIER +
                    tierIII * GlobalSettingsManager.TIER_3_PRICE_MULTIPLIER +
                    tierIV * GlobalSettingsManager.TIER_4_PRICE_MULTIPLIER +
                    tierV * GlobalSettingsManager.TIER_5_PRICE_MULTIPLIER;
        return sum;
    }

    //Slider
    public void SliderMaxValue()
    {
       
        if (settlementsManager.currentItemSelected.tradeInventoryItem == null)
        {
            sliderAmount.maxValue = settlementsManager.currentItemSelected.tradeSettlementItem.amount;
        }
        if (settlementsManager.currentItemSelected.tradeSettlementItem == null)
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

    public int ReturnCurentSliderValue()
    {
        return Mathf.RoundToInt(sliderAmount.value);
    }
    //Total Price

    public void ShowTotalPrice()
    {
        if(settlementsManager.currentItemSelected == null)
        {
            currentTradeItemPrice.text = "0";
            return ;
        }
        //BUY
        if(settlementsManager.currentItemSelected.tradeSettlementItem != null)
        {
            currentTradeItemPrice.text = (settlementsManager.currentItemSelected.tradeSettlementItem.price * ReturnCurentSliderValue()).ToString();
        }
        //SELL
        if (settlementsManager.currentItemSelected.tradeInventoryItem != null)
        {
            currentTradeItemPrice.text = (settlementsManager.currentItemSelected.tradeInventoryItem.averageValue * ReturnCurentSliderValue()).ToString();
        }
       

        
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
        }
    }

    public void SetCurrentUpgradeItem(UpgradeItemDisplay item)
    {
        settlementsManager.currentUpgradeItem= item;

        currentUpgradeItemIcon = item.iconImage;
        upgradeItemPrice.text = item.price.text;
        upgradeItemName.text= item.itemName.text;
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
    

    //############
    //CART MANAGMENT
    //############
    public void DisplayCartManagmentPanel()
    {

    }
    //############
    //REST 
    //############

    //BUTTONS FUNCTIONS
    public void ChangePage(int pageIndex)
    {
        ShowCurrentPage((CityUIPage)pageIndex);
    }

    public void LeaveSettlement()
    {
        ChangePage(0);
        //setlement.CityObjectUI.SetActive(false);
        this.gameObject.SetActive(false);
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
