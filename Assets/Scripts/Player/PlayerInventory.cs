using Mono.Cecil;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class InvetoryItem
    {
        public ResourceSO resourceSO;
        public int amount;

        public InvetoryItem(ResourceSO resourceSO,int amount)
        {
            this.resourceSO = resourceSO;
            this.amount = amount;
        }
    }
public List<InvetoryItem> Inventory = new List<InvetoryItem>();
    

    //Adding Backpack to create a volume and weight limit based on it 

    [Header("Backpack")]
    [SerializeField] private Backpack backpack;
    [SerializeField] private float maxWeight;
    [SerializeField] private float maxVolume;
    [SerializeField] private float curretnWeight;
    [SerializeField] private float currentVolume;


    private void Start()
    {
        maxVolume = backpack.GetVolume();
        maxWeight = backpack.GetWeight();
    }

    private void Update()
    {
        foreach (InvetoryItem item in Inventory)
        {
            Debug.Log(item.amount);
        }
    }

    //ADD & REMOVE

    public void AddItem(ResourceSO resource, int amount = 1)
    {
        if (!CanCarry())
        {
            return;
        }
        InvetoryItem existingItem = Inventory.Find(x => x.resourceSO == resource);

        InvetoryItem existingItem= invetory.Find( x =>x.resourceSO = resource );

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

    //WEIGHT & VOLUME
    public void HandleWeightAndVolume()
    {
        foreach (var item in Inventory)
        {
            curretnWeight += item.itemSO.weight;
            currentVolume += item.itemSO.volume;
        }
    }

    //Check if player can add items
    public bool CanCarry()
    {
        if (currentVolume >= maxVolume || curretnWeight >= maxWeight)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    

}
