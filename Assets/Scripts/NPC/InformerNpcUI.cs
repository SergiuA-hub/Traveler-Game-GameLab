using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class InformerNpcUI : MonoBehaviour
{
    public GameObject springPanel;
    public TMP_Text springPrice;
    public Button springButton;

    public GameObject summerPanel;
    public TMP_Text summerPrice;
    public Button summerButton;

    public GameObject fallPanel;
    public TMP_Text fallPrice;
    public Button fallButton;
    public GameObject winterPanel;
    public TMP_Text winterPrice;
    public Button winterButton;

    private InformerNpc infromer;
    private Player_M player;

    public void Open(InformerNpc npc, Player_M player)
    {
        infromer = npc;
        this.player = player;

        setupSeasonPanels();

        

        gameObject.SetActive(true);
    }

    public void setupSeasonPanels()
    {
        springPanel.SetActive(true);
        summerPanel.SetActive(true);
        fallPanel.SetActive(true);
        winterPanel.SetActive(true);

        Season currentSeason = GameServices.Instance.weatherManager.currentSeason;
        
        if (currentSeason == Season.Spring)
        {
            springPanel.SetActive(false);
        }

        if (currentSeason == Season.Summer)
        {
            summerPanel.SetActive(false);
        }

        if (currentSeason == Season.Fall)
        {
            fallPanel.SetActive(false);
        }

        if (currentSeason == Season.Winter)
        {
            winterPanel.SetActive(false);
        }

        springPrice.text = $"{infromer.springWeatherPrice:F1}";
        if (infromer.springWeatherPrice > player.invetory.CurrentCoins)
            springButton.interactable = false;

        summerPrice.text = $"{infromer.summerWeahterPrice:F1}";
        if (infromer.summerWeahterPrice > player.invetory.CurrentCoins)
            summerButton.interactable = false;

        fallPrice.text = $"{infromer.fallWeatherPrice:F1}";
        if (infromer.fallWeatherPrice > player.invetory.CurrentCoins)
            fallButton.interactable = false;
        
        winterPrice.text = $"{infromer.witnerWheaterPrice:F1}";
        if (infromer.witnerWheaterPrice > player.invetory.CurrentCoins)
            winterButton.interactable = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
