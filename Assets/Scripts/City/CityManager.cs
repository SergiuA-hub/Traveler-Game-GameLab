using Unity.VisualScripting;
using UnityEngine;

public class CityManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject CityUI;

    [SerializeField] private ItemsSO[] buyItems;
    
    [SerializeField] private Transform[] buyItemsPostions;

    [Header("Amount")]

    public float amount;

    //Player refrences
    private const string PlayerLayer = "Player";
    public PlayerInventory playerInventory;
    

    private void Start()
    {
        CityUI.SetActive(false);
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(PlayerLayer))
        {
            CityUI.SetActive(true);
            DisplayItemsForSale();
            
            
            if(collision.gameObject.TryGetComponent(out PlayerInventory inventory))
            {
                
                playerInventory = inventory;

            }
            

        }
    }

    
    //Display UI
    private void DisplayItemsForSale()
    {
        for(int i = 0;i< buyItems.Length; i++)
        {
            Instantiate(buyItems[i].DisplayPrefab, buyItemsPostions[i]);
        }
    }

    //Get current Production Amount based on DAYS
    
}
