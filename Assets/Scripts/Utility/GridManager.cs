using UnityEngine.Tilemaps;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    private Tilemap walkableTilemap;

    //* -------------------- UNITY LIFECYCLE --------------------

    private void Awake()
    {
        if (Instance == null) Instance = this;

        else
        {
            Destroy(gameObject);
            return;
        }
    }

    //* -------------------- GRID QUERY METHODS --------------------

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
    //* -------------------- TILEMAP SETUP --------------------
    public void SetWalkableTilemap(Tilemap tilemap)
    {
        walkableTilemap = tilemap;
    }
}