using UnityEngine;

public class DiagonalText : MonoBehaviour
{
    public float angle = 45;

    public void OnEnable()
    {
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
