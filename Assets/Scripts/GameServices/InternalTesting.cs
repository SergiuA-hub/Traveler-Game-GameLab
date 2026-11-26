using System;
using UnityEngine;

public class InternalTesting : MonoBehaviour
{
    public GameObject WinzardIntro;

    public GameObject debugButton;
    public GameObject debugOptions;
    private bool optionsEnabled;
    private bool fastForward;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        optionsEnabled = false;
        fastForward = false;

        if (GameServices.Instance.internalTesting)
            debugButton.SetActive(true);
        else debugButton.SetActive(false);

        if (GameServices.Instance.internalTesting)
        {
            WinzardIntro.SetActive(false);
        }
        else
        {
            WinzardIntro.SetActive(true);
        }

        debugOptions.SetActive(optionsEnabled);
    }

    public void addCoins()
    {
        if (!GameServices.Instance.internalTesting)
            return;

        GameServices.Instance.player.invetory.CurrentCoins += 100;
    }

    public void TenXTime()
    {
        if (!GameServices.Instance.internalTesting)
            return;

        fastForward = !fastForward;

        if (fastForward)
        {
            GameServices.Instance.timeManager.SetTimeSpeed(GlobalSettingsManager.HOUR_DURATION / 10);
            GameServices.Instance.player.stats.acceleration = 20f;
        }
        else
        {
            GameServices.Instance.timeManager.SetTimeSpeed(GlobalSettingsManager.HOUR_DURATION);
            GameServices.Instance.player.stats.acceleration = 1f;
        }
    }

    public void restoreStamina()
    {
        if (!GameServices.Instance.internalTesting)
            return;

        GameServices.Instance.player.stats.currentStamina = GameServices.Instance.player.stats.maxStamina;
    }

    public void restoreHunger()
    {
        if (!GameServices.Instance.internalTesting)
            return;

        GameServices.Instance.player.stats.currentHunger = GameServices.Instance.player.stats.maxHunger;
    }

    public void restoreThirst()
    {
        if (!GameServices.Instance.internalTesting)
            return;

        GameServices.Instance.player.stats.currentThirst = GameServices.Instance.player.stats.maxThirst;
    }

    public void doProduction()
    {
        if (!GameServices.Instance.internalTesting)
            return;

        GameServices.Instance.settlementManager.dailyEvents(new DateTime());
    }

    public void toggleDebugOptions()
    {
        if (!GameServices.Instance.internalTesting)
            return;

        optionsEnabled = !optionsEnabled;

        debugOptions.SetActive(optionsEnabled);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
