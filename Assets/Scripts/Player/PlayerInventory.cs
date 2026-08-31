using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerInventory : MonoBehaviour
{
    public List<ItemsSO> Invetory;

    //Adding Backpack to create a volume and weight limit based on it 


    [SerializeField] private float maxWeight;
    [SerializeField] private float maxVolume;
    [SerializeField] private float curretnWeight;
    [SerializeField] private float currentVolume;


    private void Start()
    {
        CheckWeightAndVolume();
    }

    //ADD & REMOVE

    public void AddItem(ItemsSO item)
    {
        Invetory.Add(item);
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
