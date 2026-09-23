using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour
{

    [Header ("References")]
    public Player_M player;
    private PlayerInventory_M inventory;


    public TimeManager timeManager;

    [Header ("UI elements - Sliders")]
    public Slider staminaSlider;
    public TextMeshProUGUI staText;
    public Slider hpSlider;
    public TextMeshProUGUI hpText;
    public Slider thirstSlider;
    public TextMeshProUGUI thirstText;
    public Slider hungerSlider; 
    public TextMeshProUGUI hungerText;

    [Header ("UI elements - Date display")]
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI seasonText;

    public string currentSeason = "Summer";


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = player.invetory;
        hpSlider.maxValue = player.stats.maxHp;
        staminaSlider.maxValue = player.stats.maxStamina;
        thirstSlider.maxValue = player.stats.maxThirst;
        hungerSlider.maxValue = player.stats.maxHunger;
        seasonText.text = currentSeason;

    }

    // Update is called once per frame
    void Update()
    {
        UpdateStats();
        UpdateTimeDisplay();
    }

    void UpdateStats()
    {
        staminaSlider.value = player.stats.currentStamina;
        hpSlider.value = player.stats.currentHp;
        hungerSlider.value = player.stats.currentHunger;
        thirstSlider.value = player.stats.currentThirst;

        

        staText.text = staminaSlider.value.ToString() + "/" + player.stats.maxStamina.ToString();
        hpText.text = hpSlider.value.ToString() + "/" + player.stats.maxHp.ToString();
        hungerText.text = hungerSlider.value.ToString() + "/" + player.stats.maxHunger.ToString();
        thirstText.text = thirstSlider.value.ToString() + "/" + player.stats.maxThirst.ToString();
    }

    void UpdateTimeDisplay()
    {
        dateText.text = "Day " + timeManager.currentTime.Day.ToString() + "\n" + timeManager.currentTime.Hour.ToString() + ":00";
    }
    public void ChangeSeason()
    {
        seasonText.text = currentSeason; 
    }
}
