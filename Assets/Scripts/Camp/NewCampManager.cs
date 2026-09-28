using System.Collections;
using TMPro;
using UnityEngine;

public class NewCampManager : MonoBehaviour
{
    public Player_M player;
    public GameObject playerCaravan;
    public GameObject playerCamp;


    public TMP_Text restScreenText;
    public GameObject restScreenPanel;
    public CanvasGroup restScreenCanvasGroup;

    public float fadeDuration = 1f;

    private Coroutine fadeCoroutine;
    public Camera playerCam;
    public float caravanCameraSize;
    public float CampCamerSize;

    private void Start()
    {
        caravanCameraSize = playerCam.orthographicSize;
    }

    public void startCamping()
    {
        player.Root();
        startFadeIn("Preparing camp...");
    }

    public void startFadeIn(string textToShow)
    {
        restScreenText.text = textToShow;
        restScreenPanel.SetActive(true);

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(fadeIn());
    }

    private IEnumerator fadeIn()
    {
        float elapsed = 0f;

        restScreenCanvasGroup.alpha = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            restScreenCanvasGroup.alpha =
                Mathf.Lerp(0f, 1f, elapsed / fadeDuration);

            yield return null;
        }

        restScreenCanvasGroup.alpha = 1f;

        fadeCoroutine = null;

        // Fade IN completed
        setupCamp();
    }

    private void setupCamp()
    {
        playerCaravan.SetActive(false);
        playerCamp.SetActive(true);

        Debug.Log("Camp setup");
        moveCameraToCamp();

        startFadeOut();
    }

    public void startFadeOut()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(fadeOut());
    }

    private IEnumerator fadeOut()
    {
        float elapsed = 0f;

        restScreenCanvasGroup.alpha = 1f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            restScreenCanvasGroup.alpha =
                Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            yield return null;
        }

        restScreenCanvasGroup.alpha = 0f;

        restScreenPanel.SetActive(false);

        fadeCoroutine = null;

    }

    public void moveCameraToCamp()
    {
        playerCam.orthographicSize = CampCamerSize;
    }
}