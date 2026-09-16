using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EventsUiManager : MonoBehaviour
{
    public EventConclussionUI eventConclussion;
    //public CampManager campManager;
    public TMP_Text EventDscription;

    public GameObject optionsPrefab;
    public GameObject endEventPrefab;
    public GameObject optionsList;
    public TMP_Text contextText;
    public SituationalEventsManager situationManager;

    [SerializeField] private float textSpeed = 0.03f;

    private Coroutine typingCoroutine;
    private List<Context> currentContexts = new List<Context>();

    public void Start()
    {
        if (situationManager == null)
        {
            Debug.LogError(
                "SituationalEventsManager reference is not set in EventsUiManager."
            );

            return;
        }
        Situation startSituation = situationManager.pickStartSituation();
        currentContexts.AddRange(startSituation.GivenContexts);

        DisplaySituations(startSituation);

    }

    public void DisplaySituations(Situation situationToShow)
    {
        foreach(Transform child in optionsList.transform)
        {
            Destroy(child.gameObject);
        }

        if (situationToShow == null)
        {
            Debug.LogError("No start situation found.");
            return;
        }
        contextText.text = $"Debug Contexts: {string.Join(", ", currentContexts)}";

        Debug.Log($"Picked start situation: {situationToShow.situationName}");

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(
            TypeText(situationToShow)
        );

        
    }

    private IEnumerator TypeText(Situation situation)
    {
        EventDscription.text = $"({situation.phase})";

        foreach (char character in situation.situationDescription)
        {
            EventDscription.text += character;

            yield return new WaitForSecondsRealtime(textSpeed);
        }

        typingCoroutine = null;

        ShowOptions(situation);
    }

    public void updateContext(Situation situation)
    {
        // Remove old narrative states first.
        foreach (Context context in situation.RemovedContexts)
        {
            if (currentContexts.Remove(context))
            {
                Debug.Log($"Removing context: {context}");
            }
        }

        // Then add the new states created by this situation.
        foreach (Context context in situation.GivenContexts)
        {
            if (!currentContexts.Contains(context))
            {
                currentContexts.Add(context);
                Debug.Log($"Adding context: {context}");
            }
        }
    }

    public void ShowOptions(Situation situation)
    {
        updateContext(situation);
        contextText.text = $"Debug Contexts: {string.Join(", ", currentContexts)}";

        Debug.Log(
            $"Current contexts after {situation.situationName}: " +
            string.Join(", ", currentContexts)
        );
        
        List<Situation> options =
            situationManager.getSituationsByPhase(
                situation.nextPhase,
                currentContexts
            );

        Debug.Log(
            $"Found {options.Count} options for phase {situation.nextPhase}"
        );

        if (options.Count == 0)
        {
            if(situation.nextPhase == Phase.End)
            {
                Debug.Log("Game Over. No more situations to display.");
                EventDscription.text += "\n\nEVENT ENDS.";
                GameObject endOption = Instantiate(endEventPrefab, optionsList.transform);
                endOption.GetComponent<OptionEndPrefab>().Setup(situation, this);
                return;
            }
            Debug.LogWarning(
                $"DEAD END after '{situation.situationName}'. " +
                $"Phase: {situation.nextPhase}. " +
                $"Contexts: {string.Join(", ", currentContexts)}"
            );
        }

        int optionCount = Mathf.Min(options.Count, 3);

        for (int i = 0; i < optionCount; i++)
        {
            Situation option = options[i];

            GameObject optionButton =
                Instantiate(optionsPrefab, optionsList.transform);

            optionButton
                .GetComponent<OptionPrefab>()
                .Setup(option, this);
        }
    }

    public void onEndEventPhase()
    {
        eventConclussion.Setup();
        eventConclussion.Show();
    }

    public void ShowNewEvent()
    {
        Situation startSituation = situationManager.pickStartSituation();

        if(startSituation == null)
        {
            Debug.LogError("No start situation found.");
            return;
        }
        currentContexts.Clear();
        currentContexts.AddRange(startSituation.GivenContexts);

        contextText.text = $"Debug Contexts: {string.Join(", ", currentContexts)}";
        DisplaySituations(startSituation);
    }

    private void OnEnable()
    {
        ShowNewEvent();
    }

    public void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ShowNewEvent();
        }
    }
}