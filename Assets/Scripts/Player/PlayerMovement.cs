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

    public Vector2 moveInput;
    public PlayerMoveState currentState;


    [Header("Components")]
    [SerializeField] private GameInput gameInput;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
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




}
