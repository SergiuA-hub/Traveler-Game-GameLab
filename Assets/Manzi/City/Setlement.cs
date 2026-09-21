using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SettlementItem
{
    public ResourceSO resourceSO;
    public int amount;
    public int dailyProduce;
    public int dailyConsume;
    public int MAX_STOCK;
    public int MIN_STOCK;
    public int price;

}
public class Setlement : MonoBehaviour
{
    [Header("Setlements")]
    [SerializeField] private string settlementName;
    [SerializeField] public List<SettlementItem> settlementItems = new List<SettlementItem>();



    [Header("UI")]
    public GameObject CityObjectUI;
    public TradeItemDisplay currentItemSelected;

    [Header("Components")]
    [SerializeField] private CityUI cityUI;
    [SerializeField] private TimeManager timeManager;
    [SerializeField] public Player_M player;

    [Header("PriceTiers")]


    private const string playerLayer = "Player";
    private void OnEnable()
    {
        //TEST ON HOUR CHANGE
        timeManager.onHourChanged.AddListener(Consume);
        timeManager.onHourChanged.AddListener(Produce);
    }
    private void Start()
    {
        cityUI = GetComponent<CityUI>();
        CityObjectUI.SetActive(false);

        
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer(playerLayer))
        {
            CityObjectUI.SetActive(true);
        }
    }


    private void Consume(DateTime dateTime) 
    {
        foreach (var item in settlementItems) 
        {
        
        }
    }

    private void Produce(DateTime dateTime)
    {
        foreach (var item in settlementItems)
        {

        }
    }

    public void BuyOrSell()
    {
        //BUY
        if(currentItemSelected.tradeSettlementItem != null)
        {
            //Check player money & space in backpack
            if(player.invetory.CurrentCoins >= currentItemSelected.tradeSettlementItem.price 
            && player.invetory.CanCarry(currentItemSelected.tradeSettlementItem.resourceSO.volume, currentItemSelected.tradeSettlementItem.resourceSO.weight))
            {
                player.invetory.AddItem(currentItemSelected.tradeSettlementItem.resourceSO);
                player.invetory.Buy(currentItemSelected.tradeSettlementItem.price);

                //Display
                cityUI.DisplayTrade();
                cityUI.DisplayPlayerCoins();
                return;
            }
            
            
        }
        //Sell
        if (currentItemSelected.tradeInventoryItem != null)
        {
            
            player.invetory.Remove(currentItemSelected.tradeSettlementItem.resourceSO);
            player.invetory.Sell(currentItemSelected.tradeSettlementItem.price);
            
            //Display
            cityUI.DisplayTrade();
            cityUI.DisplayPlayerCoins();
            return;
        }
    }

}
