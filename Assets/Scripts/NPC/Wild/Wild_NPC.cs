using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
enum Wild_NPC_State
{
    Patrol,
    Trigger,
    Chase,
    Return,

}
public class Wild_NPC : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float maxStamina;
    [SerializeField] private float currentStamina;
    [SerializeField] private float restoreStamina;
    [SerializeField] private Wild_NPC_State currentState;

    [SerializeField] private float triggerArea;
    [SerializeField] private float ChaseArea;
    [SerializeField] private Collider2D patrolArea;

    private void Awake()
    {
        currentState = Wild_NPC_State.Patrol;
    }
    private void Update()
    {
        switch (currentState)
        {
            case Wild_NPC_State.Patrol:
                Patrol();
                break;

            case Wild_NPC_State.Trigger:
                break;

            case Wild_NPC_State.Chase: 
                break;
            
            case Wild_NPC_State.Return:
                break;

                default:
                Debug.LogError("No state is set");
                break;
        }

    }

    private void Patrol()
    {
        //In this state needs to patrol the area
        
        
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(Physics2D.OverlapCircle(transform.position, triggerArea).gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //If on trigger area enter triger state and start movint to player
            //TO DO: add a timer if timer = 0 return to patrol 
            //

        }
    }
    private void ChangeState(Wild_NPC_State newState)
    {
        currentState = newState;
    }
}
