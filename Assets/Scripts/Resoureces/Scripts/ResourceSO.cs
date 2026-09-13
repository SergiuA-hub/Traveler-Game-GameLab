using UnityEngine;

public enum ResourceType
{
    Trade,
    Eat,
    Drink


}
[CreateAssetMenu( fileName = "ResourceSO",menuName ="ResoursceSO")]
public class ResourceSO : ScriptableObject
{

    public string itemName;
    public Sprite sprite;
    public GameObject CityDisplayPrefab;
    public ResourceType resourceType;

    public int baseValue;
    public int amount;
    public float weight;
    public float volume;
}
