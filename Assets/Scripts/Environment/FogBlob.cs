using UnityEngine;

public class FogBlob : MonoBehaviour
{
    [Header("Movement")]
    public float maxMoveX = 0.15f;
    public float maxMoveY = 0.15f;
    public float moveSpeed = 0.5f;

    [Header("Breathing")]
    public float scaleAmount = 0.03f;
    public float scaleSpeed = 0.4f;

    private Vector3 startPosition;
    private Vector3 startScale;
    private float offset;

    void Start()
    {
        startPosition = transform.localPosition;
        startScale = transform.localScale;

        offset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float x = Mathf.Sin(Time.time * moveSpeed + offset) * maxMoveX;
        float y = Mathf.Cos(Time.time * moveSpeed * 0.7f + offset) * maxMoveY;

        transform.localPosition = startPosition + new Vector3(x, y, 0f);

     //   float breathing = 1f + Mathf.Sin(Time.time * scaleSpeed + offset) * scaleAmount;

      //  transform.localScale = startScale * breathing;
    }
}
