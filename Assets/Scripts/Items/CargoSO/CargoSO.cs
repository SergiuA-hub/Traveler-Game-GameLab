using UnityEngine;

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
    public float price;
    public int upgradesSpaces;
    public CargoCategory category;
    public CargoSOType type;
    public string Description;
    public string[] features;

    [Header("Stats")]
    public float itemMaxWeight;
    public float itemMaxVolume;

    

    //Usable
    [Header("Player visual")]
    public GameObject Prefab;

}