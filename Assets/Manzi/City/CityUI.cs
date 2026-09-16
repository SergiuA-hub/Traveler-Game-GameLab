using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
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
    [SerializeField] private VerticalInvetoryItem itemPrefab;
    [SerializeField] private Transform Content;

    public Player_M player;

    [Header("PAGES")]
    [SerializeField] private GameObject LobbyPanel;
    [SerializeField] private GameObject ShopPanel;
    [SerializeField] private GameObject TradePanel;


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

    
    
   

    //Lobby void

    private void DisplayInvetory()
    {
        //Delete old rows
        foreach (Transform child in Content)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }

        foreach (InvetoryItem item in player.invetory.Inventory)
        {
            VerticalInvetoryItem row = Instantiate(itemPrefab, Content);
            row.CreateIcon(item);
        }
    }

    //BUTTONS FUNCTIONS
    public void ChangePage(int pageIndex)
    {
        ShowCurrentPage((CityUIPage)pageIndex);
    }

    
}
