using UnityEngine;
public enum EnemyStaminaState
{
    Full,
    NoHunger,
    NoThirst,
    NoStamina,
    NoHungerAndThirst
}

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

    public float hourDuration = 15f;
    public float moveCounter = 0;
    public float stationaryCounter = 0;
    public float currentStamina;

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
        DrainStatOnHour(ref stats.currentStamina);
        DrainStatOnHour(ref stats.currentThirst);
        DrainStatOnHour(ref stats.curretnHunger);
    }

    //Publics 
    public void GetBackpackMax()
    {
        stats.maxVolume = currentBackpack.GetVolume();
        stats.maxWeight = currentBackpack.GetVolume();
    }


    //Handle Stats


    private void DrainStatOnHour(ref float stat)
    {
        

            if (move.IsMoving())
            {
                moveCounter += Time.deltaTime;
                if(moveCounter>= hourDuration)
                {
                   
                    stat -= 1f;
                    ResetCounters();
                }

            }
            else
            {
                stationaryCounter += Time.deltaTime;
                
                if(stationaryCounter>= hourDuration)
                {
                     stat -= 0.2f;
                    ResetCounters();
                }
            }

        
        

        Debug.Log("Move counter: " +moveCounter);
        Debug.Log("Stationary counter: " + stationaryCounter);


    }

    private void ResetCounters()
    {
        moveCounter = 0;
        stationaryCounter = 0;
    }

    //HP
    public void TakeDamage(float damage)
    {
        stats.currentHp -= damage;
    }


    public void Heal(float amount)
    {
        stats.currentHp += amount;
    }
}
