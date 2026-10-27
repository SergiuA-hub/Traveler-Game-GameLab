
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HaggleUI : MonoBehaviour
{
    [Header("Info")]
    public TMP_Text attempts;
    public TMP_Text discount;
    public Image resourceIcon;
    public TMP_Text resourceName;
    public TMP_Text resourcePrice;

    [Header("Bars")]
    [SerializeField] private Image[] haggleBars = new Image[24];

    [SerializeField] private Color greenColor;
    [SerializeField] private Color redColor;
    [SerializeField] private Color neutralColor;

    [Header("Indicator")]
    [SerializeField] private RectTransform indicator;
    [SerializeField] private RectTransform indicatorPanel;
    [SerializeField] private float indicatorSpeed = 300f;

    [Header("Haggling")]
    [SerializeField] private int maxAttempts = 3;
    [SerializeField] private int discountPerHit = 5;
    [SerializeField] private float pauseBetweenAttempts = 0.75f;

    private int currentAttempt;
    private int currentDiscount;

    public TraderNpcUI baseTraderUI;
    public GameObject resourcesPanel;
    public GameObject stopButton;
    public GameObject backButton;

    private TradeGoodPrefab haggledGood;
    private HaggleBarType[] barTypes = new HaggleBarType[24];

    private float indicatorPosition;
    private int indicatorDirection = 1;
    private bool indicatorMoving;    
    private float originalPrice;
    public void startHaggleForResoruce(TradeGoodPrefab tradeGood)
    {
        haggledGood = tradeGood;

        gameObject.SetActive(true);

        GenerateHaggleBars();

        currentAttempt = 0;
        currentDiscount = 0;

        attempts.text = $"Attempts: 1/{maxAttempts}";
        discount.text = "Discount: 0%";
        discount.color = Color.black;

        resourceIcon.sprite = haggledGood.tradeGood.resource.sprite;
        resourceName.text = haggledGood.tradeGood.resource.itemName;
        originalPrice = haggledGood.tradeGood.price;
        resourcePrice.text = $"{originalPrice:F1}";

        ResetIndicator();
        stopButton.SetActive(true);
        backButton.SetActive(false);
        indicatorMoving = true;
    }


    private void Update()
    {
        if (!indicatorMoving)
            return;

        MoveIndicator();
    }


    private void ResetIndicator()
    {
        indicatorDirection = 1;

        //indicator starts in the center of first segment
        indicatorPosition = GetSegmentCenterX(0);

        UpdateIndicatorPosition();
    }



    private void MoveIndicator()
    {
        float minX = GetSegmentCenterX(0);
        float maxX = GetSegmentCenterX(haggleBars.Length - 1);

        indicatorPosition += indicatorDirection
                             * indicatorSpeed
                             * Time.unscaledDeltaTime;

        if (indicatorPosition >= maxX)
        {
            indicatorPosition = maxX;
            indicatorDirection = -1;
        }
        else if (indicatorPosition <= minX)
        {
            indicatorPosition = minX;
            indicatorDirection = 1;
        }

        UpdateIndicatorPosition();
    }

    private void UpdateIndicatorPosition()
    {
        indicator.anchoredPosition = new Vector2(
            indicatorPosition,
            indicator.anchoredPosition.y
        );
    }

    private float GetSegmentCenterX(int index)
    {
        RectTransform segmentRect =
            haggleBars[index].rectTransform;

        // center of the segment
        Vector3 worldCenter = segmentRect.TransformPoint(
            segmentRect.rect.center
        );

        //convert the points into Indicator Panel coordinates
        Vector3 localCenter =
            indicatorPanel.InverseTransformPoint(worldCenter);

        // adjust the ancors of indicator
        return localCenter.x +
               indicatorPanel.rect.width * indicatorPanel.pivot.x;
    }


    private void GenerateHaggleBars()
    {
        int greenCount = Random.Range(3, 9);
        int redCount = 12 - greenCount;

        List<HaggleBarType> types = new List<HaggleBarType>();

        for (int i = 0; i < greenCount; i++)
            types.Add(HaggleBarType.Green);

        for (int i = 0; i < redCount; i++)
            types.Add(HaggleBarType.Red);

        while (types.Count < haggleBars.Length)
            types.Add(HaggleBarType.Neutral);

        for (int i = types.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (types[i], types[j]) = (types[j], types[i]);
        }

        for (int i = 0; i < haggleBars.Length; i++)
        {
            barTypes[i] = types[i];

            haggleBars[i].color = barTypes[i] switch
            {
                HaggleBarType.Green => greenColor,
                HaggleBarType.Red => redColor,
                _ => neutralColor
            };
        }
    }

    public HaggleBarType GetBarType(int index)
    {
        return barTypes[index];
    }

    public void SetBarNeutral(int index)
    {
        barTypes[index] = HaggleBarType.Neutral;
        haggleBars[index].color = neutralColor;
    }


    public void StopIndicator()
    {
        if (!indicatorMoving || currentAttempt >= maxAttempts)
            return;

        indicatorMoving = false;

        //find closests indicator
        int hitIndex = GetCurrentSegmentIndex();

        HaggleBarType hitType = GetBarType(hitIndex);

        switch (hitType)
        {
            case HaggleBarType.Green:
                currentDiscount += discountPerHit;
                Debug.Log($"HAGGLE: GREEN +{discountPerHit}%");
                break;

            case HaggleBarType.Red:
                currentDiscount -= discountPerHit;
                Debug.Log($"HAGGLE: RED -{discountPerHit}%");
                break;

            case HaggleBarType.Neutral:
                Debug.Log("HAGGLE: MISS");
                break;
        }

        //set used segment to neutral
        SetBarNeutral(hitIndex);

        currentAttempt++;

        discount.text = $"Discount: {currentDiscount:+0;-0;0}%";

        discount.color = currentDiscount > 0 ? greenColor :
                         currentDiscount < 0 ? redColor :
                         Color.black;
        UpdateHagglePrice();

        if (currentAttempt >= maxAttempts)
        {
            attempts.text = $"Attempts: {maxAttempts}/{maxAttempts}";
            UpdateHagglePrice();

            stopButton.SetActive(false);
            backButton.SetActive(true);

            return;
        }

        //attempts.text = $"Attempts: {currentAttempt}/{maxAttempts}";

        StartCoroutine(StartNextAttempt());
    }
    private IEnumerator StartNextAttempt()
    {
        yield return new WaitForSecondsRealtime(pauseBetweenAttempts);

        attempts.text = $"Attempts: {currentAttempt + 1}/{maxAttempts}";

        ResetIndicator();
        indicatorMoving = true;
    }
    private void UpdateHagglePrice()
    {
        float multiplier = 1f - (currentDiscount / 100f);

        float newPrice = originalPrice * multiplier;

        haggledGood.tradeGood.price = newPrice;

        resourcePrice.text = $"{newPrice:F1}";

        haggledGood.updatePrice();
    }

    private int GetCurrentSegmentIndex()
    {
        int closestIndex = 0;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < haggleBars.Length; i++)
        {
            float segmentCenterX = GetSegmentCenterX(i);

            float distance = Mathf.Abs(
                indicatorPosition - segmentCenterX
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    public void backToResources()
    {
        this.gameObject.SetActive(false);
        resourcesPanel.SetActive(true);
    }

}

public enum HaggleBarType
{
    Neutral,
    Green,
    Red
}
