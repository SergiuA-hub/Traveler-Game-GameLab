using UnityEngine;

public class FogScaling : MonoBehaviour
{
    private Vector3 startScale;
    private float offset;

    public float scaleAmount = 0.03f;
    public float scaleSpeed = 0.4f;


    void Start()
    {
        startScale = transform.localScale;

        offset = Random.Range(0f, 100f);
    }

    // Update is called once per frame
    void Update()
    {
        float breathing = 1f + Mathf.Sin(Time.time * scaleSpeed + offset) * scaleAmount;

        transform.localScale = startScale * breathing;
    }
}
