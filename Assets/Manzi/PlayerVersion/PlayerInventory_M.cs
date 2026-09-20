using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InvetoryItem
{
    public ResourceSO resourceSO;
    public int amount;
    public float averageValue;

    public InvetoryItem(ResourceSO resourceSO, int amount)
    {
        this.resourceSO = resourceSO;
        this.amount = amount;
        this.averageValue = resourceSO.baseValue;

    }
    public InvetoryItem(ResourceSO resourceSO, int amount, float averageValue)
    {
        this.resourceSO = resourceSO;
        this.amount = amount;
        this.averageValue = averageValue;
    }
}
public class PlayerInventory_M : MonoBehaviour
{
    private Player_M player;
    public float currentWeight;
    public float currentVolume;

    [SerializeField] public List<InvetoryItem> Inventory = new List<InvetoryItem>();

    private void Start()
    {
        player= GetComponent<Player_M>();
    }

    public void AddItem(ResourceSO resource, int amount = 1)
    {

        InvetoryItem existingItem = Inventory.Find(x => x.resourceSO == resource);


        if (existingItem != null)
        {
            existingItem.amount += amount;
        }
        else
        {
            Inventory.Add(new InvetoryItem(resource, amount));
        }


    }

    public void Remove(ResourceSO resource, int amount = 1)
    {
        InvetoryItem existingItem = Inventory.Find(x => x.resourceSO == resource);

        if (existingItem == null) return;

        if (existingItem.amount > amount)
        {
            existingItem.amount -= amount;
        }
        else
        {
            Inventory.Remove(existingItem);
        }

    }

    public void HandleWeightAndVolume()
    {
        if(player == null|| player.stats == null)
        {
            return;
        }

        foreach (var item in Inventory)
        {

            currentVolume += item.resourceSO.volume * GetItemAmount(item);
           currentWeight += item.resourceSO.weight * GetItemAmount(item);
        }
        //Aplly Weight effect on player movement

        if (currentWeight > 0 && currentWeight <= player.stats.maxWeight / 2)
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

    public int GetItemAmount(InvetoryItem item)
    {

        return item.amount;
    }
    

}
