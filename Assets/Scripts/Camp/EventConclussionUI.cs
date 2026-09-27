using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventConclussionUI : MonoBehaviour
{
    public TMP_Text statsText;
    public CampManager campManager;

    public void EndEvent()
    {        
        campManager.eventEnded();
        gameObject.SetActive(false);
    }
    
    public void Setup(List<EventOutcome> outcomes)
    {
        // Any setup logic for the event conclusion UI can go here
        statsText.text = $"DEBUG - Event Outcomes:\n{string.Join("\n", outcomes)}";
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
