using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryDisplayUI : MonoBehaviour
{
    private Player_M player;
    private PlayerInventory_M inventory;
    public GameObject ItemPrefab;
    public GameObject ItemList;
    public Slider weightSlider;
    public Slider capSlider;
    public TextMeshProUGUI weightTxt; 
    public TextMeshProUGUI capTxt;
    public TextMeshProUGUI moneyDisplay;

    //Current Item Display
    public InventoryItem currentInventoryItem;
    public GameObject currentSelectedItemPanel;
    public Image currentItemIcon;
    public TextMeshProUGUI currentItemName;
    public TextMeshProUGUI currrntItemWeight;
    public TextMeshProUGUI currrntItemVolume;
    public TextMeshProUGUI currrntItemAveragePrice;
    public TextMeshProUGUI currentItemTextAmount;
    [SerializeField] private int currentItemAmount=1;



    private void OnEnable()
    {
        currentSelectedItemPanel.SetActive(false);

        if (player == null)
            player = GameServices.Instance.player;
         
        inventory = player.invetory;
        inventory.HandleWeightAndVolume();
        foreach (var item in inventory.Inventory)
        {
            GameObject go = Instantiate(ItemPrefab, ItemList.transform);
            go.GetComponent<DisplayInventoryItem>().Setup(item,this);
            SetCurrentIventoryItem(item);
        }
        weightTxt.text = inventory.currentWeight.ToString() + "/" + player.stats.maxWeight.ToString();
        weightSlider.maxValue = player.stats.maxWeight;
        weightSlider.minValue = 0; 
        weightSlider.value = inventory.currentWeight;
        

        capTxt.text = inventory.currentVolume.ToString() + "/" + player.stats.maxVolume.ToString();
        capSlider.value = inventory.currentVolume;
        capSlider.maxValue = player.stats.maxVolume;
        capSlider.minValue = 0;

        moneyDisplay.text = inventory.CurrentCoins.ToString();
    }

    void Start()
    {
        if (GameServices.Instance.player == null)
            Debug.LogError("ERROR");

        player = GameServices.Instance.player;
        inventory = player.invetory;
    }
    
    
    private void OnDisable()
    {
        foreach (Transform go in ItemList.transform)
        {
            Destroy(go.gameObject);
        }
    }
    public void SetCurrentIventoryItem(InventoryItem inventoryItem)
    {
        currentItemIcon.sprite =inventoryItem.resourceSO.sprite;
        currentItemName.text = inventoryItem.resourceSO.itemName;
        
        //Values Display
        currrntItemVolume.text = (inventoryItem.resourceSO.volume * currentItemAmount).ToString();
        currrntItemWeight.text = (inventoryItem.resourceSO.weight * currentItemAmount).ToString();
        currrntItemAveragePrice.text = inventoryItem.averageValue.ToString();
        currentInventoryItem = inventoryItem;
        currentSelectedItemPanel.SetActive(true);
    }
    

    public void ChangeCurrentItemAmount()
    {
        if (currentItemAmount ==1)
        {
            
            currentItemAmount = currentInventoryItem.amount;
            currentItemTextAmount.text = currentInventoryItem.amount.ToString();
        }
        else
        {
            currentItemAmount = 1;
            currentItemTextAmount.text = "1";
        }
        SetCurrentIventoryItem(currentInventoryItem);

    }
 
    
    public void ExitInventory()
    {
        currentInventoryItem = null;
        currentItemIcon.sprite = null;
        currentItemName.text ="";
        currrntItemWeight.text = "";
        currrntItemVolume.text = "";
        currrntItemAveragePrice.text = "";
        currentItemTextAmount.text = "1";
}
}
