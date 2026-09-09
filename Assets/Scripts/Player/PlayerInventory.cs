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
    

    

    [Header("Backpack")]
    [SerializeField] private Backpack currentBackpack;
    [SerializeField] private float maxWeight;
    [SerializeField] private float maxVolume;
    [SerializeField] private float currentWeight;
    [SerializeField] private float currentVolume;

    [Header("Movement modifier V/W ")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float TierIMoveModifier;

    [SerializeField] private float TierIIMoveModifier;
    [SerializeField] private float TierIIIMoveModifier;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        maxVolume = currentBackpack.GetVolume();
        maxWeight = currentBackpack.GetWeight();
        HandleWeightAndVolume();
    }

    private void Update()
    {
        
    }

    //ADD & REMOVE

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

    //WEIGHT & VOLUME
    public void HandleWeightAndVolume()
    {
        

        foreach (var item in Inventory)
        {
                     
            currentVolume += item.resourceSO.volume * GetItemAmount(item);
            currentWeight += item.resourceSO.weight * GetItemAmount(item);
        }
        //Aplly Weight effect on player movement

        if(currentWeight> 0 && currentWeight<= maxWeight/ 2)
        {
            playerMovement.speedModifier = TierIMoveModifier;
        }
        else if(currentWeight > maxWeight/ 2 && currentWeight <= maxWeight* 0.75f)
        {
            playerMovement.speedModifier= TierIIMoveModifier;
        }
        else if (currentWeight > maxWeight *0.75f)
        {
            playerMovement.speedModifier = TierIIIMoveModifier;
        }
        else
        {
            Debug.Log("Speed Modifier logic problem");
        }
    }

    //Check if player have space for new items
    public bool CanCarry(float addVolume,float addWeight)
    {
        if (currentVolume + addVolume > maxVolume || currentWeight + addWeight > maxWeight)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public int GetItemAmount(InvetoryItem item)
    {

        return item.amount;
    }
    

}
