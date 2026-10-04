using UnityEngine;

public enum WeatherType
{
    Clear,
    Rain,
    Snow,
    Storm
}
public class WeatherManager : MonoBehaviour
{
    public HUDScript HUD;
    public EnvironmentManager environmentManager;
    public WeatherType currentWeather;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentWeather = WeatherType.Clear;

        environmentManager.setWeather(currentWeather);
        updateUI();
    }

    public void updateUI()
    {
        HUD.currentWeather = currentWeather;
        HUD.ChangeWeather();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
