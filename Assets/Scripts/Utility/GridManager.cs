using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Tilemap walkableTilemap;

    private void Start()
    {
        GameObject level = GameObject.FindGameObjectWithTag("Level");

        if (level != null)
        {
            TilemapMarker[] markers = level.GetComponentsInChildren<TilemapMarker>();
            foreach (var marker in markers)
            {
                if (marker.type == TilemapMarker.TilemapType.Walkable)
                {
                    walkableTilemap = marker.GetComponent<Tilemap>();
                }
            }
        }
    }

    public Vector3Int GetCellInDirection(Vector3 worldPos, Vector2Int direction)
    {
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
}