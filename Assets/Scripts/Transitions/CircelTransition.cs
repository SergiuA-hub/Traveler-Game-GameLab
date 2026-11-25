using UnityEngine;
using System;
using System.Collections;

public class CircelTransition : MonoBehaviour
{
    public RectTransform circle;
    public GameObject hideUI;
    public GameObject showUI;
    //For grow and shrink
    public float duration = 0.5f;    
    public float hold = 0.2f;     

    bool playing;

    void Awake()
    {
        circle.gameObject.SetActive(false);
    }

   
    public void Play()
    {
        Play(null);
    }

    
    public void Play(Action onCovered)
    {
        if (!playing) StartCoroutine(Run(onCovered));


    }

    IEnumerator Run(Action onCovered)
    {
        playing = true;

        // Big enough to cover the screen corner to corner
        RectTransform canvas = (RectTransform)circle.GetComponentInParent<Canvas>(true).rootCanvas.transform;
        float size = canvas.rect.size.magnitude * 1.1f * canvas.lossyScale.x / circle.parent.lossyScale.x;
        circle.localScale = Vector3.zero;           // start invisible, no one-frame flash
        circle.sizeDelta = new Vector2(size, size);

        circle.gameObject.SetActive(true);
        circle.SetAsLastSibling();                  // draw on top of all other UI

        yield return Scale(0f, 1f);                 // grow: cover the screen

        if (hideUI) hideUI.SetActive(false);        // swap the UI while it's covered
       
        onCovered?.Invoke();
        circle.SetAsLastSibling();                  // keep the circle above the new UI

        yield return new WaitForSecondsRealtime(hold);
        if (showUI) showUI.SetActive(true);
        yield return Scale(1f, 0f);                 // shrink: reveal the new UI

        circle.gameObject.SetActive(false);
        playing = false;
    }

    IEnumerator Scale(float from, float to)
    {
        for (float t = 0f; t < duration; t += Time.unscaledDeltaTime)
        {
            float k = Mathf.SmoothStep(0f, 1f, t / duration);
            circle.localScale = Vector3.one * Mathf.Lerp(from, to, k);
            yield return null;
        }
        circle.localScale = Vector3.one * to;
    }

}
