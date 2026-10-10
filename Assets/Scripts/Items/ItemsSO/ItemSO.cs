using UnityEngine;
using UnityEngine.UI;



[CreateAssetMenu(fileName = "Item",menuName ="ItemSO")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Image icon;
    public int price;

    //Backpack & Cargo 
    public float itemMaxWeight;
    public float itemMaxVolume;

    //Usable
    [Header("In case")]
    public GameObject Prefab;

}
