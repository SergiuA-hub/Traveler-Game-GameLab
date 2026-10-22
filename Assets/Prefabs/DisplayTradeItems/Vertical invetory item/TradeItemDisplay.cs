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
    public GameObject lockedPanel;

    [Header("Current Price")]
    public TextMeshProUGUI itemCurrentPriceText;
    public Image indicatorICon;
    public Color currentColor;
    public Image currentIcon;
    public Color[] Tiers_colors;
    public Sprite[] Tiers_indicators;
    public int currentPriceTier;
    


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

        itemNameText.text = settlementItem.resource.itemName.ToString();

        itemNameAmountText.text = settlementItem.amount.ToString();
        itemBasePriceText.text = settlementItem.resource.baseValue.ToString();
        itemIcon.sprite = settlementItem.resource.sprite;
        cityUI = UI;
        tradeSettlementItem = settlementItem;
        tradeInventoryItem = null;
        itemCurrentPriceText.text = settlementItem.price.ToString();
        if(settlementItem.price == GlobalSettingsManager.TIER_5_PRICE_MULTIPLIER * settlementItem.resource.baseValue)
        {
            currentPriceTier = 4;
        }
        else if(settlementItem.price == GlobalSettingsManager.TIER_4_PRICE_MULTIPLIER * settlementItem.resource.baseValue)
        {
            currentPriceTier = 4;
        }
        else if (settlementItem.price == GlobalSettingsManager.TIER_3_PRICE_MULTIPLIER * settlementItem.resource.baseValue)
        {
            currentPriceTier = 3;
        }
        else if (settlementItem.price == GlobalSettingsManager.TIER_2_PRICE_MULTIPLIER * settlementItem.resource.baseValue)
        {
            currentPriceTier = 2;
        }
        else if (settlementItem.price == GlobalSettingsManager.TIER_1_PRICE_MULTIPLIER * settlementItem.resource.baseValue)
        {
            currentPriceTier = 1;
        }else
        {
            Debug.Log("Something went wrong");

        }
        SetVisualsPriceTier(currentPriceTier);
      
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
        itemBasePriceText.text = inventoryItem.resourceSO.baseValue.ToString();
        itemIcon.sprite = inventoryItem.resourceSO.sprite;
        cityUI = UI;
        tradeInventoryItem = inventoryItem;
        tradeSettlementItem = null;
        //set locked panel
        lockedPanel.SetActive(false);
    }

    private void SetVisualsPriceTier(int tier) 
    {
        itemCurrentPriceText.color = Tiers_colors[tier - 1];
        indicatorICon.sprite = Tiers_indicators[tier - 1];
        indicatorICon.color = Tiers_colors[tier - 1];
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