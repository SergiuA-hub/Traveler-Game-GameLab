using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SleepingBagUI : MonoBehaviour
{
    public Player_M player;
    public TimeManager timeManager;
    public NewCampManager campManager;

    public Slider staminaSlider;
    public TMP_Text staminaValueTxt;
    public TMP_Text staminaIncreaseTxt;
    public TMP_Text sleepQualityText;
    public TMP_Text sleepingOutcomeText;

    public RestOption napOption;
    public RestOption tillMorning;
    public RestOption tillFull;
    
    //used to calculate if stamina whole number changed
    private int staminaValue = 0;
    public RestOptions hoveredOption;
    private void OnEnable()
    {
        if (player == null || player.stats == null)
            return;

        staminaValue = Mathf.FloorToInt(player.stats.currentStamina);
        staminaSlider.value = player.stats.currentStamina;
        staminaValueTxt.text = $"{Mathf.FloorToInt(player.stats.currentStamina)}/{Mathf.FloorToInt(player.stats.maxStamina)}";        
        staminaIncreaseTxt.text = "";
        updateHours(timeManager.currentTime);
        
        //timeManager.onHourChanged.AddListener(updateHours);
    }

    public void OnDisable()
    {
        //timeManager.onHourChanged.RemoveListener(updateHours);
    }

    
    private void Update()
    {
        if(staminaValue != Mathf.FloorToInt(player.stats.currentStamina))
        {
            staminaSlider.value = player.stats.currentStamina;
            staminaValueTxt.text = $"{Mathf.FloorToInt(player.stats.currentStamina)}/{Mathf.FloorToInt(player.stats.maxStamina)}";

            int staminaIncease = campManager.getRestHoursUntillFull();
            tillFull.updateRestTime($"{staminaIncease}h");

            updateHours(timeManager.currentTime);
            showOptionOutcome(hoveredOption);
        }
        
        
        
    }

    public void updateHours(DateTime time)
    {
        if (timeManager.currentTime.Hour < 20 && timeManager.currentTime.Hour >= 6)
        {
            tillMorning.updateRestTime("N/A");
        }
        else
        {
            DateTime nextMorning = time.Date.AddHours(6);

            // Dacă am trecut deja de 06:00, înseamnă că vrem 06:00 de mâine
            if (time >= nextMorning)
            {
                nextMorning = nextMorning.AddDays(1);
            }

            TimeSpan timeUntilMorning = nextMorning - time;

            int hours = (int)Math.Ceiling(timeUntilMorning.TotalHours);

            tillMorning.updateRestTime($"{hours}h");
        }
    }

    public void restSelected(RestOptions restOption)
    {
        if (restOption == RestOptions.Nap)
        {
            int maxStamina = Mathf.FloorToInt(player.stats.maxStamina);
            int currentStamina = Mathf.FloorToInt(player.stats.currentStamina);
            int staminaIncease = campManager.getRestForHours(2);

            campManager.doResting(staminaIncease);
            return;
        }

        if (restOption == RestOptions.TilMorning)
        {
            if (timeManager.currentTime.Hour < 20 && timeManager.currentTime.Hour >= 6)
            {
                return;
            }
            else
            {
                DateTime nextMorning = timeManager.currentTime.Date.AddHours(6);

                if (timeManager.currentTime >= nextMorning)
                {
                    nextMorning = nextMorning.AddDays(1);
                }

                TimeSpan timeUntilMorning = nextMorning - timeManager.currentTime;

                int hours = (int)Math.Ceiling(timeUntilMorning.TotalHours);

                int maxStamina = Mathf.FloorToInt(player.stats.maxStamina);
                int currentStamina = Mathf.FloorToInt(player.stats.currentStamina);

                int staminaIncrease = campManager.getRestForHours(hours);

                campManager.doResting(staminaIncrease);
                return;
            }
        }

        if (restOption == RestOptions.TilFull)
        {
            int maxStamina = Mathf.FloorToInt(player.stats.maxStamina);
            int currentStamina = Mathf.FloorToInt(player.stats.currentStamina);
            int staminaIncease = campManager.getRestHoursUntillFull();

            campManager.doResting(staminaIncease);
            return;
        }
    }

    public void showOptionOutcome(RestOptions restOption)
    {
        hoveredOption = restOption;

        if (restOption == RestOptions.Nap)
        {
            int maxStamina = Mathf.FloorToInt(player.stats.maxStamina);
            int currentStamina = Mathf.FloorToInt(player.stats.currentStamina);
            int staminaIncease = campManager.getRestForHours(2);
            staminaIncreaseTxt.text = $"+{staminaIncease}";
            
            sleepingOutcomeText.text = $"Expected Stamina {currentStamina + staminaIncease}/{maxStamina} by {timeManager.currentTime.AddHours(2).Hour}:00";
            return;
        }

        if (restOption == RestOptions.TilMorning)
        {
            if (timeManager.currentTime.Hour < 20 && timeManager.currentTime.Hour >= 6)
            {
                sleepingOutcomeText.text = "Available only between 20:00 and 06:00";
                return;
            }
            else
            {
                DateTime nextMorning = timeManager.currentTime.Date.AddHours(6);

                if (timeManager.currentTime >= nextMorning)
                {
                    nextMorning = nextMorning.AddDays(1);
                }

                TimeSpan timeUntilMorning = nextMorning - timeManager.currentTime;

                int hours = (int)Math.Ceiling(timeUntilMorning.TotalHours);

                int maxStamina = Mathf.FloorToInt(player.stats.maxStamina);
                int currentStamina = Mathf.FloorToInt(player.stats.currentStamina);

                int staminaIncrease = campManager.getRestForHours(hours);

                staminaIncreaseTxt.text = $"+{staminaIncrease}";

                sleepingOutcomeText.text =
                    $"Expected Stamina {currentStamina + staminaIncrease}/{maxStamina} by {nextMorning.Hour}:00";

                return;
            }
        }

        if (restOption == RestOptions.TilFull)
        {
            int maxStamina = Mathf.FloorToInt(player.stats.maxStamina);
            int currentStamina = Mathf.FloorToInt(player.stats.currentStamina);
            int staminaIncease = campManager.getRestHoursUntillFull();
            
            staminaIncreaseTxt.text = $"+{staminaIncease}";
            sleepingOutcomeText.text = $"Expected Stamina {currentStamina + staminaIncease}/{maxStamina} by {timeManager.currentTime.AddHours(staminaIncease).Hour}:00";
            return;
        }
    }

    public void hideOptionOutcome()
    {
        sleepingOutcomeText.text = "";
        staminaIncreaseTxt.text = "";
    }
    public void setup()
    {

    }    
}
