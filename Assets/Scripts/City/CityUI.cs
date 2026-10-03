using TMPro;
using UnityEngine;
using UnityEngine.UI;
public enum CityUIPage
{
    Lobby=0,
    Shop=1,
    Trade=2,
    TradeGuild=3,
    CartManagment=4,
    Rest =5
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

    // BUY & SELL
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

    [Header("Shop page")]

    [SerializeField] private Transform ShopContent;
    [SerializeField] private ShopItemDisplay ItemShopPrefab;

    [Header("Current SHOP_ITEM_DISPLAY")]
    [SerializeField] private Image currentShopItemIcon;
    [SerializeField] private TextMeshProUGUI currentShopItemName;
    [SerializeField] private TextMeshProUGUI currentShopItemPrice;
    [SerializeField] private TextMeshProUGUI currentShopItemDescription;
    



    [Header("PAGES")]
    [SerializeField] private GameObject LobbyPanel;
    [SerializeField] private GameObject ShopPanel;
    [SerializeField] private GameObject TradePanel;

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
        
    }

    private void ShowCurrentPage(CityUIPage page)
    {
        currentpage = page;

        LobbyPanel.SetActive(page == CityUIPage.Lobby);
        ShopPanel.SetActive(page == CityUIPage.Shop);
        TradePanel.SetActive(page == CityUIPage.Trade);

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
            case CityUIPage.TradeGuild:

                break;
            case CityUIPage.CartManagment:

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
            TradeItemDisplay row = Instantiate(tradePrfab, buyContent);
            row.PopulateBuyIcon(item, this);
            
            if (settlementsManager.currentItemSelected != null && settlementsManager.currentItemSelected.tradeSettlementItem != null)
                if(item.resource.itemName == settlementsManager.currentItemSelected.tradeSettlementItem.resource.itemName)
                {
                    //Debug.Log($"item.resource.itemName:{item.resource.itemName}");
                    SetCurrentTradeItem(row);
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

        SetButtonText(item);
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
            row.PopulateIcons(item.shopItems, this);
        }
    }
    public void SetCurrentShopItem(ShopItemDisplay shopItem)
    {
        settlementsManager.currentShopItemSelected = shopItem;

        //Set The display Icons
        //currentShopItemIcon.sprite = shopItem.IconImage.sprite;
        currentShopItemName.text = shopItem.itemName.text;
        

    }


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
