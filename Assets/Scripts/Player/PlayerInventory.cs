using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class InvetoryItem
    {
        public ItemsSO itemSO;
        public int amount;
        
    }
    public List<InvetoryItem> invetori = new List<InvetoryItem>();
    public List<ItemsSO> Invetory;

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
        foreach (ItemsSO item in Invetory)
        {
            Debug.Log(item.amount);
        }
    }

    //ADD & REMOVE

    public void AddItem(ItemsSO item,int amount =1 )
    {
        ItemsSO existingItem = Invetory.Find(x => x ==  item);

        if(existingItem != null)
        {
            existingItem.amount += amount;
        }
        else
        {
            Invetory.Add(item);
        }

        
    }

    public void Remove(ItemsSO item)
    {
        Invetory.Remove(item);
    }

    //WEIGHT & VOLUME
    public void CheckWeightAndVolume()
    {
        foreach (var item in Invetory)
        {
            curretnWeight += item.weight;
            currentVolume += item.volume;
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
