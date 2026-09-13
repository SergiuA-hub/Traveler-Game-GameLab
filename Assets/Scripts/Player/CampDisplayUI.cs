using TMPro;
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
    public Player_M player;

    public GameObject supplyPrefab;
    public GameObject foodSuppliesList;
    public GameObject drinkSuppliesList;

    public CampManager campManager;

    public void OnEnable()
    {
        cleanSupplies();

        HP_Text.text = $" - / {player.stats.maxHp}";
        stamina_text.text = $"- / {player.stats.maxStamina}";
        hunger_text.text = $"- / {player.stats.maxHunger}";
        thirst_text.text = $"- / {player.stats.maxThirst}";

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
    }

    public void startResting()
    {
        campManager.StartRest();
    }

    public void OnDisable()
    {

    }
}
