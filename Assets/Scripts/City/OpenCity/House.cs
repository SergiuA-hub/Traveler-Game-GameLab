using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class House : MonoBehaviour
{
    public int uiPage;
    [SerializeField] string textButton;
    public Button openShop;
    [SerializeField] private TextMeshProUGUI openShopText;
    public CityUI cityUI;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            
            openShop.gameObject.SetActive(true);
            cityUI.ChangePage(uiPage);
            SetButtonText(textButton);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            openShop.gameObject.SetActive(false);
            cityUI.ChangePage(uiPage);
        }
    }

    private void SetButtonText(string houseString)
    {
        openShopText.text = houseString;
    }
}
