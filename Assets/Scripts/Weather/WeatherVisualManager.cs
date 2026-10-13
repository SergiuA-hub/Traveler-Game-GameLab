using HeyHeyThere.WeatherFX;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public enum WeatherIntensity
{
    Light,
    Normal,
    Heavy
}

public class WeatherVisualManager : MonoBehaviour
{
    [SerializeField] private float transitionDuration = 3f;
    public string effectName;
    private WeatherFX activeEffect;
    
    [Header("Lightning")]
    [SerializeField, Range(0f, 1f)] private float lightningChance = 0.01f;
    
    [SerializeField] private float maxRainDensity = 0.6f;
    [SerializeField] private float rainChangeSpeed = 0.03f;

    private bool weatherProggression;
    private float currentWeatherIntensity;
    private float targetWeatherIntensity;
    private float nextEvolution;
    private float lightningTimer;    
    private WeatherType currentWeather = WeatherType.Clear;
    private Season currentSeason = Season.Summer;
    private bool weatherInitialized = false;

    #region WEATHER EFFECTS

    private WeatherFX rainEffect;
    private WeatherFX snowEffect;
    private WeatherFX lightningEffect;
    private WeatherFX leavesEffect;
    private WeatherFX windEffect;
    private WeatherFX blizzardEffect;
    private WeatherFX frostEffect;
    private WeatherFX pollenEffect;

    #endregion
    #region EFFECT INITIALIZATION

    // ==================== RAIN ====================

    private void InitializeRain()
    {
        if (rainEffect != null)
            return;

        rainEffect = WeatherFX.Add("rain");
        rainEffect.Renderer.sortingLayerName = "Weather";
        rainEffect.Renderer.sortingOrder = 1000;
    }

    private void StopRain()
    {
        if (rainEffect == null)
            return;

        rainEffect.Stop(transitionDuration);
        rainEffect = null;
    }

    // ==================== SNOW ====================

    private void InitializeSnow()
    {
        if (snowEffect != null)
            return;

        snowEffect = WeatherFX.Add("snow");
        snowEffect.Renderer.sortingLayerName = "Weather";
        snowEffect.Renderer.sortingOrder = 1000;
    }

    private void StopSnow()
    {
        if (snowEffect == null)
            return;

        snowEffect.Stop(transitionDuration);
        snowEffect = null;
    }

    // ==================== LIGHTNING ====================

    private void InitializeLightning()
    {
        if (lightningEffect != null)
            return;

        lightningEffect = WeatherFX.Add("lightning");
        lightningEffect.Renderer.sortingLayerName = "Weather";
        lightningEffect.Renderer.sortingOrder = 1001;
    }

    private void StopLightning()
    {
        if (lightningEffect == null)
            return;

        lightningEffect.Stop(transitionDuration);
        lightningEffect = null;
    }

    // ==================== LEAVES ====================

    private void InitializeLeaves()
    {
        if (leavesEffect != null)
            return;

        leavesEffect = WeatherFX.Add("leaves");
        leavesEffect.Renderer.sortingLayerName = "Weather";
        leavesEffect.Renderer.sortingOrder = 1002;
    }

    private void StopLeaves()
    {
        if (leavesEffect == null)
            return;

        leavesEffect.Stop(transitionDuration);
        leavesEffect = null;
    }

    // ==================== WIND ====================

    private void InitializeWind()
    {
        if (windEffect != null)
            return;

        windEffect = WeatherFX.Add("wind");
        windEffect.Renderer.sortingLayerName = "Weather";
        windEffect.Renderer.sortingOrder = 1003;
    }

    private void StopWind()
    {
        if (windEffect == null)
            return;

        windEffect.Stop(transitionDuration);
        windEffect = null;
    }

    // ==================== BLIZZARD ====================

    private void InitializeBlizzard()
    {
        if (blizzardEffect != null)
            return;

        blizzardEffect = WeatherFX.Add("blizzard");
        blizzardEffect.Renderer.sortingLayerName = "Weather";
        blizzardEffect.Renderer.sortingOrder = 1002;
    }

    private void StopBlizzard()
    {
        if (blizzardEffect == null)
            return;

        blizzardEffect.Stop(transitionDuration);
        blizzardEffect = null;
    }

    // ==================== FROST ====================

    private void InitializeFrost()
    {
        if (frostEffect != null)
            return;

        frostEffect = WeatherFX.Add("frost");
        frostEffect.Renderer.sortingLayerName = "Weather";
        frostEffect.Renderer.sortingOrder = 1003;
    }

    private void StopFrost()
    {
        if (frostEffect == null)
            return;

        frostEffect.Stop(transitionDuration);
        frostEffect = null;
    }
    // ==================== POLLEN ====================

    private void InitializePollen()
    {
        if (pollenEffect != null)
            return;

        pollenEffect = WeatherFX.Add("pollen");
        pollenEffect.Renderer.sortingLayerName = "Weather";
        pollenEffect.Renderer.sortingOrder = 998;
    }

    private void StopPollen()
    {
        if (pollenEffect == null)
            return;

        pollenEffect.Stop(transitionDuration);
        pollenEffect = null;
    }
    #endregion

    public void changeWeather(Season season, WeatherType weather)
    {
        Debug.Log($"Weahter change with season:{season.ToString()} and weather:{weather.ToString()} and will not change return:{(weatherInitialized && currentSeason == season &&currentWeather == weather)}");
        
        if (weatherInitialized &&
            currentSeason == season &&
            currentWeather == weather)
            return;

        StopAllWeatherEffects();

        currentSeason = season;
        currentWeather = weather;
        weatherInitialized = true;

        switch (season)
        {
            // ================= SPRING =================
            case Season.Spring:
                switch (weather)
                {
                    case WeatherType.Clear:
                        InitializePollen();

                        pollenEffect.Set("density", 0.1f);
                        pollenEffect.Set("speed", 0.25f);
                        pollenEffect.Set("color", new Color(1f, 0.71f, 0.76f));

                        pollenEffect.FadeIn(2f);
                        break;

                    case WeatherType.Rain:
                        InitializeRain();

                        rainEffect.Set("density", 0.15f);
                        rainEffect.Set("speed", 0.6f);

                        rainEffect.FadeIn(3f);
                        break;
                }
                break;

            // ================= SUMMER =================
            case Season.Summer:
                switch (weather)
                {
                    case WeatherType.Clear:
                        //no effect on clear summer weather
                        break;

                    case WeatherType.Rain:
                        InitializeRain();
                        InitializeWind();

                        // Light rain
                        rainEffect.Set("density", 0.15f);
                        rainEffect.Set("speed", 0.6f);
                        rainEffect.FadeIn(3f);

                        // Very light wind
                        windEffect.Set("speed", 0.15f);
                        windEffect.Set("intensity", 0.08f);
                        windEffect.FadeIn(3f);
                        break;
                }
                break;

            // ================= AUTUMN =================
            case Season.Fall:
                switch (weather)
                {
                    case WeatherType.Clear:
                        InitializeLeaves();
                        InitializeWind();

                        leavesEffect.Set("speed", 2f);
                        leavesEffect.Set("density", 0.005f);
                        leavesEffect.FadeIn(3f);

                        windEffect.Set("speed", 0.15f);
                        windEffect.Set("intensity", 0.1f);
                        windEffect.FadeIn(3f);
                        break;
                    case WeatherType.Rain:
                        InitializeRain();
                        InitializeWind();
                        InitializeLeaves();

                        // Light rain
                        rainEffect.Set("density", 0.3f);
                        rainEffect.Set("speed", 0.6f);
                        rainEffect.FadeIn(3f);

                        //more leaves than clear
                        leavesEffect.Set("speed", 2f);
                        leavesEffect.Set("density", 0.02f);
                        leavesEffect.Set("wind", 1f);
                        leavesEffect.FadeIn(3f);

                        // Very light wind
                        windEffect.Set("speed", 0.15f);
                        windEffect.Set("intensity", 0.15f);
                        windEffect.FadeIn(3f);

                        break;
                    case WeatherType.Storm:
                        InitializeRain();
                        InitializeLightning();
                        InitializeWind();
                        InitializeLeaves();

                        // Heavy rain - slower fade in
                        rainEffect.Set("density", 0.65f);
                        rainEffect.Set("speed", 0.9f);
                        rainEffect.FadeIn(6f);

                        // Lightning - triggered by your existing doLighting()
                        lightningEffect.Set("frequency", 0f);
                        lightningEffect.FadeIn(3f);
                        lightningTimer = 1f;

                        // Moderate wind
                        windEffect.Set("speed", 0.7f);
                        windEffect.Set("intensity", 0.4f);
                        windEffect.FadeIn(4f);

                        // More leaves, moving faster
                        leavesEffect.Set("density", 0.05f);
                        leavesEffect.Set("speed", 3f);
                        leavesEffect.Set("wind", 1f);
                        leavesEffect.FadeIn(4f);

                        break;
                }
                break;

            // ================= WINTER =================
            case Season.Winter:
                switch (weather)
                {
                    case WeatherType.Clear:
                        //no effect on clear summer weather
                        break;
                    case WeatherType.Snow:
                        InitializeSnow();

                        snowEffect.Set("density", 0.5f);
                        snowEffect.Set("speed", 0.6f);

                        snowEffect.FadeIn(3f);
                        break;
                    case WeatherType.Blizzard:
                        InitializeBlizzard();

                        blizzardEffect.Set("density", 0.5f);
                        blizzardEffect.Set("speed", 0.6f);

                        blizzardEffect.FadeIn(8f);
                        break;
                }
                break;
        }
    }
    private void StopAllWeatherEffects()
    {
        StopRain();
        StopSnow();
        StopLightning();
        StopLeaves();
        StopWind();
        StopBlizzard();
        StopFrost();
        StopPollen();
    }
    private void OnDestroy()
    {
        if (activeEffect != null)
            activeEffect.Stop(0f);

        //if (lightningEffect != null)
        //    lightningEffect.Stop(0f);
    }

    private void doLighting()
    {
        if (lightningEffect == null)
            return;

        lightningTimer -= Time.deltaTime;

        if (lightningTimer > 0f)
            return;

        lightningTimer = 1f;

        if (Random.value <= lightningChance)
        {
            lightningEffect.Strike();
        }
    }

    private void Update()
    {
        if (currentWeather == WeatherType.Storm)
            doLighting();

        if (!weatherProggression || activeEffect == null)
            return;

        // Countdown until next weather evolution
        nextEvolution -= Time.deltaTime;

        if (nextEvolution <= 0f)
        {
            // Mostly increasing, sometimes decreasing
            float change = Random.Range(-0.04f, 0.12f);

            targetWeatherIntensity = Mathf.Clamp(
                targetWeatherIntensity + change,
                0.01f,
                maxRainDensity
            );

            nextEvolution = Random.Range(1f, 5f);
        }

        // Smooth transition towards the target
        currentWeatherIntensity = Mathf.MoveTowards(
            currentWeatherIntensity,
            targetWeatherIntensity,
            rainChangeSpeed * Time.deltaTime
        );

        activeEffect.Set("density", currentWeatherIntensity);

        // Stop progression after reaching maximum
        if (currentWeatherIntensity >= maxRainDensity - 0.001f)
        {
            weatherProggression = false;
        }
    }
    
}