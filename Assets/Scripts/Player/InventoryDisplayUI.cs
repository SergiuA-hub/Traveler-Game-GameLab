using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryDisplayUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public Player_M player;
    private PlayerInventory_M inventory;
    public GameObject ItemPrefab;
    public GameObject ItemList;
    public Slider weightSlider;
    public Slider capSlider;
    public TextMeshProUGUI weightTxt; 
    public TextMeshProUGUI capTxt;


    private void OnEnable()
    {
        inventory = player.invetory;
        inventory.HandleWeightAndVolume();
        foreach (var item in inventory.Inventory)
        {
            GameObject go = Instantiate(ItemPrefab, ItemList.transform);
            go.GetComponent<DisplayInventoryItem>().Setup(item);
        }
        weightTxt.text = inventory.currentWeight.ToString() + "/" + player.stats.maxWeight.ToString();
        weightSlider.maxValue = player.stats.maxWeight;
        weightSlider.minValue = 0; 
        weightSlider.value = inventory.currentWeight;

        capTxt.text = inventory.currentVolume.ToString() + "/" + player.stats.maxVolume.ToString();
        capSlider.value = inventory.currentVolume;
        capSlider.maxValue = player.stats.maxVolume;
        capSlider.minValue = 0;       
    }

    void Start()
    {
        
    }

   
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
