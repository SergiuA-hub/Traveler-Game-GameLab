using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class CargoManager : MonoBehaviour
{
    public CargoSO defaultCargoEqupment;
    public List<CargoSO> playerOwnedCargo = new List<CargoSO>();
    public List<UpgradesSO> playerOwnedUpgrades = new List<UpgradesSO>();
    public Cargo currentCargoSelected;

    //######
    // ITEMS
    //######
    public bool CanBuy(CargoSO item,float coins)
    {
       bool can= true;
        if(item.price > coins)
        {
            return false;
        }

        foreach (var cargo in playerOwnedCargo)
        {
            if(item.itemName == cargo.itemName)
            {
                return false;
            }
        }
       return can;
    }

    public bool playerOwnsUpgrade(UpgradesSO item)
    {
        foreach (var cargo in playerOwnedUpgrades)
        {
            if (item.itemName == cargo.itemName)
            {
                return true;
            }
        }
        return false;
    }

    public bool playerOwnsCargo(CargoSO item)
    {
        foreach (var cargo in playerOwnedCargo)
        {
            if (item.itemName == cargo.itemName)
            {
                return true;
            }
        }
        return false;
    }

    //BUY ITEM
    public void AddCargoItem(CargoSO cargo,ref float playerCoins)
    {
        playerCoins -= cargo.price;
        playerOwnedCargo.Add(cargo);
    }

    public void RemoveCargoItemFromInvetory(UpgradesSO cargo)
    {
        playerOwnedUpgrades.Remove(cargo);
    }

    public void RemoveCargoItemFromShop(UpgradesSO cargo, ref List<UpgradesSO> list)
    {
        list.Remove(cargo);
    }
}
