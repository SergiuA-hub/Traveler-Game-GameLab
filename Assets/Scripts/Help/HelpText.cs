using TMPro;
using UnityEngine;

public class HelpText : MonoBehaviour
{
    public TMP_Text helpProduceDisplay;
    public TMP_Text helpNeedTextDisplay;
    public SettlementsManager settlementManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Start()
    {
        string helpNeedText ="";
        string helpProduceText = "";

        foreach (var settlement in settlementManager.settlements)
        {
            helpNeedText += $"{settlement.settlementName} needs:\n";
            helpProduceText += $"{settlement.settlementName} produces:\n";

            foreach (var good in settlement.defaultSettlementData.consumption)
            {
                helpNeedText += $"- {good.resource.itemName}\n";
            }
            helpNeedText += $"\n";

            foreach (var good in settlement.defaultSettlementData.production)
            {
                helpProduceText += $"- {good.resource.itemName}\n";
            }
            helpProduceText += $"\n";
        }

        helpNeedTextDisplay.text = helpNeedText;
        helpProduceDisplay.text = helpProduceText;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
