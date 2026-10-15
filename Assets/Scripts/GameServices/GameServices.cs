using UnityEngine;

public class GameServices : MonoBehaviour
{
    public static GameServices Instance { get; private set; }

    [Header("Managers")]
    public TimeManager timeManager;
    public NpcManager npcManager;
    public WeatherManager weatherManager;

    [Header("UI")]
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