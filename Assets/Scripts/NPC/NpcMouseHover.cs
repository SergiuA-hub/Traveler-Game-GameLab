using UnityEngine;
using UnityEngine.EventSystems;

public class NpcMouseHover : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private NpcChatBubble chatBubble;

    public void OnPointerEnter(PointerEventData eventData)
    {
        chatBubble.Show("Hmm... a familiar face. Coming to trade?");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        chatBubble.Hide();
    }

    private void OnDisable()
    {
        if (chatBubble != null)
            chatBubble.Hide();
    }
}