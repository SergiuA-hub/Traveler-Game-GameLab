using Unity.VisualScripting;
using UnityEngine;

public class CityManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject CityUI;

    [SerializeField] private ItemsSO[] buyItems;
    [SerializeField] private Transform[] buyItemsPostions;

    private const string PlayerLayer = "Player";

    private void Start()
    {
        CityUI.SetActive(false);
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(PlayerLayer))
        {
            CityUI.SetActive(true);
            
            /*
              if(collision.gameObject.TryGetComponent(out PlayerInventory inventory))
            {
                
            }
            */

        }
    }

    private void Update()
    {
        DisplayItemsForSale();
    }

    private void DisplayItemsForSale()
    {
        foreach (var item in buyItems) 
        {
            foreach(var pos in buyItemsPostions)
            {
                Instantiate(item.DisplayPrefab, pos);
            }
        
        }
    }

    
}
