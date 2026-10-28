using UnityEngine;

public class Light : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Light globalLight;
    public TimeManager timeManager;

    public Color dayColor;
    public Color nightColor;

    public int duration;
    void Start()
    {
        globalLight = GetComponent<Light>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timeManager.currentTime.Hour >= 18)
        {
            StartTransition(dayColor, nightColor);
        }
    }

    void StartTransition(Color startC, Color targetC)
    {
        
    }
}
