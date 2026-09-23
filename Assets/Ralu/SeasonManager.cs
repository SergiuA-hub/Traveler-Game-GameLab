using UnityEngine;

public class SeasonManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string season;
    public HUDScript HUDScript;

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
    }
}
