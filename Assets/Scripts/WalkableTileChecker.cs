using UnityEngine;
using UnityEngine.Tilemaps;

public class WalkableTileChecker : MonoBehaviour
{
    [SerializeField] private Tilemap blockedTilemap;
    [SerializeField] private Tilemap walkableTilemap;

    public Vector3Int GetCellInDirection(Vector3 worldPos, Vector2Int direction)
    {
        Vector3Int currentCell = walkableTilemap.WorldToCell(worldPos);
        Vector3Int targetCell = currentCell + new Vector3Int(direction.x, direction.y, 0);
        return targetCell;
    }

    public bool IsWalkable(Vector3Int cellPos)
    {
        return walkableTilemap.HasTile(cellPos) && !blockedTilemap.HasTile(cellPos);
    }

    public Vector3 GetWorldPosition(Vector3Int cell)
    {
        return walkableTilemap.GetCellCenterWorld(cell);
    }
}