using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour
{
    [Header("Player Components")]
    public Player_M player;
    private PlayerInventory_M inventory;

    public TimeManager timeManager;

    [Header("HUD elements")]
    //Full Component
    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject CityUI;

    //Stamina
    public Slider staminaSlider;
    public TextMeshProUGUI staText;
    //HP
    public Slider hpSlider;
    public TextMeshProUGUI hpText;
    //THIRST
    public Slider thirstSlider;
    public TextMeshProUGUI thirstText;
    //Hunger
    public Slider hungerSlider; 
    public TextMeshProUGUI hungerText;

    [Header("DateTime")]
    public TextMeshProUGUI dateText; 


   
    void Start()
    {
        inventory = player.invetory;
        hpSlider.maxValue = player.stats.maxHp;
        staminaSlider.maxValue = player.stats.maxStamina;
        thirstSlider.maxValue = player.stats.maxThirst;
        hungerSlider.maxValue = player.stats.maxHunger;
    }

   
    void Update()
    {
        UpdateStats();
        UpdateTimeDisplay();
        EnablePlayerHUD();
    }

    void UpdateStats()
    {
        staminaSlider.value = player.stats.currentStamina;
        hpSlider.value = player.stats.currentHp;
        hungerSlider.value = player.stats.currentHunger;
        thirstSlider.value = player.stats.currentThirst;

        staText.text = staminaSlider.value.ToString("0") + "/" + player.stats.maxStamina.ToString();
        hpText.text = hpSlider.value.ToString("0") + "/" + player.stats.maxHp.ToString();
        hungerText.text = hungerSlider.value.ToString("0") + "/" + player.stats.maxHunger.ToString();
        thirstText.text = thirstSlider.value.ToString("0") + "/" + player.stats.maxThirst.ToString();
    }

    void UpdateTimeDisplay ()
    {
        dateText.text = "Day " + timeManager.currentTime.Day.ToString() + "\n" + timeManager.currentTime.Hour.ToString() + ":00";
    }

    private void EnablePlayerHUD()
    {
        if(CityUI.activeSelf)
        {
            //Aici playerul intra in oras
            HUD.SetActive(false);
        }
        if (!CityUI.activeSelf)
        {
            HUD.SetActive(true);
        }
    }
}
