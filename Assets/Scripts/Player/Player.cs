using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Health")]
    [SerializeField]private int currentHealth;
    [SerializeField] private int maxHealth;

    //Player UI
    [Header("Components")]
    //Get refrences from every script that the player uses
    [SerializeField] private PlayerInventory inventory;

    public void RestoreHealth(int amount)
    {
        int adaosHealth= currentHealth += amount;
        if (adaosHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += amount;
        }
    }
}
