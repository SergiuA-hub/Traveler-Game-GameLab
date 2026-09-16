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
    private void Update()
    {
        
    }
    private void FixedUpdate()
    {
        HandleMovement();
    }
    private void HandleMovement()
    {

        if (IsMoving() && HasStamina())
        {
            player.stats.moveSpeed = Mathf.MoveTowards(player.stats.moveSpeed, player.stats.MaxSpeeed, player.stats.acceleration * Time.deltaTime);
        }
        else if(!IsMoving() && HasStamina())
        {
            player.stats.moveSpeed = player.stats.startSpeed;
        }else if (!HasStamina())
        {
            player.stats.moveSpeed = player.stats.exhaustSpeed;
        }

        float modifierSpeed = player.stats.staminaSpeedModifier * player.stats.consumableSpeedModifier * player.stats.terrainSpeedModifier * player.stats.currentWeightModifier;
        
        player.stats.moveSpeed *= modifierSpeed;
        Vector2 moveInput = player.gameInput.GetMoveVectorNormalized();
        
        if(player.rooted)
        {
            moveInput = Vector2.zero;
        }
        player.rb.MovePosition(player.rb.position + moveInput * player.stats.moveSpeed * Time.fixedDeltaTime);

    }
  
    
    public bool HasStamina()
    {
        if (player.stats.currentStamina <= 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //Publics
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
