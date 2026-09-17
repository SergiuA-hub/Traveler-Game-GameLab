using UnityEngine;


public class Player_M : MonoBehaviour
{
    //Backpack
    [SerializeField] private Backpack currentBackpack;


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
        invetory.HandleWeightAndVolume();
    }

    private void Update()
    {
        //Drain stats on hour, based of movement
        DrainStatOnHour(ref stats.currentStamina,stats.staminaDrainMultiplier,stats.StationanryDrainMultiplier);
        DrainStatOnHour(ref stats.currentThirst,stats.thirstDrainMultiplier,stats.thirstDrainMultiplier);
        DrainStatOnHour(ref stats.currentHunger,stats.hungerDrainMultiplier, stats.hungerDrainMultiplier);

        //CurrentStaminaDrainModifier
        CalculateStaminaDrain();


    }

    //Publics 
    public void GetBackpackMax()
    {
        stats.maxVolume = currentBackpack.GetVolume();
        stats.maxWeight = currentBackpack.GetVolume();
    }


    //Handle Stats


    private void DrainStatOnHour(ref float stat,float drain,float idleDrain)
    {
        

            if (move.IsMoving())
            {
                moveCounter += Time.deltaTime;
                if(moveCounter>= timeManager.hour_duration)
                {
                   
                    stat -= drain;
                    ResetCounters();
                }

            }
            else
            {
                stationaryCounter += Time.deltaTime;
                
                if(stationaryCounter>= timeManager.hour_duration)
                {
                     stat -= idleDrain;
                    ResetCounters();
                }
            }

    }
    private void CalculateStaminaDrain()
    {
        stats.currentStaminaDrainMultiplier *= 1 ;
        if(stats.currentThirst <= 0 || stats.currentHunger <= 0)
        {
            stats.currentStaminaDrainMultiplier *= stats.hungerDrainMultiplier + stats.thirstDrainMultiplier;
        }

    }

    private void ResetCounters()
    {
        moveCounter = 0;
        stationaryCounter = 0;
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
