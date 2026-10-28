using TMPro;
using UnityEngine;

public class NpcChatBubble : MonoBehaviour
{
    [SerializeField] private GameObject bubbleRoot;
    [SerializeField] private TMP_Text bubbleText;

    private float hideAt;
    private bool autoHide;

    private void Awake()
    {
        Hide();
    }

    private void Update()
    {
        if (autoHide && Time.time >= hideAt)
        {
            Hide();
        }
    }

    public void Show(string message, float duration = 0f)
    {
        bubbleText.text = message;
        bubbleRoot.SetActive(true);

        autoHide = duration > 0f;
        hideAt = Time.time + duration;
    }

    public void Hide()
    {
        autoHide = false;
        bubbleRoot.SetActive(false);
    }
}