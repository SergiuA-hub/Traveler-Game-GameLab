using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour
{
    public Player_M player;
    private PlayerInventory_M inventory;

    [Header ("UI elements")]
    public Slider staminaSlider;
    public TextMeshProUGUI staText;
    public Slider hpSlider;
    public TextMeshProUGUI hpText;
    public Slider thirstSlider;
    public TextMeshProUGUI thirstText;
    public Slider hungerSlider; 
    public TextMeshProUGUI hungerText;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = player.invetory;
        hpSlider.maxValue = player.stats.maxHp;
        staminaSlider.maxValue = player.stats.maxStamina;
        thirstSlider.maxValue = player.stats.maxThirst;

    }

    // Update is called once per frame
    void Update()
    {
        UpdateStats();
    }

    void UpdateStats()
    {
        staminaSlider.value = player.stats.currentStamina;
        hpSlider.value = player.stats.currentHp;
        hungerSlider.value = player.stats.currentHunger;
        thirstSlider.value = player.stats.currentThirst;

        

        staText.text = staminaSlider.value.ToString() + "/" + player.stats.maxStamina.ToString();
        hpText.text = hpSlider.value.ToString() + "/" + player.stats.maxHp.ToString();
        hungerText.text = hungerSlider.value.ToString() + "/" + player.stats.maxHunger.ToString();
        thirstText.text = thirstSlider.value.ToString() + "/" + player.stats.maxThirst.ToString();
    }
}
