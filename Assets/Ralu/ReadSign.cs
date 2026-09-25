using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReadSign : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string directionToShow;

    public TextMeshProUGUI directionPanel;

    public void OnPointerEnter(PointerEventData eventData)
    {
        directionPanel.text = directionToShow;
        directionPanel.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        directionPanel.text = "???";
        directionPanel.gameObject.SetActive(false);
    }
}