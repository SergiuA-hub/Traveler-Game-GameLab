using UnityEngine;

public class LayerOrder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private const int IsometricRangePerYUnit = 100;

    void Update()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = -(int)((transform.position.y - renderer.bounds.size.y / 2.0) * IsometricRangePerYUnit);
        }
    }
}
