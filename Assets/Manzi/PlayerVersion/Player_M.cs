using UnityEngine;


public class Player_M : MonoBehaviour
{
    //Backpack
    [SerializeField] private Backpack currentBackpack;


    //Components
    public PlayerStats_M stats;
    public PlayerMovement_M move;
    public PlayerInvetory_M invetory;
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
        invetory = GetComponent<PlayerInvetory_M>();
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
        DrainStatOnHour(ref stats.currentStamina,stats.currentStaminaDrainMultiplier,stats.StationanryDrainMultiplier);
        DrainStatOnHour(ref stats.currentThirst,stats.thirstDrainMultiplier,stats.thirstDrainMultiplier);
        DrainStatOnHour(ref stats.curretnHunger,stats.hungerDrainMultiplier, stats.hungerDrainMultiplier);
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
        //in functie de hunger si thirst trebuie sa calculam currentStaminaDrainModifier
        //daca sunt bune ala ramane 1 daca nu e se adauga o penalitate
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
