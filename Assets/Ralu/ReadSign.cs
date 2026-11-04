using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReadSign : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string directionToShow;
    
    public NpcChatBubble bubble;

    public void OnPointerEnter(PointerEventData eventData)
    {
        bubble.Show(directionToShow);
        //directionPanel.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        bubble.Hide();
    }
}