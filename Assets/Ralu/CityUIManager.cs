using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CityUIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Canvas cityUI;
    bool playerInCity = false;
    bool UIup = false;
    public TextMeshProUGUI prompt;
    public Button tradeButton; 

    public Vector2 input;

    [SerializeField] private GameInput gameInput;


    void Start()
    {
        
    }

    private void GameInput_OnInteract(object sender, System.EventArgs e)
    {
        cityUI.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

        /*if (Input.GetKeyDown(KeyCode.E) && playerInCity && !UIup)
        {
            cityUI.gameObject.SetActive(true);
            UIup = true;
            //prompt.gameObject.SetActive(false);
        }*/

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInCity = true;
            prompt.gameObject.SetActive(true);
            //prompt.gameObject.SetActive(true);
            gameInput.OnInteract += GameInput_OnInteract;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            cityUI.gameObject.SetActive(false);
            UIup = false;
            playerInCity = false;
            prompt.gameObject.SetActive(false) ;
            // prompt.gameObject.SetActive(false);
            gameInput.OnInteract -= GameInput_OnInteract;
        }
    }
    
}
