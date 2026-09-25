using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public ResourceSO resourceSO;
    public int amount;
    public float averageValue;

    public InventoryItem(ResourceSO resourceSO, int amount)
    {
        this.resourceSO = resourceSO;
        this.amount = amount;
        this.averageValue = resourceSO.baseValue;

    }
    public InventoryItem(ResourceSO resourceSO, int amount, float averageValue)
    {
        this.resourceSO = resourceSO;
        this.amount = amount;
        this.averageValue = averageValue;
    }
}
public class PlayerInventory_M : MonoBehaviour
{

    private Player_M player;
    public float CurrentCoins;
    public float currentWeight;
    public float currentVolume;

    [SerializeField] public List<InventoryItem> Inventory = new List<InventoryItem>();

    private void Start()
    {
        player= GetComponent<Player_M>();
    }

    public void AddItem(ResourceSO resource, int amount = 1)
    {
        Debug.Log($"Add to inventory{resource.itemName} - {amount}");
        InventoryItem existingItem = Inventory.Find(x => x.resourceSO.itemName == resource.itemName);


        if (existingItem != null)
        {
            existingItem.amount += amount;
        }
        else
        {
            Inventory.Add(new InventoryItem(resource, amount));
        }

        HandleWeightAndVolume();
    }

    //player is buying goods
    public void tradeIn(ResourceSO resource, float buyPrice, int amount = 1)
    {
        InventoryItem existingItem = Inventory.Find(x => x.resourceSO.itemName == resource.itemName);

        if (existingItem != null)
        {
            float newAverage = (existingItem.averageValue * existingItem.amount + buyPrice * amount) / (existingItem.amount + amount);
            Debug.Log($"new average for {resource.itemName} is {newAverage}");
            existingItem.amount += amount;
            existingItem.averageValue = newAverage;
        }
        else
        {
            InventoryItem r = new InventoryItem(resource, amount, buyPrice);
            r.averageValue = buyPrice/ amount;

            Inventory.Add(r);

        }

        HandleWeightAndVolume();
        Buy(buyPrice * amount);
    }

    //player is sellign goods
    public void tradeOut(ResourceSO resource, float sellPrice, int amount = 1)
    {
        Remove(resource,amount);
        
        Sell(sellPrice * amount);
    }

    public bool hasResources(ResourceSO resource, int amount = 1)
    {
        InventoryItem existingItem = Inventory.Find(x => x.resourceSO.itemName == resource.itemName);

        if (existingItem == null)
            return false;
        else if (amount > existingItem.amount)
            return false;
        
        return true;
    }

    public void Remove(ResourceSO resource, int amount = 1)
    {
        InventoryItem existingItem = Inventory.Find(x => x.resourceSO.itemName == resource.itemName);

        if (existingItem == null) return;
        
        if (existingItem.amount > amount)
        {
            existingItem.amount -= amount;
        }
        else
        {
            Inventory.Remove(existingItem);
        }
        HandleWeightAndVolume();
    }

    public void HandleWeightAndVolume()
    {
        if (player == null)
            return;

        currentVolume = 0;
        currentWeight = 0;
        foreach (var item in Inventory)
        {

           currentVolume += item.resourceSO.volume * GetItemAmount(item);
           currentWeight += item.resourceSO.weight * GetItemAmount(item);
        }
        //Aplly Weight effect on player movement

        if (currentWeight <= player.stats.maxWeight / 2)
        {
            player.stats.currentWeightModifier= player.stats.TierIMoveModifier;
            
        }
        else if (currentWeight > player.stats.maxWeight / 2 && currentWeight <= player.stats.maxWeight * 0.75f)
        {
            player.stats.currentWeightModifier = player.stats.TierIIMoveModifier;
           
        }
        else if (currentWeight > player.stats.maxWeight * 0.75f)
        {
            player.stats.currentWeightModifier = player.stats.TierIIIMoveModifier;
            
        }
        else
        {
            Debug.Log("Speed Modifier logic problem");
        }
    }

    public int GetItemAmount(InventoryItem item)
    {

        return item.amount;
    }
    
    public bool CanCarry(float volume,float weight)
    {
        if (currentVolume + volume <= player.stats.maxVolume || currentWeight + weight<= player.stats.maxWeight)
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    //Coins funcs 

    public void Sell(float amount)
    {
        CurrentCoins += amount;
    }
    public void Buy(float amount)
    {
        CurrentCoins -= amount;
    }
}
