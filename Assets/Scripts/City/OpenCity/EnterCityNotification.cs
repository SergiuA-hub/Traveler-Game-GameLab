using TMPro;
using UnityEngine;

public class EnterCityNotification : MonoBehaviour
{
    [Header("Notification")]
    [SerializeField] private GameObject target;
    [SerializeField] private Transform entryPoint;
    [SerializeField] private float fadeInTime;
    [SerializeField] private float visibleTime;
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
        if (canvasGroup == null) canvasGroup = target.AddComponent<CanvasGroup>();

        //Start on hidden
        canvasGroup.alpha = 0;
        target.gameObject.SetActive(false);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 ToEntry = entryPoint.position - transform.position;
            Vector2 ToPlayer = collision.transform.position - transform.position;
            if (Vector2.Dot(ToEntry, ToPlayer) <= 0) return;

            if (!target.activeSelf)
            {
                canvasGroup.alpha = 0f;
                target.SetActive(true);
                SettlementNameText.text = settlementSO.settlementName;
            }
   
        }
        control = canvasGroup.alpha * fadeInTime;
    }

    private void Update()
    {
        if (control < 0) return;
        control += Time.deltaTime;

        float fadeOutStart = fadeInTime + visibleTime;
        float end = fadeOutStart + fadeOutTime;

        if (control < fadeInTime)
            canvasGroup.alpha = control / fadeInTime;                               // fading in
        else if (control < fadeOutStart)
            canvasGroup.alpha = 1f;                                                 // fully visible
        else if (control < end)
            canvasGroup.alpha = 1f - (control - fadeOutStart) / fadeOutTime;        // fading out
        else
        {
            canvasGroup.alpha = 0f;
            target.SetActive(false);
            control = -1f;
        }

    }


}
