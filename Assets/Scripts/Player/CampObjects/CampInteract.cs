using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.EventSystems;

public class CampInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public SpriteRenderer spriteRenderer;
    public CampItem campItem;

    [Header("Hover")]
    public float hoverScale = 1.08f;
    //public Color hoverColor = new Color(1f, 1f, 0.75f, 1f);
    private Vector3 originalScale;
    //private Color originalColor;

    private void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        originalScale = transform.localScale;
        //originalColor = spriteRenderer.color;
    }
    public NewCampManager campManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = originalScale * hoverScale;
        //spriteRenderer.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
        //spriteRenderer.color = originalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        campManager.campElementSelected(campItem);
    }
}
