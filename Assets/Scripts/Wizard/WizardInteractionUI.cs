using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WizardInteractionUI : MonoBehaviour
{
    private int buyTimeValue;
    private float currentCost;

    public TMP_Text currentTimerText;
    public TMP_Text buyTimeAmountText;
    public TMP_Text buyTimeCostText;
    public Button extendProtection;
    private void OnEnable()
    {
        initialize();
    }

    public void initialize()
    {
        float maxTime = (GameServices.Instance.timeManager.timerMax - GameServices.Instance.timeManager.timerCurrent) / 60;
        if (buyTimeValue > (int)maxTime)
        {
            buyTimeValue = (int)maxTime;
        }
        currentCost = buyTimeValue * GameServices.Instance.wizardManager.getBuffCost(buyTimeValue);
        buyTimeAmountText.text = $"{buyTimeValue} mins";
        buyTimeCostText.text = $"{currentCost:F1}";
        extendProtection.interactable = currentCost <= GameServices.Instance.player.invetory.CurrentCoins;
    }

    public void AddTime()
    {
        buyTimeValue += 10;
        float maxTime = (GameServices.Instance.timeManager.timerMax - GameServices.Instance.timeManager.timerCurrent)/60;
        if (buyTimeValue > (int)maxTime)
        {
            buyTimeValue = (int)maxTime;
        }
        currentCost = buyTimeValue * GameServices.Instance.wizardManager.getBuffCost(buyTimeValue);
        buyTimeAmountText.text = $"{buyTimeValue} mins";
        buyTimeCostText.text = $"{currentCost:F1}";
        extendProtection.interactable = currentCost <= GameServices.Instance.player.invetory.CurrentCoins;
    }

    public void RemoveTime()
    {
        buyTimeValue -= 10;
        if(buyTimeValue < 10)
        {
            buyTimeValue = 0;
        }
        currentCost = buyTimeValue * GameServices.Instance.wizardManager.getBuffCost(buyTimeValue);
        buyTimeAmountText.text = $"{buyTimeValue} mins";
        buyTimeCostText.text = $"{currentCost:F1}";
        extendProtection.interactable = currentCost <= GameServices.Instance.player.invetory.CurrentCoins;
    }

    public void AddTimeToBuff()
    {
        if(currentCost > GameServices.Instance.player.invetory.CurrentCoins)
        {
            return;
        }

        if(buyTimeValue <= 0)
        {
            return;
        }

        GameServices.Instance.timeManager.timerCurrent += buyTimeValue * 60;
        initialize();
    }

    private void OnDisable()
    {
        GameServices.Instance.wizardManager.finalizeInteractionWithWizard();
    }

    private void Update()
    {
        var timesp = new TimeSpan(0, 0, (int)GameServices.Instance.timeManager.timerCurrent);
        currentTimerText.text = timesp.ToString("mm\\:ss");
    }
}
