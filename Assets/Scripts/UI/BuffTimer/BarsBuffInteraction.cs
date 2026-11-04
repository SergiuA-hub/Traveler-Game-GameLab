
using UnityEngine;
using UnityEngine.EventSystems;

public class BarsBuffInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public EmblemBuffInteraction emblem;

    public void OnPointerEnter(PointerEventData eventData)
    {
        emblem.SetBarsHovered(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        emblem.SetBarsHovered(false);
    }
}
