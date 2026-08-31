using UnityEngine;
using UnityEngine.Rendering;

public class PlayerInventory : MonoBehaviour
{
    public ItemsSO[] Invetory;

    //Adding Backpack to create a volume and weight limit based on it 


    [SerializeField] private float maxWeight;
    [SerializeField] private float maxVolume;
    [SerializeField] private float curretnWeight;
    [SerializeField] private float currentVolume;


    private void Start()
    {
        CheckWeightAndVolume();
    }

    public void CheckWeightAndVolume()
    {
        foreach (var item in Invetory)
        {
            curretnWeight += item.weight;
            currentVolume += item.volume;
        }
    }
    

}
