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
            player.stats.baseSpeed = Mathf.MoveTowards(player.stats.baseSpeed,player.stats.MaxSpeeed,player.stats.acceleration * Time.deltaTime);
        }
        else if (!IsMoving() && HasStamina())
        {
            player.stats.baseSpeed = player.stats.startSpeed;
        }
        else if (!HasStamina())
        {
            player.stats.baseSpeed = player.stats.exhaustSpeed;
        }

        float modifierSpeed =player.stats.staminaSpeedModifier * player.stats.consumableSpeedModifier *player.stats.terrainSpeedModifier *player.stats.currentWeightModifier;

        player.stats.moveSpeed = player.stats.baseSpeed * modifierSpeed;

        Vector2 moveInput = player.gameInput.GetMoveVectorNormalized();

        //Gradele
        //45 = 0.70710678f
        //const float rotationFactor = 0.70710678f;

        //65
        float sin = 0.8660254f;
        float cos = 0.5f;

        Vector2 moveDirection = new Vector2((moveInput.x - moveInput.y) * sin,(moveInput.x + moveInput.y) * cos);
        
        if (player.rooted)
        {
            moveDirection = Vector2.zero;
        }

        player.rb.MovePosition(player.rb.position +moveDirection * player.stats.moveSpeed * Time.fixedDeltaTime);

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
