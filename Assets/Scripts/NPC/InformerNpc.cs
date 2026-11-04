using System.Collections.Generic;
using UnityEngine;

public class InformerNpc : MonoBehaviour, INpcInteraction
{
    public Npc npc;
    public List<string> replicas;
    public float weatherBasePrice;
    public float springWeatherPrice;
    public float summerWeahterPrice;
    public float fallWeatherPrice;
    public float witnerWheaterPrice;
    public List<string> weatherForecastText;

    [TextArea(3, 6)]
    public List<string> tradeDealsText;

    [Header("Trade Deals")]
    public InformerTradeDeal springHighResource;
    public InformerTradeDeal springLowResource;
    public float springTradePrice;

    public InformerTradeDeal summerHighResource;
    public InformerTradeDeal summerLowResource;
    public float summerTradePrice;

    public InformerTradeDeal fallHighResource;
    public InformerTradeDeal fallLowResource;
    public float fallTradePrice;

    public InformerTradeDeal winterHighResource;
    public InformerTradeDeal winterLowResource;
    public float winterTradePrice;
    public void Interact()
    {
        Debug.Log("Open Informer UI");

        if (!npc.interested)
        {
            npc.showNotCurious();
            return;
        }
        GameServices.Instance.informerNpcUI.Open(this, GameServices.Instance.player);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        npc = GetComponent<Npc>();

        Season startSeason = GetComponent<Npc>().currentRoad.startSettlement.region;
        Season endSeason = GetComponent<Npc>().currentRoad.endSettlement.region;

        if (startSeason == Season.Spring || endSeason == Season.Spring)
        {
            springWeatherPrice = weatherBasePrice;
        }
        else springWeatherPrice = weatherBasePrice * 1.2f;

        if (startSeason == Season.Summer || endSeason == Season.Summer)
        {
            summerWeahterPrice = weatherBasePrice;
        }
        else summerWeahterPrice = weatherBasePrice * 1.2f;

        if (startSeason == Season.Fall || endSeason == Season.Fall)
        {
            fallWeatherPrice = weatherBasePrice;
        }
        else fallWeatherPrice = weatherBasePrice * 1.2f;

        if (startSeason == Season.Winter || endSeason == Season.Winter)
        {
            witnerWheaterPrice = weatherBasePrice;
        }
        else witnerWheaterPrice = weatherBasePrice * 1.2f;

        FindBestDeals(Season.Spring, out springHighResource, out springLowResource);
        springTradePrice = springHighResource.tradeGood.resource.baseValue;

        FindBestDeals(Season.Summer, out summerHighResource, out summerLowResource);
        summerTradePrice = summerHighResource.tradeGood.resource.baseValue;

        FindBestDeals(Season.Fall, out fallHighResource, out fallLowResource);
        fallTradePrice = fallHighResource.tradeGood.resource.baseValue;

        FindBestDeals(Season.Winter, out winterHighResource, out winterLowResource);
        winterTradePrice = winterHighResource.tradeGood.resource.baseValue;
    }

    private void FindBestDeals(Season region,out InformerTradeDeal highest,out InformerTradeDeal lowest)
    {
        List<InformerTradeDeal> highestCandidates = new();
        List<InformerTradeDeal> lowestCandidates = new();

        int highestTier = int.MaxValue;
        int lowestTier = int.MinValue;

        foreach (var settlement in GameServices.Instance.settlementManager.settlements)
        {
            if (settlement.defaultSettlementData.region != region)
                continue;

            foreach (var good in settlement.settlementStock)
            {
                // Only consider goods that the settlement actually trades
                var tradeData = settlement.defaultSettlementData.production
                    .Find(x => x.resource == good.resource);

                if (tradeData == null)
                {
                    tradeData = settlement.defaultSettlementData.consumption
                        .Find(x => x.resource == good.resource);
                }

                // Exclude unwanted goods
                if (tradeData == null)
                    continue;

                int tier = tradeData.getPriceTier(good.amount);

                InformerTradeDeal deal =
                    new InformerTradeDeal(settlement, good, tier);

                // Highest price = lowest tier number
                if (tier < highestTier)
                {
                    highestTier = tier;
                    highestCandidates.Clear();
                }

                if (tier == highestTier)
                    highestCandidates.Add(deal);

                // Lowest price = highest tier number
                if (tier > lowestTier)
                {
                    lowestTier = tier;
                    lowestCandidates.Clear();
                }

                if (tier == lowestTier)
                    lowestCandidates.Add(deal);
            }
        }

        highest = highestCandidates.Count > 0
            ? highestCandidates[Random.Range(0, highestCandidates.Count)]
            : null;

        lowest = lowestCandidates.Count > 0
            ? lowestCandidates[Random.Range(0, lowestCandidates.Count)]
            : null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class InformerTradeDeal
{
    public SettlementRuntime settlement;
    public TradeGood tradeGood;
    public int tier;

    public InformerTradeDeal(SettlementRuntime settlement,TradeGood tradeGood,int tier)
    {
        this.settlement = settlement;
        this.tradeGood = tradeGood;
        this.tier = tier;
    }
}