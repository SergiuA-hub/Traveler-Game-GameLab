using TMPro;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    [SerializeField] private Transform player;

    [Header("UI")]
    [SerializeField] private RectTransform arrow;
    [SerializeField] private TMP_Text destinationText;
    [SerializeField] private GameObject navigationPanel;

    private Transform currentDestination;

    private void Update()
    {
        UpdateNavigation();
    }

    private void UpdateNavigation()
    {
        if (currentDestination == null)
        {
            navigationPanel.SetActive(false);
            return;
        }

        navigationPanel.SetActive(true);

        Vector2 direction =
            currentDestination.position - player.position;

        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // dacă sprite-ul săgeții indică UP by default
        arrow.rotation = Quaternion.Euler(
            0f,
            0f,
            angle - 90f
        );
    }

    public void SetDestination(
        Transform destination,
        string destinationName)
    {
        currentDestination = destination;

        destinationText.text = destinationName;
    }

    public void ClearDestination()
    {
        currentDestination = null;
        destinationText.text = "";
    }
}