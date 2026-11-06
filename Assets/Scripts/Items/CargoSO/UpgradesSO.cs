using UnityEngine;

[CreateAssetMenu(fileName = "Cargo Upgrade", menuName = "UpgradeSO")]
public class UpgradesSO : ScriptableObject
{
    [Header("Cargo info")]
    public string itemName;
    public Sprite icon;
    public int price;
    public CargoCategory category;
    public string Description;
    public string[] features;

    [Header("Stats")]
    public float additionalWeight;
    public float additionalVolume;
}
