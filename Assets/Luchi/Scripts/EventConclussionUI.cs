using UnityEngine;

public class EventConclussionUI : MonoBehaviour
{
    public CampManager campManager;

    public void EndEvent()
    {        
        campManager.eventEnded();
        gameObject.SetActive(false);
    }
    
    public void Setup()
    {
        // Any setup logic for the event conclusion UI can go here
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
