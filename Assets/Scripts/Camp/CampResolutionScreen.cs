using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CampResolutionScreen : MonoBehaviour
{
    public EnvironmentManager envManager;
    public NewCampManager campManager;
    public Player_M player;

    public GameObject resourcesPanelContent;
    public GameObject resPrefab;

    public GameObject firePanelContainer;
    public GameObject shelterPanelContainer;
    public GameObject cargoPanelContainer;
    public GameObject traderPanelContainer;
    public GameObject sleepQPanelContainer;

    [Header("Trader Wellbeing")]
    public TMP_Text traderWellbeingLevel;
    public GameObject traderWellbeingBarsContainer;
    public Slider hungerSlider;
    public Slider thirstSlider;
    public TMP_Text traderWellbeingScore;
    public TMP_Text hungerValue;
    public TMP_Text thistValue;

    public TMP_Text sleepQuality;
    public Slider sleepQualitySlider;
    public TMP_Text staminaRatio;

    [Header("Animation Settings")]
    public float sliderDuration = 0.4f;
    public float delayBetweenSliders = 0.15f;
    public float delayBeforeBars = 0.2f;
    public float barDelay = 0.1f;

    private Coroutine traderAnimationCoroutine;
    [Header("Result Colors")]
    public Color color1 = new Color32(185, 76, 69, 255);   // Red
    public Color color2 = new Color32(217, 130, 54, 255);  // Orange
    public Color color3 = new Color32(212, 179, 76, 255);  // Yellow
    public Color color4 = new Color32(145, 173, 88, 255);  // Light Green
    public Color color5 = new Color32(79, 155, 101, 255);  // Green;

    private void OnEnable()
    {
        firePanelContainer.SetActive(false);
        shelterPanelContainer.SetActive(false);
        cargoPanelContainer.SetActive(false);
        traderPanelContainer.SetActive(false);        
    }
    private IEnumerator ShowResourcesSequentially()
    {
        //// 1. Show assigned resources - hiden from UI
        //foreach (var res in campManager.welnessSupplies)
        //{
        //    GameObject go = Instantiate(resPrefab, assignedResourcesList.transform);
        //    go.GetComponent<ResourceDropPrefab>().setup(res);
        //    go.GetComponent<ResourceDropPrefab>().SetTextColor(Color.white);

        //    yield return new WaitForSeconds(0.2f);
        //}

        //yield return new WaitForSeconds(0.5f);

        // 2. Camp resolution
        firePanelContainer.SetActive(true);
        
        RectTransform rect = firePanelContainer.GetComponent<RectTransform>();

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        firePanelContainer.GetComponent<StatResolution>().setup("Fire", campManager.campStats.fireLevel, "Fire", (int)envManager.temperatureCondition + 1, "Temperature", envManager.temperatureCondition.ToString());

        yield return new WaitForSeconds(0.5f);

        shelterPanelContainer.SetActive(true);
        rect = shelterPanelContainer.GetComponent<RectTransform>();

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        shelterPanelContainer.GetComponent<StatResolution>().setup("Shelter", campManager.campStats.shelterLevel, "Shelter", (int)envManager.weatherCondition + 1, "Weather", envManager.weatherCondition.ToString());
        yield return new WaitForSeconds(0.5f);

        cargoPanelContainer.SetActive(true);
        rect = cargoPanelContainer.GetComponent<RectTransform>();

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        cargoPanelContainer.GetComponent<StatResolution>().setup("Cargo", campManager.campStats.cargoSafetyLevel, "Cargo", (int)envManager.safetyCondition + 1, "Cargo Safety", envManager.safetyCondition.ToString());
        yield return new WaitForSeconds(0.5f);

        // 3. Trader wellbeing
        traderPanelContainer.SetActive(true);
        updateTraderData();

        yield return new WaitForSeconds(0.5f);

        updateSleepQualityPanel();

        // 4. Final result
        sleepQPanelContainer.SetActive(true);
    }

    public void updateSleepQualityPanel()
    {
        sleepQuality.text = $"{campManager.campStats.sleepQuality}%";
        sleepQualitySlider.value = campManager.campStats.sleepQuality;
        staminaRatio.text = $"Stamina {campManager.campStats.staminaRestoreRatio}/h";
    }

    public void updateTraderData()
    {
        // Stop previous animation if still running
        if (traderAnimationCoroutine != null)
        {
            StopCoroutine(traderAnimationCoroutine);
            traderAnimationCoroutine = null;
        }

        // Reset sliders
        hungerSlider.value = 0;
        thirstSlider.value = 0;

        // Reset texts
        traderWellbeingLevel.text = "";
        traderWellbeingScore.text = "";

        // Hide all bars
        foreach (Transform child in traderWellbeingBarsContainer.transform)
        {
            child.gameObject.SetActive(false);
        }

        traderAnimationCoroutine = StartCoroutine(AnimateTraderData());
    }
    private IEnumerator AnimateTraderData()
    {
        yield return new WaitForSeconds(0.15f);

        // 1. Hunger
        yield return StartCoroutine(
            AnimateSlider(hungerSlider, player.stats.currentHunger)
        );

        yield return new WaitForSeconds(delayBetweenSliders);
        hungerValue.text = $"{player.stats.currentHunger.ToString("F0")}/{player.stats.maxHunger.ToString("F0")}";
        // 2. Thirst
        yield return StartCoroutine(
            AnimateSlider(thirstSlider, player.stats.currentThirst)
        );

        yield return new WaitForSeconds(delayBeforeBars);
        thistValue.text = $"{player.stats.currentThirst.ToString("F0")}/{player.stats.maxThirst.ToString("F0")}";

        // 3. Trader Wellbeing
        int wellbeingLevel = campManager.campStats.traderWellbeingLevel;

        traderWellbeingLevel.text = wellbeingLevel.ToString();
        traderWellbeingScore.text = $"{wellbeingLevel * 20}%";

        int activeBars = Mathf.Clamp(wellbeingLevel, 0, 5);

        Color resultColor = GetResultColor(activeBars);

        // 4. Animate bars
        for (int i = 0; i < activeBars; i++)
        {
            GameObject bar = traderWellbeingBarsContainer.transform.GetChild(i).gameObject;

            bar.GetComponent<Image>().color = resultColor;
            bar.SetActive(true);

            yield return new WaitForSeconds(barDelay);
        }

        traderAnimationCoroutine = null;
    }
    private IEnumerator AnimateSlider(Slider slider, float target)
    {
        float elapsed = 0f;

        while (elapsed < sliderDuration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / sliderDuration);

            // Ease Out
            progress = 1f - Mathf.Pow(1f - progress, 3f);

            slider.value = Mathf.Lerp(0f, target, progress);

            yield return null;
        }

        slider.value = target;
    }
    private Color GetResultColor(int activeBars)
    {
        switch (activeBars)
        {
            case 1: return color1;
            case 2: return color2;
            case 3: return color3;
            case 4: return color4;
            default: return color5;
        }
    }

    public void launchCampResolution()
    {
        StartCoroutine(ShowResourcesSequentially());

    }

    public void continuePressed()
    {
        campManager.resumeRestingAfterResolution();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
