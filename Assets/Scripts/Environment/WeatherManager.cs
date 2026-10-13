using System;
using UnityEngine;

public enum WeatherType
{
    Clear,
    Rain,
    Storm,
    Snow,
    Blizzard
}
public class WeatherManager : MonoBehaviour
{
    public TimeManager timeManager;
    public HUDScript HUD;
    public EnvironmentManager environmentManager;
    public WeatherType currentWeather;
    public Season currentSeason;

    public WeatherVisualManager weatherVisual;
    public WeatherType springWeather = WeatherType.Clear;
    public int springWeatherDuration;
    public float springWeatherChangeChance = 20;
    public WeatherType summerWeather = WeatherType.Clear;
    public int summerWeatherDuration;
    public float summerWeatherChangeChance = 20;
    public WeatherType fallWeather = WeatherType.Clear;
    public int fallWeatherDuration;
    public float fallWeatherChangeChance = 20;
    public WeatherType winterWeather = WeatherType.Clear;
    public int winterWeatherDuration;
    public float winterWeatherChangeChance = 20;
    public int minWeatherDuration;
    public int maxWeatherDuration;
    public int maxStormDuration;

    private int weatherDurationHours;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentWeather = WeatherType.Clear;
        currentSeason = Season.Summer;

        environmentManager.setWeather(currentWeather);
        updateUI();
        timeManager.onHourChanged.AddListener(hourlyWeatherCheck);
    }

    public void updateUI()
    {
        HUD.currentWeather = currentWeather;
        HUD.ChangeWeather();
    }

    public void hourlyWeatherCheck(DateTime t)
    {
        if (springWeather == WeatherType.Clear)
        {
            //roll for weather change for spring
            float weatherRoll = UnityEngine.Random.Range(0, 100);
            if (weatherRoll < springWeatherChangeChance)
            {
                springWeather = WeatherType.Rain;
                springWeatherDuration = UnityEngine.Random.Range(minWeatherDuration, maxWeatherDuration);
            }
        }
        else
        {
            springWeatherDuration--;
            if (springWeatherDuration <= 0)
            {
                springWeather = WeatherType.Clear;
            }
        }
        if (currentSeason == Season.Spring)
            weatherVisual.changeWeather(currentSeason, springWeather);

        if (summerWeather == WeatherType.Clear)
        {
            //roll for weather change
            float weatherRoll = UnityEngine.Random.Range(0, 100);
            if (weatherRoll < summerWeatherChangeChance)
            {
                summerWeather = WeatherType.Rain;                
                summerWeatherDuration = UnityEngine.Random.Range(minWeatherDuration, maxWeatherDuration);
            }
        }
        else
        {
            summerWeatherDuration--;
            if (summerWeatherDuration <= 0)
            {
                summerWeather = WeatherType.Clear;
                //weatherVisual.SetWeather(summerWeather);
            }
        }
        if (currentSeason == Season.Summer)
            weatherVisual.changeWeather(currentSeason, summerWeather);

        if (fallWeather == WeatherType.Clear)
        {
            //roll for weather change
            float weatherRoll = UnityEngine.Random.Range(0, 100);
            if (weatherRoll < fallWeatherChangeChance)
            {
                float stormChance = UnityEngine.Random.Range(0, 100);
                if (stormChance < 10)
                {
                    fallWeather = WeatherType.Storm;
                    fallWeatherDuration = UnityEngine.Random.Range(minWeatherDuration, maxStormDuration);
                }
                else
                {
                    fallWeather = WeatherType.Rain;
                    fallWeatherDuration = UnityEngine.Random.Range(minWeatherDuration, maxWeatherDuration);
                }
            }
        }
        else{
            fallWeatherDuration--;
            if (fallWeatherDuration <= 0)
            {
                fallWeather = WeatherType.Clear;
                //weatherVisual.SetWeather(fallWeather);
            }
        }
        if (currentSeason == Season.Fall)
            weatherVisual.changeWeather(currentSeason, fallWeather);

        if (winterWeather == WeatherType.Clear)
        {
            //roll for weather change
            float weatherRoll = UnityEngine.Random.Range(0, 100);
            if (weatherRoll < winterWeatherChangeChance)
            {
                float stormChance = UnityEngine.Random.Range(0, 100);
                if (stormChance < 10)
                {
                    winterWeather = WeatherType.Blizzard;
                    winterWeatherDuration = UnityEngine.Random.Range(minWeatherDuration, maxStormDuration);
                }
                else
                {
                    winterWeather = WeatherType.Snow;
                    winterWeatherDuration = UnityEngine.Random.Range(minWeatherDuration, maxWeatherDuration);
                }
                
            }
        }
        else
        {
            winterWeatherDuration--;
            if (winterWeatherDuration <= 0)
            {
                winterWeather = WeatherType.Clear;
                //weatherVisual.SetWeather(winterWeather);
            }
        }
        if (currentSeason == Season.Winter)
            weatherVisual.changeWeather(currentSeason, winterWeather);

    }   

    public void setCurrentSeason(Season season)
    {
        currentSeason = season;
        weatherVisual.changeWeather(currentSeason, currentWeather);

        if (currentSeason == Season.Spring)
        {
            //weatherVisual.changeWeather(currentSeason, currentWeather);            
            //weatherVisual.SetWeather(springWeather);
        }

        if (currentSeason == Season.Summer)
        {
            //weatherVisual.SetWeather(summerWeather);
        }

        if (currentSeason == Season.Fall)
        {
            //weatherVisual.SetWeather(fallWeather);
        }

        if (currentSeason == Season.Winter)
        {
            //weatherVisual.SetWeather(winterWeather);
        }
    }
}
