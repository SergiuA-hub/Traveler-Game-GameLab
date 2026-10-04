using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CargoCampUI : MonoBehaviour
{
    public Player_M player;
    public TimeManager timeManager;
    public NewCampManager campManager;    
    public Slider fireSlider;
    public Slider tempFireSlider;
    public TMP_Text fireValue;
    public TMP_Text additionalFireValue;
    public TMP_Text cargoResCount;
    public GameObject cargoResList;
    public GameObject cargoResourcePrefab;
    int cargoCounter = 0;
    public void setup()
    {
        fireSlider.maxValue = CampStats.MAX_CARGO_SAFETY_LEVEL;
        tempFireSlider.maxValue = CampStats.MAX_CARGO_SAFETY_LEVEL;

        createSuppliesList();
        updateStats();
    }
    public void updateStats()
    {
        float tempFire = campManager.getSecuringAmount();

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
        cargoCounter = 0;

        foreach (Transform child in cargoResList.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var res in player.invetory.Inventory)
        {
            if (res.resourceSO.resourceType.Contains(ResourceType.Security))
            {
                int amount = res.amount - campManager.getTempAmount(res.resourceSO);
                if (amount <= 0)
                    continue;

                cargoCounter++;
                GameObject go = Instantiate(cargoResourcePrefab, cargoResList.transform);
                
                go.GetComponent<CargoResourcePrefab>().setup(new ResourceAmount(res.resourceSO, amount), this);
            }

        }

        cargoResCount.text = $"Cargo({cargoCounter})";
    }

    public void addSecurityToCamp(ResourceAmount item)
    {
        Debug.Log($"Adding {item.amount} of {item.resourceSO.itemName} to camp cargo safety and {campManager.campStats.cargoSafetyLevel + campManager.getSecuringAmount()}");
        if (campManager.campStats.cargoSafetyLevel + campManager.getSecuringAmount() >= CampStats.MAX_CARGO_SAFETY_LEVEL)
            return;

        Debug.Log($"Adding {item.amount} of {item.resourceSO.itemName} to camp cargo safety");
        campManager.addResourceToCampUsage(item.resourceSO);
        
        createSuppliesList();
        updateStats();
    }
}
