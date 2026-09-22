
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
public class CityUI : MonoBehaviour
{

    public CityUIPage currentpage;
    [Header("Lobby Panel")]
    
    [SerializeField] private VerticalInvetoryItem itemPrefab;
    [SerializeField] private Transform Content;

    [Header("Buy")]

    //BUY
    [SerializeField] private TradeItemDisplay tradePrfab;
    [SerializeField] private Transform buyContent;

    //Sell
    [Header("Sell")]
    [SerializeField] private TradeItemDisplay tradePlayerPrefab;
    [SerializeField] private Transform SellContent;

    //Components
    public Setlement setlement;
    public PlayerInventory_M playerInventory;

    //Player money
    [SerializeField] private TextMeshProUGUI playerCoinsDisplay;


    [Header("Current Trade Item Display")]
    
    [SerializeField] private Image currentTradeItemImage;
    [SerializeField] private TextMeshProUGUI currentTradeItemName;
    [SerializeField] private TextMeshProUGUI currentTradeItemPrice;

    

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
        //Delete List
        foreach(Transform child in Content)
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
        //Delete List 
        foreach(Transform child in buyContent)
        {
            Destroy(child.gameObject);
        }

        foreach(Transform child in SellContent)
        {
            Destroy(child.gameObject);
        }

        //Create list
        foreach(SettlementItem item in setlement.settlementItems)
        {
            TradeItemDisplay row = Instantiate(tradePrfab, buyContent);
            
            row.PopulateBuyIcon(item,this);
        }

        foreach (InventoryItem item in playerInventory.Inventory)
        {
            TradeItemDisplay row = Instantiate(tradePlayerPrefab, SellContent);
            
            row.PopulateSellIcon(item,this);
        }

        
    }

    public void DisplayPlayerCoins()
    {
     playerCoinsDisplay.text= ": "+playerInventory.CurrentCoins.ToString();   
    }

    public void SetCurrentTradeItem(TradeItemDisplay item)
    {
        
        setlement.currentItemSelected= item;
        currentTradeItemImage.sprite = item.itemIcon.sprite;
        currentTradeItemName.text = item.itemNameText.text;
        
        
    }


   



    //BUTTONS FUNCTIONS
    public void ChangePage(int pageIndex)
    {
        ShowCurrentPage((CityUIPage)pageIndex);
    }

    public void LeaveSettlement()
    {
        ChangePage(0);
        setlement.CityObjectUI.SetActive(false);
    }
}
