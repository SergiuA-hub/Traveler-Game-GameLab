using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ContractManager : MonoBehaviour
{
    public Button ContractPanelButton;
    public GameObject contractsWoodenPanel;
    public List<Contract> contracts = new();
    public Contract activeContract;
    private bool isContractActive;
    private int amountBought;
    private int amountSold;
    private SettlementContractsPanel panelInRange;
    public ContractPanelUI contractdetailsUI;
    public ContractProgressPanelUI contractProgressUI;
    public Button contractButton;
    public TMP_Text contractButtonText;

    public void Start()
    {
        isContractActive = false;
        contractButton.gameObject.SetActive(false);

        initializeQuests();
        resetPanelFlags();

        GameServices.Instance.timeManager.onWeekChanged.AddListener(resetPanelContracts);
    }

    public void resetPanelContracts(DateTime t)
    {
        initializeQuests();
        Debug.Log("Resetting Contracts");

        if (contractsWoodenPanel.gameObject.activeSelf)
        {
            contractdetailsUI.gameObject.SetActive(false);
            contractsWoodenPanel.GetComponent<ContractsBoardUI>().setup(panelInRange);
            resetPanelFlags(panelInRange);
            return;
        }
        resetPanelFlags();
    }

    public void resetPanelFlags(SettlementContractsPanel ignorePanel = null)
    {
        foreach (var settlement in GameServices.Instance.settlementManager.settlements)
        {
            if (ignorePanel == null)
                settlement.contractsPanel.setNewFlag(true);
            else
            {
                if (settlement.contractsPanel == ignorePanel)
                    settlement.contractsPanel.setNewFlag(false);
            }
        }
    }

    public void initializeQuests()
    {        
        contracts.Clear();

        Debug.Log(GameServices.Instance.settlementManager.settlements.Count);
        foreach (var settlement in GameServices.Instance.settlementManager.settlements)
        {
            settlement.contractsPanel.clearContracts();

            var production = settlement.defaultSettlementData.production;

            if (production.Count >= 2)
            {
                int index1 = UnityEngine.Random.Range(0, production.Count);

                int index2 = UnityEngine.Random.Range(0, production.Count - 1);
                if (index2 >= index1)
                    index2++;

                int amount = UnityEngine.Random.Range(10, 50);
                float rewardCoins = amount * production[index1].resource.baseValue * 0.1f;
                float totalContractTime = UnityEngine.Random.Range(85, 115);
                float rewardReputation = (int)UnityEngine.Random.Range(amount * production[index1].resource.baseValue * 0.21f, amount * production[index1].resource.baseValue * 0.25f); 
                Contract contract1 = new Contract(settlement.defaultSettlementData, ContractType.Export, production[index1].resource, amount, totalContractTime, rewardCoins, rewardReputation);



                amount = UnityEngine.Random.Range(10, 50);
                float rewardCoins2 = amount * production[index2].resource.baseValue * 0.1f;
                float rewardReputation2 = (int)UnityEngine.Random.Range(amount * production[index2].resource.baseValue * 0.21f, amount * production[index2].resource.baseValue * 0.25f);
                float totalContractTime2 = UnityEngine.Random.Range(85, 115);
                Contract contract2 = new Contract(settlement.defaultSettlementData, ContractType.Export, production[index2].resource, amount, totalContractTime2, rewardCoins2, rewardReputation2);

                contracts.Add(contract1);
                contracts.Add(contract2);
                settlement.contractsPanel.AddContract(contract1);
                settlement.contractsPanel.AddContract(contract2);
            }
            
            var consumption = settlement.defaultSettlementData.consumption;

            if (consumption.Count >= 2)
            {
                int index1 = UnityEngine.Random.Range(0, consumption.Count);

                int index2 = UnityEngine.Random.Range(0, consumption.Count - 1);
                if (index2 >= index1)
                    index2++;

                int amount = UnityEngine.Random.Range(10, 50);
                float rewardCoins = amount * consumption[index1].resource.baseValue * 0.1f;
                float rewardReputation = (int)UnityEngine.Random.Range(amount * consumption[index1].resource.baseValue * 0.21f, amount * consumption[index1].resource.baseValue * 0.25f);
                float totalContractTime = UnityEngine.Random.Range(85, 115);
                Contract contract1 = new Contract(settlement.defaultSettlementData, ContractType.Import, consumption[index1].resource, amount, totalContractTime, rewardCoins, rewardReputation);

                amount = UnityEngine.Random.Range(10, 50);
                float rewardCoins2 = amount * consumption[index2].resource.baseValue * 0.1f;
                float rewardReputation2 = (int)UnityEngine.Random.Range(amount * consumption[index2].resource.baseValue * 0.21f, amount * consumption[index2].resource.baseValue * 0.25f);
                float totalContractTime2 = UnityEngine.Random.Range(85, 115);
                Contract contract2 = new Contract(settlement.defaultSettlementData, ContractType.Import, consumption[index2].resource, amount, totalContractTime2, rewardCoins2, rewardReputation2);

                contracts.Add(contract1);
                contracts.Add(contract2);
                settlement.contractsPanel.AddContract(contract1);
                settlement.contractsPanel.AddContract(contract2);
            }
        }
    }    

    public void playerMovedCloseTo(SettlementContractsPanel contractPanel)
    {
        panelInRange = contractPanel;
        ContractPanelButton.gameObject.SetActive(true);
    }

    public void playetMovedAwayFrom(SettlementContractsPanel panel)
    {
        panelInRange = null;
        ContractPanelButton.gameObject.SetActive(false);
    }

    public void checkBoard()
    {
        panelInRange.setNewFlag(false);

        GameServices.Instance.player.Root();

        contractsWoodenPanel.GetComponent<ContractsBoardUI>().setup(panelInRange);

        ContractPanelButton.gameObject.SetActive(false);
        contractsWoodenPanel.SetActive(true);
    }

    public void showContractDetails(Contract contract)
    {
        contractdetailsUI.setup(contract);
        contractdetailsUI.gameObject.SetActive(true);
    }

    public bool canAcceptContract()
    {
        return !isContractActive;
    }

    public void acceptContract(Contract contract)
    {
        if (contract == null)
            return;

        if (canAcceptContract())
        {
            isContractActive = true;
            amountBought = 0;
            amountSold = 0;

            activeContract = contract;
            Debug.Log($"Contracts Count:{contracts.Count}");
            contracts.Remove(contract);
            panelInRange.RemoveContract(contract);

            Debug.Log($"Contracts Count:{contracts.Count}");
            contractsWoodenPanel.GetComponent<ContractsBoardUI>().refreshQuestPanel();

            GameServices.Instance.timeManager.onHourChanged.AddListener(hourlyUpdateContractTimer);
            contractButton.gameObject.SetActive(true);
            contractButtonText.text = $"{activeContract.contractRemainingHours:F0} h";
        }
    }

    public void hourlyUpdateContractTimer(DateTime t)
    {
        if (activeContract != null)
        {
            activeContract.contractRemainingHours -= 1f;
            if (activeContract.contractRemainingHours <= 0f)
            {
                finalizeContract();
            }
            contractButtonText.text = $"{activeContract.contractRemainingHours:F0}h";
        }
    }

    public void finalizeContract()
    {
        isContractActive = false;
        activeContract = null;
        contractButton.gameObject.SetActive(false);

        GameServices.Instance.timeManager.onHourChanged.RemoveListener(hourlyUpdateContractTimer);
    }   

    public void showContractProgress()
    {
        if (!isContractActive)
            return;

        contractProgressUI.setup(activeContract);
        contractProgressUI.gameObject.SetActive(true);
    }

    public void sellResources(ResourceSO res, float amount, SettlementSO settlement)
    {
        if(activeContract == null)
            return;

        if(activeContract.contractType == ContractType.Import && activeContract.contractRequest == res && activeContract.settlement == settlement)
        {
            activeContract.contractRemainingAmount -= amount;
            if (activeContract.contractRemainingAmount <= 0f)
            {
                GameServices.Instance.player.invetory.CurrentCoins += activeContract.rewardCoins;
                GameServices.Instance.settlementManager.addReputation(activeContract.settlement, activeContract.rewardReputation);
                finalizeContract();
                return;
            }
        }

        if (activeContract.contractType == ContractType.Export && activeContract.contractRequest == res && activeContract.settlement != settlement)
        {
            if(amount > amountBought)
            {
                activeContract.contractRemainingAmount -= amountBought;
                amountBought = 0;
            }
            else
            {
                activeContract.contractRemainingAmount -= amount;
                amountBought -= (int)amount;
            }

            if (activeContract.contractRemainingAmount <= 0f)
            {
                GameServices.Instance.player.invetory.CurrentCoins += activeContract.rewardCoins;
                GameServices.Instance.settlementManager.addReputation(activeContract.settlement, activeContract.rewardReputation);
                finalizeContract();
            }
        }
    }

    public void buyResources(ResourceSO res, float amount, SettlementSO settlement)
    {
        if (activeContract == null)
            return;

        if(activeContract.contractType == ContractType.Export && activeContract.contractRequest == res && activeContract.settlement == settlement)
        {
            amountBought += (int)amount;            
        }
    }    
}
