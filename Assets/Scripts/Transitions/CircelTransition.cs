using UnityEngine;

using System.Collections;

public class CircelTransition : MonoBehaviour
{
    public RectTransform circle;
    public float duration = 0.8f;

    public void StartTransition( )
    {
        StartCoroutine(Transition());
    }

    IEnumerator Transition()
    {
        float time = 0f;
        

        circle.localScale = Vector3.zero;
        circle.gameObject.SetActive(true);

        // suficient de mare cât să acopere ecranul
        float targetScale = 30f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;
            
            t = Mathf.SmoothStep(0f, 1f, t);

            circle.localScale =
                Vector3.Lerp(Vector3.zero,Vector3.one * targetScale,t);

            yield return null;
        }

        circle.localScale = Vector3.one * targetScale;
        if( circle.localScale == Vector3.one * targetScale)
        {
            time += Time.deltaTime;

            float t = time / duration;
            t = Mathf.SmoothStep(0f, 1f, t);
            
            circle.localScale =
               Vector3.Lerp(Vector3.one * targetScale, Vector3.zero,t);
        }


    }
}
