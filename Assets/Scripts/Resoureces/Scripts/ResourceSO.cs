using UnityEngine;

public enum ResourceType
{
    Goods,
    Eat,
    Drink


}
[CreateAssetMenu(menuName = "SO", fileName = "ResourceSO", order = 1)]
public class ResourceSO : ScriptableObject
{

    public string itemName;
    public Sprite sprite;
    public GameObject CityDisplayPrefab;
    public ItemType itemType;

    public int baseValue;
    public int amount;
    public float weight;
    public float volume;
}
