using UnityEngine;


public class Player_M : MonoBehaviour
{
    //Backpack
    [SerializeField] private Backpack currentBackpack;

    public bool isResting = false;
    public bool rooted = false;

    //Components
    public PlayerStats_M stats;
    public PlayerMovement_M move;
    public PlayerInventory_M invetory;
    public PlayerVisual_M visual;
    public Rigidbody2D rb;
    public GameInput gameInput;
    public TimeManager timeManager;      
    
    public float moveCounter = 0;
    public float stationaryCounter = 0;
    

    private void Awake()
    {
        //Player Components
        stats = GetComponent<PlayerStats_M>();
        move = GetComponent<PlayerMovement_M>();
        invetory = GetComponent<PlayerInventory_M>();
        visual = GetComponent<PlayerVisual_M>();

        //OtherComponents
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void Start()
    {
        GetBackpackMax();
        
    }
    

    private void Update()
    {
        invetory.HandleWeightAndVolume();
        if (timeManager.time_stopped)
            return;

        if(isResting)
            return;

        if (move.IsMoving() && !rooted)
        {
            stats.currentStamina -= (stats.BASE_STAMINA_DROP_PER_H * stats.currentStaminaDrainMultiplier / GlobalSettingsManager.HOUR_DURATION) * Time.deltaTime;            
            stats.currentThirst -= (stats.THIRST_DRAIN_PER_H / GlobalSettingsManager.HOUR_DURATION) * Time.deltaTime;
        }
        else
        {
            stats.currentStamina -= (stats.BASE_STAMINA_IDLE_DRAIN * stats.currentStaminaDrainMultiplier / GlobalSettingsManager.HOUR_DURATION) * Time.deltaTime;
            stats.currentThirst -= (stats.BASE_THIRST_IDLE_DRAIN / GlobalSettingsManager.HOUR_DURATION) * Time.deltaTime;
        }

        //not related to movement, but still needs to be updated
        stats.currentHunger -= (stats.HUNGER_DRAIN_PER_H / GlobalSettingsManager.HOUR_DURATION) * Time.deltaTime;

        //update stamina drain multiplier based on hunger and thirst thresholds
        if (stats.currentThirst < GlobalSettingsManager.PLAYER_THIRST_THRESHOLD || stats.currentHunger < GlobalSettingsManager.PLAYER_HUNGER_THRESHOLD)
        {
            float thirsP = stats.currentThirst<GlobalSettingsManager.PLAYER_THIRST_THRESHOLD ? stats.THIRST_PENALTY : 0f;
            float hungerP = stats.currentHunger < GlobalSettingsManager.PLAYER_HUNGER_THRESHOLD ? stats.HUNGER_PENALTY : 0f;

            stats.currentStaminaDrainMultiplier = stats.STAMINA_DRAIN_MULTIPLIER + thirsP + hungerP;
        }else stats.currentStaminaDrainMultiplier = stats.STAMINA_DRAIN_MULTIPLIER;

        stats.currentStamina = Mathf.Max(0, stats.currentStamina);
        stats.currentHunger = Mathf.Max(0, stats.currentHunger);
        stats.currentThirst = Mathf.Max(0, stats.currentThirst);
    }

    //Publics 
    public void GetBackpackMax()
    {
        stats.maxVolume = currentBackpack.GetVolume();
        stats.maxWeight = currentBackpack.GetVolume();
    }

    // ------HP------ 
    public void TakeDamage(float damage)
    {
        stats.currentHp -= damage;
    }


    public void Heal(float amount)
    {
        stats.currentHp += amount;
    }
}
