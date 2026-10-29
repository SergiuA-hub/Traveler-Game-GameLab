using System;
using System.Collections.Generic;
using UnityEngine;

public class ContractManager : MonoBehaviour
{
    public List<Contract> contracts = new();
    private SettlementContractsPanel playerInRange;

    public void Start()
    {
        initializeQuests();
    }

    public void initializeQuests()
    {
        contracts.Clear();

        DateTime nextEndDateBatch = GetNextMonday();
        Debug.Log(GameServices.Instance.settlementManager.settlements.Count);
        foreach (var settlement in GameServices.Instance.settlementManager.settlements)
        {
            var production = settlement.defaultSettlementData.production;

            if (production.Count >= 2)
            {
                int index1 = UnityEngine.Random.Range(0, production.Count);

                int index2 = UnityEngine.Random.Range(0, production.Count - 1);
                if (index2 >= index1)
                    index2++;

                int amount = UnityEngine.Random.Range(10, 50);
                ResourceAmount res1 = new ResourceAmount(production[index1].resource,amount);
                Contract contract1 = new Contract(settlement.defaultSettlementData, ContractType.Export, res1, nextEndDateBatch, production[index1].resource.baseValue, amount * 1.5f);



                amount = UnityEngine.Random.Range(10, 50);
                ResourceAmount res2 = new ResourceAmount(production[index2].resource, amount);
                Contract contract2 = new Contract(settlement.defaultSettlementData, ContractType.Export, res2, nextEndDateBatch, production[index2].resource.baseValue, amount * 1.5f);

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
                ResourceAmount res1 = new ResourceAmount(consumption[index1].resource, amount);
                Contract contract1 = new Contract(settlement.defaultSettlementData, ContractType.Import, res1, nextEndDateBatch, consumption[index1].resource.baseValue, amount * 1.5f);



                amount = UnityEngine.Random.Range(10, 50);
                ResourceAmount res2 = new ResourceAmount(production[index2].resource, amount);
                Contract contract2 = new Contract(settlement.defaultSettlementData, ContractType.Import, res2, nextEndDateBatch, consumption[index2].resource.baseValue, amount * 1.5f);

                contracts.Add(contract1);
                contracts.Add(contract2);
                settlement.contractsPanel.AddContract(contract1);
                settlement.contractsPanel.AddContract(contract2);
            }
            
        }
    }
    public DateTime GetNextMonday()
    {
        DateTime now = GameServices.Instance.timeManager.currentTime;

        int daysUntilMonday = ((int)DayOfWeek.Monday - (int)now.DayOfWeek + 7) % 7;

        // Dacă este deja luni, următorul reset este peste 7 zile.
        if (daysUntilMonday == 0)
            daysUntilMonday = 7;

        return now.Date.AddDays(daysUntilMonday);
    }

    public void playerMovedCloseTo(SettlementContractsPanel contractPanel)
    {
        playerInRange = contractPanel;
    }

    public void playetMovedAwayFrom(SettlementContractsPanel panel)
    {
        playerInRange = null;
    }
}
