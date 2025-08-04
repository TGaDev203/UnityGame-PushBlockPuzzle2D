using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PushableBox : MonoBehaviour
{
    [SerializeField] private Tilemap boxTilemap;
    [SerializeField] private Tilemap walkableTilemap;
    [SerializeField] private Tilemap blockedTilemap;

    public bool TryPush(Vector2Int dir, Tilemap blockedTilemap, List<GameObject> allBoxes)
    {
        Vector3Int currentCell = blockedTilemap.WorldToCell(transform.position);
        Vector3Int targetCell = currentCell + new Vector3Int(dir.x, dir.y, 0);

        if (blockedTilemap.HasTile(targetCell) || allBoxes.Exists(box => box != this && walkableTilemap.WorldToCell(box.transform.position) == targetCell)) return false;

        transform.position = blockedTilemap.GetCellCenterWorld(targetCell);
        return true;
    }
}