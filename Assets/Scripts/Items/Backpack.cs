using UnityEngine;

public class Backpack : MonoBehaviour
{

    [SerializeField] private float volume;
    [SerializeField] private float weight;


    public float GetVolume()
    {
        return volume;
    }

    public float GetWeight()
    {
        return weight;
    }
}
