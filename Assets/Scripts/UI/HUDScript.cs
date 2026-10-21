using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Data;
using System;

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
    public TextMeshProUGUI weatherText;
    public Season currentSeason = Season.Summer;
    public WeatherType currentWeather = WeatherType.Clear;
    public Image timeProgress;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI extendedTimerText;
    public GameObject level1BuffPanel;
    public GameObject level2BuffPanel;
    public GameObject level3BuffPanel;
    public GameObject level4BuffPanel;
    public GameObject level5BuffPanel;
    public BuffFlames flames;

    void Start()
    {
        inventory = player.invetory;
        hpSlider.maxValue = player.stats.maxHp;
        staminaSlider.maxValue = player.stats.maxStamina;
        thirstSlider.maxValue = player.stats.maxThirst;
        hungerSlider.maxValue = player.stats.maxHunger;
        timeProgress.fillAmount = 1;
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

        float speedValue = player.stats.currentSpeed / (GlobalSettingsManager.HOUR_DURATION / timeManager.getHourDuration());
        speed.text = $"{speedValue.ToString("F1")}";
    }

    void UpdateTimeDisplay ()
    {
        dateText.text = "Day " + timeManager.currentTime.Day.ToString() + "\n" + timeManager.currentTime.Hour.ToString() + ":00";
        //var timesp = new TimeSpan((long)timeManager.timerCurrent * 10000000);
        var timesp = new TimeSpan(0, 0, (int)timeManager.timerCurrent);

        timerText.text = timesp.ToString("mm\\:ss");
        extendedTimerText.text = $"{timesp.ToString("mm\\:ss")} remaining";
        
        timeProgress.fillAmount = timeManager.timerCurrent / timeManager.timerMax;

        float ratio = Mathf.Clamp01((float)timeManager.timerCurrent / timeManager.timerMax);

        level1BuffPanel.SetActive(ratio > 0f);
        level2BuffPanel.SetActive(ratio >= 0.2f);
        level3BuffPanel.SetActive(ratio >= 0.4f);
        level4BuffPanel.SetActive(ratio >= 0.6f);
        level5BuffPanel.SetActive(ratio >= 0.8f);

        Color flameColor;

        if (ratio >= 0.8f)
            flameColor = new Color32(67, 200, 255, 255);  // Level 5
        else if (ratio >= 0.6f)
            flameColor = new Color32(60, 155, 255, 255);  // Level 4
        else if (ratio >= 0.4f)
            flameColor = new Color32(79, 116, 243, 255);  // Level 3
        else if (ratio >= 0.2f)
            flameColor = new Color32(106, 85, 232, 255);  // Level 2
        else
            flameColor = new Color32(155, 69, 223, 255);  // Level 1

        flames.SetFlameColor(flameColor);
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
        seasonText.text = currentSeason.ToString();
    }
    
    public void ChangeWeather()
    {
        weatherText.text = currentWeather.ToString();
    }
}
