using System.Collections.Generic;
using UnityEngine;

public class TraderNpc : MonoBehaviour, INpcInteraction
{
    public Npc npc;
    public List<TradeGoodChance> possibleTradingGoodsList;
    public int maxTradeCount;

    public List<TradeGood> tradeGoods = new List<TradeGood>();

    public void Interact()
    {
        Debug.Log("Open Trader UI");
        GameServices.Instance.traderNpcUI.Open(this,GameServices.Instance.player);
        // Open trader screen
    }

    private void Start()
    {
        npc = GetComponent<Npc>();

        tradeGoods.Clear();

        foreach (var good in possibleTradingGoodsList)
        {
            float roll = Random.Range(0f, 100f);

            if (roll > good.spawnChance)
                continue;

            int amount = Random.Range(1, good.maxAmount + 1);
            float priceMultiplier = Random.Range(0.8f, 1.2f);

            TradeGood tradegood = new TradeGood(
                good.resource,
                amount,
                good.resource.baseValue * priceMultiplier
            );

            tradeGoods.Add(tradegood);

            if (tradeGoods.Count >= maxTradeCount)
                break;
        }
    }
}

[System.Serializable]
public class TradeGoodChance
{
    public ResourceSO resource;
    public int maxAmount;

    [Range(0f, 100f)]
    public float spawnChance;
}