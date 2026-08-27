using UnityEngine;

public class GameInput : MonoBehaviour
{
    private PlayerInputAction inputs;

    private void Start()
    {
        inputs =new PlayerInputAction();
        inputs.Enable();
    }

    public Vector2 GetMoveVectorNormalized()
    {
        Vector2 input = inputs.Player.Move.ReadValue<Vector2>();
        return input;
    }
}

