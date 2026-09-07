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
    public List<InvetoryItem> invetory = new List<InvetoryItem>();
    

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

    }

    //ADD & REMOVE

    public void AddItem(ResourceSO resource, int amount = 1)
    {
        if (!CanCarry())
        {
            return;
        }

        InvetoryItem existingItem= invetory.Find( x =>x.resourceSO = resource );

        if (existingItem != null)
        {
            existingItem.amount += amount;
        }
        else
        {
            invetory.Add(new InvetoryItem(resource,amount));
        }


    }

    public void Remove(ResourceSO resource, int amount = 1)
    {
        InvetoryItem existingItem = invetory.Find(x => x.resourceSO = resource);

        if (existingItem.amount ==1)
        {
            //Sterge acest item
            invetory.Remove(existingItem);
        }

        if(existingItem.amount >1)
        {
            existingItem.amount -= amount;
        }

    }

    //WEIGHT & VOLUME
    public void HandleWeightAndVolume()
    {

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
