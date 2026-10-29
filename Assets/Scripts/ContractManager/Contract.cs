using System;
using UnityEngine;

[Serializable]
public class Contract
{
    public SettlementSO settlement;
    public ContractType contractType;
    public ResourceAmount contractRequest;
    public float contractRemainingHours;
    public DateTime contractEndDate;
    public float rewardCoins;
    public float rewardReputation;

    public Contract(SettlementSO settlementSO, ContractType cType, ResourceAmount res, DateTime endDate, float rewardC, float rewardRep)
    {
        settlement = settlementSO;
        contractType = cType;
        contractRequest = res;
        contractEndDate = endDate;
        rewardCoins = rewardC;
        rewardReputation = rewardRep;
    }
}

public enum ContractType
{
    Import,
    Export
}
