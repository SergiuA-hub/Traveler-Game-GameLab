using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootOutcome", menuName = "Loot Outcome")]
public class LootOutcome : ScriptableObject
{
    public LootType outcomeForLoot;

    [TextArea(3, 6)]
    public string outcomeStory;
    public Sprite outcomeImage;

    public float outcomeChance;
    public int outcomeCoins;
    public List<ResourceDrop> lootDrop;
    public float hpLoss;
    public float staminaLoss;
    public float investigationDuration;
}

[System.Serializable]
public class ResourceDrop
{
    public ResourceSO resource;
    public int amount;
    public float dropChance;

    public ResourceDrop(ResourceSO res, int qty, float chance)
    {
        resource = res;
        amount = qty;
        dropChance = chance;
    }
}