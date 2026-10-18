using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BarterNpc : MonoBehaviour, INpcInteraction
{
    public Npc npc;
    public List<ResourceSO> posibleBarterTradeGoods;
    public List<string> replicas;

    public TradeGood requestedGood;
    public List<TradeGood> offerGoods;
    public bool tradeCompleted;
    public void Interact()
    {
        Debug.Log("Open Barter UI");
        if (!npc.interested)
        {
            npc.showNotCurious();
            return;
        }
        GameServices.Instance.barterNpcUI.Open(this, GameServices.Instance.player);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        npc = GetComponent<Npc>();
        tradeCompleted = false;

        ResourceSO tmp;
        tmp = posibleBarterTradeGoods[UnityEngine.Random.Range(0, posibleBarterTradeGoods.Count)];        
        requestedGood = new TradeGood(tmp, UnityEngine.Random.Range(1, 5), tmp.baseValue);
        List<ResourceSO> randomGoods = posibleBarterTradeGoods.
            Distinct()
            .OrderBy(x => UnityEngine.Random.value)
            .Take(3)
            .ToList();

        foreach (var res in randomGoods)
        {
            TradeGood offeredGood = new TradeGood(res, 1, tmp.baseValue);
            offeredGood.amount = Mathf.CeilToInt((requestedGood.resource.baseValue * requestedGood.amount) / offeredGood.resource.baseValue);
            
            offerGoods.Add(offeredGood);
        }
    }

    public void completeTrade()
    {
        tradeCompleted = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
