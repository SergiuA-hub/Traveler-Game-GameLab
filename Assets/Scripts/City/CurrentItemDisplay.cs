using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

public class CurrentItemDisplay : MonoBehaviour
{
    public Image itemImage;
    public TextMeshProUGUI itemName;

    public void DisplayCurrentItem(TradeItemDisplay tradeItemDisplay)
    {
        itemImage.sprite = tradeItemDisplay.settlementItemDisplay.resourceSO.sprite;
        itemName.text = tradeItemDisplay.settlementItemDisplay.resourceSO.itemName;
    }
}
