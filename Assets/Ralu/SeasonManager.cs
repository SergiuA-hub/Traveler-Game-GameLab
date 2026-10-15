using UnityEngine;

public enum Season
{
    Spring,
    Summer,
    Fall,
    Winter
}

public class SeasonManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Season season;
    public HUDScript HUDScript;
    public EnvironmentManager environmentManager;
    public WeatherManager weatherManager;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        HUDScript.currentSeason = season;
        HUDScript.ChangeSeason();
        environmentManager.setSeason(season);
        weatherManager.setCurrentSeason(season);
    }
}
