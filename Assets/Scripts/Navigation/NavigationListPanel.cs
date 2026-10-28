using UnityEngine;
using UnityEngine.EventSystems;

public class NavigationListPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isClosing = false;
    public float closingTimer = 1f;
    private float currentTimer = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isClosing = false;
        currentTimer = 0f;
    }

    public void stopClosing()
    {
        isClosing = false;
        currentTimer = 0f;
    }

    public void startClosing()
    {
        isClosing = true;
        currentTimer = 0f;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        stopClosing();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        startClosing();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isClosing)
            return;
        currentTimer += Time.deltaTime;
        if(currentTimer >= closingTimer)
        {
            this.gameObject.SetActive(false);
            isClosing = true;
        }
    }
}
