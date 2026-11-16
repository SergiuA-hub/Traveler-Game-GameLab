using System.Collections.Generic;
using UnityEngine;

public class PlayerAttachments_M : MonoBehaviour
{
    public CargoSO carryCargo;    
    public List<UpgradesSO> carryUpgrades = new List<UpgradesSO>();

    public float GetTotalWeight()
    {
        float totalWeight = 0f;
        if (carryCargo != null)
        {
            totalWeight += carryCargo.itemMaxWeight;
        }
        foreach (var upgrade in carryUpgrades)
        {
            totalWeight += upgrade.additionalWeight;
        }
        return totalWeight;
    }

    public float GetTotalVolume()
    {
        float totalVolume = 0f;
        if (carryCargo != null)
        {
            totalVolume += carryCargo.itemMaxVolume;
        }
        foreach (var upgrade in carryUpgrades)
        {
            totalVolume += upgrade.additionalVolume;
        }
        return totalVolume;
    }

    public float GetTotalUpgradesVolume()
    {
        float totalVolume = 0f;

        foreach (var upgrade in carryUpgrades)
        {
            totalVolume += upgrade.additionalVolume;
        }
        return totalVolume;
    }

    public float GetTotalUpgradesWeight()
    {
        float totalWeight = 0f;

        foreach (var upgrade in carryUpgrades)
        {
            totalWeight += upgrade.additionalWeight;
        }
        return totalWeight;
    }

    public void AddCargo(CargoSO cargo)
    {
        carryCargo = cargo;
        GameServices.Instance.player.calculateInventorySize();
    }

    public bool canEquipMore()
    {
        return carryUpgrades.Count < carryCargo.upgradesSpaces;
    }

    public void AddUpgrade(UpgradesSO upgrade)
    {
        if (!canEquipMore())
            return;

        if (!carryUpgrades.Contains(upgrade))
        {
            carryUpgrades.Add(upgrade);
            GameServices.Instance.player.calculateInventorySize();
        }
    }

    public void RemoveUpgrade(UpgradesSO upgrade)
    {
        if (carryUpgrades.Contains(upgrade))
        {
            carryUpgrades.Remove(upgrade);
            GameServices.Instance.player.calculateInventorySize();
        }
    }

    public void SetCargo(CargoSO cargo)
    {
        carryCargo = cargo;
        GameServices.Instance.player.calculateInventorySize();
    }

    public void RemoveCargo()
    {
        carryCargo = null;
        carryUpgrades.Clear();

        GameServices.Instance.player.calculateInventorySize();
    }

    public bool HasUpgrade(UpgradesSO upgrade)
    {
        return carryUpgrades.Contains(upgrade);
    }

    public bool HasCargo()
    {
        return carryCargo != null;
    }

    public bool HasCargo(CargoSO cargo)
    {
        return carryCargo == cargo;
    }
}
