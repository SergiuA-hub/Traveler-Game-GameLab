using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TradeItemDisplay : MonoBehaviour,IPointerClickHandler
{
    [Header("Display")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemNameAmountText;
    public TextMeshProUGUI itemPriceText;
    public CityUI cityUI;
    

    public SettlementItem settlementItemDisplay;
    public InventoryItem inventoryItemDisplay;


    public void PopulateIcon(SettlementItem settlementItem)
    {
        settlementItem = settlementItemDisplay;

        itemNameText.text = settlementItem.resourceSO.itemName.ToString();
        itemNameAmountText.text = settlementItem.amount.ToString();
        itemPriceText.text = settlementItem.price.ToString();
        itemIcon.sprite = settlementItem.resourceSO.sprite;
    }
    public void PopulateSellIcon(InventoryItem inventoryItem)
    {
        inventoryItem = inventoryItemDisplay;

        itemNameText.text = inventoryItem.resourceSO.itemName.ToString();
        itemNameAmountText.text = inventoryItem.amount.ToString();
        itemPriceText.text = inventoryItem.resourceSO.baseValue.ToString();
        itemIcon.sprite = inventoryItem.resourceSO.sprite;
    }
    //Fucntie care updateze pretul dupa ce cumperi

    public void SetCityUI(CityUI ui)
    {
        cityUI = ui;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        cityUI.DisplayCurrentTradeItem(this);
    }

    
}
