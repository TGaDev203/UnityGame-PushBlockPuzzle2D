using UnityEngine;
using UnityEngine.Tilemaps;

public class BoxTilemapSpawner : MonoBehaviour
{
    [SerializeField] private Tilemap boxTileMap;
    [SerializeField] private GameObject boxPrefab;

    private void Start()
    {
        foreach (Vector3Int pos in boxTileMap.cellBounds.allPositionsWithin)
        {
            if (!boxTileMap.HasTile(pos)) continue;
            Vector3 worldPos = boxTileMap.CellToWorld(pos) + boxTileMap.cellSize / 2f;
            Instantiate(boxPrefab, worldPos, Quaternion.identity);
        }

        boxTileMap.gameObject.SetActive(false);
    }
}