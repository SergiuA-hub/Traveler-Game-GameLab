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
        player.rooted = true;

        HP_Text.text = $" {player.stats.currentHp} / {player.stats.maxHp}";
        stamina_text.text = $" {player.stats.currentStamina} / {player.stats.maxStamina}";
        hunger_text.text = $" {player.stats.currentHunger} / {player.stats.maxHunger}";
        thirst_text.text = $" {player.stats.currentThirst} / {player.stats.maxThirst}";

        foreach (var item in player.invetory.Inventory)
        {
            if (item.resourceSO.resourceType == ResourceType.Eat    )
            {
                GameObject go = Instantiate(supplyPrefab, foodSuppliesList.transform);
                go.GetComponent<ResourceListPrefab>().Setup(item);
            }
            else if (item.resourceSO.resourceType == ResourceType.Drink)
            {
                GameObject go = Instantiate(supplyPrefab, drinkSuppliesList.transform);
                go.GetComponent<ResourceListPrefab>().Setup(item);
            }
        }
        camp.CalculateFood();
        camp.CalculateDrink();

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

        float finalHP = Mathf.Min(player.stats.currentHp + totalHPRestore,player.stats.maxHp);
        float finalStamina = Mathf.Min(player.stats.currentStamina + totalStaminaRestore, player.stats.maxStamina);
        float finalHunger = Mathf.Min(player.stats.currentHunger + totalHungerRestore, player.stats.maxHunger);
        float finalThirst = Mathf.Min(player.stats.currentThirst + totalThirstRestore, player.stats.maxThirst);
        
        newHP.text = $"New HP: {finalHP}";
        newStamina.text = $"New Stamina: {finalStamina}";
        newHunger.text = $"New Hunger: {finalHunger}";
        newThirst.text = $"New Thirst: {finalThirst}";
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
