using UnityEngine;

public class InformerNpc : MonoBehaviour, INpcInteraction
{
    public float weatherBasePrice;

    public float springWeatherPrice;
    public float summerWeahterPrice;
    public float fallWeatherPrice;
    public float witnerWheaterPrice;

    public void Interact()
    {
        Debug.Log("Open Informer UI");
        GameServices.Instance.informerNpcUI.Open(this, GameServices.Instance.player);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Season startSeason = GetComponent<Npc>().currentRoad.startSettlement.region;
        Season endSeason = GetComponent<Npc>().currentRoad.endSettlement.region;

        if (startSeason == Season.Spring || endSeason == Season.Spring)
        {
            springWeatherPrice = weatherBasePrice;
        }
        else springWeatherPrice = weatherBasePrice * 1.2f;

        if (startSeason == Season.Summer || endSeason == Season.Summer)
        {
            summerWeahterPrice = weatherBasePrice;
        }
        else summerWeahterPrice = weatherBasePrice * 1.2f;

        if (startSeason == Season.Fall || endSeason == Season.Fall)
        {
            fallWeatherPrice = weatherBasePrice;
        }
        else fallWeatherPrice = weatherBasePrice * 1.2f;

        if (startSeason == Season.Winter || endSeason == Season.Winter)
        {
            witnerWheaterPrice = weatherBasePrice;
        }
        else witnerWheaterPrice = weatherBasePrice * 1.2f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
