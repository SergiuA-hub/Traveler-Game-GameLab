using TMPro;
using UnityEngine;

public class EnterCityNotification : MonoBehaviour
{
    [Header("Notification")]
    [SerializeField] private GameObject target;
    [SerializeField] private float fadeInTime;
    [SerializeField] private float vizibleTime;
    [SerializeField] private float fadeOutTime;


    [Header("Components")]
    [SerializeField] private TextMeshProUGUI SettlementNameText;
    [SerializeField] private SettlementSO settlementSO;

    //Private Components
    private CanvasGroup canvasGroup;
    private float control = -1f;

    private void Awake()
    {
        canvasGroup= target.GetComponent<CanvasGroup>();
        
        //Start on hidden
        canvasGroup.alpha = 0;
        target.gameObject.SetActive(false);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            SettlementNameText.text = settlementSO.settlementName;
            
        }
    }

    private void Update()
    {
        if (control < 0) return;

    }


}
