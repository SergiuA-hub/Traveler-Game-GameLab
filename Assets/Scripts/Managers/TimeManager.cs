using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class TimeManager : MonoBehaviour
{
    [Header("Player")]
    public Player_M player;
    
    [Header("Day / Night")]
    [SerializeField] private Light2D globalLight;
    [SerializeField] private float nightIntensity = 0.20f;
    [SerializeField] private float dayIntensity = 1.0f;

    [SerializeField] private float sunriseStart = 5f;
    [SerializeField] private float sunriseEnd = 7f;

    [SerializeField] private float sunsetStart = 20f;
    [SerializeField] private float sunsetEnd = 23f;
    [SerializeField] private Color dayColor = new Color(1f, 0.96f, 0.90f);
    [SerializeField] private Color nightColor = new Color(0.57f, 0.65f, 0.84f);

    [Header("Time Settings")]
    private float hour_duration = GlobalSettingsManager.HOUR_DURATION;
    public bool time_stopped = false;

    [Header("Current Time")]
    public DateTime currentTime;
    private float timeLeft;

    [Header("Events")]
    public UnityEvent<DateTime> onHourChanged;
    public UnityEvent<DateTime> onDayChanged;
    public UnityEvent<DateTime> onWeekChanged;
    public UnityEvent<DateTime> onMonthChanged;
    public UnityEvent<DateTime> onYearChanged;
    public UnityEvent<DateTime> onMorning;
    public UnityEvent<DateTime> onEvening;
    private int lastDay;
    private int lastMonth;
    private int lastYear;
    void Start()
    {
        hour_duration = GlobalSettingsManager.HOUR_DURATION;
        currentTime = new DateTime(1251, 3, 25, 0, 0, 0);
        timeLeft = hour_duration;

        lastDay = currentTime.Day;
        lastMonth = currentTime.Month;
        lastYear = currentTime.Year;
        onHourChanged?.Invoke(currentTime);
    }

    public void pause()
    {
        time_stopped = true;
    }

    public void resume()
    {
        time_stopped = false;
    }

    public void fastForwardTime()
    {
        timeLeft = 2;
        SetTimeSpeed(GlobalSettingsManager.REST_HOUR_DURATION);
        //hour_duration = GlobalSettingsManager.REST_HOUR_DURATION;
    }

    public void normalTime()
    {
        SetTimeSpeed(GlobalSettingsManager.HOUR_DURATION);
        //hour_duration = GlobalSettingsManager.HOUR_DURATION;
    }

    private void HandleTimeInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            time_stopped = true;
        }

        if (keyboard.digit1Key.wasPressedThisFrame)
        {
            normalTime();
        }

        if (keyboard.digit2Key.wasPressedThisFrame)
        {
            SetTimeSpeed(GlobalSettingsManager.HOUR_DURATION / 2);
        }

        if (keyboard.digit3Key.wasPressedThisFrame)
        {
            SetTimeSpeed(GlobalSettingsManager.HOUR_DURATION / 3);
        }

        if (keyboard.digit4Key.wasPressedThisFrame)
        {
            SetTimeSpeed(GlobalSettingsManager.HOUR_DURATION / 4);
        }
    }

    private void UpdateDayNightLight()
    {
        if (globalLight == null)
            return;

        // Cât de mult am progresat prin ora curentă
        float hourProgress = 1f - (timeLeft / hour_duration);

        // Ex:
        // currentTime.Hour = 6
        // hourProgress = 0.5
        // => 06:30
        float currentHour = currentTime.Hour + hourProgress;

        float targetIntensity;

        // Sunrise: 05:00 -> 07:00
        if (currentHour >= sunriseStart && currentHour < sunriseEnd)
        {
            float t = Mathf.InverseLerp(
                sunriseStart,
                sunriseEnd,
                currentHour
            );

            targetIntensity = Mathf.Lerp(
                nightIntensity,
                dayIntensity,
                t
            );
        }

        // Day
        else if (currentHour >= sunriseEnd && currentHour < sunsetStart)
        {
            targetIntensity = dayIntensity;
        }

        // Sunset: 20:00 -> 23:00
        else if (currentHour >= sunsetStart && currentHour < sunsetEnd)
        {
            float t = Mathf.InverseLerp(
                sunsetStart,
                sunsetEnd,
                currentHour
            );

            targetIntensity = Mathf.Lerp(
                dayIntensity,
                nightIntensity,
                t
            );
        }

        // Night
        else
        {
            targetIntensity = nightIntensity;
        }

        globalLight.intensity = targetIntensity;

        float colorTransition = Mathf.InverseLerp(
            nightIntensity,
            dayIntensity,
            targetIntensity
        );

        globalLight.color = Color.Lerp(
            nightColor,
            dayColor,
            colorTransition
        );
    }

    public float getHourDuration()
    {
        return hour_duration;
    }

    void Update()
    {
        HandleTimeInput();
        UpdateDayNightLight();

        if (time_stopped) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = hour_duration;
            currentTime = currentTime.AddHours(1);

            onHourChanged?.Invoke(currentTime);

            if (currentTime.Hour == 7)
            {
                onMorning?.Invoke(currentTime);
            }

            if (currentTime.Hour == 20)
            {
                onEvening?.Invoke(currentTime);
            }

            if (currentTime.Day != lastDay)
            {
                lastDay = currentTime.Day;
                onDayChanged?.Invoke(currentTime);

                if (currentTime.DayOfWeek == DayOfWeek.Monday)
                {
                    onWeekChanged?.Invoke(currentTime);
                }
            }

            if (currentTime.Month != lastMonth)
            {
                lastMonth = currentTime.Month;
                onMonthChanged?.Invoke(currentTime);
            }

            if (currentTime.Year != lastYear)
            {
                lastYear = currentTime.Year;
                onYearChanged?.Invoke(currentTime);
            }
        }
    }

    public void SetTimeSpeed(float speed)
    {
        if (speed <= 0f)
        {
            time_stopped = true;
        }
        else
        {
            time_stopped = false;

            float oldHourDuration = hour_duration;

            // If first frame or switching from paused, prevent division by zero
            if (oldHourDuration <= 0f) oldHourDuration = 0.001f;

            // Calculate % progress already passed
            float progressPercent = 1f - (timeLeft / oldHourDuration);

            // Apply new hour duration
            hour_duration = speed;
            player.stats.setMaxSpeed(hour_duration);
            // Recalculate timeLeft to preserve progress
            timeLeft = hour_duration * (1f - progressPercent);
        }
    }
}