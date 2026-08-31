using UnityEngine;
public enum ItemType
{
    Goods,
    Eat,
    Drink
    
}
[CreateAssetMenu(menuName ="SO",fileName ="ItemSO",order =1)]
public class ItemsSO : ScriptableObject
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
