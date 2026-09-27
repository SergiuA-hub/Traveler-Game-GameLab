using TMPro;
using UnityEngine;

public class OptionEndPrefab : MonoBehaviour
{
    public TMP_Text optionDescription;

    private Situation situation;
    private EventsUiManager callBack;
    public void Setup(Situation s, EventsUiManager callBack_)
    {
        situation = s;
        callBack = callBack_;
        optionDescription.text = s.situationDescription;
    }

    public void OnOptionSelected()
    {
        if (callBack != null)
        {
            Debug.Log($"Option selected: {situation.situationName} with phase {situation.phase}");
            callBack.onEndEventPhase();
        }
        else
        {
            Debug.LogError("EventsUiManager reference is not set in OptionPrefab.");
        }
    }
}
