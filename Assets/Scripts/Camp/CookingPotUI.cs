using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CookingPotUI : MonoBehaviour
{
    public Player_M player;
    public NewCampManager campManager;

    public GameObject foodList;
    public GameObject drinkList;
    public GameObject CookingResourcePrefab;
    public TMP_Text foodGroupText;
    public TMP_Text drinkGroupText;
    //Hunger
    public Slider hungerSlider;
    public Slider tempHungerSlider;
    public TextMeshProUGUI hungerText;
    public TextMeshProUGUI additionalHungerText;

    //THIRST
    public Slider thirstSlider;
    public Slider tempThirstSlider;

    public TextMeshProUGUI thirstText;
    public TextMeshProUGUI additionalThirstText;

    //HP
    public Slider hpSlider;
    public Slider tempHpSlider;

    public TextMeshProUGUI hpText;
    public TextMeshProUGUI additionalHpText;

    private int foodCoutner = 0;
    private int drinkCoutner = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void setup()
    {
        updateStatsSliders();

        createSuppliesList();
        
    }

    private void OnDisable()
    {
        cleanSuppliesList();
    }

    public void cleanChildGameObjects(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void cleanSuppliesList()
    {
        cleanChildGameObjects(foodList);
        cleanChildGameObjects(drinkList);
    }

    public void createSuppliesList()
    {
        foodCoutner = 0;
        drinkCoutner = 0;

        foreach (var res in player.invetory.Inventory)
        {
            if (res.resourceSO.resourceType == ResourceType.Eat)
            {
                int amount = res.amount - campManager.getTempAmount(res.resourceSO);
                if (amount <= 0)
                    continue;

                foodCoutner++;
                GameObject go = Instantiate(CookingResourcePrefab, foodList.transform);
                go.GetComponent<CookingResourcePrefab>().setup(new ResourceAmount(res.resourceSO, res.amount), this);
            }

            if (res.resourceSO.resourceType == ResourceType.Drink)
            {
                int amount = res.amount - campManager.getTempAmount(res.resourceSO);
                if (amount <= 0)
                    continue;

                drinkCoutner++;
                GameObject go = Instantiate(CookingResourcePrefab, drinkList.transform);
                go.GetComponent<CookingResourcePrefab>().setup(new ResourceAmount(res.resourceSO, res.amount), this);
            }
        }
        updateCoutnerList();
    }

    public void updateCoutnerList()
    {
        foodGroupText.text = $"Food ({foodCoutner})";
        drinkGroupText.text = $"Drink ({drinkCoutner})";
    }

    public void updateStatsSliders()
    {
        float tempHunger = campManager.getFoodAmount();
        float tempThirst = campManager.getDrinkAmount();
        float tempHp = campManager.getHPAmount();

        hungerSlider.value = player.stats.currentHunger;
        thirstSlider.value = player.stats.currentThirst;
        hpSlider.value = player.stats.currentHp;
        
        tempHungerSlider.value = player.stats.currentHunger + tempHunger;
        tempThirstSlider.value = player.stats.currentThirst + tempThirst;
        tempHpSlider.value = player.stats.currentHp + tempHp;

        hpText.text = $"{Mathf.FloorToInt(player.stats.currentHp)}/{Mathf.FloorToInt(player.stats.maxHp)}";
        hungerText.text = $"{Mathf.FloorToInt(player.stats.currentHunger)}/{Mathf.FloorToInt(player.stats.maxHunger)}";
        thirstText.text = $"{Mathf.FloorToInt(player.stats.currentThirst)}/{Mathf.FloorToInt(player.stats.maxThirst)}";
    }

    public void startStatsRestore(ResourceAmount item)
    {
        int hungerNeedToRestore = Mathf.CeilToInt(player.stats.maxHunger - player.stats.currentHunger);

        int thirstNeedToRestore = Mathf.CeilToInt(player.stats.maxThirst - player.stats.currentThirst);

        int hpNeedToRestore = Mathf.CeilToInt(player.stats.maxHp - player.stats.currentHp);

        //reset all stats restore
        stopStatsRestore(item);

        if(item.resourceSO.resourceType == ResourceType.Eat)
        {
            string stats_toShow = $"+{hungerNeedToRestore}";
            if (hungerNeedToRestore > item.resourceSO.stat_restore)
                stats_toShow = $"+{item.resourceSO.stat_restore}";
            
            additionalHungerText.text = stats_toShow;
            
            if(item.resourceSO.HP_restore > 0)
            {
                string hp_toShow = $"+{hpNeedToRestore}";
                if (hpNeedToRestore > item.resourceSO.HP_restore)
                    hp_toShow = $"+{item.resourceSO.HP_restore}";
                
                additionalHpText.text = hp_toShow;
            }
                
        }

        if (item.resourceSO.resourceType == ResourceType.Drink)
        {
            string stats_toShow = $"+{thirstNeedToRestore}";
            if (thirstNeedToRestore > item.resourceSO.stat_restore)
                stats_toShow = $"+{item.resourceSO.stat_restore}";
            
            additionalThirstText.text = stats_toShow;
        }
            
        
    }

    public void stopStatsRestore(ResourceAmount item)
    {
        additionalHungerText.text = "";
        additionalThirstText.text = "";
        additionalHpText.text = "";
    }

    public void consume(ResourceAmount item)
    {
        float amount = item.amount - campManager.getTempAmount(item.resourceSO);
        if(amount < 0)
            return;

        if (item.resourceSO.resourceType == ResourceType.Eat)
        {
            bool hungerFull = player.stats.currentHunger + campManager.getFoodAmount() >= player.stats.maxHunger;
            bool hpFull = player.stats.currentHp + campManager.getHPAmount() >= player.stats.maxHp;

            if (hungerFull && hpFull)
                return;
        }

        if (item.resourceSO.resourceType == ResourceType.Drink)
        {
            if (player.stats.currentThirst + campManager.getDrinkAmount() >= player.stats.maxThirst)
                return;
        }

        campManager.addResourceToCampUsage(item.resourceSO);
        item.amount--;

        updateQTY(item);
        updateStatsSliders();
    }

    public void updateQTY(ResourceAmount item)
    {
        if(item.resourceSO.resourceType == ResourceType.Eat)
        {
            foreach (Transform UIItem in foodList.transform)
            {
                CookingResourcePrefab coockingRes = UIItem.GetComponent<CookingResourcePrefab>();
                if (coockingRes.hasRes(item.resourceSO))
                {
                    if(item.amount <= 0)
                    {
                        Destroy(UIItem.gameObject);
                        return;
                    }
                    coockingRes.updateQTY();
                    return;
                }
                    
            }
        }

        if (item.resourceSO.resourceType == ResourceType.Drink)
        {
            foreach (Transform UIItem in drinkList.transform)
            {
                CookingResourcePrefab coockingRes = UIItem.GetComponent<CookingResourcePrefab>();
                if (coockingRes.hasRes(item.resourceSO))
                {
                    coockingRes.updateQTY();
                    return;
                }
                    
            }
        }
    }

    public void RemoveResourceFromUI(InventoryItem item)
    {
        if (item.resourceSO.resourceType == ResourceType.Eat)
        {
            foreach (Transform UIItem in foodList.transform)
            {
                CookingResourcePrefab coockingRes = UIItem.GetComponent<CookingResourcePrefab>();
                if (coockingRes.hasRes(item.resourceSO))
                {
                    Destroy(UIItem.gameObject);
                    return;
                }
                    
            }
        }

        if (item.resourceSO.resourceType == ResourceType.Drink)
        {
            foreach (Transform UIItem in drinkList.transform)
            {
                CookingResourcePrefab coockingRes = UIItem.GetComponent<CookingResourcePrefab>();
                if (coockingRes.hasRes(item.resourceSO))
                {
                    Destroy(UIItem.gameObject);
                    return;
                }
                    
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateStatsSliders();
    }
}
