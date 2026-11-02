using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContractPanelUI : MonoBehaviour
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
    public Button acceptContractButton;
    public TMP_Text acceptButtoneText;
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

            contractDescription.text = $"Purchase {amount:F0} {resource} from <b>{settlement}</b> to help distribute its local produce.";
        }
        else if (contract.contractType == ContractType.Import)
        {
            contractTypeIcon.color = new Color32(163, 78, 66, 255);
            contractTypeIcon.GetComponentInChildren<TMP_Text>().text = "Import";

            contractDescription.text = $"Deliver and sell {amount:F0} {resource} to <b>{settlement}</b> to help replenish its supplies.";
        }

        resourceAmount.text = $"{contract.contractTotalAmount:F0}";
        resAmoutnIncon.sprite = contract.contractRequest.sprite;
        contractTime.text = $"{contract.contractDuration:F0} h";
        contracTRewardCoins.text = $"{contract.rewardCoins:F1}";
        contracTRewardReputation.text = $"{contract.rewardReputation:F1}";

        bool canAccept = GameServices.Instance.licenseManager.CanTrade(contract.contractRequest, contract.settlement);
        acceptContractButton.interactable = canAccept;
        if (canAccept)
        {
            if(GameServices.Instance.contractManager.canAcceptContract())
            {
                acceptButtoneText.text = "Accept Contract";
            }
            else
            {
                acceptContractButton.interactable = false;
                acceptButtoneText.text = "Already have an active contract";
            }
            
        }
        else
        {
            acceptButtoneText.text = "Trading license required";
        }
    }

    public void acceptContract()
    {
        GameServices.Instance.contractManager.acceptContract(contract);
        this.gameObject.SetActive(false);
    }

}
