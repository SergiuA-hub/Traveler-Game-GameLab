using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement_M : MonoBehaviour
{
    private Player_M player;
    private PlayerAutoNavigation autoNavigation;
    private bool currentlyMoving;

    private void Start()
    {
        player = GetComponent<Player_M>();
        autoNavigation = GetComponent<PlayerAutoNavigation>();

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
        Vector2 manualInput =
            player.gameInput.GetMoveVectorNormalized();

        bool manualMovement =
            manualInput.sqrMagnitude > 0.001f;

        // Manual input always overrides auto navigation.
        if (manualMovement &&
            autoNavigation != null &&
            autoNavigation.IsNavigating)
        {
            autoNavigation.StopNavigation();
        }

        Vector2 moveInput = manualInput;
        if (autoNavigation != null && autoNavigation.IsNavigating)
        {
            Debug.Log(
                $"[MOVE] AutoNav ACTIVE | " +
                $"Manual: {manualMovement} | " +
                $"Rooted: {player.isRooted()}"
            );
        }
        if (autoNavigation != null && autoNavigation.IsNavigating)
        {
            Debug.Log(
                $"[MOVE] Direction: {moveInput}, " +
                $"Rooted: {player.isRooted()}, " +
                $"Speed: {player.stats.currentSpeed}"
            );
        }
        if (!manualMovement &&
            autoNavigation != null &&
            autoNavigation.IsNavigating &&
            !player.isRooted())
        {
            moveInput = autoNavigation.GetMoveDirection(
                player.rb.position
            );
        }

        if (player.isRooted())
            moveInput = Vector2.zero;

        bool isMoving = moveInput.sqrMagnitude > 0.001f;

        // Existing speed calculation.
        if (!HasStamina())
        {
            player.stats.baseSpeed = player.stats.exhaustSpeed;
        }
        else if (isMoving)
        {
            player.stats.baseSpeed = Mathf.MoveTowards(
                player.stats.baseSpeed,
                player.stats.MaxSpeeed,
                player.stats.acceleration * Time.fixedDeltaTime
            );
        }
        else
        {
            player.stats.baseSpeed = player.stats.startSpeed;
        }

        float modifierSpeed =
            player.stats.staminaSpeedModifier *
            player.stats.consumableSpeedModifier *
            player.stats.terrainSpeedModifier *
            player.stats.currentWeightModifier;

        player.stats.moveSpeed = Mathf.Max(
            0f,
            player.stats.baseSpeed * modifierSpeed
        );

        player.stats.currentSpeed =
            isMoving ? player.stats.moveSpeed : 0f;

        Vector2 nextPosition =
            player.rb.position +
            moveInput *
            player.stats.currentSpeed *
            Time.fixedDeltaTime;

        // Prevent overshooting automatic waypoints.
        if (autoNavigation != null &&
            autoNavigation.IsNavigating &&
            !manualMovement &&
            isMoving)
        {
            nextPosition = Vector2.MoveTowards(
                player.rb.position,
                autoNavigation.CurrentTarget,
                player.stats.currentSpeed * Time.fixedDeltaTime
            );
        }

        player.rb.MovePosition(nextPosition);

        currentlyMoving = isMoving &&
                          player.stats.currentSpeed > 0f;

        // Existing sprite orientation.
        if (moveInput.x < -0.01f)
        {
            player.transform.localScale = new Vector3(
                Mathf.Abs(player.transform.localScale.x),
                player.transform.localScale.y,
                player.transform.localScale.z
            );
        }
        else if (moveInput.x > 0.01f)
        {
            player.transform.localScale = new Vector3(
                -Mathf.Abs(player.transform.localScale.x),
                player.transform.localScale.y,
                player.transform.localScale.z
            );
        }
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
        return currentlyMoving;
    }
}
