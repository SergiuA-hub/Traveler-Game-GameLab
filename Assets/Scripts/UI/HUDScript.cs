using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour
{
    [Header("Player Components")]
    public Player_M player;
    private PlayerInventory_M inventory;

    public TimeManager timeManager;

    [Header("HUD elements")]
    //Full Component
    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject CityUI;

    //Stamina
    public Slider staminaSlider;
    public TextMeshProUGUI staText;
    public TextMeshProUGUI staminaConsumptionText;
    //HP
    public Slider hpSlider;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI hpConsumptionText;
    //THIRST
    public Slider thirstSlider;
    public TextMeshProUGUI thirstText;
    public TextMeshProUGUI thirstConsumptionText;
    //Hunger
    public Slider hungerSlider; 
    public TextMeshProUGUI hungerText;
    public TextMeshProUGUI hungerConsumptionText;

    [Header("Camp")]
    public Button campButton;

    public TextMeshProUGUI speed;
    [Header("DateTime")]
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI seasonText;

    public string currentSeason = "Summer";



    void Start()
    {
        inventory = player.invetory;
        hpSlider.maxValue = player.stats.maxHp;
        staminaSlider.maxValue = player.stats.maxStamina;
        thirstSlider.maxValue = player.stats.maxThirst;
        hungerSlider.maxValue = player.stats.maxHunger;
    }

   
    void Update()
    {
        UpdateStats();
        UpdateTimeDisplay();
        EnablePlayerHUD();
    }

    void UpdateStats()
    {
        staminaSlider.value = player.stats.currentStamina;
        hpSlider.value = player.stats.currentHp;
        hungerSlider.value = player.stats.currentHunger;
        thirstSlider.value = player.stats.currentThirst;

        staText.text = $"{Mathf.FloorToInt(player.stats.currentStamina)}/{Mathf.FloorToInt(player.stats.maxStamina)}";
        hpText.text = $"{Mathf.FloorToInt(player.stats.currentHp)}/{Mathf.FloorToInt(player.stats.maxHp)}";
        hungerText.text = $"{Mathf.FloorToInt(player.stats.currentHunger)}/{Mathf.FloorToInt(player.stats.maxHunger)}";
        thirstText.text = $"{Mathf.FloorToInt(player.stats.currentThirst)}/{Mathf.FloorToInt(player.stats.maxThirst)}";

        string staminaToShow = player.stats.currentStamina == 0 ? "-" : $"- {player.staminDropPerHour}/h";
        staminaConsumptionText.text = staminaToShow;
        hpConsumptionText.text = $"";
        
        if(player.stats.currentStamina < GlobalSettingsManager.CAMP_STAMINA_POSSIBLE && !player.isCamping)
        {            
            campButton.interactable = true;
        }else campButton.interactable = false;

        string hungerToShow = player.stats.currentHunger == 0 ? "-" : $"- {player.hungerDropPerHour}/h";
        hungerConsumptionText.text = hungerToShow;


        string thirstToShow = player.stats.currentThirst == 0 ? "-" : $"- {player.thirstDropPerHour}/h";
        thirstConsumptionText.text = thirstToShow;

        speed.text = $"{player.stats.currentSpeed.ToString("F1")}";
    }

    void UpdateTimeDisplay ()
    {
        dateText.text = "Day " + timeManager.currentTime.Day.ToString() + "\n" + timeManager.currentTime.Hour.ToString() + ":00";
    }

    private void EnablePlayerHUD()
    {
        if(CityUI.activeSelf)
        {
            //Aici playerul intra in oras
            HUD.SetActive(false);
        }
        if (!CityUI.activeSelf)
        {
            HUD.SetActive(true);
        }
    }
    
    public void ChangeSeason()
    {
        seasonText.text = currentSeason;
    }
}
