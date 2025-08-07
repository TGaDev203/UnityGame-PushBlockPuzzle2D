using UnityEngine;
using UnityEngine.Tilemaps;

public class GameObjSpawner : MonoBehaviour
{
    [SerializeField] private Tilemap boxTileMap;
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private Tilemap pointTilemap;
    [SerializeField] private GameObject pointPrefab;

    private void Start()
    {
        GameObject level = GameObject.FindGameObjectWithTag("Level");

        if (level != null)
        {
            TilemapMarker[] markers = level.GetComponentsInChildren<TilemapMarker>();
            foreach (var marker in markers)
            {
                if (marker.type == TilemapMarker.TilemapType.Box)
                {
                    boxTileMap = marker.GetComponent<Tilemap>();
                }

                else if (marker.type == TilemapMarker.TilemapType.Point)
                {
                    pointTilemap = marker.GetComponent<Tilemap>();
                }
            }
        }

        // Spawn Box
        foreach (Vector3Int pos in boxTileMap.cellBounds.allPositionsWithin)
        {
            if (!boxTileMap.HasTile(pos)) continue;
            Vector3 worldPos = boxTileMap.GetCellCenterWorld(pos);

            Instantiate(boxPrefab, worldPos, Quaternion.identity, transform);
        }

        // Spawn Box
        foreach (Vector3Int pos in pointTilemap.cellBounds.allPositionsWithin)
        {
            if (!pointTilemap.HasTile(pos)) continue;

            Vector3 worldPos = pointTilemap.GetCellCenterWorld(pos);

            Instantiate(pointPrefab, worldPos, Quaternion.identity, transform).tag = "Point";
        }

        boxTileMap.gameObject.SetActive(false);
        pointTilemap.gameObject.SetActive(false);
    }
}