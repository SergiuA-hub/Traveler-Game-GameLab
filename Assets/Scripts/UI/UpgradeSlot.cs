using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UpgradeSlot : MonoBehaviour, IDropHandler
{
    [Tooltip("If off, dropping on an occupied slot is rejected instead of swapping.")]
    public bool allowSwap = true;

    public DragableUpgrade CurrentItem => GetComponentInChildren<DragableUpgrade>();

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        DragableUpgrade dropped = eventData.pointerDrag.GetComponent<DragableUpgrade>();
        if (dropped == null) return;

        // The dragged item has been moved to the canvas root while dragging,
        // so any DraggableItem still under this slot is a different item.
        DragableUpgrade current = CurrentItem;

        if (current != null)
        {
            if (!allowSwap) return; // dropped item will snap back home

            // Send the existing item to the slot the dragged one came from.
            current.transform.SetParent(dropped.parentAfterDrag, false);
            current.transform.localPosition = Vector3.zero;
        }

        // Tell the dragged item to land here (applied in its OnEndDrag).
        dropped.parentAfterDrag = transform;
    }
}



