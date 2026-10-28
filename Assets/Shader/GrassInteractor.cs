using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Add this to the player (or anything that should push grass aside).
/// Works with the "Custom/2D/Grass Wind" shader. Up to 8 interactors at once.
/// </summary>
public class GrassInteractor : MonoBehaviour
{
    [Tooltip("How far from this object grass is affected (world units).")]
    public float radius = 0.8f;

    [Tooltip("How far the grass tips are pushed (world units).")]
    public float strength = 0.25f;

    [Tooltip("Offset from the transform, e.g. to place the push point at the feet.")]
    public Vector2 offset = Vector2.zero;

    const int MaxInteractors = 8;

    static readonly List<GrassInteractor> Active = new List<GrassInteractor>();
    static readonly Vector4[] Data = new Vector4[MaxInteractors];
    static readonly int InteractorsId = Shader.PropertyToID("_GrassInteractors");
    static readonly int CountId = Shader.PropertyToID("_GrassInteractorCount");
    static int lastUploadFrame = -1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        Active.Clear();
        lastUploadFrame = -1;
    }

    void OnEnable()
    {
        if (!Active.Contains(this)) Active.Add(this);
    }

    void OnDisable()
    {
        Active.Remove(this);
        Upload();
    }

    void LateUpdate()
    {
        // Upload once per frame, no matter how many interactors exist
        if (lastUploadFrame == Time.frameCount) return;
        lastUploadFrame = Time.frameCount;
        Upload();
    }

    static void Upload()
    {
        int count = Mathf.Min(Active.Count, MaxInteractors);
        for (int i = 0; i < MaxInteractors; i++)
        {
            if (i < count)
            {
                GrassInteractor g = Active[i];
                Vector2 p = (Vector2)g.transform.position + g.offset;
                Data[i] = new Vector4(p.x, p.y, g.radius, g.strength);
            }
            else
            {
                Data[i] = Vector4.zero;
            }
        }
        Shader.SetGlobalVectorArray(InteractorsId, Data);
        Shader.SetGlobalFloat(CountId, count);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.3f, 1f, 0.3f, 0.6f);
        Gizmos.DrawWireSphere((Vector2)transform.position + offset, radius);
    }
}
