using Unity.VisualScripting;
using UnityEngine;

public class NoCampingArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            GameServices.Instance.campManager.allowCamp(false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            GameServices.Instance.campManager.allowCamp(true);
        Debug.Log($"For camping itneract OUT with {collision.gameObject.tag}");
    }
}
