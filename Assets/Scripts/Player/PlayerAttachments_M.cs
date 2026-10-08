using NUnit.Framework;
using System;
using UnityEngine;

public class PlayerAttachments_M : MonoBehaviour
{
    public TimeManager timeManager;

    public ShopItem[] playerItems;
    public GameObject travelLamp;

    public void Start()
    {
        timeManager.onMorning.AddListener(turnOffLamp);
        timeManager.onEvening.AddListener(turnOnLamp);
    }

    public void turnOffLamp(DateTime t)
    {
        travelLamp.SetActive(false);
    }

    public void turnOnLamp(DateTime t)
    {
        travelLamp.SetActive(true);
    }
}
