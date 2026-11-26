
using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

enum Wild_NPC_State
{
    Idle,
    Patrol,
    Attack,
    JumpAttack,
    Chase,
    Return,

}
public class Wild_NPC : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintSpeed;
    [Header("Behaivior")]
    [SerializeField] private bool interested = true;

    
    [Header("Idle")]
    [SerializeField] private float timeToIdle = 2f;
    
    [Header("Patrol")]
    [SerializeField] private Vector2 targetPos;
    [SerializeField] private bool isMoving;
    [SerializeField] private float jumpAttackTimer;

    
    [Header("Attack")]
    [SerializeField] private bool isAttcking;
    [SerializeField] private float jumpAttackCharge;
    //Secundele inainte sa sara
    [SerializeField] private float prepareJumpAttackTime;
    [SerializeField] private Coroutine jumpCoroutine;

    [Header("Components")]
    [SerializeField] private Wild_NPC_State currentState;
    [SerializeField] private ParticleSystem chargeVFX;
    [SerializeField] private ParticleSystem attackVFX;
    [SerializeField] private SpriteRenderer npcRenderer;


    [Header("Areas")]
    [SerializeField] private float attackRange;
    [SerializeField] private float chaseDistance;
    [SerializeField] private Collider2D patrolArea;

    //Test

    private void Awake()
    {
        currentState = Wild_NPC_State.Idle;
       
    }
    private void Update()
    {
        
        switch (currentState)
        {
            case Wild_NPC_State.Idle:
                Idle();
                break;

            case Wild_NPC_State.Patrol:
                Patrol();
                break;

            case Wild_NPC_State.Attack:
                Attack();
                break;

            case Wild_NPC_State.JumpAttack:
                SpecialAttack();
                break;

            case Wild_NPC_State.Chase: 
                Chase();
                break;
            
            case Wild_NPC_State.Return:
                Return();
                break;

                default:
                Debug.LogError("No state is set");
                break;
        }

    }
    private void Idle()
    {

        if (DistanceFromPlayer() <= chaseDistance)
        {
            ChangeState(Wild_NPC_State.Chase);
        }
        isMoving = false;
      
        if (timeToIdle > 0f)
        {
            
            timeToIdle -= Time.deltaTime;

        }
        else
        {
            ChangeState(Wild_NPC_State.Patrol);
           
        }
       
    }
    private void Patrol()
    {
        if(DistanceFromPlayer()<= chaseDistance)
        {
            ChangeState(Wild_NPC_State.Chase);
        }
        if (!isMoving)
        {
            targetPos = GetRandomPoint();
        }

        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        isMoving = true;
        if (Vector2.Distance(transform.position, targetPos) < 0.5f)
        {
            timeToIdle = 2f;
            ChangeState(Wild_NPC_State.Idle);
        }
    }

    private void Chase()
    {
        isMoving = false;
        //Daca nu este interesat nu il urmareste
        if (!interested)
        {
            ChangeState(Wild_NPC_State.Return);
        }

        if (DistanceFromPlayer() >= chaseDistance )
        {
            ChangeState(Wild_NPC_State.Return);
        }
        if(DistanceFromPlayer() <= attackRange)
        {
            ChangeState (Wild_NPC_State.Attack);
        }
        if(jumpAttackTimer >= jumpAttackCharge)
        {
            ChangeState(Wild_NPC_State.JumpAttack);
        }
        //Chase the player
        Vector3 playerPos = GameServices.Instance.player.transform.position;
        transform.position = Vector2.MoveTowards(transform.position, playerPos, moveSpeed * Time.deltaTime);
        
        //Chanell Charge Attack
        jumpAttackTimer += Time.deltaTime;
       
    }

    private void Attack()
    {
        jumpAttackTimer = 0;
        GameServices.Instance.player.spriteContainer.gameObject.SetActive(false);
        npcRenderer.gameObject.SetActive(false);
        GameServices.Instance.player.Root();
        attackVFX.Play();

        StartCoroutine(WaitforVFX());
        //After the vfx
        
        GameServices.Instance.player.spriteContainer.gameObject.SetActive(true);
        npcRenderer.gameObject.SetActive(true);
        GameServices.Instance.player.UnRoot();



        ChangeState(Wild_NPC_State.Return);

    }

    private void SpecialAttack()
    {
        //Wait a second and start VFX
        chargeVFX.Play();
        jumpCoroutine = StartCoroutine(Wait());
        transform.position = Vector3.MoveTowards(transform.position, GameServices.Instance.player.transform.position, sprintSpeed * Time.deltaTime);
        jumpCoroutine = StartCoroutine(Wait());
        if (DistanceFromPlayer() <= attackRange)
        {
            ChangeState (Wild_NPC_State.Attack);
        }
        //Start minigame to A AND D
    }

    private void Return()
    {
        if(transform.position.x == targetPos.x && transform.position.x == targetPos.x)
        {
            ChangeState(Wild_NPC_State.Idle);
        }
        jumpAttackTimer = 0;
        if (!isMoving)
        {
            targetPos = GetRandomPoint();
            targetPos.y += 5;
            targetPos.x += 5;

            isMoving = true;
        }
        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }


    
    private void ChangeState(Wild_NPC_State newState)
    {
        currentState = newState;
    }

    
 
    private Vector2 GetRandomPoint()
    {
        Bounds bounds = patrolArea.bounds;
        for (int i = 0; i < 50; i++)
        {
            Vector2 p = new Vector2(Random.Range(bounds.min.x, bounds.max.x),
                                    Random.Range(bounds.min.y, bounds.max.y));

            if (patrolArea.OverlapPoint(p)) return p;
        }
        return bounds.center;
    }

    private float DistanceFromPlayer()
    {
        Vector3 playerPos = GameServices.Instance.player.transform.position;
        float target = Vector3.Distance(playerPos , transform.position);
        return target;
    }

    //Pentru special attack
    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(prepareJumpAttackTime);
    }

    //Pentru VFX Attack
    private IEnumerator WaitforVFX()
    {
        yield return new WaitWhile(() => attackVFX.IsAlive(true));
    }
}
