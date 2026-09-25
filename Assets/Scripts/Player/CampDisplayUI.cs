using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CampDisplayUI : MonoBehaviour
{
    public Image HP_Progress;
    public TMP_Text HP_Text;

    public Image stamina_progress;
    public TMP_Text stamina_text;

    public Image hunger_progress;
    public TMP_Text hunger_text;

    public Image thirst_progress;
    public TMP_Text thirst_text;
    public TMP_Text hours_to_rest_text;
    public TMP_Text newHP;
    public TMP_Text newStamina;
    public TMP_Text newHunger;
    public TMP_Text newThirst;
    public Player_M player;
    public CampManager camp;

    public GameObject supplyPrefab;
    public GameObject foodSuppliesList;
    public GameObject campConsumptionResPrefab;
    public GameObject ConsumptionList;

    public GameObject drinkSuppliesList;

    public CampManager campManager;

    public void OnEnable()
    {
        cleanSupplies();
        
        camp.resetConsumption();
        player.rooted = true;


        //camp.CalculateFood();
        //camp.CalculateDrink();
        updateInventory();
        updateConsumption();
    }

    public void updateInventory()
    {
        HP_Text.text = $" {player.stats.currentHp:0} / {player.stats.maxHp:0}";
        stamina_text.text = $" {player.stats.currentStamina:0.#} / {player.stats.maxStamina:0.#}";
        hunger_text.text = $" {player.stats.currentHunger:0.#} / {player.stats.maxHunger:0.#}";
        thirst_text.text = $" {player.stats.currentThirst:0.#} / {player.stats.maxThirst:0.#}";

        foreach (var item in player.invetory.Inventory)
        {
            if (item.resourceSO.resourceType == ResourceType.Eat)
            {
                GameObject go = Instantiate(supplyPrefab, foodSuppliesList.transform);
                go.GetComponent<ResourceListPrefab>().Setup(item, this);
            }
            else if (item.resourceSO.resourceType == ResourceType.Drink)
            {
                GameObject go = Instantiate(supplyPrefab, drinkSuppliesList.transform);
                go.GetComponent<ResourceListPrefab>().Setup(item, this);
            }
        }
    }

    public void updateConsumption()
    {        
        int totalHoursToRest = camp.calculateRestDuration();
        float totalHPRestore = 0f;
        float totalStaminaRestore = totalHoursToRest * 1f;
        float totalHungerRestore = 0f;
        float totalThirstRestore = 0f;

        foreach (var item in camp.drinkItems)
        {
            totalThirstRestore += item.amount * item.resourceSO.stat_restore;
            GameObject go = Instantiate(campConsumptionResPrefab, ConsumptionList.transform);
            go.GetComponent<CampConsumedResourcePrefab>().Setup(item);
        }

        foreach (var item in camp.foodItems)
        {
            totalHungerRestore += item.amount * item.resourceSO.stat_restore;
            totalHPRestore += item.amount * item.resourceSO.HP_restore;
            GameObject go = Instantiate(campConsumptionResPrefab, ConsumptionList.transform);
            go.GetComponent<CampConsumedResourcePrefab>().Setup(item);
        }

        hours_to_rest_text.text = $"Hours to Rest: {totalHoursToRest}";

        float finalHP = Mathf.Min(player.stats.currentHp + totalHPRestore, player.stats.maxHp);
        float finalStamina = Mathf.Min(player.stats.currentStamina + totalStaminaRestore, player.stats.maxStamina);
        float finalHunger = Mathf.Min(player.stats.currentHunger + totalHungerRestore, player.stats.maxHunger);
        float finalThirst = Mathf.Min(player.stats.currentThirst + totalThirstRestore, player.stats.maxThirst);

        newHP.text = $"New HP: {finalHP}";
        newStamina.text = $"New Stamina: {finalStamina:0.#}";
        newHunger.text = $"New Hunger: {finalHunger:0.#}";
        newThirst.text = $"New Thirst: {finalThirst:0.#}";
    }

    public void addResourceToConsumption(InventoryItem item)
    {
        cleanSupplies();
        if(item.resourceSO.resourceType == ResourceType.Eat)
            camp.addToFood(item);
        if (item.resourceSO.resourceType == ResourceType.Drink)
            camp.addToDrink(item);

        updateInventory();
        updateConsumption();
    }

    public void cleanSupplies()
    {
        foreach (Transform go in foodSuppliesList.transform)
        {
            Destroy(go.gameObject);
        }
        foreach (Transform go in drinkSuppliesList.transform)
        {
            Destroy(go.gameObject);
        }
        clearConsumption();
    }

    public void clearConsumption()
    {
        foreach (Transform go in ConsumptionList.transform)
        {
            Destroy(go.gameObject);
        }
    }

    public void startResting()
    {
        campManager.StartRest();
    }

    public void unRootPlayer()
    {
        player.rooted = false;
    }

    public void OnDisable()
    {

    }
}
