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
        

    }

    //FOR MOVESPEED
    //hunger
    //Thirst
    //Terrain


    //Publics 
    public void GetBackpackMax()
    {
        stats.maxVolume = currentBackpack.GetVolume();
        stats.maxWeight = currentBackpack.GetVolume();
    }
}
