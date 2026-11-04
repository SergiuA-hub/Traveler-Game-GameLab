
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class EmblemBuffInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject emblemTimer;
    public GameObject barBuffDetails;

    [SerializeField] private float exitDelay = 3f;

    private bool isEmblemHovered;
    private bool isBarsHovered;

    private Coroutine exitCoroutine;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isEmblemHovered = true;

        CancelHide();

        emblemTimer.SetActive(false);
        barBuffDetails.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isEmblemHovered = false;
        TryStartHide();
    }

    public void SetBarsHovered(bool hovered)
    {
        isBarsHovered = hovered;

        if (hovered)
            CancelHide();
        else
            TryStartHide();
    }

    private void TryStartHide()
    {
        if (isEmblemHovered || isBarsHovered)
            return;

        if (exitCoroutine == null)
            exitCoroutine = StartCoroutine(HideDetailsAfterDelay());
    }

    private void CancelHide()
    {
        if (exitCoroutine == null)
            return;

        StopCoroutine(exitCoroutine);
        exitCoroutine = null;
    }

    private IEnumerator HideDetailsAfterDelay()
    {
        yield return new WaitForSeconds(exitDelay);

        if (!isEmblemHovered && !isBarsHovered)
        {
            emblemTimer.SetActive(true);
            barBuffDetails.SetActive(false);
        }

        exitCoroutine = null;
    }

    private void OnDisable()
    {
        CancelHide();
    }
}
