using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class CargoManager : MonoBehaviour
{
    public static CargoManager instance { get; private set; }

    public List<Cargo> PlayerOwnedCargo = new List<Cargo>();

    public List<Cargo> PlayerOwnedUpgrades = new List<Cargo>();
    public Cargo currentCargoSelected;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    
  

    
    //######
    // ITEMS
    //######
    public bool CanBuy(Cargo item,float coins)
    {
       bool can= false;
       if(coins >= item.cargoSO.price)
        {
            foreach(var cargo in PlayerOwnedCargo)
            {
                if(item.cargoSO == cargo.cargoSO)
                {
                    can = false;
                    return false;
                }
                else
                {
                    can = true;
                }
            }
        }
        else
        {
            //cant buy
            can=  false;
        }
       return can;

       
    }

    //BUY ITEM
    public void AddCargoItem(Cargo cargo,ref float playerCoins)
    {
        playerCoins -= cargo.cargoSO.price;
        PlayerOwnedCargo.Add(cargo);
    }
    public void AddUpgradeItem(Cargo cargo, ref float playerCoins)
    {
        if (playerCoins < cargo.cargoSO.price)
        {
            return;
        }
        playerCoins -= cargo.cargoSO.price;
        PlayerOwnedUpgrades.Add(cargo);

    }
    public void RemoveCargoItemFromInvetory(Cargo cargo)
    {
        PlayerOwnedUpgrades.Remove(cargo);
    }

    public void RemoveCargoItemFromShop(Cargo cargo, ref List<Cargo> list)
    {
        list.Remove(cargo);
    }

    //######
    // UPGRADES
    //######



}
