using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Bakes a smooth mask of the road tiles (in cell space) and feeds it to the
/// SunkenRoadTilemap shader. Works with Rectangle and Isometric grids.
/// Add it to the road Tilemap. Rebakes automatically when tiles are painted or erased.
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(Tilemap), typeof(TilemapRenderer))]
public class RoadMaskBaker : MonoBehaviour
{
    [Tooltip("Mask resolution per cell. 8 is plenty for most maps.")]
    [Range(2, 16)] public int pixelsPerCell = 8;

    [Tooltip("How far the mask is blurred, in cells. Keep it low (<= 0.4) if you have 1-tile-wide roads.")]
    [Range(0f, 1f)] public float blurCells = 0.3f;

    [Tooltip("Empty cells added around the map so edges fade cleanly.")]
    [Range(1, 4)] public int paddingCells = 2;

    static readonly int RoadMaskId   = Shader.PropertyToID("_RoadMask");
    static readonly int CellOriginId = Shader.PropertyToID("_CellOrigin");
    static readonly int CellInvId    = Shader.PropertyToID("_CellInv");
    static readonly int MaskSizeId   = Shader.PropertyToID("_MaskSize");

    Tilemap tilemap;
    TilemapRenderer tilemapRenderer;
    MaterialPropertyBlock mpb;
    Texture2D maskTex;
    bool dirty = true;

    void OnEnable()
    {
        tilemap = GetComponent<Tilemap>();
        tilemapRenderer = GetComponent<TilemapRenderer>();
        mpb ??= new MaterialPropertyBlock();
        Tilemap.tilemapTileChanged += OnTilesChanged;
        Bake();
    }

    void OnDisable()
    {
        Tilemap.tilemapTileChanged -= OnTilesChanged;
        if (tilemapRenderer != null) tilemapRenderer.SetPropertyBlock(null);
        DestroyTexture();
    }

    void OnValidate() => dirty = true;

    void OnTilesChanged(Tilemap changed, Tilemap.SyncTile[] tiles)
    {
        if (changed == tilemap) dirty = true;
    }

    void LateUpdate()
    {
        if (dirty || transform.hasChanged)
        {
            transform.hasChanged = false;
            Bake();
        }
    }

    [ContextMenu("Bake Now")]
    public void Bake()
    {
        dirty = false;
        if (tilemap == null || tilemapRenderer == null) return;

        tilemap.CompressBounds();
        BoundsInt b = tilemap.cellBounds;
        int pad = paddingCells;
        int ppc = pixelsPerCell;

        int cellsW = Mathf.Max(1, b.size.x) + pad * 2;
        int cellsH = Mathf.Max(1, b.size.y) + pad * 2;

        const int maxSize = 8192;
        if (cellsW * ppc > maxSize || cellsH * ppc > maxSize)
        {
            ppc = Mathf.Max(1, Mathf.Min(maxSize / cellsW, maxSize / cellsH));
            Debug.LogWarning($"RoadMaskBaker: map is large, mask reduced to {ppc} px per cell.", this);
        }
        int w = cellsW * ppc;
        int h = cellsH * ppc;

        // 1. Rasterize road cells in cell space.
        var buf = new float[w * h];
        foreach (var cell in b.allPositionsWithin)
        {
            if (!tilemap.HasTile(cell)) continue;
            int x0 = (cell.x - b.xMin + pad) * ppc;
            int y0 = (cell.y - b.yMin + pad) * ppc;
            for (int y = y0; y < y0 + ppc; y++)
            {
                int row = y * w;
                for (int x = x0; x < x0 + ppc; x++) buf[row + x] = 1f;
            }
        }

        // 2. Blur (two box passes per axis ~ gaussian): rounds corners, gives the edge a gradient.
        int r = Mathf.RoundToInt(blurCells * ppc);
        if (r > 0)
        {
            var tmp = new float[w * h];
            for (int pass = 0; pass < 2; pass++)
            {
                BoxBlur(buf, tmp, w, h, r, true);
                BoxBlur(tmp, buf, w, h, r, false);
            }
        }

        // 3. Upload.
        if (maskTex == null || maskTex.width != w || maskTex.height != h)
        {
            DestroyTexture();
            maskTex = new Texture2D(w, h, TextureFormat.R8, false, true)
            {
                name = "RoadMask",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.HideAndDontSave
            };
        }

        var bytes = new byte[w * h];
        for (int i = 0; i < bytes.Length; i++)
            bytes[i] = (byte)Mathf.RoundToInt(Mathf.Clamp01(buf[i]) * 255f);
        maskTex.SetPixelData(bytes, 0);
        maskTex.Apply(false, false);

        // 4. World -> cell affine mapping (works for Rectangle and Isometric layouts).
        Vector3 o  = tilemap.CellToWorld(Vector3Int.zero);
        Vector3 ex = tilemap.CellToWorld(new Vector3Int(1, 0, 0)) - o;
        Vector3 ey = tilemap.CellToWorld(new Vector3Int(0, 1, 0)) - o;
        float det = ex.x * ey.y - ey.x * ex.y;
        if (Mathf.Abs(det) < 1e-8f) det = 1e-8f;
        var inv = new Vector4(ey.y / det, -ey.x / det, -ex.y / det, ex.x / det);

        tilemapRenderer.GetPropertyBlock(mpb);
        mpb.SetTexture(RoadMaskId, maskTex);
        mpb.SetVector(CellOriginId, new Vector4(o.x, o.y, b.xMin - pad, b.yMin - pad));
        mpb.SetVector(CellInvId, inv);
        mpb.SetVector(MaskSizeId, new Vector4(cellsW, cellsH, 1f / cellsW, 1f / cellsH));
        tilemapRenderer.SetPropertyBlock(mpb);
    }

    static void BoxBlur(float[] src, float[] dst, int w, int h, int r, bool horizontal)
    {
        float inv = 1f / (2 * r + 1);
        int len = horizontal ? w : h;
        int lines = horizontal ? h : w;

        for (int line = 0; line < lines; line++)
        {
            float sum = 0f;
            for (int k = -r; k <= r; k++) sum += src[Index(Mathf.Clamp(k, 0, len - 1), line)];

            for (int i = 0; i < len; i++)
            {
                dst[Index(i, line)] = sum * inv;
                sum += src[Index(Mathf.Min(i + r + 1, len - 1), line)]
                     - src[Index(Mathf.Max(i - r, 0), line)];
            }
        }

        int Index(int i, int line) => horizontal ? line * w + i : i * w + line;
    }

    void DestroyTexture()
    {
        if (maskTex == null) return;
        if (Application.isPlaying) Destroy(maskTex);
        else DestroyImmediate(maskTex);
        maskTex = null;
    }
}
