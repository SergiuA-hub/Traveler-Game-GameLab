using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float startSpeed;
    public float acceleration;
    public float MaxSpeeed;

    [Header("Stamina")]

    public float currentStamina;
    public float maxStamina;

    [Header("Invetory")]
    
    [SerializeField] private float maxWeight;
    [SerializeField] private float maxVolume;
    
    

    [Header("Movement modifier V/W ")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float TierIMoveModifier;

    [SerializeField] private float TierIIMoveModifier;
    [SerializeField] private float TierIIIMoveModifier;
}
