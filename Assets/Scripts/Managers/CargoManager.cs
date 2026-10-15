using System;
using System.Collections.Generic;

using UnityEngine;

public class CargoManager : MonoBehaviour
{
    public static CargoManager instance { get; private set; }

    public List<Cargo> PlayerOwnedCargo = new List<Cargo>();
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
    
    public int GetHighestOwnedLevel(CargoCategory type) 
    {
        int highest = 0;

        foreach(var item in PlayerOwnedCargo)
        {
            if( item != null && item.itemType== type && item.cargoSO.level > highest)
            {
                highest = item.cargoSO.level;
            }
        }

            return highest;

    }

    //Buy conditions : Upgrade
    public Cargo GetOwnedCargo(CargoSO so)  
    {
        if (so == null) return null;
        return PlayerOwnedCargo.Find(x => x != null && x.cargoSO == so);
    }

    public bool CanBuy(Cargo item)
    {
        bool alreadyOwned = PlayerOwnedCargo.Exists(x => x != null && x.cargoSO == item.cargoSO);
        if (alreadyOwned) return false;

        return item.cargoSO.level == GetHighestOwnedLevel(item.itemType) + 1;
    }
    //BUY ITEM
    public void AddItem(Cargo cargo,ref float playerCoins)
    {
        playerCoins -= cargo.cargoSO.price;
        PlayerOwnedCargo.Add(cargo);
        cargo.itemStatus = Status.Owned;
    }

    //BUY UPGRADE


    

}
