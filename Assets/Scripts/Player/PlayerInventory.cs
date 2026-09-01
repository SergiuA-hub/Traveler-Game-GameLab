using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class InvetoryItem
    {
        public ItemsSO itemSO;
        public int amount;
        
    }
    public List<InvetoryItem> Inventory = new List<InvetoryItem>();
    

    //Adding Backpack to create a volume and weight limit based on it 

    [Header("Backpack")]
    [SerializeField] private float maxWeight;
    [SerializeField] private float maxVolume;
    [SerializeField] private float curretnWeight;
    [SerializeField] private float currentVolume;


    private void Start()
    {
        CheckWeightAndVolume();
    }

    private void Update()
    {
        foreach (InvetoryItem item in Inventory)
        {
            Debug.Log(item.amount);
        }
    }

    //ADD & REMOVE

    public void AddItem(ItemsSO item,int amount =1 )
    {
        InvetoryItem existingItem = Inventory.Find(x => x.itemSO ==  item);

        if(existingItem != null)
        {
            existingItem.amount += amount;
        }
        else
        {
            Inventory.Add(new InvetoryItem{itemSO = item,amount = amount});
        }


    }

    public void RemoveItem(ItemsSO item, int amount = 1)
    {
        InvetoryItem existingItem = Inventory.Find(x => x.itemSO == item);

        if(existingItem.amount > 1)
        {
            existingItem.amount -= 1;

        }if (existingItem.amount == 1)
        { 
            Inventory.Remove(existingItem);
        }


    }

    //WEIGHT & VOLUME
    public void CheckWeightAndVolume()
    {
        foreach (var item in Inventory)
        {
            curretnWeight += item.itemSO.weight;
            currentVolume += item.itemSO.volume;
        }
    }

    public bool CanCarry()
    {
        if(currentVolume>= maxVolume && curretnWeight >= maxWeight)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    

}
