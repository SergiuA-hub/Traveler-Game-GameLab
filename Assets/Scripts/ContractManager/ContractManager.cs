using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
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

    public IEnumerator Start()
    {
        isContractActive = false;
        contractButton.gameObject.SetActive(false);
        // Wait until SettlementManager is initialized

        float timeout = Time.realtimeSinceStartup + 5f;

        yield return new WaitUntil(() =>
            (GameServices.Instance != null &&
             GameServices.Instance.settlementManager != null &&
             GameServices.Instance.settlementManager.settlements != null &&
             GameServices.Instance.settlementManager.settlements.Count > 0)
            || Time.realtimeSinceStartup >= timeout
        );

        if (Time.realtimeSinceStartup >= timeout)
        {
            Debug.LogError("ContractManager initialization timed out!");
            yield break;
        }

        initializeQuests();
        resetPanelFlags();
        activeContract = null;

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

        foreach (var settlement in GameServices.Instance.settlementManager.settlements)
        {
            settlement.contractsPanel.clearContracts();

            //var production = settlement.defaultSettlementData.production;
            var production = settlement.defaultSettlementData.production.Where(p => p.resource.itemName != "Water").ToList();

            if (production.Count >= 2)
            {
                int index1 = UnityEngine.Random.Range(0, production.Count);

                int index2 = UnityEngine.Random.Range(0, production.Count - 1);
                if (index2 >= index1)
                    index2++;

                int maxPerWeight = Mathf.FloorToInt(GameServices.Instance.player.stats.maxWeight / production[index1].resource.weight);
                int maxPerVolume = Mathf.FloorToInt(GameServices.Instance.player.stats.maxVolume / production[index1].resource.volume);
                int amount = Mathf.Min(maxPerWeight, maxPerVolume);

                float rewardCoins = amount * production[index1].resource.baseValue * 0.1f;
                
                float rewardReputation = (int)UnityEngine.Random.Range(amount * production[index1].resource.baseValue * 0.21f, amount * production[index1].resource.baseValue * 0.25f); 
                Contract contract1 = new Contract(settlement.defaultSettlementData, ContractType.Export, production[index1].resource, amount, UnityEngine.Random.Range(85, 115), rewardCoins, rewardReputation);
                contract1.fromSettlement = settlement.settlementName;
                contract1.toSettlement = GameServices.Instance.settlementManager.getSettlementWihtNeed(contract1.contractRequest);
                if (contract1.toSettlement == "")
                    Debug.LogWarning($"Contrat {contract1.contractType} for {contract1.contractRequest} was not found in any settlement");

                float totalContractTime = GameServices.Instance.settlementManager.getDistance(contract1.fromSettlement, contract1.toSettlement);
                if (totalContractTime > 0)
                    contract1.contractDuration = (totalContractTime / GameServices.Instance.player.stats.MaxSpeeed) * 2;

                maxPerWeight = Mathf.FloorToInt(GameServices.Instance.player.stats.maxWeight / production[index2].resource.weight);
                maxPerVolume = Mathf.FloorToInt(GameServices.Instance.player.stats.maxVolume / production[index2].resource.volume);
                amount = Mathf.Min(maxPerWeight, maxPerVolume);

                float rewardCoins2 = amount * production[index2].resource.baseValue * 0.1f;
                float rewardReputation2 = (int)UnityEngine.Random.Range(amount * production[index2].resource.baseValue * 0.21f, amount * production[index2].resource.baseValue * 0.25f);

                Contract contract2 = new Contract(settlement.defaultSettlementData, ContractType.Export, production[index2].resource, amount, UnityEngine.Random.Range(85, 115), rewardCoins2, rewardReputation2);
                contract2.fromSettlement = settlement.settlementName;
                contract2.toSettlement = GameServices.Instance.settlementManager.getSettlementWihtNeed(contract2.contractRequest);

                totalContractTime = GameServices.Instance.settlementManager.getDistance(contract2.fromSettlement, contract2.toSettlement);
                if (totalContractTime > 0)
                    contract2.contractDuration = (totalContractTime / GameServices.Instance.player.stats.MaxSpeeed) * 2;

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

                int maxPerWeight = Mathf.FloorToInt(GameServices.Instance.player.stats.maxWeight / consumption[index1].resource.weight);
                int maxPerVolume = Mathf.FloorToInt(GameServices.Instance.player.stats.maxVolume / consumption[index1].resource.volume);
                int amount = Mathf.Min(maxPerWeight, maxPerVolume);

                float rewardCoins = amount * consumption[index1].resource.baseValue * 0.1f;

                float rewardReputation = (int)UnityEngine.Random.Range(amount * consumption[index1].resource.baseValue * 0.21f, amount * consumption[index1].resource.baseValue * 0.25f);
                Contract contract1 = new Contract(settlement.defaultSettlementData, ContractType.Import, consumption[index1].resource, amount, UnityEngine.Random.Range(85, 115), rewardCoins, rewardReputation);
                contract1.fromSettlement = GameServices.Instance.settlementManager.getSettlementWithProduction(contract1.contractRequest); 
                contract1.toSettlement = settlement.settlementName;
                if (contract1.toSettlement == "")
                    Debug.LogWarning($"Contrat {contract1.contractType} for {contract1.contractRequest} was not found in any settlement");

                float totalContractTime = GameServices.Instance.settlementManager.getDistance(contract1.fromSettlement, contract1.toSettlement);
                if (totalContractTime > 0)
                    contract1.contractDuration = (totalContractTime / GameServices.Instance.player.stats.MaxSpeeed) * 2;

                maxPerWeight = Mathf.FloorToInt(GameServices.Instance.player.stats.maxWeight / consumption[index2].resource.weight);
                maxPerVolume = Mathf.FloorToInt(GameServices.Instance.player.stats.maxVolume / consumption[index2].resource.volume);
                amount = Mathf.Min(maxPerWeight, maxPerVolume);

                float rewardCoins2 = amount * consumption[index2].resource.baseValue * 0.1f;
                float rewardReputation2 = (int)UnityEngine.Random.Range(amount * consumption[index2].resource.baseValue * 0.21f, amount * consumption[index2].resource.baseValue * 0.25f);

                Contract contract2 = new Contract(settlement.defaultSettlementData, ContractType.Import, consumption[index2].resource, amount, UnityEngine.Random.Range(85, 115), rewardCoins2, rewardReputation2);
                contract2.fromSettlement = GameServices.Instance.settlementManager.getSettlementWithProduction(contract2.contractRequest); 
                contract2.toSettlement = settlement.settlementName;

                totalContractTime = GameServices.Instance.settlementManager.getDistance(contract2.fromSettlement, contract2.toSettlement);
                if (totalContractTime > 0)
                    contract2.contractDuration = (totalContractTime / GameServices.Instance.player.stats.MaxSpeeed) * 2;

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
            contractButtonText.text = $"{activeContract.contractRemainingHours:F0}h";
            if (activeContract.contractRemainingHours <= 0f)
            {
                finalizeContract();
            }
        }
    }

    public void finalizeContract()
    {
        isContractActive = false;
        activeContract = null;
        contractButton.gameObject.SetActive(false);
        contractProgressUI.gameObject.SetActive(false);

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
        if (!isContractActive)
            return;

        if (activeContract == null)
            return;

        if (settlement.settlementName == activeContract.toSettlement)
            activeContract.sellAmount = Mathf.Clamp(activeContract.sellAmount + (int)amount,0,(int)activeContract.contractTotalAmount);

        if (settlement.settlementName == activeContract.fromSettlement)
            activeContract.buyAmount = Mathf.Clamp(activeContract.buyAmount - (int)amount,0,(int)activeContract.contractTotalAmount);

        checkContractCompletion();
    }

    public void buyResources(ResourceSO res, float amount, SettlementSO settlement)
    {
        if (!isContractActive)
            return;

        if (activeContract == null)
            return;

        if (activeContract.contractRequest.itemName != res.itemName)
            return;
        
        if (settlement.settlementName == activeContract.fromSettlement)
            activeContract.buyAmount = Mathf.Clamp(activeContract.buyAmount + (int)amount, 0, (int)activeContract.contractTotalAmount);

        if (settlement.settlementName == activeContract.toSettlement)
            activeContract.sellAmount = Mathf.Clamp(activeContract.sellAmount - (int)amount, 0, (int)activeContract.contractTotalAmount);
        
        checkContractCompletion();
    }

    public void checkContractCompletion()
    {
        if (!isContractActive)
            return;

        if (activeContract == null)
            return;

        if (activeContract.buyAmount >= activeContract.contractTotalAmount && activeContract.sellAmount >= activeContract.contractTotalAmount)
        {
            GameServices.Instance.player.invetory.CurrentCoins += activeContract.rewardCoins;
            GameServices.Instance.settlementManager.addReputation(activeContract.settlement, activeContract.rewardReputation);
            finalizeContract();
        }
    }
}
