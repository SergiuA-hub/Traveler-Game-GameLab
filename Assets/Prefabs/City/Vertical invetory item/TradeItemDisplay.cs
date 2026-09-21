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
    public CityUI cityUI;

    public void PopulateIcon(SettlementItem settlementItem,CityUI UI)
    {
        itemNameText.text = settlementItem.resourceSO.itemName.ToString();
        itemNameAmountText.text = settlementItem.amount.ToString();
        itemPriceText.text = settlementItem.price.ToString();
        itemIcon.sprite = settlementItem.resourceSO.sprite;
        cityUI = UI;
    }

    public void PopulateSellIcon(InventoryItem inventoryItem,CityUI UI)
    {
        itemNameText.text = inventoryItem.resourceSO.itemName.ToString();
        itemNameAmountText.text = inventoryItem.amount.ToString();
        itemPriceText.text = inventoryItem.resourceSO.baseValue.ToString();
        itemIcon.sprite = inventoryItem.resourceSO.sprite;
        cityUI = UI;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        cityUI.SetCurrentTradeItem(this);
    }

   

    public void SetCityUI(CityUI city)
    {
        this.cityUI = city;
    }
}