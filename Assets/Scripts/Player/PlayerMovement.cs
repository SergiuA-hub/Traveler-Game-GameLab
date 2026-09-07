using UnityEngine;

public enum PlayerMoveState
{
    Stay,
    Moving,
    Running,
    OnCart
}

public class PlayerMovement : MonoBehaviour
{
    //Character Stats SO
    [Header("Movement")]

    //Make them private
    public float currentSpeed;
    public float moveSpeed;
    public float startSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float MaxSpeeed;
    public float speedModifier;

    [Header("Stamina")]
    [SerializeField] private float currentStamina;
    public float maxStamina;
    [SerializeField] private float staminaDrainMultiplier;


    //LATER
    public Vector2 moveInput;
    public PlayerMoveState currentState;


    [Header("Components")]
    [SerializeField] private GameInput gameInput;
    private PlayerInventory playerInventory;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInventory = GetComponent<PlayerInventory>();
        currentStamina = maxStamina;
    }

    private void Update()
    {
        //Decrease Stamina on moving 
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        
        if (IsMoving())
        {
            moveSpeed = Mathf.MoveTowards(moveSpeed, MaxSpeeed, acceleration * Time.deltaTime);
        }
        else
        {
            moveSpeed = startSpeed;
        }
        

        currentSpeed = moveSpeed * speedModifier;

        moveInput = gameInput.GetMoveVectorNormalized();

        rb.MovePosition(rb.position + moveInput * currentSpeed * Time.fixedDeltaTime);

    }

    
    public bool IsMoving()
    {
        Vector2 inputMove = gameInput.GetMoveVectorNormalized();
        if(inputMove.magnitude > 0)
        {
            return true;
            
        }
        else{
            return false;
        }
    }

    private void RestoreStamina()
    {
        currentStamina = maxStamina;
    }


}
