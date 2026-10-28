using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TraderNpcUI : MonoBehaviour
{
    private TraderNpc trader;
    private Player_M player;

    public GameObject tradeGoodList;
    public GameObject tradeGoodPrefab;
    private TradeGoodPrefab selectedTrageGood;

    public GameObject selectedItemPanel;
    public Image selectedGoodIcon;
    public TMP_Text selectedGoodBasePrice;
    public TMP_Text selectedGoodTraderPrice;
    public TMP_Text selectedGoodWeight;
    public TMP_Text selectedGoodSize;
    public Slider weightSlider;
    public TMP_Text weightValue;
    public Slider sizeSlider;
    public TMP_Text volumeValue;
    public TMP_Text traderCoins;
    public void Open(TraderNpc trader, Player_M player)
    {
        this.trader = trader;
        this.player = player;

        trader.npc.Root();
        player.Root();

        refreshTradeGoods();

        selectedItemPanel.SetActive(false);
        updateInventoryData();

        gameObject.SetActive(true);
    }

    public void OnDisable()
    {
        if(trader != null)
            trader.npc.UnRoot();
        
        if (player == null)
            return;

        player.UnRoot();
    }

    public void refreshTradeGoods()
    {
        clearGoodsList();

        foreach (var res in trader.tradeGoods)
        {
            if (res.amount <= 0)
                continue;

            GameObject go = Instantiate(tradeGoodPrefab, tradeGoodList.transform);
            go.GetComponent<TradeGoodPrefab>().setup(res, this);
        }
    }

    public void clearGoodsList()
    {
        foreach(Transform good in tradeGoodList.transform)
        {
            Destroy(good.gameObject);
        }
    }

    public void itemSelected(TradeGoodPrefab tradeGood)
    {
        selectedTrageGood = tradeGood;

        selectedGoodIcon.sprite = selectedTrageGood.tradeGood.resource.sprite;
        selectedGoodBasePrice.text = $"{selectedTrageGood.tradeGood.resource.baseValue}";
        selectedGoodTraderPrice.text = $"{selectedTrageGood.tradeGood.price}";
        selectedGoodWeight.text = $"{selectedTrageGood.tradeGood.resource.weight}";
        selectedGoodSize.text = $"{selectedTrageGood.tradeGood.resource.volume}";
        selectedItemPanel.SetActive(true);
    }

    public void updateInventoryData()
    {
        if (player == null)
            return;

        weightSlider.maxValue = player.stats.maxWeight;
        sizeSlider.maxValue = player.stats.maxVolume;

        weightSlider.value = player.invetory.currentWeight;
        weightValue.text = $"{player.invetory.currentWeight} / {player.stats.maxWeight}";

        sizeSlider.value = player.invetory.currentVolume;
        volumeValue.text = $"{player.invetory.currentVolume} / {player.stats.maxVolume}";
        traderCoins.text = $"{player.invetory.CurrentCoins:F1}";
    }

    public void tradeSelectedItem()
    {
        if (selectedTrageGood.tradeGood.amount <= 0)
            return;

        if (player.invetory.CurrentCoins < selectedTrageGood.tradeGood.price)
            return;

        if(player.invetory.CanCarry(selectedTrageGood.tradeGood.resource.volume, selectedTrageGood.tradeGood.resource.weight))
        {
            player.invetory.tradeIn(selectedTrageGood.tradeGood.resource, selectedTrageGood.tradeGood.price,1);
            selectedTrageGood.tradeGood.amount--;
            if(selectedTrageGood.tradeGood.amount <= 0)
            {
                selectedItemPanel.SetActive(false);                
                selectedTrageGood = null;

                refreshTradeGoods();
            }
            else selectedTrageGood.updateQty();
            trader.npc.interested = false;
        }
        updateInventoryData();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
