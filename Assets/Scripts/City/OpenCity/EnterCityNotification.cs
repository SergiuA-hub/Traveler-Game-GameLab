using TMPro;
using UnityEngine;

public class EnterCityNotification : MonoBehaviour
{
    [SerializeField] private GameObject EnterVilageNotificator;
    [SerializeField] private TextMeshProUGUI SettlementNameText;
    [SerializeField] private SettlementSO settlementSO;
    [SerializeField] private float duration;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            SettlementNameText.text = settlementSO.settlementName;
            
            
                EnterVilageNotificator.SetActive(true);
                
            



        }
    }

   
}
