using UnityEngine;

public class LayerOrder : MonoBehaviour
{

    private const int IsometricRangePerYUnit = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = -(int)((transform.position.y - renderer.bounds.size.y / 2.0) * IsometricRangePerYUnit);
        }
    }

    

    void Update()
    {
        
    }
}
