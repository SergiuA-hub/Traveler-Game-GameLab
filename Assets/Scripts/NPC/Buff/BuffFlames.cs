using UnityEngine;
using UnityEngine.UI;

public class BuffFlames : MonoBehaviour
{
    [SerializeField] private RawImage magicFlames;
    private Material flameMaterial;

    private void Awake()
    {
        // Creează o instanță separată a materialului
        flameMaterial = new Material(magicFlames.material);
        magicFlames.material = flameMaterial;
    }

    public void SetFlameColor(Color color)
    {
        flameMaterial.SetColor("_FlameColor", color);
    }

    private void OnDestroy()
    {
        if (flameMaterial != null)
            Destroy(flameMaterial);
    }
}
