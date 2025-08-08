using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    private Tilemap walkableTilemap;

    public Vector3Int GetCellInDirection(Vector3 worldPos, Vector2Int direction)
    {
        if (walkableTilemap == null) return Vector3Int.zero;

        Vector3Int currentCell = walkableTilemap.WorldToCell(worldPos);
        Vector3Int targetCell = currentCell + new Vector3Int(direction.x, direction.y, 0);
        return targetCell;
    }

    public bool IsBlocked(Vector3Int cell)
    {
        return !walkableTilemap.HasTile(cell);
    }

    public bool IsWalkable(Vector3Int cell)
    {
        return !IsBlocked(cell);
    }

    public Vector3 GetWorldCenter(Vector3Int cell)
    {
        return walkableTilemap.GetCellCenterWorld(cell);
    }

    public void SetWalkableTilemap(Tilemap tilemap)
    {
        walkableTilemap = tilemap;
    }
}