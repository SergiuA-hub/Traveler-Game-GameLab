using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CampItem
{
    None,
    CookingPot,
    SleepingBag,
    Backpack,
    Cart,
    FireCamp,
    Guest
}

[Serializable]
public class CampStats
{
    [Header("Fire")]
    [Range(1, 5)]
    public int fireLevel = 1;

    public const int MIN_FIRE_LEVEL = 1;
    public const int MAX_FIRE_LEVEL = 5;

    public void IncreaseFire(int amount = 1)
    {
        fireLevel = Mathf.Clamp(
            fireLevel + amount,
            MIN_FIRE_LEVEL,
            MAX_FIRE_LEVEL
        );
    }

    public void DecreaseFire(int amount = 1)
    {
        fireLevel = Mathf.Clamp(
            fireLevel - amount,
            MIN_FIRE_LEVEL,
            MAX_FIRE_LEVEL
        );
    }
}

[System.Serializable]
public class ResourceAmount
{
    public ResourceSO resourceSO;
    public int amount;
    public ResourceAmount(ResourceSO resourceSO, int amount)
    {
        this.resourceSO = resourceSO;
        this.amount = amount;
    }
}

public class NewCampManager : MonoBehaviour
{
    public Button campButton;
    public Player_M player;
    public CampStats campStats = new CampStats();
    private int restTimeCounter = 0;
    private int restDuration = 0;
    public TimeManager timeManager;
    public GameObject playerCaravan;
    public GameObject playerCamp;

    public TMP_Text restScreenText;
    public GameObject restScreenPanel;
    public CanvasGroup restScreenCanvasGroup;

    public float fadeDuration = 1f;

    private Coroutine fadeCoroutine;
    public Camera playerCam;
    public float caravanCameraSize;
    public float CampCamerSize;
    public GameObject campingPanelUI;
    
    public CookingPotUI cookingPotPanel;
    public SleepingBagUI sleepingBagPanel;    
    public FireCampUI fireCampPanel;

    public GameObject campResPanel;
    public GameObject campResList;
    public GameObject campResPrefab;
    public GameObject introText;

    public List<ResourceAmount> campSupplies = new List<ResourceAmount>();
    private void Start()
    {
        caravanCameraSize = playerCam.orthographicSize;
    }

    public void startCamping()
    {
        player.Root();
        startFadeIn("Preparing camp...");
    }

    public void startFadeIn(string textToShow)
    {
        restScreenText.text = textToShow;
        restScreenPanel.SetActive(true);

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(fadeIn());
    }

    private IEnumerator fadeIn()
    {
        float elapsed = 0f;

        restScreenCanvasGroup.alpha = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            restScreenCanvasGroup.alpha =
                Mathf.Lerp(0f, 1f, elapsed / fadeDuration);

            yield return null;
        }

        restScreenCanvasGroup.alpha = 1f;

        fadeCoroutine = null;

        // Fade IN completed
        setupCamp();
    }

    private void setupCamp()
    {
        campSupplies.Clear();
        player.isCamping = true;

        campButton.interactable = false;
        playerCaravan.SetActive(false);
        playerCamp.SetActive(true);
        timeManager.pause();
        campResPanel.SetActive(true);

        Debug.Log("Camp setup");
        moveCameraToCamp();
        
        startFadeOut();
    }

    public void dismissCamp()
    {
        campSupplies.Clear();
        
        updateTempResList();
        campResPanel.SetActive(false);

        playerCaravan.SetActive(true);
        playerCamp.SetActive(false);
        timeManager.resume();
        campButton.interactable = true;
        
        player.isCamping = false;
        Debug.Log("Camp dismissed");
        moveCameraToCaravan();
    }

    public void startFadeOut()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(fadeOut());
    }

    private IEnumerator fadeOut()
    {
        float elapsed = 0f;

        restScreenCanvasGroup.alpha = 1f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            restScreenCanvasGroup.alpha =
                Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            yield return null;
        }

        restScreenCanvasGroup.alpha = 0f;

        restScreenPanel.SetActive(false);

        fadeCoroutine = null;

    }

    public void moveCameraToCamp()
    {
        playerCam.orthographicSize = CampCamerSize;
    }

    public void moveCameraToCaravan()
    {
        playerCam.orthographicSize = caravanCameraSize;
    }

    public void campElementSelected(CampItem campElement)
    {
        if (campElement == CampItem.CookingPot)
            ShowCookingPot();

        if(campElement == CampItem.SleepingBag)
        {
            ShowSleepingBag();
        }

        if(campElement == CampItem.FireCamp)
        {
            ShowFireCamp();
        }
    }

    public void ShowCookingPot()
    {
        campingPanelUI.SetActive(true);

        cookingPotPanel.setup();

        cookingPotPanel.gameObject.SetActive(true);
    }    

    public void dismissCookingPot()
    {
        cookingPotPanel.gameObject.SetActive(false);

        campingPanelUI.SetActive(false);
    }

    public void ShowSleepingBag()
    {
        campingPanelUI.SetActive(true);
        
        sleepingBagPanel.setup();

        sleepingBagPanel.gameObject.SetActive(true);
    }

    public void dismissSleepingBag()
    {
        sleepingBagPanel.gameObject.SetActive(false);        
        campingPanelUI.SetActive(false);
    }

    public void ShowFireCamp()
    {
        campingPanelUI.SetActive(true);

        fireCampPanel.setup();

        fireCampPanel.gameObject.SetActive(true);
    }

    public void dismissFireCamp()
    {
        fireCampPanel.gameObject.SetActive(false);
        campingPanelUI.SetActive(false);
    }
    public int getRestForHours(int hours)
    {
        int currentStamina = Mathf.FloorToInt(player.stats.currentStamina);
        int maxStamina = Mathf.FloorToInt(player.stats.maxStamina);

        int staminaNeeded = maxStamina - currentStamina;

        if (staminaNeeded <= 0)
            return 0;

        //one stamina per hour => hours == stamina restoure
        return Mathf.Min(hours, staminaNeeded);
    }

    public int getRestHoursUntillFull()
    {
        int currentStamina = Mathf.FloorToInt(player.stats.currentStamina);
        int maxStamina = Mathf.FloorToInt(player.stats.maxStamina);

        //one stamina per hour => hours == stamina restoure
        return maxStamina - currentStamina;
    }

    public void doResting(int staminaIncrease)
    {
        campingPanelUI.SetActive(false);
        sleepingBagPanel.gameObject.SetActive(false);

        //staminaIncrease means hours because the restore is 1/h
        Debug.Log($"Rest for {staminaIncrease}");

        player.isResting = true;

        timeManager.fastForwardTime();
        restTimeCounter = 0;

        restScreenPanel.SetActive(true);
        
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeRestScreen(0f, 1f, false));
        restScreenPanel.GetComponent<RestingUI>().showRestingMessage();
        restDuration = staminaIncrease;

        timeManager.onHourChanged.AddListener(hourlyUpdate);
    }

    public void hourlyUpdate(DateTime t)
    {
        int remainingHours = restDuration - restTimeCounter;
        
        if (remainingHours > 0)
        {
            restScreenText.text = $"{remainingHours} hours remaining";
            player.stats.currentStamina = Mathf.Min(player.stats.currentStamina + 1, player.stats.maxThirst);
        }
        else
        {
            restScreenText.text = $"Preparing to wake up...";
        }
        
        if (restTimeCounter >= restDuration)
        {
            timeManager.onHourChanged.RemoveListener(hourlyUpdate);
            StopRest();
        }
        restTimeCounter++;
    }

    public void StopRest()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);        

        fadeCoroutine = StartCoroutine(FadeRestScreen(1f, 0f, true));
        restScreenPanel.GetComponent<RestingUI>().hideRestingMessage();        

        timeManager.normalTime();

        player.isResting = false;
        dismissCamp();
        player.UnRoot();
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

    public void updateTempResList()
    {
        foreach (Transform child in campResList.transform)
        {
            Destroy(child.gameObject);
        }

        if(campSupplies.Count == 0)
        {
            introText.SetActive(true);
            return;
        }else introText.SetActive(false);

        foreach (var res in campSupplies)
        {
            Debug.Log($"Adding {res.resourceSO.itemName} x {res.amount} to camp temp list");
            GameObject go = Instantiate(campResPrefab, campResList.transform);
            go.GetComponent<ResourceDropPrefab>().setup(res);
        }
    }

    public void addResourceToCampUsage(ResourceSO res)
    {
        foreach (ResourceAmount resA in campSupplies)
        {
            if (resA.resourceSO == res)
            {
                resA.amount++;
                updateTempResList();
                return;
            }
        }

        campSupplies.Add(new ResourceAmount(res, 1));
        
        updateTempResList();
    }

    public int getTempAmount(ResourceSO res)
    {
        foreach (ResourceAmount resA in campSupplies)
        {
            if (resA.resourceSO == res)
            {
                return resA.amount;
            }
        }
        return 0;
    }

    public float getFuelAmount()
    {
        float totalFuel = 0;
        foreach (ResourceAmount resA in campSupplies)
        {
            if (resA.resourceSO.resourceType == ResourceType.Fuel)
            {
                totalFuel += resA.resourceSO.stat_restore * resA.amount;
            }
        }
        return totalFuel;
    }

    public float getFoodAmount()
    {
        float totalFood = 0;
        foreach (ResourceAmount resA in campSupplies)
        {
            if (resA.resourceSO.resourceType == ResourceType.Eat)
            {
                totalFood += resA.resourceSO.stat_restore * resA.amount;
            }
        }
        return totalFood;
    }

    public float getDrinkAmount()
    {
        float totalDrink = 0;
        foreach (ResourceAmount resA in campSupplies)
        {
            if (resA.resourceSO.resourceType == ResourceType.Drink)
            {
                totalDrink += resA.resourceSO.stat_restore * resA.amount;
            }
        }
        return totalDrink;
    }

    public float getHPAmount()
    {
        float totalHP = 0;
        foreach (ResourceAmount resA in campSupplies)
        {
            if (resA.resourceSO.HP_restore > 0)
            {
                totalHP += resA.resourceSO.HP_restore * resA.amount;
            }
        }
        return totalHP;
    }
}