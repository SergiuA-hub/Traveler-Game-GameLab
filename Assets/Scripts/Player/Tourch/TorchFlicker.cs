using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TorchFlicker : MonoBehaviour
{
    [SerializeField] private Light2D torchLight;

    [Header("Flicker Settings")]
    [SerializeField] private float baseIntensity = 1f;
    [SerializeField] private float flickerAmount = 0.15f;
    [SerializeField] private float flickerSpeed = 5f;

    private float noiseOffset;

    private void Start()
    {
        noiseOffset = Random.Range(0f, 1000f);
    }

    private void Update()
    {
        if (torchLight == null || !torchLight.enabled)
            return;

        float noise = Mathf.PerlinNoise(
            Time.time * flickerSpeed,
            noiseOffset
        );

        float flicker = (noise * 2f - 1f) * flickerAmount;

        torchLight.intensity = baseIntensity + flicker;
    }
}