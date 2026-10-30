using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class DragableUpgrade :MonoBehaviour
{
    [Tooltip("Transparency while dragging.")]
    [Range(0f, 1f)] public float dragAlpha = 0.8f;

    // The parent the item returns to when the drag ends.
    // ItemSlot.OnDrop changes this before OnEndDrag runs.
    [HideInInspector] public Transform parentAfterDrag;

    RectTransform rectTransform;
    CanvasGroup canvasGroup;
    Canvas rootCanvas;
    RectTransform canvasRect;
    Vector3 grabOffset;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
        canvasRect = (RectTransform)rootCanvas.transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;

        // Move to the top of the canvas so it draws above everything else.
        transform.SetParent(rootCanvas.transform, true);
        transform.SetAsLastSibling();

        // Let raycasts pass through the item so the slot underneath gets OnDrop.
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = dragAlpha;

        // Remember where on the item we grabbed it, so it doesn't jump.
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvasRect, eventData.position, eventData.pressEventCamera, out Vector3 pointerWorld))
        {
            grabOffset = rectTransform.position - pointerWorld;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Works for Screen Space Overlay, Screen Space Camera and World Space canvases.
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvasRect, eventData.position, eventData.pressEventCamera, out Vector3 pointerWorld))
        {
            rectTransform.position = pointerWorld + grabOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Either the new slot (set in ItemSlot.OnDrop) or the original parent.
        transform.SetParent(parentAfterDrag, false);
        transform.localPosition = Vector3.zero; // snap to the slot's center

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
    }
}
