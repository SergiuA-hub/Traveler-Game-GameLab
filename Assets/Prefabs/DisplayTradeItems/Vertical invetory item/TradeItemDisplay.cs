using TMPro;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TradeItemDisplay : MonoBehaviour, IPointerClickHandler
{
    [Header("Display")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemNameAmountText;
    public TextMeshProUGUI itemBasePriceText;
    public TextMeshProUGUI itemAveragePriceText;
    public GameObject lockedPanel;

    [Header("Current Price")]
    public TextMeshProUGUI itemCurrentPriceText;
    public Image indicatorICon;
    public Color currentColor;
    public Image currentIcon;
    public Color[] Tiers_colors;
    public Sprite[] Tiers_indicators;
    public int currentPriceTier;
    private int currentColorTier;


    [Header("Item Components")]
    public bool isLocked;
    public TradeGood tradeSettlementItem;
    public InventoryItem tradeInventoryItem;
    public CityUI cityUI;

    public float ReturnAmount()
    {
        if (tradeInventoryItem.amount != 0)
        {
            return tradeInventoryItem.amount;
        }
        else if (tradeSettlementItem.amount != 0)
        {
            return tradeSettlementItem.amount;
        }
        else
        {
            return 0;
        }

    }

    public void PopulateBuyIcon(TradeGood settlementItem, CityUI UI, bool locked)
    {
        itemAveragePriceText.gameObject.SetActive(false);

        itemNameText.text = settlementItem.resource.itemName.ToString();

        itemNameAmountText.text = settlementItem.amount.ToString();
        
        //itemBasePriceText.text = settlementItem.resource.baseValue.ToString();        
        //show empty space to keep formating
        itemBasePriceText.text = "";

        itemIcon.sprite = settlementItem.resource.sprite;
        cityUI = UI;
        tradeSettlementItem = settlementItem;
        tradeInventoryItem = null;
        itemCurrentPriceText.text = $"{settlementItem.price:F1}";
        
        //Profit indicator
        if(settlementItem.price > settlementItem.resource.baseValue)
        {
            currentPriceTier = 5;
            currentColorTier = 1;
        }
        else if(settlementItem.price == settlementItem.resource.baseValue)
        {
            currentPriceTier = 3;
            currentColorTier = 3;
        }
        else if (settlementItem.price < settlementItem.resource.baseValue)
        {
            currentPriceTier = 1;
            currentColorTier = 5;
        }

        setVisualColor(currentColorTier);
        setVisualPriceIndicator(currentPriceTier);

        //lockedPanel
        if (locked)
        {
            lockedPanel.SetActive(true);
        }
        else
        {
            lockedPanel.SetActive(false);
        }

        isLocked = locked;
    }

    public void PopulateSellIcon(InventoryItem inventoryItem, CityUI UI)
    {
        itemNameText.text = inventoryItem.resourceSO.itemName.ToString();
        itemNameAmountText.text = inventoryItem.amount.ToString();
        //itemBasePriceText.text = inventoryItem.resourceSO.baseValue.ToString();
        itemBasePriceText.text = "";

        itemIcon.sprite = inventoryItem.resourceSO.sprite;
        cityUI = UI;
        tradeInventoryItem = inventoryItem;
        tradeSettlementItem = null;
        itemAveragePriceText.text = $"{inventoryItem.averageValue:F1}";
        //itemCurrentPriceText.text = (cityUI.settlementsManager.currentSettlement.getStockPrice(this.tradeInventoryItem.resourceSO)- inventoryItem.averageValue  ).ToString();
        itemCurrentPriceText.text = "";
        
        //Price indicator

        if (cityUI.settlementsManager.currentSettlement.getStockPrice(this.tradeInventoryItem.resourceSO) > inventoryItem.averageValue)
        {
            currentPriceTier = 5;
            currentColorTier = 5;
        }
        if (cityUI.settlementsManager.currentSettlement.getStockPrice(this.tradeInventoryItem.resourceSO) == inventoryItem.averageValue)
        {
            currentPriceTier = 3;
            currentColorTier = 3;
        }
        if (cityUI.settlementsManager.currentSettlement.getStockPrice(this.tradeInventoryItem.resourceSO) < inventoryItem.averageValue)
        {
            currentPriceTier = 1;
            currentColorTier = 1;
        }
        setVisualColor(currentColorTier);
        setVisualPriceIndicator(currentPriceTier);

        lockedPanel.SetActive(false);
    }

    private void setVisualColor(int tier)
    {
        itemCurrentPriceText.color = Tiers_colors[tier - 1];
        indicatorICon.color = Tiers_colors[tier - 1];
    }

    public void setVisualPriceIndicator(int tier)
    {
        indicatorICon.sprite = Tiers_indicators[tier - 1];
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        cityUI.SetCurrentTradeItem(this);
       
        if (!isLocked || tradeInventoryItem != null)
        {
            cityUI.CurrentLockedPanel.SetActive(false);
        }
        else
        {
            cityUI.CurrentLockedPanel.SetActive(true);
        }
        
    }
}