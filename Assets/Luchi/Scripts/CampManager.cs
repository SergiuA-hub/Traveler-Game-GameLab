using System.Collections;
using UnityEngine;

public class CampManager : MonoBehaviour
{
    public GameObject timeManagerGO;
    public GameObject restScreenPanel;
    public GameObject eventsPanel;
    public EventsUiManager eventsUIManager;
    public CanvasGroup restScreenCanvasGroup;
    public float restDuration = 5f;
    public float fadeDuration = 1f;
    
    private TimeManager timeManager;
    private Coroutine fadeCoroutine;
    private float restTimer = 0f;
    private bool resting = false;
    private bool eventOccurred = false;
    private bool eventInProgress = false;
    private void Start()
    {
        if(timeManagerGO == null)
        {
            Debug.LogError("TimeManager GameObject reference is not set in CampManager.");
            return;
        }
        timeManager = timeManagerGO.GetComponent<TimeManager>();

        restScreenCanvasGroup.alpha = 0f;
        restScreenPanel.SetActive(false);
    }

    public void StartRest()
    {
        restScreenPanel.SetActive(true);
        resting = true;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeRestScreen(0f, 1f, false));

        restScreenPanel.GetComponent<RestingUI>().showRestingMessage();
        if (Random.value < 0.3f)
        {
            // 30% chance
            eventOccurred = true;
        }

        if (eventOccurred)
        {
            Debug.Log("An event has occurred during rest.");
        }
        else
        {
            Debug.Log("No event occurred during rest.");
        }
    }

    public void StopRest()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeRestScreen(1f, 0f, true));
        restScreenPanel.GetComponent<RestingUI>().hideRestingMessage();
        eventOccurred = false;
        resting = false;
    }

    private IEnumerator FadeRestScreen(float from, float to, bool disableAtEnd)
    {
        float elapsed = 0f;
        restScreenCanvasGroup.alpha = from;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            restScreenCanvasGroup.alpha =
                Mathf.Lerp(from, to, elapsed / fadeDuration);

            yield return null;
        }

        restScreenCanvasGroup.alpha = to;

        if (disableAtEnd)
            restScreenPanel.SetActive(false);

        fadeCoroutine = null;
    }

    public void eventEnded()
    {
        eventsPanel.SetActive(false);
        resting = false;
        eventOccurred = false;
        eventInProgress = false;
        StopRest();
    }

    // Update is called once per frame
    void Update()
    {
        if (!resting)
            return;
        restTimer += Time.deltaTime;

        if(restTimer >= restDuration/2)
        {
            if(eventOccurred && !eventInProgress)
            {
                eventsPanel.SetActive(true);
                //eventsUIManager.ShowNewEvent();
                eventInProgress = true;
            }
        }

        if (restTimer >= restDuration)
        {
            Debug.Log("Rest timer completed.");
            restTimer = 0f;
            if(!eventOccurred)
            {
                StopRest();
            }
            
        }
    }
}