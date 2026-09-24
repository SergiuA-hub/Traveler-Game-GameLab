using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayInventoryItem : MonoBehaviour
{
    
    public TextMeshProUGUI amntText;
    public TextMeshProUGUI nameText;
    public Image img;

    public void Setup(InventoryItem invetoryItem)
    {
        amntText.text = invetoryItem.amount.ToString();
        nameText.text = invetoryItem.resourceSO.itemName;
        img.sprite = invetoryItem.resourceSO.sprite;
    }
}
