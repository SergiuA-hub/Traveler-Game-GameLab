using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    Usable,
    Backpack,
    HandCart,
    OxCart,
    HorseCart

   

}

[CreateAssetMenu(fileName = "Item",menuName ="ItemSO")]
public class ItemsSO : ScriptableObject
{
    public string itemName;
    public Image icon;
    public ItemType type;
    public int price;

    //Backpack & Cargo 
    public float itemMaxWeight;
    public float itemMaxVolume;

    //Usable
    [Header("In case")]
    public GameObject Prefab;

}
