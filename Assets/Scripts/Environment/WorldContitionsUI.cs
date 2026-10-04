using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldContitionsUI : MonoBehaviour
{
    public EnvironmentManager environmentManager;
    public Player_M player;

    public TMP_Text temperatureValue;
    public Image temperatureBackground;

    public TMP_Text safetyValue;
    public Image safetyBackground;

    public TMP_Text weatherValue;
    public Image weatherBackground;

    public Slider staminaSlider;
    public TMP_Text staminaText;

    public Slider hpSlider;
    public TMP_Text hpText;

    public Slider thirstSlider;
    public TMP_Text thirstText;

    public Slider hungerSlider;
    public TMP_Text hungerText;

    public void UpdateWorldConditionsUI()
    {
        switch (environmentManager.temperatureCondition)
        {
            case TemperatureLevel.HOT:
                temperatureBackground.color = Hex("#659653");
                break;

            case TemperatureLevel.WARM:
                temperatureBackground.color = Hex("#9BAE61");
                break;

            case TemperatureLevel.MILD:
                temperatureBackground.color = Hex("#D2BE65");
                break;

            case TemperatureLevel.COLD:
                temperatureBackground.color = Hex("#D4924D");
                break;

            case TemperatureLevel.FREEZING:
                temperatureBackground.color = Hex("#B94C43");
                break;
        }

        switch(environmentManager.safetyCondition)
        {
            case SafetyLevel.SAFE:
                safetyBackground.color = Hex("#659653");
                break;
            case SafetyLevel.MODERATE:
                safetyBackground.color = Hex("#D2BE65");
                break;
            case SafetyLevel.UNSAFE:
                safetyBackground.color = Hex("#B94C43");
                break;
        }
        switch (environmentManager.weatherCondition)
        {
            case WeatherCondition.FAVORABLE:
                weatherBackground.color = Hex("#659653");
                break;
            case WeatherCondition.HARSH:
                weatherBackground.color = Hex("#B94C43");
                break;
        }

        temperatureValue.text = environmentManager.temperatureCondition.ToString();
        safetyValue.text = environmentManager.safetyCondition.ToString();
        weatherValue.text = environmentManager.weatherCondition.ToString();
    }
    private Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color color);
        return color;
    }

    public void UpdatePlayerStatsUI()
    {
        staminaSlider.value = player.stats.currentStamina;
        staminaText.text = player.stats.currentStamina.ToString("F0") + "/" + player.stats.maxStamina.ToString("F0");
        hpSlider.value = player.stats.currentHp;
        hpText.text = player.stats.currentHp.ToString("F0") + "/" + player.stats.maxHp.ToString("F0");
        thirstSlider.value = player.stats.currentThirst;
        thirstText.text = player.stats.currentThirst.ToString("F0") + "/" + player.stats.maxThirst.ToString("F0");
        hungerSlider.value = player.stats.currentHunger;
        hungerText.text = player.stats.currentHunger.ToString("F0") + "/" + player.stats.maxHunger.ToString("F0");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(this.gameObject.activeSelf)
        {
            UpdatePlayerStatsUI();
        }
    }
}
