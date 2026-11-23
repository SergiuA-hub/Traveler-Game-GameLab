using UnityEngine;
using UnityEngine.Rendering;

public class LayerOrder1 : MonoBehaviour
{

    SortingGroup sortingGroup;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sortingGroup = GetComponent<SortingGroup>();
    }

    private const int IsometricRangePerYUnit = 100;

    void Update()
    {

        if (sortingGroup != null)
        {
            sortingGroup.sortingOrder = -(int)((transform.position.y) * IsometricRangePerYUnit);
        }
    //    Renderer renderer = GetComponent<Renderer>();
     //   if (renderer != null)
     //   {
   //         renderer.sortingOrder = -(int)((transform.position.y - renderer.bounds.size.y / 2.0) * IsometricRangePerYUnit);
    //    }
    }
}
