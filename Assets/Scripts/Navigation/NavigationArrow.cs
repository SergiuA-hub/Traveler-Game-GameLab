using UnityEngine;

public class NavigationArrow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform destination;
    [SerializeField] private RectTransform arrowUI;

    private void Update()
    {
        if (player == null || destination == null)
        {
            arrowUI.gameObject.SetActive(false);
            return;
        }

        arrowUI.gameObject.SetActive(true);

        Vector2 direction = destination.position - player.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        arrowUI.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    public void SetDestination(Transform newDestination)
    {
        destination = newDestination;
    }

    public void ClearDestination()
    {
        destination = null;
    }
}