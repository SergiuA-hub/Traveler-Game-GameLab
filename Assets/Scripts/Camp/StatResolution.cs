using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatResolution : MonoBehaviour
{
    public TMP_Text panelName;
    public TMP_Text campStatName;
    public TMP_Text campStatLevel;
    public Slider campStatSlider;
    public TMP_Text worldConditionName;
    public TMP_Text worldConditionLevel;
    public Slider worldConditionSlider;
    public TMP_Text worldConditonValueName;
    public Image worldConditionBakground;
    public TMP_Text statResolutionScore;

    public GameObject barsPanel;
    
    [Header("Animation Settings")]
    public float initialDelay = 0.15f;
    public float sliderDuration = 0.4f;
    public float delayBetweenSliders = 0.15f;
    public float delayBeforeBars = 0.2f;
    public float barDelay = 0.1f;

    [Header("Result Colors")]
    public Color color1 = new Color32(185, 76, 69, 255);   // Red
    public Color color2 = new Color32(217, 130, 54, 255);  // Orange
    public Color color3 = new Color32(212, 179, 76, 255);  // Yellow
    public Color color4 = new Color32(145, 173, 88, 255);  // Light Green
    public Color color5 = new Color32(79, 155, 101, 255);  // Green;
    private Coroutine animationCoroutine;

    public void setup(
        string panelName,
        int campStatLevel,
        string campStatName,
        int worldConditionLevel,
        string ConditionName,
        string conditionValueName)
    {
        // Stop previous animation (if any)
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        // Setup text
        this.panelName.text = panelName;
        this.campStatName.text = campStatName;
        this.worldConditionName.text = ConditionName;
        this.worldConditonValueName.text = conditionValueName;
        
        setconditionColor(worldConditionLevel);
        // Reset UI
        campStatSlider.value = 0;
        worldConditionSlider.value = 0;

        this.campStatLevel.text = "0";
        this.worldConditionLevel.text = "0";

        foreach (Transform child in barsPanel.transform)
        {
            child.gameObject.SetActive(false);
        }

        // Start animation
        animationCoroutine = StartCoroutine(
            AnimateResolution(campStatLevel, worldConditionLevel)
        );

        
    }

    private void setconditionColor(int level)
    {
        switch (level)
        {
            case 1:
                worldConditionBakground.color = Hex("#659653");
                break;

            case 2:
                worldConditionBakground.color = Hex("#9BAE61");
                break;

            case 3:
                worldConditionBakground.color = Hex("#D2BE65");
                break;

            case 4:
                worldConditionBakground.color = Hex("#D4924D");
                break;

            case 5:
                worldConditionBakground.color = Hex("#B94C43");
                break;
        }
    }
    private Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color color);
        return color;
    }

    private IEnumerator AnimateResolution(int campLevel, int worldLevel)
    {
        yield return new WaitForSeconds(initialDelay);

        // 1. Animate Camp Stat
        yield return StartCoroutine(
            AnimateSlider(campStatSlider, campStatLevel, campLevel)
        );

        yield return new WaitForSeconds(delayBetweenSliders);

        // 2. Animate World Condition
        yield return StartCoroutine(
            AnimateSlider(worldConditionSlider, worldConditionLevel, worldLevel)
        );

        yield return new WaitForSeconds(delayBeforeBars);

        // 3. Calculate result
        int activeBars = Mathf.Clamp(5 + campLevel - worldLevel, 1, 5);

        statResolutionScore.text = $"{activeBars * 20}%";
        Color resultColor = GetResultColor(activeBars);

        // 4. Reveal bars sequentially
        for (int i = 0; i < activeBars; i++)
        {
            GameObject bar = barsPanel.transform.GetChild(i).gameObject;

            // Set bar color
            bar.GetComponent<Image>().color = resultColor;

            // Show bar
            bar.SetActive(true);

            yield return new WaitForSeconds(barDelay);
        }

        animationCoroutine = null;
    }

    private IEnumerator AnimateSlider(Slider slider, TMP_Text valueText, int target)
    {
        float elapsed = 0f;

        while (elapsed < sliderDuration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / sliderDuration);

            // Smooth animation (ease out)
            progress = 1f - Mathf.Pow(1f - progress, 3f);

            float currentValue = Mathf.Lerp(0, target, progress);

            slider.value = currentValue;
            valueText.text = Mathf.RoundToInt(currentValue).ToString();

            yield return null;
        }

        // Ensure exact final values
        slider.value = target;
        valueText.text = target.ToString();
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
}
