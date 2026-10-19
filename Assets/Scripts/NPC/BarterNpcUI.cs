using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.VirtualTexturing.Procedural;

public class BarterNpcUI : MonoBehaviour
{
    private Player_M player;
    private BarterNpc barter;
    public GameObject offerList;
    public GameObject requestList;
    public GameObject BarterTradeGoodPrefab;

    public TMP_Text barterReplica;
    private TradeGood requestedTrade;
    public TMP_Text requestStatus;
    private int showOfferIndex;
    public TMP_Text offerCount;
    public Button nextOfferButton;
    public Button AcceptDealButton;

    public void Open(BarterNpc barter, Player_M player)
    {
        
        this.barter = barter;
        this.player = player;

        if (barter == null)
            return;
        if (player == null)
            return;

        barter.npc.Root();
        player.Root();

        barterReplica.text = barter.replicas[UnityEngine.Random.Range(0,barter.replicas.Count)];
        showOfferIndex = 0;

        clearRequestList();
        clearOfferList();

        GameObject goReq = Instantiate(BarterTradeGoodPrefab, requestList.transform);
        goReq.GetComponent<BarterTradeGoodPrefab>().setup(barter.requestedGood);        

        GameObject goOff = Instantiate(BarterTradeGoodPrefab, offerList.transform);
        goOff.GetComponent<BarterTradeGoodPrefab>().setup(barter.offerGoods[showOfferIndex]);
        offerCount.text = $"{showOfferIndex+1} / {barter.offerGoods.Count}";
        nextOfferButton.interactable = player.invetory.CurrentCoins >= 0.5;

        bool canTrade = player.invetory.hasResources(barter.requestedGood.resource,(int)barter.requestedGood.amount);        
        if (canTrade)
        {
            requestStatus.text = "Ready to trade!";
            requestStatus.color = new Color32(113, 135, 91, 255);
        }
        else
        {
            requestStatus.text = "You can't afford this trade.";
            requestStatus.color = new Color32(173, 98, 87, 255);
            nextOfferButton.interactable = canTrade;
        }

        AcceptDealButton.interactable = canTrade;
        gameObject.SetActive(true);
    }

    public void OnDisable()
    {
        if (barter != null)
            barter.npc.UnRoot();
        
        if (player == null)
            return;

        player.UnRoot();
    }

    public void clearRequestList()
    {
        foreach (Transform good in requestList.transform)
        {
            Destroy(good.gameObject);
        }
    }

    public void clearOfferList()
    {
        foreach (Transform good in offerList.transform)
        {
            Destroy(good.gameObject);
        }
    }

    public void NextOffer()
    {
        showOfferIndex++;
        if (showOfferIndex >= barter.offerGoods.Count)
        {
            showOfferIndex--;
            return;
        }

        clearOfferList();        

        GameObject goOff = Instantiate(BarterTradeGoodPrefab, offerList.transform);
        goOff.GetComponent<BarterTradeGoodPrefab>().setup(barter.offerGoods[showOfferIndex]);
        offerCount.text = $"{showOfferIndex + 1} / {barter.offerGoods.Count}";

        nextOfferButton.interactable = player.invetory.CurrentCoins >= 0.5;
    }

    //called by accept deal button
    public void acceptDeal()
    {
        player.invetory.Remove(barter.requestedGood.resource, (int)barter.requestedGood.amount);
        player.invetory.AddItem(barter.offerGoods[showOfferIndex].resource, (int)barter.offerGoods[showOfferIndex].amount);
        barter.tradeCompleted = true;
        barter.npc.interested = false;
        gameObject.SetActive(false);
    }

    //called by decline deal button
    public void declineDeal()
    {
        barter.npc.interested = false;
        gameObject.SetActive(false);
    }
}
