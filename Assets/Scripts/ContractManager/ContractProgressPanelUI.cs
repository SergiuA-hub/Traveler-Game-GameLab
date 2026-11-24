using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContractProgressPanelUI : MonoBehaviour
{
    public Image contractResIcon;
    public Image contractTypeIcon;
    public TMP_Text contractName;
    public TMP_Text contractDescription;
    
    public TMP_Text fromSettlement;
    public Image fromResIcon;
    public TMP_Text fromBuyProgress;

    public TMP_Text toSettlement;
    public Image toResIcon;
    public TMP_Text toSellProgress;


    public TMP_Text contractTime;
    public TMP_Text contracTRewardCoins;
    public TMP_Text contracTRewardReputation;
    private Contract contract;
    public void setup(Contract contract)
    {
        this.contract = contract;
        contractResIcon.sprite = contract.contractRequest.sprite;
        contractName.text = contract.contractRequest.itemName;

        string resource = contract.contractRequest.itemName;
        string settlement = contract.settlement.settlementName;

        float remaining = contract.contractRemainingAmount;

        if (contract.contractType == ContractType.Export)
        {
            contractTypeIcon.color = new Color32(84, 122, 69, 255);
            contractTypeIcon.GetComponentInChildren<TMP_Text>().text = "Export";

            contractDescription.text = $"Purchase <b>{resource}</b> from <b>{contract.fromSettlement}</b> and sell it to <b>{contract.toSettlement}</b> to complete your contract.";
        }
        else if (contract.contractType == ContractType.Import)
        {
            contractTypeIcon.color = new Color32(163, 78, 66, 255);
            contractTypeIcon.GetComponentInChildren<TMP_Text>().text = "Import";

            contractDescription.text = $"Purchase <b>{resource}</b> from <b>{contract.fromSettlement}</b> and sell it to <b>{contract.toSettlement}</b> to complete your contract.";
        }

        fromSettlement.text = $"{contract.fromSettlement}";
        fromResIcon.sprite = contract.contractRequest.sprite;
        fromBuyProgress.text = $"{contract.buyAmount:F0} / {contract.contractTotalAmount:F0}";

        toSettlement.text = $"{contract.toSettlement}";
        toResIcon.sprite = contract.contractRequest.sprite;
        toSellProgress.text = $"{contract.sellAmount:F0} / {contract.contractTotalAmount:F0}";

        contractTime.text = $"{contract.contractRemainingHours:F0} h";
        contracTRewardCoins.text = $"{contract.rewardCoins:F1}";
        contracTRewardReputation.text = $"{contract.rewardReputation:F0}";
    }
}
