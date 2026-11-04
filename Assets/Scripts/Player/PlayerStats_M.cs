using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStats_M : MonoBehaviour
{
   

    [Header("Movement")]
    public float moveSpeed;
    public float currentSpeed;
    public float baseSpeed = 1f;
    public float startSpeed;
    public float acceleration;
    public float MaxSpeeed;
    public float exhaustSpeed;
    //maxBaseSpeed used to limit max speed based on time speed
    private float maxBaseSpeed = 1f;
    [Header("SpeedModifier")]
    public float staminaSpeedModifier;
    public float terrainSpeedModifier;
    public float consumableSpeedModifier;
    public float WEATHER_SPEED_MODIFIER;

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
    public float STAMINA_DRAIN_MULTIPLIER;
    public float WEATHER_STAMINA_DRAIN_MODIFIER;
    public float BASE_STAMINA_DROP_PER_H;
    public float BASE_STAMINA_IDLE_DRAIN;
    public float BASE_THIRST_IDLE_DRAIN;
    
    public float HUNGER_PENALTY;
    public float THIRST_PENALTY;

    //Thirts
    public float THIRST_DRAIN_PER_H;    
    //hunger
    public float HUNGER_DRAIN_PER_H;

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

    private Player_M player;
    private void Awake()
    {
        player = GetComponent<Player_M>();
    }
    
    public void Start()
    {
        currentHp = maxHp;
        currentHunger = maxHunger;
        currentThirst = maxThirst;
        currentStamina = maxStamina;

        maxBaseSpeed = GlobalSettingsManager.PLAYER_BASE_SPEED;

        currentStaminaDrainMultiplier = STAMINA_DRAIN_MULTIPLIER;

        setMaxSpeed(GlobalSettingsManager.HOUR_DURATION);
        GetItemValues(player.attachments.currentItemSelected);

    }

    public void setMaxSpeed(float timeModifier)
    {
        MaxSpeeed = maxBaseSpeed * (GlobalSettingsManager.HOUR_DURATION / timeModifier);
    }

    private void GetItemValues(Cargo cargo)
    {
        maxVolume = cargo.cargoSO.itemMaxVolume;
        maxWeight = cargo.cargoSO.itemMaxWeight;
    }
    public void setWeatherModifiers(WeatherType weather)
    {
        switch (weather)
        {
            case WeatherType.Clear:
                WEATHER_SPEED_MODIFIER = 1f;
                WEATHER_STAMINA_DRAIN_MODIFIER = 1f;
                break;

            case WeatherType.Rain:
                WEATHER_SPEED_MODIFIER = 0.95f;
                WEATHER_STAMINA_DRAIN_MODIFIER = 1.05f;
                break;

            case WeatherType.Storm:
                WEATHER_SPEED_MODIFIER = 0.8f;
                WEATHER_STAMINA_DRAIN_MODIFIER = 1.25f;
                break;

            case WeatherType.Snow:
                WEATHER_SPEED_MODIFIER = 0.95f;
                WEATHER_STAMINA_DRAIN_MODIFIER = 1.1f;
                break;

            case WeatherType.Blizzard:
                WEATHER_SPEED_MODIFIER = 0.7f;
                WEATHER_STAMINA_DRAIN_MODIFIER = 1.4f;
                break;
            default:
                WEATHER_SPEED_MODIFIER = 1f;
                WEATHER_STAMINA_DRAIN_MODIFIER = 1f;
                break;
        }
    }
}
