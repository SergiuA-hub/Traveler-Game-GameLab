using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CampManager : MonoBehaviour
{
    public Player_M player;
    public GameObject timeManagerGO;
    public GameObject restScreenPanel;
    public TMP_Text restScreenText;
    public GameObject eventsPanel;
    public EventsUiManager eventsUIManager;
    public CanvasGroup restScreenCanvasGroup;
    public float fadeDuration = 1f;
    
    private TimeManager timeManager;
    private Coroutine fadeCoroutine;
    private float restTimer = 0f;
    private bool resting = false;
    private bool eventOccurred = false;
    private bool eventInProgress = false;
    public int restDuration = 8;
    private int restTimeCounter = 0;
    private int hourForEventTrigger;
    public List<ConsumedResource> foodItems = new List<ConsumedResource>();
    public List<ConsumedResource> drinkItems = new List<ConsumedResource>();

    private float hungerRestoreAmount = 0f;
    private float thirstRestoreAmount = 0f;

    private void Start()
    {
        if(timeManagerGO == null)
        {
            Debug.LogError("TimeManager GameObject reference is not set in CampManager.");
            return;
        }
        timeManager = timeManagerGO.GetComponent<TimeManager>();

        restScreenCanvasGroup.alpha = 0f;
        restScreenPanel.SetActive(false);
    }

    public void StartRest()
    {
        player.isResting = true;

        restScreenText.text = $"Resting - Preparing to sleep...";
        
        restTimeCounter = 0;
        timeManager.fastForwardTime();

        restScreenPanel.SetActive(true);
        resting = true;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeRestScreen(0f, 1f, false));

        restScreenPanel.GetComponent<RestingUI>().showRestingMessage();
        Debug.Log($"Player has started resting and evnet chances are {GlobalSettingsManager.EVENT_HOURLY_CHANCE * restDuration}");
        if (UnityEngine.Random.value < GlobalSettingsManager.EVENT_HOURLY_CHANCE * restDuration)
        {
            eventOccurred = true;
            hourForEventTrigger = Random.Range(1, restDuration + 1);
            Debug.Log($"An event will occur during rest at hour {hourForEventTrigger}.");
        }

        if (eventOccurred)
        {
            Debug.Log("An event has occurred during rest.");
        }
        else
        {
            Debug.Log("No event occurred during rest.");
        }

        player.stats.currentHunger += hungerRestoreAmount;
        player.stats.currentThirst += thirstRestoreAmount;

        timeManager.onHourChanged.AddListener(hourlyUpdate);
    }

    public void StopRest()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        player.stats.currentStamina = player.stats.maxStamina;

        fadeCoroutine = StartCoroutine(FadeRestScreen(1f, 0f, true));
        restScreenPanel.GetComponent<RestingUI>().hideRestingMessage();
        eventOccurred = false;
        resting = false;

        timeManager.normalTime();
        
        player.isResting = false;
    }

    private IEnumerator FadeRestScreen(float from, float to, bool disableAtEnd)
    {
        float elapsed = 0f;
        restScreenCanvasGroup.alpha = from;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            restScreenCanvasGroup.alpha =
                Mathf.Lerp(from, to, elapsed / fadeDuration);

            yield return null;
        }

        restScreenCanvasGroup.alpha = to;

        if (disableAtEnd)
            restScreenPanel.SetActive(false);

        fadeCoroutine = null;
    }

    public void eventEnded()
    {
        eventsPanel.SetActive(false);
        resting = false;
        eventOccurred = false;
        eventInProgress = false;
        timeManager.resume();
    }

    public int calculateRestDuration()
    {
        restDuration = Mathf.Max(1, Mathf.RoundToInt(player.stats.maxStamina - player.stats.currentStamina));
        return restDuration;
    }

    public void CalculateFood()
    {
        foodItems.Clear();

        float statsToRestore =
            player.stats.maxHunger - player.stats.currentHunger;

        if (statsToRestore <= 0)
            return;

        var tempFoodItems = new List<InvetoryItem>();

        foreach (var item in player.invetory.Inventory)
        {
            if (item.resourceSO.resourceType != ResourceType.Eat)
                continue;

            if (item.resourceSO.stat_restore <= 0)
                continue;

            if (item.amount <= 0)
                continue;

            tempFoodItems.Add(item);
        }

        tempFoodItems.Sort((a, b) =>
        {
            float aCostPerPoint =
                a.averageValue / a.resourceSO.stat_restore;

            float bCostPerPoint =
                b.averageValue / b.resourceSO.stat_restore;

            return aCostPerPoint.CompareTo(bCostPerPoint);
        });

        foreach (var item in tempFoodItems)
        {
            if (statsToRestore <= 0)
                break;

            float restorePerItem = item.resourceSO.stat_restore;

            int itemsNeeded =
                Mathf.CeilToInt(statsToRestore / restorePerItem);

            int itemsToConsume =
                Mathf.Min(itemsNeeded, item.amount);

            float restoredAmount =
                itemsToConsume * restorePerItem;

            foodItems.Add(new ConsumedResource(item.resourceSO, itemsToConsume, item.averageValue));

            Debug.Log(
                $"{item.resourceSO.name}: consume {itemsToConsume}, " +
                $"restore {restoredAmount}"
            );

            statsToRestore -= restoredAmount;
        }

        hungerRestoreAmount = 0f;
        foreach (var consumed in foodItems)
        {
            hungerRestoreAmount += consumed.amount * consumed.resourceSO.stat_restore;
        }
    }

    public void CalculateDrink()
    {
        drinkItems.Clear();

        float statsToRestore =
            player.stats.maxThirst - player.stats.currentThirst;

        if (statsToRestore <= 0)
            return;

        var tempDrinkItems = new List<InvetoryItem>();

        foreach (var item in player.invetory.Inventory)
        {
            if (item.resourceSO.resourceType != ResourceType.Drink)
                continue;

            if (item.resourceSO.stat_restore <= 0)
                continue;

            if (item.amount <= 0)
                continue;

            tempDrinkItems.Add(item);
        }

        tempDrinkItems.Sort((a, b) =>
        {
            float aCostPerPoint =
                a.averageValue / a.resourceSO.stat_restore;

            float bCostPerPoint =
                b.averageValue / b.resourceSO.stat_restore;

            return aCostPerPoint.CompareTo(bCostPerPoint);
        });

        foreach (var item in tempDrinkItems)
        {
            if (statsToRestore <= 0)
                break;

            float restorePerItem = item.resourceSO.stat_restore;

            int itemsNeeded =
                Mathf.CeilToInt(statsToRestore / restorePerItem);

            int itemsToConsume =
                Mathf.Min(itemsNeeded, item.amount);

            float restoredAmount =
                itemsToConsume * restorePerItem;

            drinkItems.Add(new ConsumedResource(item.resourceSO, itemsToConsume, item.averageValue));

            Debug.Log(
                $"{item.resourceSO.name}: consume {itemsToConsume}, " +
                $"restore {restoredAmount}"
            );

            statsToRestore -= restoredAmount;
        }

        thirstRestoreAmount = 0f;
        foreach (var consumed in drinkItems)
        {
            thirstRestoreAmount += consumed.amount * consumed.resourceSO.stat_restore;
        }
    }

    public void hourlyUpdate(DateTime t)
    {        
        restTimeCounter++;
        int remainingHours = restDuration - restTimeCounter;
        if (remainingHours > 0)
        {
            restScreenText.text = $"{remainingHours} hours remaining";
        }
        else
        {
            restScreenText.text = $"Preparing to wake up...";
        }
        Debug.Log($"Hour changed to: {t.Hour}. Rest timer: {restTimeCounter}/{restDuration}");
        if(eventOccurred && !eventInProgress && hourForEventTrigger == restTimeCounter)
        {
            eventsPanel.SetActive(true);
            //eventsUIManager.ShowNewEvent();
            eventInProgress = true;
            timeManager.pause();
        }

        if (restTimeCounter >= restDuration)
        {
            timeManager.onHourChanged.RemoveListener(hourlyUpdate);
            StopRest();
        }
    }
}

public class ConsumedResource
{
    public ResourceSO resourceSO;
    public int amount;
    public float averageValue;
    public ConsumedResource(ResourceSO resourceSO, int amount)
    {
        this.resourceSO = resourceSO;
        this.amount = amount;
        this.averageValue = resourceSO.baseValue;
    }

    public ConsumedResource(ResourceSO resourceSO, int amount, float averageValue)
    {
        this.resourceSO = resourceSO;
        this.amount = amount;
        this.averageValue = averageValue;
    }
}