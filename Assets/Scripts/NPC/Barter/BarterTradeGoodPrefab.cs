using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarterTradeGoodPrefab : MonoBehaviour
{
    private TradeGood tradeGood;
    public Image tradeIcon;
    public TMP_Text goodResourceName;
    public TMP_Text goodTradeAmount;
    public void setup(TradeGood good)
    {
        tradeGood = good;

        tradeIcon.sprite = tradeGood.resource.sprite;
        goodResourceName.text = tradeGood.resource.itemName;
        goodTradeAmount.text = $"x {tradeGood.amount}";
    }
}
