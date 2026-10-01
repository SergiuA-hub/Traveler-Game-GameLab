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
        Vector2 moveInput = player.gameInput.GetMoveVectorNormalized();

        if (player.isRooted())
            moveInput = Vector2.zero;

        bool isMoving = moveInput.sqrMagnitude > 0.001f;

        // Base speed calculation
        if (!HasStamina())
        {
            player.stats.baseSpeed = player.stats.exhaustSpeed;
        }
        else if (isMoving)
        {
            player.stats.baseSpeed = Mathf.MoveTowards(player.stats.baseSpeed, player.stats.MaxSpeeed, player.stats.acceleration * Time.fixedDeltaTime);
        }
        else
        {
            player.stats.baseSpeed = player.stats.startSpeed;
        }

        // Apply all modifiers
        float modifierSpeed = player.stats.staminaSpeedModifier * player.stats.consumableSpeedModifier * player.stats.terrainSpeedModifier * player.stats.currentWeightModifier;

        player.stats.moveSpeed = Mathf.Max(0f, player.stats.baseSpeed * modifierSpeed);

        // Actual current speed (zero when stationary)
        player.stats.currentSpeed = isMoving ? player.stats.moveSpeed : 0f;

        // Apply movement
        player.rb.MovePosition( player.rb.position + moveInput * player.stats.currentSpeed * Time.fixedDeltaTime);
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
