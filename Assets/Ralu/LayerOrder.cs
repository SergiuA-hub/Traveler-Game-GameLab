using Unity.VisualScripting;
using UnityEngine;

public class LayerOrder : MonoBehaviour
{
    private const int IsometricRangePerYUnit = 100;
    public Renderer render;


    void Start()
    {
        render = GetComponent<Renderer>();
        if (render != null)
        {
            render.sortingOrder = -(int)((transform.position.y - render.bounds.size.y / 2.0) * IsometricRangePerYUnit);
        }
    }

    

    void Update()
    {
        
    }
}
