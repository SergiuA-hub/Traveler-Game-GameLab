using UnityEngine;

using System.Collections;

public class CircelTransition : MonoBehaviour
{
    public RectTransform circle;
    public float duration = 0.8f;
    [SerializeField]private float targetScale = 30f;

    [SerializeField] private float time;
    private bool isExpanding;
    private bool isShrinking;

    
    public void StartCircleTransition()
    {
        if (isExpanding) return;
        isExpanding = true;
        isShrinking = false;

        
    }

    public void EndCircleTransition()
    {
        isExpanding = false;
        isShrinking = true;

    }

   public void Transition(GameObject ui)
    {
        StartCircleTransition();
        Vector3 fullscale = Vector3.one * targetScale;
        if (isExpanding)
        {
           
            circle.localScale = Vector3.Lerp(Vector3.zero, fullscale, duration);   
        
            if(circle.localScale == fullscale)
            {
                ui.SetActive(true);
                EndCircleTransition();
            }
        }

        if (isShrinking)
        {
            circle.localScale = Vector3.Lerp(fullscale, Vector3.zero, duration);
        }
        
    }
}
