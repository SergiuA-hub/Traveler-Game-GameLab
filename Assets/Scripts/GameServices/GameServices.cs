using UnityEngine;

public class GameServices : MonoBehaviour
{
    public static GameServices Instance { get; private set; }

    [Header("Managers")]
    public TimeManager timeManager;
    public NpcManager npcManager;
    public WeatherManager weatherManager;
    public NewCampManager campManager;
    public SettlementsManager settlementManager;
    public LicenseManager licenseManager;
    public ContractManager contractManager;
    public EnvironmentManager environmentManager;
    public WeatherVisualManager weatherVisual;

    [Header("UI")]
    public HUDScript HUD;
    public TraderNpcUI traderNpcUI;
    public BarterNpcUI barterNpcUI;
    public InformerNpcUI informerNpcUI;

    [Header("Player")]
    public Player_M player;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}