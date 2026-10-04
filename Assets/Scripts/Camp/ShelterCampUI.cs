using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShelterCampUI : MonoBehaviour
{
    public Player_M player;
    public TimeManager timeManager;
    public NewCampManager campManager;    
    public Slider shelterSlider;
    public Slider tempshelterSlider;
    public TMP_Text shelterValue;
    public TMP_Text additionalShelterValue;
    public TMP_Text resCount;
    public GameObject resList;
    public GameObject resResourcePrefab;
    int reCoutner = 0;
    public void setup()
    {
        shelterSlider.maxValue = CampStats.MAX_SHELTER_LEVEL;
        tempshelterSlider.maxValue = CampStats.MAX_SHELTER_LEVEL;

        createSuppliesList();
        updateStats();
    }
    public void updateStats()
    {
        float tempShelter = campManager.getReinforcementAmount();

        float currentShelter = campManager.campStats.shelterLevel;

        float previewShelter = Mathf.Min(
            currentShelter + tempShelter,
            CampStats.MAX_SHELTER_LEVEL
        );

        shelterSlider.value = currentShelter;
        tempshelterSlider.value = previewShelter;

        shelterValue.text = $"{previewShelter:0}/{CampStats.MAX_SHELTER_LEVEL}";
    }
    public void createSuppliesList()
    {
        reCoutner = 0;

        foreach (Transform child in resList.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var res in player.invetory.Inventory)
        {
            if (res.resourceSO.resourceType.Contains(ResourceType.Reinforcement))
            {
                int amount = res.amount - campManager.getTempAmount(res.resourceSO);
                if (amount <= 0)
                    continue;

                reCoutner++;
                GameObject go = Instantiate(resResourcePrefab, resList.transform);
                
                go.GetComponent<ShelterResourcePrefab>().setup(new ResourceAmount(res.resourceSO, amount), this);
            }

        }

        resCount.text = $"Reinforcements({reCoutner})";
    }

    public void addReinforcementToCamp(ResourceAmount item)
    {
        if (campManager.campStats.fireLevel + campManager.getReinforcementAmount() >= CampStats.MAX_FIRE_LEVEL)
            return;

        campManager.addResourceToCampUsage(item.resourceSO);
        
        createSuppliesList();
        updateStats();
    }
}
