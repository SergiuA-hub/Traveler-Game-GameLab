using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VerticalInvetoryItem : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemNameAmountText;



    public void CreateIcon(InvetoryItem invetoryItem)
    {
    
        itemNameText.text = invetoryItem.resourceSO.itemName.ToString();
        itemNameAmountText.text = invetoryItem.amount.ToString();
        itemIcon.sprite = invetoryItem.resourceSO.sprite; 
    }

    
}
