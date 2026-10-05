using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class GrassRoadDepth : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap grassTilemap;
    public Tilemap roadTilemap;
    public Tilemap overlayTilemap;

    [Header("Effect")]
    [Range(0.01f, 0.3f)]
    public float grassOverlap = 0.08f;

    [ContextMenu("Generate Grass Overlap")]
    public void Generate()
    {
        if (grassTilemap == null ||
            roadTilemap == null ||
            overlayTilemap == null)
        {
            Debug.LogWarning("Assign all Tilemaps.");
            return;
        }

        overlayTilemap.ClearAllTiles();

        BoundsInt bounds = grassTilemap.cellBounds;

        foreach (Vector3Int cell in bounds.allPositionsWithin)
        {
            TileBase grassTile = grassTilemap.GetTile(cell);

            if (grassTile == null)
                continue;

            // ROAD RIGHT
            if (HasRoad(cell + Vector3Int.right))
            {
                PlaceGrass(
                    cell,
                    grassTile,
                    new Vector3(grassOverlap, 0, 0)
                );
            }

            // ROAD LEFT
            else if (HasRoad(cell + Vector3Int.left))
            {
                PlaceGrass(
                    cell,
                    grassTile,
                    new Vector3(-grassOverlap, 0, 0)
                );
            }

            // ROAD ABOVE
            else if (HasRoad(cell + Vector3Int.up))
            {
                PlaceGrass(
                    cell,
                    grassTile,
                    new Vector3(0, grassOverlap, 0)
                );
            }

            // ROAD BELOW
            else if (HasRoad(cell + Vector3Int.down))
            {
                PlaceGrass(
                    cell,
                    grassTile,
                    new Vector3(0, -grassOverlap, 0)
                );
            }
        }
    }

    bool HasRoad(Vector3Int cell)
    {
        return roadTilemap.HasTile(cell);
    }

    void PlaceGrass(
        Vector3Int cell,
        TileBase tile,
        Vector3 offset)
    {
        overlayTilemap.SetTile(cell, tile);

        overlayTilemap.SetTileFlags(
            cell,
            TileFlags.None
        );

        Matrix4x4 matrix = Matrix4x4.TRS(
            offset,
            Quaternion.identity,
            Vector3.one
        );

        overlayTilemap.SetTransformMatrix(
            cell,
            matrix
        );
    }
}