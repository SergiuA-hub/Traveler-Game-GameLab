using UnityEngine;

public class PixelAlignment : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float ppu;
    public float offsetX; 
    public float offsetY;
    void Start()
    {
        ppu = 126;
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.localPosition = new Vector3(Mathf.Round(offsetX)/ppu, Mathf.Round(offsetY)/ppu, transform.localPosition.z);
    }
}
