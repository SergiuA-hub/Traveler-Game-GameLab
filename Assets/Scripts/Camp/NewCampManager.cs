using System;
using System.Collections;
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
    Fire,
    Guest
}

public class NewCampManager : MonoBehaviour
{
    public Button campButton;
    public Player_M player;
    private int restTimeCounter = 0;
    private bool resting = false;
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
        campButton.interactable = false;
        playerCaravan.SetActive(false);
        playerCamp.SetActive(true);
        timeManager.pause();

        Debug.Log("Camp setup");
        moveCameraToCamp();

        startFadeOut();
    }

    public void dismissCamp()
    {
        playerCaravan.SetActive(true);
        playerCamp.SetActive(false);
        timeManager.resume();
        campButton.interactable = true;

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

        timeManager.fastForwardTime();
        restTimeCounter = 0;

        restScreenPanel.SetActive(true);
        resting = true;

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
        resting = false;

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
}