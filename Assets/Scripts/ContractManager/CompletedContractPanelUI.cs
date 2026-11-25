using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompletedContractPanelUI : MonoBehaviour
{
    public Image contractResIcon;
    public Image contractTypeIcon;
    public TMP_Text contractName;
    public TMP_Text contractDescription;
    public Image resAmoutnIncon;
    public TMP_Text resourceAmount;
    public TMP_Text contracTRewardCoins;
    public TMP_Text contracTRewardReputation;
    private Contract contract;
    public void setup(Contract contract)
    {
        this.contract = contract;
        contractResIcon.sprite = contract.contractRequest.sprite;
        contractName.text = contract.contractRequest.itemName;

        string amount = contract.contractRemainingAmount.ToString();
        string resource = contract.contractRequest.itemName;
        string settlement = contract.settlement.settlementName;

        if (contract.contractType == ContractType.Export)
        {
            contractTypeIcon.color = new Color32(84, 122, 69, 255);
            contractTypeIcon.GetComponentInChildren<TMP_Text>().text = "Export";

            contractDescription.text = $"Purchase {amount:F0} {resource} from <b>{contract.fromSettlement}</b> and sell them in <b>{contract.toSettlement}</b> to help reduce our surplus stock.";
        }
        else if (contract.contractType == ContractType.Import)
        {
            contractTypeIcon.color = new Color32(163, 78, 66, 255);
            contractTypeIcon.GetComponentInChildren<TMP_Text>().text = "Import";

            contractDescription.text = $"Purchase {amount:F0} {resource} from <b>{contract.fromSettlement}</b> and sell them in <b>{contract.toSettlement}</b> to help meet our local demand.";
        }

        resourceAmount.text = $"{contract.contractTotalAmount:F0}";
        resAmoutnIncon.sprite = contract.contractRequest.sprite;
        contracTRewardCoins.text = $"{contract.rewardCoins:F1}";
        contracTRewardReputation.text = $"{contract.rewardReputation:F1}";


        bool canAccept = GameServices.Instance.licenseManager.canAquire(contract.contractRequest, contract.fromSettlement);
    }
}
