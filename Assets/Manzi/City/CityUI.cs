using System;
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

    [Header("PAGES")]
    [SerializeField] private GameObject LobbyPanel;
    [SerializeField] private GameObject ShopPanel;
    [SerializeField] private GameObject TradePanel;

    
    public void Start()
    {
        currentpage = CityUIPage.Lobby;
    }

    

    private void Update()
    {
        switch (currentpage)
        {
            case CityUIPage.Lobby:
                Lobby();
                break;
            case CityUIPage.Shop:
                Shop();
                break;
            case CityUIPage.Trade:
                Trade();
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

    private void Lobby()
    {
        Debug.Log("Esti in lobby ");
    }

    private void Shop()
    {

        Debug.Log("Esti in Shop");
    }

    private void Trade()
    {

        Debug.Log("Esti in Trade");
    }

    public void ChangePage(int pageIndex)
    {
        ShowCurrentPage((CityUIPage)pageIndex);
    }

    private void ShowCurrentPage(CityUIPage page)
    {
        LobbyPanel.SetActive(page == CityUIPage.Lobby);
        ShopPanel.SetActive(page == CityUIPage.Shop);
        TradePanel.SetActive(page == CityUIPage.Trade);
    }
}
