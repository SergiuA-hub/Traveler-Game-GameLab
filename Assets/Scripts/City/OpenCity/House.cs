using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class House : MonoBehaviour
{
    [SerializeField] private int uiPage;
    [SerializeField] private string textForEnterButton;
    [SerializeField] private  CityUIManager cityUIManager;

    private void Awake()
    {
        cityUIManager = transform.parent.GetComponent<CityUIManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SetButtonText(textForEnterButton);
            cityUIManager.tradeButton.gameObject.SetActive(true);
            cityUIManager.settlementManager.cityUI.ChangePage(uiPage);
           
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            cityUIManager.tradeButton.gameObject.SetActive(false);
            
        }
    }

    private void SetButtonText(string houseString)
    {
        //cityUIManager.tradeButtonText.text = houseString;
    }
}
