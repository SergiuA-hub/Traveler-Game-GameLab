using System.Collections;
using TMPro;
using UnityEngine;

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
    public Player_M player;
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
        playerCaravan.SetActive(false);
        playerCamp.SetActive(true);

        Debug.Log("Camp setup");
        moveCameraToCamp();

        startFadeOut();
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
        //staminaIncrease means hours because the restore is 1/h

    }
}