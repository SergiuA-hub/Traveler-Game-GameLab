using UnityEngine;

public class InventoryDisplayUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public PlayerInventory inventory;
    public GameObject ItemPrefab;
    public GameObject ItemList;


    private void OnEnable()
    {
        foreach (var item in inventory.Inventory)
        {
            GameObject go = Instantiate(ItemPrefab, ItemList.transform);
            go.GetComponent<DisplayInventoryItem>().Setup(item);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDisable()
    {
        foreach (Transform go in ItemList.transform)
        {
            Destroy(go.gameObject);
        }
    }
}
