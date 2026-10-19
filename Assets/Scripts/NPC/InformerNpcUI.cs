using HeyHeyThere.WeatherFX;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using static UnityEditor.Rendering.MaterialUpgrader;

public class InformerNpcUI : MonoBehaviour
{
    public GameObject optionsPanel;
    public GameObject weatherPanel;
    public GameObject tradeDealsPanel;
    public TMP_Text headerReplicas;

    [Header("Weather")]
    public GameObject springWeatherPanel;
    public TMP_Text springWeatherPrice;
    public Button springWeatherButton;

    public GameObject summerWeatherPanel;
    public TMP_Text summerWeatherPrice;
    public Button summerWeatherButton;

    public GameObject fallWeatherPanel;
    public TMP_Text fallWeatherPrice;
    public Button fallWeatherButton;

    public GameObject winterWeatherPanel;
    public TMP_Text winterWeatherPrice;
    public Button winterWeatherButton;

    public TMP_Text weatherInfoText;

    [Header("Trade")]
    public GameObject springTradePanel;
    public TMP_Text springTradePrice;
    public Button springTradeButton;

    public GameObject summerTradePanel;
    public TMP_Text summerTradePrice;
    public Button summerTradeButton;

    public GameObject fallTradePanel;
    public TMP_Text fallTradePrice;
    public Button fallTradeButton;

    public GameObject winterTradePanel;
    public TMP_Text winterTradePrice;
    public Button winterTradeButton;

    public TMP_Text tradeInfoText;
    private Season requestedSeason;
    private WeatherType requestedWeather;
    private float weatherRemainingDuration;
    private InformerNpc informer;
    private Player_M player;

    public void Open(InformerNpc npc, Player_M player)
    {
        informer = npc;
        this.player = player;

        if (informer == null)
            return;

        if (player == null)
            return;

        informer.npc.Root();
        player.Root();
        headerReplicas.text = informer.replicas[UnityEngine.Random.Range(0, informer.replicas.Count)];

        optionsPanel.SetActive(true);
        weatherPanel.SetActive(false);
        tradeDealsPanel.SetActive(false);
        
        setupSeasonPanels();
        setupTradePanels();

        gameObject.SetActive(true);
    }

    public void setupSeasonPanels()
    {
        springWeatherPanel.SetActive(true);
        springWeatherButton.interactable = true;

        summerWeatherPanel.SetActive(true);
        summerWeatherButton.interactable = true;

        fallWeatherPanel.SetActive(true);
        fallWeatherButton.interactable = true;

        winterWeatherPanel.SetActive(true);
        winterWeatherButton.interactable = true;

        Season currentSeason = GameServices.Instance.weatherManager.currentSeason;
        
        if (currentSeason == Season.Spring)
        {
            springWeatherPanel.SetActive(false);
        }

        if (currentSeason == Season.Summer)
        {
            summerWeatherPanel.SetActive(false);
        }

        if (currentSeason == Season.Fall)
        {
            fallWeatherPanel.SetActive(false);
        }

        if (currentSeason == Season.Winter)
        {
            winterWeatherPanel.SetActive(false);
        }

        springWeatherPrice.text = $"{informer.springWeatherPrice:F1}";
        if (informer.springWeatherPrice > player.invetory.CurrentCoins)
            springWeatherButton.interactable = false;

        summerWeatherPrice.text = $"{informer.summerWeahterPrice:F1}";
        if (informer.summerWeahterPrice > player.invetory.CurrentCoins)
            summerWeatherButton.interactable = false;

        fallWeatherPrice.text = $"{informer.fallWeatherPrice:F1}";
        if (informer.fallWeatherPrice > player.invetory.CurrentCoins)
            fallWeatherButton.interactable = false;
        
        winterWeatherPrice.text = $"{informer.witnerWheaterPrice:F1}";
        if (informer.witnerWheaterPrice > player.invetory.CurrentCoins)
            winterWeatherButton.interactable = false;
    }

    public void setupTradePanels()
    {
        Debug.Log($"Spring price:{informer.springTradePrice} vs inv coins:{player.invetory.CurrentCoins}");
        springTradePanel.SetActive(true);        
        springTradePrice.text = $"{informer.springTradePrice:F1}";
        springTradeButton.interactable = player.invetory.CurrentCoins >= informer.springTradePrice;

        Debug.Log($"Summer price:{informer.summerTradePrice} vs inv coins:{player.invetory.CurrentCoins}");
        summerTradePanel.SetActive(true);
        summerTradePrice.text = $"{informer.summerTradePrice:F1}";
        summerTradeButton.interactable = player.invetory.CurrentCoins >= informer.summerTradePrice;

        Debug.Log($"Fall price:{informer.fallTradePrice} vs inv coins:{player.invetory.CurrentCoins}");
        fallTradePanel.SetActive(true);
        fallTradePrice.text = $"{informer.fallTradePrice:F1}";
        fallTradeButton.interactable = player.invetory.CurrentCoins >= informer.fallTradePrice;

        Debug.Log($"Winter price:{informer.winterTradePrice} vs inv coins:{player.invetory.CurrentCoins}");
        winterTradePanel.SetActive(true);
        winterTradePrice.text = $"{informer.winterTradePrice:F1}";
        winterTradeButton.interactable = player.invetory.CurrentCoins >= informer.winterTradePrice;
    }

    //called by spring Werather ui button
    public void springWeatherSelected()
    {
        requestedSeason = Season.Spring;
        requestedWeather = GameServices.Instance.weatherManager.springWeather;
        weatherRemainingDuration = GameServices.Instance.weatherManager.springWeatherDuration;
        
        summerWeatherPanel.SetActive(false);
        fallWeatherPanel.SetActive(false);
        winterWeatherPanel.SetActive(false);
        player.invetory.CurrentCoins -= informer.springWeatherPrice;

        springWeatherButton.interactable = false;

        string infoText = informer.weatherForecastText[UnityEngine.Random.Range(0, informer.weatherForecastText.Count)];
        infoText = infoText.Replace("{season}", requestedSeason.ToString())
           .Replace("{weather}", requestedWeather.ToString())
           .Replace("{hours}", weatherRemainingDuration.ToString());
        weatherInfoText.text = infoText;
    }

    public void springTradeSelected()
    {        
        summerTradePanel.SetActive(false);
        fallTradePanel.SetActive(false);
        winterTradePanel.SetActive(false);
        springTradeButton.interactable = false;

        player.invetory.CurrentCoins -= informer.springTradePrice;

        requestedSeason = Season.Spring;
        string infoText = informer.tradeDealsText[UnityEngine.Random.Range(0, informer.weatherForecastText.Count)];

        infoText = infoText.Replace("{season}", Season.Spring.ToString())
            .Replace("{highResource}", informer.springHighResource.tradeGood.resource.itemName)
            .Replace("{highSettlement}", informer.springHighResource.settlement.settlementName)
            .Replace("{highPrice}", informer.springHighResource.tradeGood.price.ToString("0.#"))
            .Replace("{lowResource}", informer.springLowResource.tradeGood.resource.itemName)
            .Replace("{lowSettlement}", informer.springLowResource.settlement.settlementName)
            .Replace("{lowPrice}", informer.springLowResource.tradeGood.price.ToString("0.#"));

        tradeInfoText.text = infoText;
    }

    //called by summer Weather ui button
    public void summerWeatherSelected()
    {
        requestedSeason = Season.Summer;
        requestedWeather = GameServices.Instance.weatherManager.summerWeather;
        weatherRemainingDuration = GameServices.Instance.weatherManager.summerWeatherDuration;
        player.invetory.CurrentCoins -= informer.summerWeahterPrice;

        springWeatherPanel.SetActive(false);
        fallWeatherPanel.SetActive(false);
        winterWeatherPanel.SetActive(false);

        string infoText = informer.weatherForecastText[UnityEngine.Random.Range(0, informer.weatherForecastText.Count)];
        infoText = infoText.Replace("{season}", Season.Spring.ToString())
           .Replace("{weather}", requestedWeather.ToString())
           .Replace("{hours}", weatherRemainingDuration.ToString());
        weatherInfoText.text = infoText;
        summerWeatherButton.interactable = false;
    }

    public void summerTradeSelected()
    {
        springTradePanel.SetActive(false);
        fallTradePanel.SetActive(false);
        winterTradePanel.SetActive(false);
        summerTradeButton.interactable = false;

        player.invetory.CurrentCoins -= informer.summerTradePrice;

        string infoText = informer.tradeDealsText[UnityEngine.Random.Range(0, informer.weatherForecastText.Count)];

        infoText = infoText.Replace("{season}", Season.Summer.ToString())
            .Replace("{highResource}", informer.summerHighResource.tradeGood.resource.itemName)
            .Replace("{highSettlement}", informer.summerHighResource.settlement.settlementName)
            .Replace("{highPrice}", informer.summerHighResource.tradeGood.price.ToString("0.#"))
            .Replace("{lowResource}", informer.summerLowResource.tradeGood.resource.itemName)
            .Replace("{lowSettlement}", informer.summerLowResource.settlement.settlementName)
            .Replace("{lowPrice}", informer.summerLowResource.tradeGood.price.ToString("0.#"));

        tradeInfoText.text = infoText;
    }

    //called by fall Weather ui button
    public void fallWeatherSelected()
    {
        requestedSeason = Season.Fall;
        requestedWeather = GameServices.Instance.weatherManager.fallWeather;
        weatherRemainingDuration = GameServices.Instance.weatherManager.fallWeatherDuration;
        player.invetory.CurrentCoins -= informer.fallWeatherPrice;

        springWeatherPanel.SetActive(false);
        summerWeatherPanel.SetActive(false);
        winterWeatherPanel.SetActive(false);

        string infoText = informer.weatherForecastText[UnityEngine.Random.Range(0, informer.weatherForecastText.Count)];

        if (requestedWeather == WeatherType.Clear)
            infoText = "Ah, {season}! The skies are peaceful, and the winds are gentle. How long they'll stay that way, no one can say. These lands have a mind of their own!";
        
        infoText = infoText.Replace("{season}", requestedSeason.ToString())
           .Replace("{weather}", requestedWeather.ToString())
           .Replace("{hours}", weatherRemainingDuration.ToString());
        
        weatherInfoText.text = infoText;
        
        fallWeatherButton.interactable = false;
    }

    public void fallTradeSelected()
    {
        springTradePanel.SetActive(false);
        summerTradePanel.SetActive(false);
        winterTradePanel.SetActive(false);
        fallTradeButton.interactable = false;

        player.invetory.CurrentCoins -= informer.fallTradePrice;

        string infoText = informer.tradeDealsText[UnityEngine.Random.Range(0, informer.weatherForecastText.Count)];

        infoText = infoText.Replace("{season}", Season.Fall.ToString())
            .Replace("{highResource}", informer.fallHighResource.tradeGood.resource.itemName)
            .Replace("{highSettlement}", informer.fallHighResource.settlement.settlementName)
            .Replace("{highPrice}", informer.fallHighResource.tradeGood.price.ToString("0.#"))
            .Replace("{lowResource}", informer.fallLowResource.tradeGood.resource.itemName)
            .Replace("{lowSettlement}", informer.fallLowResource.settlement.settlementName)
            .Replace("{lowPrice}", informer.fallLowResource.tradeGood.price.ToString("0.#"));

        tradeInfoText.text = infoText;
    }

    //called by winter weahter ui button
    public void winterWeatherSelected()
    {
        requestedSeason = Season.Winter;
        requestedWeather = GameServices.Instance.weatherManager.winterWeather;
        weatherRemainingDuration = GameServices.Instance.weatherManager.winterWeatherDuration;
        player.invetory.CurrentCoins -= informer.witnerWheaterPrice;

        springWeatherPanel.SetActive(false);
        summerWeatherPanel.SetActive(false);
        fallWeatherPanel.SetActive(false);

        string infoText = informer.weatherForecastText[UnityEngine.Random.Range(0, informer.weatherForecastText.Count)];
        infoText = infoText.Replace("{season}", requestedSeason.ToString())
           .Replace("{weather}", requestedWeather.ToString())
           .Replace("{hours}", weatherRemainingDuration.ToString());
        weatherInfoText.text = infoText;
        winterWeatherButton.interactable = false;
    }

    public void winterTradeSelected()
    {
        springTradePanel.SetActive(false);
        summerTradePanel.SetActive(false);
        fallTradePanel.SetActive(false);
        winterTradeButton.interactable = false;

        player.invetory.CurrentCoins -= informer.winterTradePrice;

        string infoText = informer.tradeDealsText[UnityEngine.Random.Range(0, informer.weatherForecastText.Count)];

        infoText = infoText.Replace("{season}", Season.Winter.ToString())
            .Replace("{highResource}", informer.winterHighResource.tradeGood.resource.itemName)
            .Replace("{highSettlement}", informer.winterHighResource.settlement.settlementName)
            .Replace("{highPrice}", informer.winterHighResource.tradeGood.price.ToString("0.#"))
            .Replace("{lowResource}", informer.winterLowResource.tradeGood.resource.itemName)
            .Replace("{lowSettlement}", informer.winterLowResource.settlement.settlementName)
            .Replace("{lowPrice}", informer.winterLowResource.tradeGood.price.ToString("0.#"));

        tradeInfoText.text = infoText;
    }

    private void OnDisable()
    {
        if (informer == null)
            return;

        if (player == null)
            return;

        informer.npc.UnRoot();
        player.UnRoot();
        informer.npc.interested = false;
    }

    public void leaveTrader()
    {
        gameObject.SetActive(false);
    }
}
