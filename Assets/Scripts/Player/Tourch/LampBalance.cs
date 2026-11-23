using UnityEngine;
using UnityEngine.U2D.Animation;

public class LampBalance : MonoBehaviour
{
    [SerializeField] private SpriteResolver lampResolver;
    [SerializeField] private float pixelsPerUnit = 100f;

    private readonly Vector2[] frameOffsets =
    {
        new(0f, 0f),       // Walk_0
        new(1.5f, 0.5f),   // Walk_1
        new(-0.5f, 0.5f),  // Walk_2
        new(-1f, 0.5f),    // Walk_3
        new(-1f, -0.5f),   // Walk_4
        new(0.5f, -2f),    // Walk_5
        new(2.5f, -1.5f),  // Walk_6
        new(0f, -0.5f),    // Walk_7
        new(-1.5f, 1f)    // Walk_8
    };

    private Vector3 startPosition;

    private void Awake()
    {
        startPosition = transform.localPosition;
    }

    private void LateUpdate()
    {
        if (lampResolver == null)
            return;

        string label = lampResolver.GetLabel();

        if (!label.StartsWith("Walk_"))
            return;

        if (!int.TryParse(label.Substring(5), out int frame))
            return;

        if (frame < 0 || frame >= frameOffsets.Length)
            return;

        Vector2 offset = frameOffsets[frame] / pixelsPerUnit;

        transform.localPosition = startPosition +
            new Vector3(offset.x, offset.y, 0f);
    }

    private void OnDisable()
    {
        transform.localPosition = startPosition;
    }
}