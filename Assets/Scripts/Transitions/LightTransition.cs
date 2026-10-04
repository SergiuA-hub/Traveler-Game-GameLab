using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightTransition : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Light2D globalLight;
    public TimeManager timeManager;

    public Color dayColor;
    public Color nightColor;

    public int duration;
    void Start()
    {
        // float t = 0f;
        globalLight = GetComponent<Light2D>();
        //globalLight.color = dayColor;

    }

    // Update is called once per frame
    void Update()
    {
        double fact = ((timeManager.currentTime.TimeOfDay.TotalSeconds * 10.0) % (24 * 3600)) / (24.0 * 3600.0);
        //Color.Lerp(*)
        globalLight.color = Color.Lerp(dayColor, nightColor, (float)fact);
        //if (timeManager.currentTime.Hour >= 18)
        //{
        //    StartTransition(dayColor, nightColor);
        //}
    }

    void StartTransition(Color startC, Color targetC)
    {

    }
}
