using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VerticalInvetoryItem : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemNameAmountText;



    public void CreateIcon(InventoryItem inventoryItem)
    {
    
        itemNameText.text = inventoryItem.resourceSO.itemName.ToString();
        itemNameAmountText.text = inventoryItem.amount.ToString();
        itemIcon.sprite = inventoryItem.resourceSO.sprite; 
    }

    
}
