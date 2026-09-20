using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TradeItemDisplay : MonoBehaviour,IPointerClickHandler
{
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemNameAmountText;
    public TextMeshProUGUI itemPriceText;

    public void PopulateIcon(SettlementItem settlementItem)
    {
        itemNameText.text = settlementItem.resourceSO.itemName.ToString();
        itemNameAmountText.text = settlementItem.amount.ToString();
        //De adaugat pretul in functie de cate sunt 
        itemPriceText.text = settlementItem.price.ToString();
        itemIcon.sprite = settlementItem.resourceSO.sprite;
    }
    public void PopulateSellIcon(InvetoryItem invetoryItem)
    {

        itemNameText.text = invetoryItem.resourceSO.itemName.ToString();
        itemNameAmountText.text = invetoryItem.amount.ToString();
        itemPriceText.text = invetoryItem.resourceSO.baseValue.ToString();
        itemIcon.sprite = invetoryItem.resourceSO.sprite;
    }
    //Fucntie care updateze pretul dupa ce cumperi

    public void SelectCurrentItem()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked" );
    }

    
}
