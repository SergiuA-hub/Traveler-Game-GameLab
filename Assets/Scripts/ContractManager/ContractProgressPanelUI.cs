using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContractProgressPanelUI : MonoBehaviour
{
    public Image contractResIcon;
    public Image contractTypeIcon;
    public TMP_Text contractName;
    public TMP_Text contractDescription;
    public Image resAmoutnIncon;
    public TMP_Text resourceAmount;
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

            contractDescription.text = $"Purchase the remaining <b>{remaining:F0} {resource}</b> from <b>{settlement}</b> to complete your contract.";
        }
        else if (contract.contractType == ContractType.Import)
        {
            contractTypeIcon.color = new Color32(163, 78, 66, 255);
            contractTypeIcon.GetComponentInChildren<TMP_Text>().text = "Import";

            contractDescription.text = $"Deliver and sell the remaining <b>{remaining:F0} {resource}</b> to <b>{settlement}</b> to fulfill your contract.";
        }

        resourceAmount.text = $"{remaining:F0}";
        ;

        resAmoutnIncon.sprite = contract.contractRequest.sprite;
        contractTime.text = $"{contract.contractRemainingHours:F0} h";
        contracTRewardCoins.text = $"{contract.rewardCoins:F1}";
        contracTRewardReputation.text = $"{contract.rewardReputation:F0}";
    }
}
