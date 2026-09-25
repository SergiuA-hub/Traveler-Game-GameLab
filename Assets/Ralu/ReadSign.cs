using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReadSign : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string directionToShow;

    public TextMeshProUGUI directionPanel;

    public void OnPointerEnter(PointerEventData eventData)
    {
        directionPanel.text = $"<b>{directionToShow}</b>";
        //directionPanel.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        directionPanel.text = "Follow road signs to find your way.";
        //directionPanel.gameObject.SetActive(false);
    }
}