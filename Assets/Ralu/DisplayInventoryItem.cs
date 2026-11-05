using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DisplayInventoryItem : MonoBehaviour, IPointerClickHandler
{
    
    public TextMeshProUGUI amntText;
    public TextMeshProUGUI nameText;
    public Image img;
    public InventoryDisplayUI invetoryDisplay;
    public InventoryItem inventoryItem;

    public void Setup(InventoryItem invetoryItem,InventoryDisplayUI inventoryDisplayUI)
    {
        amntText.text = invetoryItem.amount.ToString();
        nameText.text = invetoryItem.resourceSO.itemName;
        img.sprite = invetoryItem.resourceSO.sprite;
        invetoryDisplay = inventoryDisplayUI;
        inventoryItem= invetoryItem;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       invetoryDisplay.SetCurrentIventoryItem(this.inventoryItem); 
    }

}
