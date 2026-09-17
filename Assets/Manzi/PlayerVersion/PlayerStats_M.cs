using UnityEngine;

public class PlayerStats_M : MonoBehaviour
{
    [Header("Coins")]
    public int playerCoins;

    [Header("Movement")]
    public float moveSpeed;
    public float baseSpeed = 1f; 
    public float startSpeed;
    public float acceleration;
    public float MaxSpeeed;
    public float exhaustSpeed;

    [Header("SpeedModifier")]
    public float staminaSpeedModifier;
    public float terrainSpeedModifier;
    public float consumableSpeedModifier;

    [Header("Weight speed modifier")]
    public float currentWeightModifier;
    public float TierIMoveModifier;
    public float TierIIMoveModifier;
    public float TierIIIMoveModifier;

    [Header("Stamina")]
    public float maxStamina;
    public float currentStaminaDrainMultiplier;


    [Header("Drain Modifiers")]
    //stamina
    public float staminaDrainMultiplier;
    public float StationanryDrainMultiplier;
    

    //Thirts
    public float thirstDrainMultiplier;
    

    //hunger
    public float hungerDrainMultiplier;

    [Header("Consumable_CURRENT")]
    public float currentStamina;
    public float currentHunger;
    public float currentThirst;
    public float currentHp;

    [Header("Consumable_MAX")]
    public float maxHunger;
    public float maxThirst;
    public int maxHp;

    [Header("Invetory")]
    public float maxVolume;
    public float maxWeight;


    public void Start()
    {
        currentHp = maxHp;
        currentHunger = maxHunger;
        currentThirst = maxThirst;
        currentStamina = maxStamina;

    }

    


}
