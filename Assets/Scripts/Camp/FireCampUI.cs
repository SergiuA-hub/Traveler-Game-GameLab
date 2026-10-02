using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FireCampUI : MonoBehaviour
{
    public Player_M player;
    public TimeManager timeManager;
    public NewCampManager campManager;    
    public Slider fireSlider;
    public Slider tempFireSlider;
    public TMP_Text fireValue;
    public TMP_Text additionalFireValue;
    public TMP_Text fuelCount;
    public GameObject fuelList;
    public GameObject FuelResourcePrefab;
    int fuelCoutner = 0;
    public void setup()
    {
        fireSlider.maxValue = CampStats.MAX_FIRE_LEVEL;
        tempFireSlider.maxValue = CampStats.MAX_FIRE_LEVEL;

        createSuppliesList();
        updateStats();
    }
    public void updateStats()
    {
        float tempFire = campManager.getFuelAmount();

        float currentFire = campManager.campStats.fireLevel;

        float previewFire = Mathf.Min(
            currentFire + tempFire,
            CampStats.MAX_FIRE_LEVEL
        );

        fireSlider.value = currentFire;
        tempFireSlider.value = previewFire;

        fireValue.text = $"{previewFire:0}/{CampStats.MAX_FIRE_LEVEL}";
    }
    public void createSuppliesList()
    {
        fuelCoutner = 0;

        foreach (Transform child in fuelList.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var res in player.invetory.Inventory)
        {
            if (res.resourceSO.resourceType == ResourceType.Fuel)
            {
                int amount = res.amount - campManager.getTempAmount(res.resourceSO);
                if (amount <= 0)
                    continue;

                fuelCoutner++;
                GameObject go = Instantiate(FuelResourcePrefab, fuelList.transform);
                
                go.GetComponent<FuelResourcePrefab>().setup(new ResourceAmount(res.resourceSO, amount), this);
            }

        }

        fuelCount.text = $"Fuel({fuelCoutner})";
    }

    public void addFuelToCamp(ResourceAmount item)
    {
        if (campManager.campStats.fireLevel + campManager.getFuelAmount() >= CampStats.MAX_FIRE_LEVEL)
            return;

        campManager.addResourceToCampUsage(item.resourceSO);
        
        createSuppliesList();
        updateStats();
    }
}
