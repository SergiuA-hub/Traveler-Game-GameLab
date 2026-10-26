using UnityEngine;
using UnityEngine.UI;

public enum CargoSOType
{
    Item,
    Upgrade
}

[CreateAssetMenu(fileName = "Cargo",menuName ="CargoSO")]
public class CargoSO : ScriptableObject
{
    [Header("Cargo info")]
    public string itemName;
    public Sprite icon;
    public int price;
    public int level;
    public CargoSOType type;
    public string Description;
    public string[] features;

    [Header("Stats")]
    public float itemMaxWeight;
    public float itemMaxVolume;

    //Speed modifiers

    //Usable
    [Header("Player visual")]
    public GameObject Prefab;

}
