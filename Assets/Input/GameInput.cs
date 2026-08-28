using UnityEngine;
using System;

public class GameInput : MonoBehaviour
{
    private PlayerInputAction inputs;

    //Events
    public event EventHandler OnInteract;
    public event EventHandler OnInventory;

    public event EventHandler OnPauseGame;
    private void Start()
    {
        inputs =new PlayerInputAction();
        inputs.Enable();

        //Actions
        inputs.Player.Interact.performed += Interact_performed;
        inputs.Player.Invetory.performed += Invetory_performed;
        inputs.Player.Pause.performed += Pause_performed;
    }

    

    private void Invetory_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInventory?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteract?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMoveVectorNormalized()
    {
        Vector2 input = inputs.Player.Move.ReadValue<Vector2>();
        return input;
    }

    //Pause Game
    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPauseGame?.Invoke(this, EventArgs.Empty);
    }
}

