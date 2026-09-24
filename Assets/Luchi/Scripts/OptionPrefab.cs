using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class OptionPrefab : MonoBehaviour, IPointerClickHandler
{
    public TMP_Text optionDescription;

    private Situation situation;
    private EventsUiManager callBack;
    public void Setup(Situation s, EventsUiManager callBack_)
    {
        situation = s;
        callBack = callBack_;
        optionDescription.text = s.act;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        OnOptionSelected();
    }

    public void OnOptionSelected()
    {
        if (callBack != null)
        {
            Debug.Log($"Option selected: {situation.situationName} with phase {situation.phase}");
            callBack.DisplaySituations(situation);
        }
        else
        {
            Debug.LogError("EventsUiManager reference is not set in OptionPrefab.");
        }
    }
}
