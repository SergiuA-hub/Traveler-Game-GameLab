using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TradeItemDisplay : MonoBehaviour, IPointerClickHandler
{
    [Header("Display")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemNameAmountText;
    public TextMeshProUGUI itemPriceText;

    //Items Data
    public SettlementItem tradeSettlementItem;
    public InventoryItem tradeInventoryItem;
    public CityUI cityUI;


    
    public void PopulateBuyIcon(SettlementItem settlementItem,CityUI UI)
    {
       
        itemNameText.text = settlementItem.resourceSO.itemName.ToString();
        itemNameAmountText.text = settlementItem.amount.ToString();
        itemPriceText.text = settlementItem.price.ToString();
        itemIcon.sprite = settlementItem.resourceSO.sprite;
        cityUI = UI;
        tradeSettlementItem = settlementItem;
        tradeInventoryItem = null;
    }

    public void PopulateSellIcon(InventoryItem inventoryItem,CityUI UI)
    {
        itemNameText.text = inventoryItem.resourceSO.itemName.ToString();
        itemNameAmountText.text = inventoryItem.amount.ToString();
        itemPriceText.text = inventoryItem.resourceSO.baseValue.ToString();
        itemIcon.sprite = inventoryItem.resourceSO.sprite;
        cityUI = UI;
        tradeInventoryItem = inventoryItem;
        tradeSettlementItem = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        cityUI.SetCurrentTradeItem(this);
    }


    
}