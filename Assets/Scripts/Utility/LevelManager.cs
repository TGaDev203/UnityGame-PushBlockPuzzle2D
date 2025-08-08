using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    // [SerializeField] private GameObjSpawner gameObjSpawner;
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private GameObject gameObjSpawner;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] levelPrefabs;
    [SerializeField] private GridManager gridManager;
    private GameObject currentLevel;
    private List<Transform> allPoints;
    private List<Box> allBoxes;
    private int currentLevelIndex;
    public bool isLevelCompleted = false;
    private Tilemap boxTileMap;
    private Tilemap pointTilemap;
    private Tilemap walkableTilemap;

    private void Start()
    {
        InitWalkableTilemap();

        DebugManager.instance.enableRuntimeUI = false;
        currentLevelIndex = 1;
        StartCoroutine(LoadLevelWithDelay(currentLevelIndex));
    }

    private void Update()
    {
        CheckLevelComplete();
    }

    private IEnumerator LoadLevelWithDelay(int index)
    {
        if (currentLevel != null)
        {
            player.transform.position = Vector3.zero;
            playerMovement.StopAllCoroutines();
            Destroy(currentLevel);
        }

        currentLevel = Instantiate(levelPrefabs[index], Vector3.zero, Quaternion.identity, transform);

        yield return null;

        InitWalkableTilemap();

        SpawnBoxesAndPointsFromTilemap();
        yield return null;

        Transform spawnPoint = currentLevel.transform.Find("SpawnPoint");
        if (spawnPoint == null)
        {
            foreach (Transform t in currentLevel.GetComponentsInChildren<Transform>())
            {
                if (t.CompareTag("SpawnPoint"))
                {
                    spawnPoint = t;
                    break;
                }
            }
        }

        if (spawnPoint != null)
        {
            player.transform.position = spawnPoint.position;
        }

        else Debug.Log("SpawnPoint not found in level");

        UpdateBoxesSpritesOnLoad();
        playerMovement.SetBoxesAndPoints(allBoxes, allPoints);
        playerMovement.EnableMovement();

        isLevelCompleted = false;
    }

    private void SpawnBoxesAndPointsFromTilemap()
    {
        foreach (Transform child in gameObjSpawner.transform)
        {
            Destroy(child.gameObject);
        }

        // Spawn box
        foreach (Vector3Int pos in boxTileMap.cellBounds.allPositionsWithin)
        {
            if (!boxTileMap.HasTile(pos)) continue;
            Vector3 worldPos = boxTileMap.GetCellCenterWorld(pos);
            Instantiate(boxPrefab, worldPos, Quaternion.identity, gameObjSpawner.transform);
        }

        // Spawn point
        foreach (Vector3Int pos in pointTilemap.cellBounds.allPositionsWithin)
        {
            if (!pointTilemap.HasTile(pos)) continue;
            Vector3 worldPos = pointTilemap.GetCellCenterWorld(pos);
            GameObject point = Instantiate(pointPrefab, worldPos, Quaternion.identity, gameObjSpawner.transform);
            point.tag = "Point";
        }

        boxTileMap.gameObject.SetActive(false);
        pointTilemap.gameObject.SetActive(false);
    }

    private void UpdateBoxesSpritesOnLoad()
    {
        allPoints = new List<Transform>();
        GameObject[] pointObjs = GameObject.FindGameObjectsWithTag("Point");
        foreach (var p in pointObjs)
        {
            allPoints.Add(p.transform);
        }

        allBoxes = new List<Box>(FindObjectsByType<Box>(FindObjectsSortMode.None));

        foreach (var box in allBoxes)
        {
            box.RefreshSpriteOnLoad(allPoints, gridManager);
        }
    }

    private void CheckLevelComplete()
    {
        if (isLevelCompleted) return;

        if (allBoxes == null || allBoxes.Count == 0) return;

        foreach (var box in allBoxes)
        {
            if (!box.IsOnPoint())
            {
                return;
            }
        }

        isLevelCompleted = true;
        playerMovement.DisableMovement();

        StartCoroutine(DelayBeforeLoadNextLevel());
    }

    private IEnumerator DelayBeforeLoadNextLevel()
    {
        Debug.Log($"Level {currentLevelIndex} Completed");

        yield return new WaitForSeconds(1.5f);

        if (currentLevelIndex >= levelPrefabs.Length)
        {
            Debug.Log("Game Completed!");
            yield break;
        }

        currentLevelIndex += 1;
        StartCoroutine(LoadLevelWithDelay(currentLevelIndex));
    }

    public void InitWalkableTilemap()
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
                    gridManager.SetWalkableTilemap(walkableTilemap);
                }

                else if (marker.type == TilemapMarker.TilemapType.Box)
                {
                    boxTileMap = marker.GetComponent<Tilemap>();
                }

                else if (marker.type == TilemapMarker.TilemapType.Point)
                {
                    pointTilemap = marker.GetComponent<Tilemap>();
                }
            }
        }
    }
}