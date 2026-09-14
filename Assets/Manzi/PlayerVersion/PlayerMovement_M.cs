using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement_M : MonoBehaviour
{
    private Player_M player;


   
    private void Start()
    {
        player = GetComponent<Player_M>();
        
    }
    
    private void FixedUpdate()
    {
        HandleMovement();
    }
    private void HandleMovement()
    {

        if (IsMoving())
        {
            player.stats.moveSpeed = Mathf.MoveTowards(player.stats.moveSpeed, player.stats.MaxSpeeed, player.stats.acceleration * Time.deltaTime);
        }
        else
        {
            player.stats.moveSpeed = player.stats.startSpeed;
        }

        float modifierSpeed = player.stats.staminaSpeedModifier * player.stats.consumableSpeedModifier * player.stats.terrainSpeedModifier * player.stats.currentWeightModifier;
        
        player.stats.moveSpeed *= modifierSpeed;
        Vector2 moveInput = player.gameInput.GetMoveVectorNormalized();

        player.rb.MovePosition(player.rb.position + moveInput * player.stats.moveSpeed * Time.fixedDeltaTime);

    }

    public bool IsMoving()
    {
        Vector2 inputMove = player.gameInput.GetMoveVectorNormalized();
        if (inputMove.magnitude > 0)
        {
            return true;

        }
        else
        {
            return false;
        }
    }
}
