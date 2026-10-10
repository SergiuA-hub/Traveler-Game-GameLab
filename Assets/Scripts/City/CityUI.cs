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

    //Components
    public Setlement setlement;
    public PlayerInventory_M playerInventory;
    [SerializeField] private TextMeshProUGUI playerCoinsDisplay;


    [Header("Current TRADE_ITEM_DISPLAY")]
    
    [SerializeField] private Image currentTradeItemImage;
    [SerializeField] private TextMeshProUGUI currentTradeItemName;
    [SerializeField] private TextMeshProUGUI currentTradeItemPrice;
    [SerializeField] private TextMeshProUGUI buySellButtonText;
    [SerializeField] private TextMeshProUGUI currentItemAmount;


    [Header("SLIDER_TRADE")]
    public Slider sliderAmount;
    [SerializeField] private TextMeshProUGUI currentTextAmount;
    

    [Header("SHOP PAGE")]

    [SerializeField] private Transform ShopContent;
    [SerializeField] private ShopItemDisplay ItemShopPrefab;

    [Header("Current SHOP_ITEM_DISPLAY")]
    [SerializeField] private Image currentShopItemIcon;
    [SerializeField] private TextMeshProUGUI currentShopItemName;
    [SerializeField] private TextMeshProUGUI currentShopItemPrice;
    [SerializeField] private TextMeshProUGUI currentShopItemDescription;

    [Header("UPGRADE PAGE")]
    [SerializeField] private Transform UpgradeContent;
    [SerializeField] ShopItemDisplay upgradeItemShopPrefab;

    [Header("Current UPGRADE_SHOP_ITEM_DISPLAY")]
    [SerializeField] private Image currentUpgradeItemIcon;
    [SerializeField] private TextMeshProUGUI upgradeItemName;
    [SerializeField] private TextMeshProUGUI upgradeItemPrice;
    [SerializeField] private TextMeshProUGUI upgradeItemDescription;

    [Header("TRADE GUILD PAGE")]
    [SerializeField] private Transform TradeGuildContent;
    //Aici se poate sa am nevoie de alt prefab ,acm e destul de bun
    [SerializeField] private ShopItemDisplay LicensesDisplayPrefab;

    [Header("Current LICENSE DISPLAY")]
    [SerializeField] private Image LicenseIcon;
    [SerializeField] private TextMeshProUGUI LicenseName;
    [SerializeField] private TextMeshProUGUI LicensePrice;
    [SerializeField] private TextMeshProUGUI LicenseUnlock;

    //Cart Managment

    [Header("PAGES")]
    [SerializeField] private GameObject LobbyPanel;
    [SerializeField] private GameObject ShopPanel;
    [SerializeField] private GameObject TradePanel;
    [SerializeField] private GameObject UpgradePanel;

    [SerializeField] private GameObject GuildPanel;
    [SerializeField] private GameObject CartManagmentPanel;

    private void Awake()
    {
        setlement = GetComponent<Setlement>();
    }
    private void OnEnable()
    {
        setlement = GetComponent<Setlement>();        
        ShowCurrentPage(currentpage);
    }

    private void Update()
    {
        DisplayPlayerCoins();
        currentTextAmount.text = sliderAmount.value.ToString();
        
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

            if (LicenseManager.instance.CanTrade(item.resource))
            {
                 row = Instantiate(tradePrfab, buyContent);
                row.PopulateBuyIcon(item, this);
            }
            
            

            if (settlementsManager.currentItemSelected != null && settlementsManager.currentItemSelected.tradeSettlementItem != null)
                if(item.resource.itemName == settlementsManager.currentItemSelected.tradeSettlementItem.resource.itemName)
                {
                    //Debug.Log($"item.resource.itemName:{item.resource.itemName}");
                    //SetCurrentTradeItem(row);
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
        


        if (item.tradeSettlementItem != null)
            currentTradeItemPrice.text = settlementsManager.currentSettlement.getStockPrice(item.tradeSettlementItem.resource).ToString("0.0");
        else currentTradeItemPrice.text = settlementsManager.currentSettlement.getStockPrice(item.tradeInventoryItem.resourceSO).ToString("0.0");

        //Buttons
        SetButtonText(item);
        SliderMaxValue();

    }

    public void SliderMaxValue()
    {
        //Slider
        if (settlementsManager.currentItemSelected.tradeInventoryItem == null)
        {
            sliderAmount.maxValue = settlementsManager.currentItemSelected.tradeSettlementItem.amount;
        }
        if (settlementsManager.currentItemSelected.tradeSettlementItem == null)
        {
            sliderAmount.maxValue = settlementsManager.currentItemSelected.tradeInventoryItem.amount;
        }
        
    }

    public int ReturnCurentSliderValue()
    {
        return Mathf.RoundToInt(sliderAmount.value);
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
            row.PopulateIcons(item.cargoSO, this);
        }
    }
    public void SetCurrentShopItem(ShopItemDisplay shopItem)
    {
        settlementsManager.currentShopItemSelected = shopItem;

        //Set The display Icons
        //currentShopItemIcon.sprite = shopItem.IconImage.sprite;
        currentShopItemName.text = shopItem.itemName.text;
        

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
            ShopItemDisplay row = Instantiate(upgradeItemShopPrefab, UpgradeContent);
            row.PopulateIcons(item.cargoSO, this);
        }
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
            ShopItemDisplay row = Instantiate(LicensesDisplayPrefab, TradeGuildContent);
            row.PopulateLicense(license, this);
        }
        //current  license selected
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
    
    //initial setup
    public void prepareSettlement(SettlementRuntime settlement)
    {
        Debug.Log($"prepareSettlement({settlement.settlementName})");
        currentSettlement = settlement;
        DisplayTrade();
    }

}
