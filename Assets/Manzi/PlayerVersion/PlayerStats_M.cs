using UnityEngine;

public class PlayerStats_M : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float startSpeed;
    public float acceleration;
    public float MaxSpeeed;

    [Header("SpeedModifier")]
    public float staminaSpeedModifier;
    public float terrainSpeedModifier;
    public float consumableSpeedModifier;

    [Header("Weight speed modifier")]
    public float TierIMoveModifier;
    public float TierIIMoveModifier;
    public float TierIIIMoveModifier;

    [Header("Stamina")]
    public float maxStamina;
    public float staminaDrainMultiplier;
    
    [Header("Consumable")]
    public float maxHunger;
    public float maxThirst;
    public int maxHp;

    [Header("Invetory")]
    public float maxVolume;
    public float maxWeight;
    

    //Components





}
