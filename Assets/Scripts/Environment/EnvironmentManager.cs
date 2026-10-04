using System;
using UnityEngine;

public enum TimeOfDay
{
    Day,
    Night
}

public enum TemperatureLevel
{
    HOT,
    WARM,
    MILD,
    COLD,
    FREEZING
}

public enum SafetyLevel
{
    SAFE,
    MODERATE,
    UNSAFE,
    DANGEROUS
}

public enum WeatherCondition
{
    FAVORABLE,
    HARSH
}
public class EnvironmentManager : MonoBehaviour
{
    public TimeManager timeManager;
    public WorldContitionsUI worldContitionsUI;
    public Season currentSeason;
    public WeatherType currentWeather;
    public TimeOfDay time;

    public TemperatureLevel temperatureCondition;
    public SafetyLevel safetyCondition;
    public WeatherCondition weatherCondition;

    public void Start()
    {
        timeManager.onEvening.AddListener(eveningEvent);
        timeManager.onMorning.AddListener(morningEvent);
    }

    private void OnDestroy()
    {
        timeManager.onEvening.RemoveListener(eveningEvent);
        timeManager.onMorning.RemoveListener(morningEvent);
    }
    public void eveningEvent(DateTime t)
    {
        time = TimeOfDay.Night;
        calculateConditions();
    }

    public void morningEvent(DateTime t)
    {
        time = TimeOfDay.Day;
        calculateConditions();
    }

    public void setSeason(Season season)
    {
        currentSeason = season;
        calculateConditions();
    }

    public void setWeather(WeatherType weather)
    {
        currentWeather = weather;
        calculateConditions();
    }

    public void calculateConditions()
    {
        weatherCondition = (currentWeather == WeatherType.Clear ? WeatherCondition.FAVORABLE : WeatherCondition.HARSH);

        temperatureCondition = currentSeason switch
        {
            Season.Spring => TemperatureLevel.WARM,
            Season.Summer => TemperatureLevel.HOT,
            Season.Fall => TemperatureLevel.MILD,
            Season.Winter => TemperatureLevel.COLD,
            _ => throw new ArgumentOutOfRangeException()
        };

        switch (time)
        {
            case TimeOfDay.Day:
                break;

            case TimeOfDay.Night:
                if(temperatureCondition != TemperatureLevel.FREEZING)
                    temperatureCondition += 1;
                if(safetyCondition != SafetyLevel.UNSAFE)
                    safetyCondition += 1;
                break;

            default:
                break;
        }

        updateWorldConditionsUI();
    }

    public void updateWorldConditionsUI()
    {
        worldContitionsUI.UpdateWorldConditionsUI();
    }
}