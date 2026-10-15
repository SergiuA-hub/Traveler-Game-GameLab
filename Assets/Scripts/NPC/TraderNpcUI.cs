using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TraderNpcUI : MonoBehaviour
{
    public TraderNpc trader;
    public Player_M player;

    public GameObject tradeGoodList;
    public GameObject tradeGoodPrefab;
    private TradeGoodPrefab selectedTrageGood;

    public GameObject selectedItemPanel;
    public Image selectedGoodIcon;
    public TMP_Text selectedGoodBasePrice;
    public TMP_Text selectedGoodTraderPrice;
    public TMP_Text selectedGoodWeight;
    public TMP_Text selectedGoodSize;
    public void Open(TraderNpc trader, Player_M player)
    {
        this.trader = trader;
        this.player = player;

        trader.npc.Root();

        clearGoodsList();

        foreach (var res in trader.tradeGoods)
        {
            Debug.LogWarning($"Setting up {res.resource.name}");
            GameObject go = Instantiate(tradeGoodPrefab, tradeGoodList.transform);
            go.GetComponent<TradeGoodPrefab>().setup(res, this);
        }
        selectedItemPanel.SetActive(false);
        gameObject.SetActive(true);
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
