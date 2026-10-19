using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TradeGoodPrefab : MonoBehaviour, IPointerClickHandler
{
    public Image goodIcon;

    public TMP_Text resourceName;
    public TMP_Text amount;
    public TMP_Text price;

    public TradeGood tradeGood;
    private TraderNpcUI callback;
    public void setup(TradeGood tg, TraderNpcUI cb)
    {
        tradeGood = tg;
        callback = cb;

        goodIcon.sprite = tradeGood.resource.sprite;
        
        resourceName.text = tradeGood.resource.itemName;
        amount.text = $"{tradeGood.amount}";
        price.text = $"{tradeGood.price:F1}";
    }

    public void updateQty()
    {
        amount.text = $"{tradeGood.amount}";
    }

    public void goodSelected()
    {
        callback.itemSelected(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        goodSelected();
    }
}
