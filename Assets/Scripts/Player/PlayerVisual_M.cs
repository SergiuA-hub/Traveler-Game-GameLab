using UnityEngine;
using UnityEngine.UI;


public class PlayerVisual_M : MonoBehaviour
{
    public Player_M player;
    private PlayerMovement_M playerMovement;
    Animator anim;
    int currentState; // 0: idle, 1: moving
    private void Start()
    {
      //  player = GetComponent<Player_M>();
        playerMovement = player.gameObject.GetComponent<PlayerMovement_M>();
        currentState = 0;
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (playerMovement.IsMoving() && currentState !=1)
        {
            anim.SetTrigger("IsWalking");
            currentState = 1;
        }
        if (!playerMovement.IsMoving() && currentState != 0)
        {
            anim.SetTrigger("IsIdle");
            currentState = 0;
        }
    }

}
