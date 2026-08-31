using UnityEngine;

[CreateAssetMenu(menuName ="SO",fileName ="ItemSO",order =1)]
public class ItemsSO : ScriptableObject
{
    public Sprite sprite;
    public GameObject DisplayPrefab;
    public string itemName;
    public int baseValue; 
    public int amount;
    public float weight;
    public float volume;
    
}
