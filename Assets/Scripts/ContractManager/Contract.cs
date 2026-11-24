using System;
using UnityEngine;

[Serializable]
public class Contract
{
    public SettlementSO settlement;
    public ContractType contractType;
    public string fromSettlement;
    public int buyAmount;
    public string toSettlement;
    public int sellAmount;
    public ResourceSO contractRequest;
    public float contractTotalAmount;
    public float contractRemainingHours;
    public float contractRemainingAmount;
    public float contractDuration;
    public float rewardCoins;
    public float rewardReputation;
    public Contract(SettlementSO settlementSO, ContractType cType, ResourceSO res,float amount, float duration, float rewardC, float rewardRep)
    {
        settlement = settlementSO;
        contractType = cType;
        contractRequest = res;
        contractTotalAmount = amount;
        contractRemainingAmount = contractTotalAmount;

        contractDuration = duration;
        contractRemainingHours = contractDuration;
        rewardCoins = rewardC;
        rewardReputation = rewardRep;

        buyAmount = 0;
        sellAmount = 0;
    }
}

public enum ContractType
{
    Import,
    Export
}
