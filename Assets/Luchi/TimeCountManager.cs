using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TimeCountManager : MonoBehaviour
{
    [Header("Time Settings")]
    public float hour_duration = 5f;
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
            SetTimeSpeed(5f);
        }

        if (keyboard.digit2Key.wasPressedThisFrame)
        {
            SetTimeSpeed(2.5f);
        }

        if (keyboard.digit3Key.wasPressedThisFrame)
        {
            SetTimeSpeed(1.25f);
        }

        if (keyboard.digit4Key.wasPressedThisFrame)
        {
            SetTimeSpeed(0.3f);
        }            
    }



    void Update()
    {
        HandleTimeInput();

        if (time_stopped) return;
        
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = hour_duration;
            currentTime = currentTime.AddHours(1);

            onHourChanged?.Invoke(currentTime);
            
            if(currentTime.Hour == 7)
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

            // Recalculate timeLeft to preserve progress
            timeLeft = hour_duration * (1f - progressPercent);
        }
    }
}
