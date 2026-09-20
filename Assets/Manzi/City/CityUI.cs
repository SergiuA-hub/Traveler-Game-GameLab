
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

    [Header("Trade Panel")]

    //BUY
    [SerializeField] private TradeItemDisplay tradePrfab;
    [SerializeField] private Transform buyContent;
    
    //Sell

    [SerializeField] private TradeItemDisplay tradePlayerPrefab;
    [SerializeField] private Transform SellContent;
    public Setlement setlement;
    public PlayerInventory_M playerInventory;

    [Header("Current Trade Item Display")]
    [SerializeField] private CurrentItemDisplay currentItemDisplay;
    

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

    
    
  

    //Lobby 

    private void DisplayInvetory()
    {
        
        foreach (InventoryItem item in playerInventory.Inventory)
        {
            VerticalInvetoryItem row = Instantiate(itemPrefab, Content);
            row.CreateIcon(item);
        }
    }
    //Trade 
    private void DisplayTrade()
    {
        foreach(SettlementItem item in setlement.settlementItems)
        {
            TradeItemDisplay row = Instantiate(tradePrfab, buyContent);
            row.SetCityUI(this);
            row.PopulateIcon(item);
        }

        foreach (InventoryItem item in playerInventory.Inventory)
        {
            TradeItemDisplay row = Instantiate(tradePlayerPrefab, SellContent);
            row.SetCityUI(this);
            row.PopulateSellIcon(item);
        }
    }
    public void DisplayCurrentTradeItem(TradeItemDisplay displayItem)
    {
        currentItemDisplay.DisplayCurrentItem(displayItem);

    }


    //BUTTONS FUNCTIONS
    public void ChangePage(int pageIndex)
    {
        ShowCurrentPage((CityUIPage)pageIndex);
    }

    
}
